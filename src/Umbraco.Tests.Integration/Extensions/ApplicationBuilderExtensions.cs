﻿using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Umbraco.Configuration.Models;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Tests.Integration.Testing;
using Umbraco.Tests.Testing;

namespace Umbraco.Tests.Integration.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Creates a LocalDb instance to use for the test
        /// </summary>
        /// <param name="app"></param>
        /// <param name="dbFilePath"></param>
        /// <param name="integrationTest"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTestLocalDb(this IApplicationBuilder app,
            string dbFilePath,
            UmbracoIntegrationTest integrationTest)
        {
            // get the currently set db options
            var testOptions = TestOptionAttributeBase.GetTestOptions<UmbracoTestAttribute>();

            if (testOptions.Database == UmbracoTestOptions.Database.None)
                return app;

            // need to manually register this factory
            DbProviderFactories.RegisterFactory(Constants.DbProviderNames.SqlServer, SqlClientFactory.Instance);

            if (!Directory.Exists(dbFilePath))
                Directory.CreateDirectory(dbFilePath);

            var db = UmbracoIntegrationTest.GetOrCreate(dbFilePath,
                app.ApplicationServices.GetRequiredService<ILogger>(),
                app.ApplicationServices.GetRequiredService<IGlobalSettings>(),
                app.ApplicationServices.GetRequiredService<IUmbracoDatabaseFactory>());

            switch (testOptions.Database)
            {
                case UmbracoTestOptions.Database.NewSchemaPerTest:

                    // Add teardown callback
                    integrationTest.OnTestTearDown(() => db.Detach());

                    // New DB + Schema
                    db.AttachSchema();

                    // We must re-configure our current factory since attaching a new LocalDb from the pool changes connection strings
                    var dbFactory = app.ApplicationServices.GetRequiredService<IUmbracoDatabaseFactory>();
                    if (!dbFactory.Configured)
                    {
                        dbFactory.Configure(db.ConnectionString, Umbraco.Core.Constants.DatabaseProviders.SqlServer);
                    }

                    // In the case that we've initialized the schema, it means that we are installed so we'll want to ensure that
                    // the runtime state is configured correctly so we'll force update the configuration flag and re-run the
                    // runtime state checker.
                    // TODO: This wouldn't be required if we don't store the Umbraco version in config

                    // right now we are an an 'Install' state
                    var runtimeState = (RuntimeState)app.ApplicationServices.GetRequiredService<IRuntimeState>();
                    Assert.AreEqual(RuntimeLevel.Install, runtimeState.Level);

                    // dynamically change the config status
                    var umbVersion = app.ApplicationServices.GetRequiredService<IUmbracoVersion>();
                    var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
                    config[GlobalSettings.Prefix + "ConfigurationStatus"] = umbVersion.SemanticVersion.ToString();

                    // re-run the runtime level check
                    var profilingLogger = app.ApplicationServices.GetRequiredService<IProfilingLogger>();
                    runtimeState.DetermineRuntimeLevel(dbFactory, profilingLogger);

                    Assert.AreEqual(RuntimeLevel.Run, runtimeState.Level);

                    break;
                case UmbracoTestOptions.Database.NewEmptyPerTest:

                    // Add teardown callback
                    integrationTest.OnTestTearDown(() => db.Detach());

                    db.AttachEmpty();

                    break;
                case UmbracoTestOptions.Database.NewSchemaPerFixture:

                    // Add teardown callback
                    integrationTest.OnFixtureTearDown(() => db.Detach());

                    break;
                case UmbracoTestOptions.Database.NewEmptyPerFixture:

                    // Add teardown callback
                    integrationTest.OnFixtureTearDown(() => db.Detach());

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(testOptions), testOptions, null);
            }

            return app;
        }

    }


}
