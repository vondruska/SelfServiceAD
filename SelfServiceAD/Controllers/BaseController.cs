using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SelfServiceAD.Controllers
{
    using MvcThrottle;

    [EnableThrottling]
    public class BaseController : Controller
    {
    }
}