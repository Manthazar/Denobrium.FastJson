using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Denobrium.Json.Tests.Helpers
{
    [DataContract]
    public class GuidClass
    {
        [DataMember]
        public Guid Guid
        {
            get;
            set;
        }

        [DataMember]
        public Guid? NullableGuid
        {
            get;
            set;
        }
    }
}
