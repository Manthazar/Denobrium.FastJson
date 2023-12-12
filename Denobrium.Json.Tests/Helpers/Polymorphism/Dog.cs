using System.Runtime.Serialization;

namespace Denobrium.Json.Tests.Helpers.Polymorphism
{
    [DataContract(Name="doggy")]
    public class Dog : Animal
    {
        [DataMember]
        public int Power = 4;
    }
}
