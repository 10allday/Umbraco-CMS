﻿using Microsoft.Owin;
using Umbraco.Core.Models.Identity;
using Umbraco.Web.Models.Identity;

namespace Umbraco.Web.Security
{
    /// <summary>
    /// This interface is only here due to the fact that IOwinContext Get / Set only work in generics, if they worked
    /// with regular 'object' then we wouldn't have to use this work around but because of that we have to use this
    /// class to resolve the 'real' type of the registered user manager
    /// </summary>
    internal interface IBackOfficeUserManagerMarker
    {
        BackOfficeUserManager2<BackOfficeIdentityUser> GetManager(IOwinContext owin);
    }
    /// <summary>
    /// This interface is only here due to the fact that IOwinContext Get / Set only work in generics, if they worked
    /// with regular 'object' then we wouldn't have to use this work around but because of that we have to use this
    /// class to resolve the 'real' type of the registered user manager
    /// </summary>
    internal interface IBackOfficeUserManagerMarker2
    {
        BackOfficeUserManager2<BackOfficeIdentityUser> GetManager(IOwinContext owin);
    }
}
