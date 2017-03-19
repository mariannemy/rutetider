using System.Web.Optimization;

namespace RuterApp.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/main").Include(
                      "~/Scripts/script.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/styles.css"));
        }
    }
}