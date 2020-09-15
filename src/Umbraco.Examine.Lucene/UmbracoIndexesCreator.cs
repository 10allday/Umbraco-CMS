﻿using System.Collections.Generic;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Lucene.Net.Analysis.Standard;
using Examine.LuceneEngine;
using Examine;
using Umbraco.Core.Configuration;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Hosting;
using Umbraco.Core.IO;

namespace Umbraco.Examine
{

    /// <summary>
    /// Creates the indexes used by Umbraco
    /// </summary>
    public class UmbracoIndexesCreator : LuceneIndexCreator, IUmbracoIndexesCreator
    {
        // TODO: we should inject the different IValueSetValidator so devs can just register them instead of overriding this class?

        public UmbracoIndexesCreator(
            ITypeFinder typeFinder,
            IProfilingLogger profilingLogger,
            ILogger<UmbracoContentIndex> contentIndexLogger, ILogger<UmbracoExamineIndex> examineIndexLogger,
            ILogger<UmbracoExamineIndexDiagnostics> examineIndexDiagnosticsLogger,
            ILocalizationService languageService,
            IPublicAccessService publicAccessService,
            IMemberService memberService,
            IUmbracoIndexConfig umbracoIndexConfig,
            IHostingEnvironment hostingEnvironment,
            IRuntimeState runtimeState,
            IIndexCreatorSettings settings,
            ILuceneDirectoryFactory directoryFactory) : base(typeFinder, hostingEnvironment, settings)
        {
            ProfilingLogger = profilingLogger ?? throw new System.ArgumentNullException(nameof(profilingLogger));
            ContentIndexLogger = contentIndexLogger;
            ExamineIndexLogger = examineIndexLogger;
            ExamineIndexDiagnosticsLogger = examineIndexDiagnosticsLogger;
            LanguageService = languageService ?? throw new System.ArgumentNullException(nameof(languageService));
            PublicAccessService = publicAccessService ?? throw new System.ArgumentNullException(nameof(publicAccessService));
            MemberService = memberService ?? throw new System.ArgumentNullException(nameof(memberService));
            UmbracoIndexConfig = umbracoIndexConfig;
            HostingEnvironment = hostingEnvironment ?? throw new System.ArgumentNullException(nameof(hostingEnvironment));
            RuntimeState = runtimeState ?? throw new System.ArgumentNullException(nameof(runtimeState));
            DirectoryFactory = directoryFactory;
        }

        protected IProfilingLogger ProfilingLogger { get; }
        protected ILogger<UmbracoContentIndex> ContentIndexLogger { get; }
        protected ILogger<UmbracoExamineIndex> ExamineIndexLogger { get; }
        protected ILogger<UmbracoExamineIndexDiagnostics> ExamineIndexDiagnosticsLogger { get; }
        protected IHostingEnvironment HostingEnvironment { get; }
        protected IRuntimeState RuntimeState { get; }
        protected ILuceneDirectoryFactory DirectoryFactory { get; }
        protected ILocalizationService LanguageService { get; }
        protected IPublicAccessService PublicAccessService { get; }
        protected IMemberService MemberService { get; }
        protected IUmbracoIndexConfig UmbracoIndexConfig { get; }

        /// <summary>
        /// Creates the Umbraco indexes
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IIndex> Create()
        {
            return new[]
            {
                CreateInternalIndex(),
                CreateExternalIndex(),
                CreateMemberIndex()
            };
        }

        private IIndex CreateInternalIndex()
        {
            var index = new UmbracoContentIndex(
                Constants.UmbracoIndexes.InternalIndexName,
                DirectoryFactory.CreateDirectory(Constants.UmbracoIndexes.InternalIndexPath),
                new UmbracoFieldDefinitionCollection(),
                new CultureInvariantWhitespaceAnalyzer(),
                ProfilingLogger,
                ContentIndexLogger,
                ExamineIndexLogger,
                ExamineIndexDiagnosticsLogger,
                HostingEnvironment,
                RuntimeState,
                LanguageService,
                UmbracoIndexConfig.GetContentValueSetValidator()
                );
            return index;
        }

        private IIndex CreateExternalIndex()
        {
            var index = new UmbracoContentIndex(
                Constants.UmbracoIndexes.ExternalIndexName,
                DirectoryFactory.CreateDirectory(Constants.UmbracoIndexes.ExternalIndexPath),
                new UmbracoFieldDefinitionCollection(),
                new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30),
                ProfilingLogger,
                ContentIndexLogger,
                ExamineIndexLogger,
                ExamineIndexDiagnosticsLogger,
                HostingEnvironment,
                RuntimeState,
                LanguageService,
                UmbracoIndexConfig.GetPublishedContentValueSetValidator());
            return index;
        }

        private IIndex CreateMemberIndex()
        {
            var index = new UmbracoMemberIndex(
                Constants.UmbracoIndexes.MembersIndexName,
                new UmbracoFieldDefinitionCollection(),
                DirectoryFactory.CreateDirectory(Constants.UmbracoIndexes.MembersIndexPath),
                new CultureInvariantWhitespaceAnalyzer(),
                ProfilingLogger,
                ExamineIndexLogger,
                ExamineIndexDiagnosticsLogger,
                HostingEnvironment,
                RuntimeState,
                UmbracoIndexConfig.GetMemberValueSetValidator()
                );
            return index;
        }
    }
}
