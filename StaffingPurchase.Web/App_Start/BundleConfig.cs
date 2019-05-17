using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.Optimization;
using Newtonsoft.Json;

namespace StaffingPurchase.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Reset and re-add ignore list
            bundles.IgnoreList.Clear();
            AddDefaultIgnorePatterns(bundles.IgnoreList);

            // Create bundles based on config files
            CreateBundle(bundles, "~/App_Data/BundleScripts.json", BundleType.Script);
            CreateBundle(bundles, "~/App_Data/BundleStyles.json", BundleType.Style);
        }

        #region Nested
        internal enum BundleType
        {
            Script,
            Style
        }
        
        /// <summary>
        /// 
        /// </summary>
        private class BundleConfigItem
        {
            [JsonProperty("virtualPath")]
            public string VirtualPath { get; set; }

            [JsonProperty("files")]
            public string[] Files { get; set; }
        }

        #endregion
        
        #region Utility
        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
                throw new ArgumentNullException("ignoreList");
            ignoreList.Ignore("*.intellisense.js");
            ignoreList.Ignore("*-vsdoc.js");
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
        }

        private static void CreateBundle(BundleCollection bundles, string configFile, BundleType bundleType)
        {
            string scriptConfigFile = HostingEnvironment.MapPath(configFile);
            string scriptConfigs = File.ReadAllText(scriptConfigFile);
            var configItems = JsonConvert.DeserializeObject<IList<BundleConfigItem>>(scriptConfigs);
            foreach (var item in configItems)
            {
                bundles.Add(bundleType == BundleType.Script
                    ? new ScriptBundle(item.VirtualPath).Include(item.Files)
                    : new StyleBundle(item.VirtualPath).Include(item.Files));
            }
        }

        #region Create bundle based on physical files approach
        //private static void CreateBundleFromFolder(BundleCollection bundles, string folder)
        //{
        //    var bundleName = Path.GetFileName(folder);
        //    var files = Directory.GetFiles(folder, "*.js", SearchOption.AllDirectories);

        //    CreateBundle(bundles, bundleName, files);
        //}

        //private static void CreateBundle(BundleCollection bundles, string bundleName, string[] files)
        //{
        //    var scriptBundle = new Bundle(string.Format(APP_BUNDLE_PATH_FORMAT, bundleName));
        //    foreach (var scriptFile in files)
        //    {
        //        scriptBundle.Include(CommonHelper.ConvertToVirtualRelativePath(scriptFile, AppDomain.CurrentDomain.BaseDirectory));
        //    }

        //    bundles.Add(scriptBundle);
        //} 
        #endregion 
        #endregion
    }
}
