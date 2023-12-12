using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace Denobrium.Json.Benchmark.DataObjects
{
    [DataContract(Name = "BenchmarkDataClass")]
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class BenchmarkDataClass
    {
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

        #region Collection/ Enumeration Types

        public Hashtable HashTableValue { get; set; }
        public Knife[] KnifeArray { get; set; }

        public byte[] BytesValue { get; set; }
        public int[] IntArray { get; set; }

        #endregion

        #region Complex types

        /// <summary>
        /// A list which contains actual implementations of baseclass.
        /// </summary>
        public List<Spoon> SpoonList { get; set; }

        public ProductTypes EnumValue { get; set; }

        public DataSet DataSet { get; set; }

        /// <summary>
        /// A map of spoons.
        /// </summary>
        public Dictionary<string, Spoon> SpoonMap { get; set; }

        #endregion

        #region Polymorphism

        /// <summary>
        /// A map of products which can be forks, knifes or spoons.
        /// </summary>
        public Dictionary<string, Product> ProductMap { get; set; }

        /// <summary>
        /// A list of products which can be forks, knifes or spoons.
        /// </summary>
        public List<Product> ProductList { get; set; }

        #endregion
    }
}
