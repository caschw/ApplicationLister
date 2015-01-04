using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ApplicationLister.Extensions
{
    public static class ExpandoExtensions
    {
        /// <summary>
        /// Removes duplicates based upon DisplayName property of ExpandoObject
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static List<ExpandoObject> RemoveDuplicateResults(this IEnumerable<ExpandoObject> results)
        {
            var distinctResults = new List<ExpandoObject>();

            foreach (dynamic result in results)
            {
                if (((IDictionary<string, object>)result).ContainsKey("DisplayName") && !string.IsNullOrEmpty(result.DisplayName))
                {
                    if (distinctResults.All(x => ((dynamic)x).DisplayName != result.DisplayName))
                    {
                        distinctResults.Add(result);
                    }
                }
            }
            return distinctResults;
        }
    }
}
