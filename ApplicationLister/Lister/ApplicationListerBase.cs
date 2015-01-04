using System.Collections.Generic;
using System.Dynamic;

namespace ApplicationLister.Lister
{
    public abstract class ApplicationListerBase
    {
        public abstract List<ExpandoObject> GetApplications();
    }
}
