using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SelfServiceAD
{
    using System.Net;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class MvcThrottleCustomFilter : MvcThrottle.ThrottlingFilter
    {
        protected override ActionResult QuotaExceededResult(RequestContext filterContext, string message, HttpStatusCode responseCode, string requestId)
        {
            var rateLimitedView = new ViewResult
            {
                ViewName = "RateLimited"
            };

            filterContext.HttpContext.Response.StatusDescription = "Too Many Requests";

            return rateLimitedView;
        }
    }
}