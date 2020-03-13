﻿using LightInject;
using LightInject.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Umbraco.Core.Composing.LightInject;
using Umbraco.Core.Configuration;

namespace Umbraco.Core.Composing
{
    /// <summary>
    /// Creates the container.
    /// </summary>
    public static class RegisterFactory
    {
        /// <summary>
        /// Creates a new <see cref="IRegister"/> based on an existing MSDI IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IRegister CreateFrom(IServiceCollection services, out IServiceProvider serviceProvider)
        {
            var liContainer = new ServiceContainer(ContainerOptions.Default.WithMicrosoftSettings());
            serviceProvider = liContainer.CreateServiceProvider(services);
            return new LightInjectContainer(liContainer);
        }

        //TODO: The following can die when net framework is gone

        // cannot use typeof().AssemblyQualifiedName on the web container - we don't reference it
        // a normal Umbraco site should run on the web container, but an app may run on the core one
        private const string CoreLightInjectContainerTypeName = "Umbraco.Core.Composing.LightInject.LightInjectContainer,Umbraco.Core";
        private const string WebLightInjectContainerTypeName = "Umbraco.Web.Composing.LightInject.LightInjectContainer,Umbraco.Web";

        /// <summary>
        /// Creates a new instance of the configured container.
        /// </summary>
        /// <remarks>
        /// To override the default LightInjectContainer, add an appSetting named 'Umbraco.Core.RegisterType' with
        /// a fully qualified type name to a class with a static method "Create" returning an IRegister.
        /// </remarks>
        public static IRegister Create(IGlobalSettings globalSettings)
        {
            Type type;

            var configuredTypeName = globalSettings.RegisterType;
            if (configuredTypeName.IsNullOrWhiteSpace())
            {
                // try to get the web LightInject container type,
                // else the core LightInject container type
                type = Type.GetType(configuredTypeName = WebLightInjectContainerTypeName) ??
                       Type.GetType(configuredTypeName = CoreLightInjectContainerTypeName);
            }
            else
            {
                // try to get the configured type
                type = Type.GetType(configuredTypeName);
            }

            if (type == null)
                throw new Exception($"Cannot find register factory class '{configuredTypeName}'.");

            var factoryMethod = type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
            if (factoryMethod == null)
                throw new Exception($"Register factory class '{configuredTypeName}' does not have a public static method named Create.");

            var container = factoryMethod.Invoke(null, Array.Empty<object>()) as IRegister;
            if (container == null)
                throw new Exception($"Register factory '{configuredTypeName}' did not return an IRegister implementation.");

            return container;
        }
    }
}
