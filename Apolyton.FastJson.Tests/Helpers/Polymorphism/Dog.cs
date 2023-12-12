using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers.Polymorphism
{
    [DataContract(Name="doggy")]
    public class Dog : Animal
    {
        [DataMember]
        public int Power = 4;
    }
}
