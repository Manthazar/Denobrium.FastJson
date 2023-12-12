using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers
{
    [DataContract]
    public class DictionaryClass<TKey, TValue>
    {
        [DataMember]
        public Dictionary<TKey, TValue> Dictionary
        {
            get;
            set;
        }
    }
}
