﻿using Abp.AspNetCore.Mvc.Extensions;
using Abp.Auditing;
using Abp.Extensions;
using Abp.Localization;
using Abp.Timing;
using Abp.Web.Mvc.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.Mvc.Controllers
{
    public class AbpLocalizationController : AbpController
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public AbpLocalizationController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [DisableAuditing]
        public virtual ActionResult ChangeCulture(string cultureName, string returnUrl = "")
        {
            if (!GlobalizationHelper.IsValidCultureCode(cultureName))
            {
                throw new AbpException("Unknown language: " + cultureName + ". It must be a valid culture!");
            }

            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureName, cultureName));

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                cookieValue,
                new CookieOptions {Expires = Clock.Now.AddYears(2)}
            );

            if (Request.IsAjaxRequest())
            {
                return Json(new MvcAjaxResponse());
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(_hostingEnvironment.WebRootPath.EnsureEndsWith('/'));
        }
    }
}