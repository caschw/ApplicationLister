using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationLister.Extensions;
using ApplicationLister.Lister;

namespace ApplicationLister
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Get Apps from a registry search
            var registryApps = new ApplicationRegistryLister().GetApplications();
            // Get Apps through the management API
            var managementApps = new ApplicationManagementLister().GetApplications();

            // Merge lists and remove dupes
            // registry apps provide more details than through the api, the way that dupes are removed, 
            // it favors items at the beginning of the list.
            registryApps.AddRange(managementApps);
            registryApps = registryApps.RemoveDuplicateResults();

            // Output app list to console and file
            var i = 0;
            using (var file = new System.IO.StreamWriter(@"C:\test\InstalledPrograms.txt"))
            {
                foreach (IDictionary<string, object> application in registryApps.OrderBy(x => (x as IDictionary<string, object>)["DisplayName"]))
                {
                    // Get all the properties this application has filled out
                    var keys = application.Keys.ToList();

                    // Alphabetize and display only properties that have values
                    foreach (var key in keys.Where(key => !string.IsNullOrEmpty(application[key].ToString())).OrderBy(x => x))
                    {
                        var line = string.Format("{0}: \t{1}", key, application[key]);
                        file.WriteLine(line);
                        Console.WriteLine(line);
                    }

                    // Extra blank line in between apps for readability
                    file.WriteLine();
                    Console.WriteLine();

                    // Alternate colors on alternating apps in console to make them easier to read
                    i++;
                    Console.ForegroundColor = i%2 == 0 ? ConsoleColor.DarkCyan : ConsoleColor.DarkYellow;
                }
            }
            Console.ReadKey();
        }
    }
}
