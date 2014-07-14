using System.Web;
using System.Web.Mvc;

namespace SelfServiceAD
{
    using MvcThrottle;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            var throttleFilter = new MvcThrottleCustomFilter
            {
                Policy = new ThrottlePolicy(perSecond: 3, perMinute: 20)
                {
                    IpThrottling = true
                },
                Repository = new CacheRepository()
            };

            filters.Add(throttleFilter);
        }
    }
}
