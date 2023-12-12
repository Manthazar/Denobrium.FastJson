using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace consoletest.DataObjects
{
    [DataContract(Name = "FastJsonBenchmarkClass")]
    public class BenchmarkDataClass
    {
        public BenchmarkDataClass()
        {
            items = new List<BaseClass>();
            date = DateTime.Now;
            multilineString = @"
            AJKLjaskljLA
       ahjksjkAHJKS سلام فارسی
       AJKHSKJhaksjhAHSJKa
       AJKSHajkhsjkHKSJKash
       ASJKhasjkKASJKahsjk
            ";
            isNew = true;
            booleanValue = true;
            ordinaryDouble = 0.001;
            gender = Gender.Female;
            intarray = new int[5] { 1, 2, 3, 4, 5 };
        }

        public bool booleanValue { get; set; }
        public DateTime date { get; set; }
        public string multilineString { get; set; }
        public List<BaseClass> items { get; set; }
        public decimal ordinaryDecimal { get; set; }
        public double ordinaryDouble { get; set; }
        public bool isNew { get; set; }
        public string laststring { get; set; }
        public Gender gender { get; set; }

        public DataSet dataset { get; set; }
        public Dictionary<string, BaseClass> stringDictionary { get; set; }
        public Dictionary<BaseClass, BaseClass> objectDictionary { get; set; }
        public Dictionary<int, BaseClass> intDictionary { get; set; }
        public Guid? nullableGuid { get; set; }
        public decimal? nullableDecimal { get; set; }
        public double? nullableDouble { get; set; }
        public Hashtable hash { get; set; }
        public BaseClass[] arrayType { get; set; }
        public byte[] bytes { get; set; }
        public int[] intarray { get; set; }
    }
}
