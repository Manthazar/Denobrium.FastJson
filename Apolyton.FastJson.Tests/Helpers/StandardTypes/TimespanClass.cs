
using System.Runtime.Serialization;
using System;

namespace Apolyton.FastJson.Tests.Helpers
{
    [DataContract]
    public class TimespanClass
    {
        [DataMember]
        public TimeSpan TimeSpan
        {
            get;
            set;
        }

        [DataMember]
        public TimeSpan? NullableTimeSpan
        {
            get;
            set;
        }
    }
}
