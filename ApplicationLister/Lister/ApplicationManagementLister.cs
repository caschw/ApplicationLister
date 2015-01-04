using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApplicationLister.Lister
{
    public class ApplicationManagementLister : ApplicationListerBase
    {
        public override List<ExpandoObject> GetApplications()
        {
            var s = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            var applications = s.Get();

            var applicationList = new List<ExpandoObject>();

            foreach (var application in applications)
            {
                var appString = application.ToString().Split(',');

                // Manually pull out property values.
                var name = ExtractCleanPropertyValue(appString, "Name");
                var version = ExtractCleanPropertyValue(appString, "Version");

                dynamic item = new ExpandoObject();
                item.DisplayName = name;
                item.Version = version;
                applicationList.Add(item);
            }
            return applicationList;
        }

        private string ExtractCleanPropertyValue(IEnumerable<string> appString, string propertyName)
        {
            // Grab the item that begins with the key
            var property = appString.FirstOrDefault(x => x.StartsWith(propertyName));
            
            // Remove the keyname and extra cruft from the string
            property = Regex.Replace(property, propertyName + @"=""", "");
            property = Regex.Replace(property, @"""", "");
            return property;
        }
    }
}
