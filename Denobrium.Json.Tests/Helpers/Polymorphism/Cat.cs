using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Denobrium.Json.Tests.Helpers.Polymorphism
{
    [DataContract(Name = "kitty")]
    public class Cat : Animal
    {
        [DataMember]
        public int Cuteness = 2;
    }
}
