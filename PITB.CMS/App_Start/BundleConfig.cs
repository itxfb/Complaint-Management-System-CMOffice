using System.Web.Optimization;

namespace PITB.CMS.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js",
            //             "~/Scripts/jquery.validate.js",
            //            "~/Scripts/jquery.unobtrusive-ajax.js",
            //            "~/Scripts/jquery.validate.unobtrusive.js"
            //            ));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                     "~/plugins/jQuery/jQuery-2.1.4.min.js"
                     //"~/Scripts/jquery.validate.js",

                     //"~/Scripts/jquery.validate.unobtrusive.js"
                     //"~/Scripts/MvcFoolproofJQueryValidation.min.js"
                     ));
            bundles.Add(new ScriptBundle("~/bundles/bridgeLibs").Include(
                "~/js/Bridge/FormBuilder/bridge.js",
                "~/js/Bridge/FormBuilder/bridge.meta.js",
                "~/js/Bridge/FormBuilder/bridge.console.js",
                "~/js/Bridge/FormBuilder/jquery-2.2.4.js",
                "~/js/Bridge/FormBuilder/newtonsoft.json.js"));

            bundles.Add(new ScriptBundle("~/bundles/bridgeFormBuilder").Include(
                        // "~/js/Bridge/FormBuilder/bridge.js",
                        //"~/js/Bridge/FormBuilder/bridge.meta.js",
                        // "~/js/Bridge/FormBuilder/bridge.console.js",
                        "~/js/Bridge/FormBuilder/BridgeClassLib.js",
                        "~/js/Bridge/FormBuilder/BridgeClassLib.meta.js",
                        // "~/js/Bridge/FormBuilder/jquery-2.2.4.js",
                        // "~/js/Bridge/FormBuilder/newtonsoft.json.js",
                        "~/js/Bridge/FormBuilder/Form.js",
                        "~/js/Bridge/FormBuilder/FormBuilder.js",
                        "~/js/Bridge/FormBuilder/FormInitializer.js",
                        "~/js/Bridge/FormBuilder/FormEventHandler.js",
                        "~/js/Bridge/FormBuilder/FormValidationHandler.js",
                        "~/js/Bridge/FormBuilder/FormCommonMethods.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/js/MyCustomValidation.js",
                         //"~/Scripts/MicrosoftMvcAjax.js",
                         "~/Scripts/jquery.unobtrusive-ajax.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/bootstrap/js/bootstrap.js",
                      "~/plugins/iCheck/icheck.js"));

            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                          "~/js/app.js",
                          "~/js/sjcl.js",
                      "~/js/sweetalert2.js",
                      "~/js/select2.full.js",
                     "~/js/demo.js",
                     "~/plugins/datatables/jquery.dataTables.js",
                     "~/plugins/datatables/jquery.dataTables.columnFilter",
                     "~/plugins/datatables/extensions/TableTools/js/dataTables.tableTools.js",
                     "~/plugins/datatables/jquery.dataTables.responsive.js",

                     "~/plugins/datatables/dataTables.bootstrap.js",
                     "~/plugins/datepicker/bootstrap-datepicker.js",
                      "~/Scripts/bootstrap-multiselect.js",
                      "~/Scripts/bootstrap-multiselect-collapsible-groups.js",
                      "~/js/notify.js",
                      "~/plugins/daterangepicker/moment.min.js",
                      "~/plugins/daterangepicker/daterangepicker.js",
                     "~/plugins/BootstrapValidator/validator.js",
                     "~/plugins/toastr/toastr.js"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                     "~/js/DropDownListHandler.js",
                     "~/js/RuntimeValidation.js",
                     "~/js/UIUtility.js",
                     "~/js/HelperFunctions.js",
                      "~/js/common.js",
                      "~/js/MessageHandler.js",
                      "~/js/HtmlTableHandler.js"

                     ));

            bundles.Add(new ScriptBundle("~/bundles/inlineStyler").Include(
                      "~/js/jquery.inlineStyler.src.js",
                      "~/js/inlineStyler.jquery.json",
                      "~/js/table2excel.js"

                     ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/bootstrap/css/bootstrap.css",
                      "~/css/crm.css",
                      "~/css/Dashboard.css",
                      "~/css/skins/_all-skins.css",
                      "~/css/sweetalert2.css",
                      "~/css/select2.min.css",
                      "~/plugins/datatables/dataTables.bootstrap.css",
                      "~/plugins/datatables/extensions/TableTools/css/dataTables.tableTools.css",
                       "~/plugins/datatables/jquery.dataTables.responsive.css",
                      "~/plugins/datepicker/datepicker3.css",
                      "~/Content/bootstrap-multiselect.css",
                      "~/plugins/daterangepicker/daterangepicker.css",
                      "~/plugins/BootstrapValidator/validator.css",
                      "~/plugins/toastr/toastr.css"));

            bundles.Add(new StyleBundle("~/css/AdminLTE").Include("~/css/skins/_all-skins.min.css",
                "~/css/AdminLTE/AdminLTE.min.css"));

            bundles.Add(new Bundle("~/bundles/dataTables").Include(
                 "~/Scripts/DataTables/jquery.dataTables.js",
                        "~/Scripts/DataTables/dataTables.bootstrap.js",
                        "~/Scripts/DataTables/dataTables.responsive.js"));


            bundles.Add(new ScriptBundle("~/bundles/MvcFoolproof").Include(
                       // "~/Scripts/MicrosoftAjax.js",
                       // "~/Scripts/MicrosoftMvcAjax.js",
                       // "~/Scripts/MicrosoftMvcValidation.js",
                       //"~/Client Scripts/MvcFoolproofJQueryValidation.min.js",
                       //"~/Client Scripts/MvcFoolproofValidation.min.js",
                       "~/Client Scripts/mvcfoolproof.unobtrusive.min.js"
                       ));
            bundles.Add(new ScriptBundle("~/bundles/custom-validator").Include(
                                "~/Scripts/script-custom-validator.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}