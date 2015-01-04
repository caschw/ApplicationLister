using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ApplicationLister
{
    public class RegKeyConfig
    {
        public string Attribute { get; set; }
        public RegistryHive Hive { get; set; }
    }
}
