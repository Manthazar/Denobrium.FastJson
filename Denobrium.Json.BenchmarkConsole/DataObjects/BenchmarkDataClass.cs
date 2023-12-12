using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Denobrium.Json.Benchmark.DataObjects
{
    [DataContract(Name = "FastJsonBenchmarkClass")]
    [Serializable]
    public class BenchmarkDataClass
    {
        public BenchmarkDataClass()
        {
        }

        #region simple types

        public bool BooleanValue { get; set; }
        public DateTime DateValue { get; set; }

        public Guid Guid { get; set; }

        public decimal DecimalValue { get; set; }
        public double DoubleValue { get; set; }

        public Guid? NullableGuid { get; set; }

        public DateTime? NullableDateTime{ get; set; }

        public decimal? NullableDecimal { get; set; }
        public double? NullableDouble { get; set; }

        public string MultilineString { get; set; }

        public string SimpleString { get; set; }

        #endregion

        #region complex types

        public List<BaseClass> ItemList { get; set; }

        #endregion

        public Gender EnumValue { get; set; }
        public DataSet DataSet { get; set; }

        public Dictionary<string, BaseClass> StringDictionary { get; set; }
        public Dictionary<BaseClass, BaseClass> ObjectDictionary { get; set; }
        public Dictionary<int, BaseClass> IntDictionary { get; set; }

        
        public Hashtable HashTableValue { get; set; }
        public BaseClass[] ComplexTypeArray { get; set; }

        public byte[] BytesValue { get; set; }
        public int[] IntArray { get; set; }
    }
}
