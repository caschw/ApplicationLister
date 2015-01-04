using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ApplicationLister.Extensions;
using Microsoft.Win32;

namespace ApplicationLister.Lister
{
    public class ApplicationRegistryLister : ApplicationListerBase
    {
        private readonly List<RegKeyConfig> _regKeyConfigList; 

        /// <summary>
        /// Default constructor, initializes default registry key locations
        /// </summary>
        public ApplicationRegistryLister()
        {
            // Use magic strings for key path locations
            _regKeyConfigList = new List<RegKeyConfig>
            {
                new RegKeyConfig{Attribute = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", Hive = RegistryHive.CurrentUser},
                new RegKeyConfig{Attribute = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", Hive = RegistryHive.LocalMachine},
                new RegKeyConfig{Attribute = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall", Hive = RegistryHive.LocalMachine}
            };
        }

        /// <summary>
        /// Load custom registry key path values with this constructor
        /// </summary>
        /// <param name="regKeyConfig"></param>
        public ApplicationRegistryLister(List<RegKeyConfig> regKeyConfig)
        {
            _regKeyConfigList = regKeyConfig;
        }

        public override List<ExpandoObject> GetApplications()
        {
            var results = new List<ExpandoObject>();
            foreach (var regKeyConfig in _regKeyConfigList)
            {
                results.AddRange(GetApplicationMetadata(Environment.MachineName, regKeyConfig.Hive, regKeyConfig.Attribute));
            }

            return results.RemoveDuplicateResults();
        }

        private List<ExpandoObject> GetApplicationMetadata(string machineName, RegistryHive hive, string subKeyPath)
        {
            var aList = new List<ExpandoObject>();

            using (var regHive = RegistryKey.OpenRemoteBaseKey(hive, machineName))
            {
                using (var regKey = regHive.OpenSubKey(subKeyPath))
                {
                    if (regKey != null)
                    {
                        foreach (var kn in regKey.GetSubKeyNames())
                        {
                            using (var subkey = regKey.OpenSubKey(kn))
                            {
                                var names = subkey.GetValueNames().ToList();
                                if (names.Any())
                                {
                                    var expando = new ExpandoObject();
                                    var dict = expando as IDictionary<string, object>;
                                    foreach (var name in names)
                                    {
                                        var subkeyval = subkey.GetValue(name);
                                        var cleanedPropertyName = name.Replace(" ", "");
                                        dict[cleanedPropertyName] = subkeyval;
                                    }
                                    aList.Add(expando);
                                }
                            }
                        }
                    }
                }
            }
            return aList;
        }
    }
}
