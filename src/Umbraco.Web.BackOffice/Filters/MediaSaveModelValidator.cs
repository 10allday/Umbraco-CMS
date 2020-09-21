﻿using Microsoft.Extensions.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Security;

namespace Umbraco.Web.BackOffice.Filters
{
    /// <summary>
    /// Validator for <see cref="MediaItemSave"/>
    /// </summary>
    internal class MediaSaveModelValidator : ContentModelValidator<IMedia, MediaItemSave, IContentProperties<ContentPropertyBasic>>
    {
        public MediaSaveModelValidator(
            ILogger<MediaSaveModelValidator> logger,
            IWebSecurity webSecurity,
            ILocalizedTextService textService,
            IPropertyValidationService propertyValidationService)
            : base(logger, webSecurity, textService, propertyValidationService)
        {
        }
    }
}
