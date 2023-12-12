using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers.RegistryHelpers
{
    public class DuplicateMemberNameClass
    {
        [DataMember(Name="p")]
        public int Property1
        {
            get;
            set;
        }

        [DataMember(Name="p")]
        public int Property2
        {
            get;
            set;
        }
    }
}
