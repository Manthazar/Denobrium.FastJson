using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers.Polymorphism
{
    [DataContract(Name = "kitty")]
    public class Cat : Animal
    {
        [DataMember]
        public int Cuteness = 2;
    }
}
