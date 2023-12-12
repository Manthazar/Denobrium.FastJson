using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace consoletest.DataObjects
{
    [DataContract(Name = "Gender")]
    public enum Gender
    {
        Male,
        Female
    }
}
