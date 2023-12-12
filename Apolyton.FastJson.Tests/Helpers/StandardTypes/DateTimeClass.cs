using System;
using System.Runtime.Serialization;
using Apolyton.FastJson.Serialization;

namespace Apolyton.FastJson.Tests.Helpers
{
    [DataContract]
    public class DateTimeClass
    {
        /// <summary>
        /// Date time created from a local string (no kind specified).
        /// </summary>
        [DataMember]
        public DateTime DateTime
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? NullableDateTime
        {
            get;
            set;
        }

        [DataMember]
        [JsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LocalDateTime
        {
            get;
            set;
        }


        [DataMember]
        [JsonDateTimeOptions(Kind= DateTimeKind.Utc)]
        public DateTime UtcDateTime
        {
            get;
            set;
        }

        [DataMember]
        [JsonDateTimeOptions(Kind = DateTimeKind.Local, Format="yyyy.MM.dd")]
        public DateTime CustomFormatDateTime
        {
            get;
            set;
        }
    }
}
