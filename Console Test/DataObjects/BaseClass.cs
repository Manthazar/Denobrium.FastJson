using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace consoletest.DataObjects
{
    [DataContract(Name="Base")]
    public class BaseClass
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
