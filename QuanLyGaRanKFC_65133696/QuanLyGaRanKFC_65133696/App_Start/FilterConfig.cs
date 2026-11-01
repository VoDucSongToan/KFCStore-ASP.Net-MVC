using System.Web;
using System.Web.Mvc;

namespace QuanLyGaRanKFC_65133696
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
