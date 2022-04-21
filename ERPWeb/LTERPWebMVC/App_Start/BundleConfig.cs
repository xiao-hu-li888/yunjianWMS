using System.Web;
using System.Web.Optimization;

namespace LTERPWebMVC
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui/jquery-ui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备就绪，请使用 https://modernizr.com 上的生成工具仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));
            bundles.Add(new ScriptBundle("~/Scripts/main").Include("~/Scripts/main.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      //  "~/Content/ace.min.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/page").Include("~/Content/page.css"));
            //错误页样式
            bundles.Add(new StyleBundle("~/Error/css").Include(
                 "~/Content/error.css"));
            bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
                "~/Scripts/jquery-ui/jquery-ui.min.css",
                 "~/Scripts/jquery-ui/jquery-ui.theme.min.css"));
            //日历控件
            bundles.Add(new ScriptBundle("~/Scripts/My97DatePicker/js").Include(
                      "~/Scripts/My97DatePicker/WdatePicker.js"));
            //bundles.Add(new StyleBundle("~/Scripts/My97DatePicker/skin/css").Include(
            //    "~/Scripts/My97DatePicker/skin/WdatePicker.css"
            //    ));
            //bundles.Add(new StyleBundle("~/Scripts/My97DatePicker/skin/twoer/css").Include(
            // "~/Scripts/My97DatePicker/skin/twoer/datepicker.css"
            // ));
        }
    }
}
