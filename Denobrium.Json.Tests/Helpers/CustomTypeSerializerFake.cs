using System;
using Denobrium.Json.Serialization;
using Denobrium.Json.Tests.Helpers.ParameterRelated;

namespace Denobrium.Json.Tests.Helpers
{
    internal class CustomTypeSerializerFake : ICustomTypeSerializer
    {
        public Type Type
        {
            get { return typeof(CustomTypeClass.CustomType); }
        }

        public string TypeName
        {
            get { return null; }
        }

        public bool CanSerialize
        {
            get { return true; }
        }

        public bool CanDeserialize
        {
            get { return false; }
        }

        public string Serialize(object data)
        {
            return "yes";
        }

        public object Deserialize(string jsonValueString)
        {
            throw new NotImplementedException();
        }
    }
}
