using Denobrium.Json.Benchmark.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Denobrium.Json.Benchmark
{
    internal static class BenchmarkDataFactory
    {
        internal static BenchmarkDataClass CreateDefaultDataClass()
        {
            var result = new BenchmarkDataClass()
            {

                ItemList = new List<BaseClass>(),
                DateValue = DateTime.Now,
                MultilineString = @"
            AJKLjaskljLA
       ahjksjkAHJKS سلام فارسی
       AJKHSKJhaksjhAHSJKa
       AJKSHajkhsjkHKSJKash
       ASJKhasjkKASJKahsjk
            ",
                BooleanValue = true,
                DoubleValue = 0.001,
                EnumValue = Gender.Female,
                IntArray = new int[5] { 1, 2, 3, 4, 5 },

                DecimalValue = 3,
            };

            if (BenchmarkOptions.Current.IncludeComplexTypes)
            {
                result.NullableGuid = Guid.NewGuid();
                result.HashTableValue = new Hashtable();
                result.BytesValue = new byte[1024];
                result.StringDictionary = new Dictionary<string, BaseClass>();
                result.ObjectDictionary = new Dictionary<BaseClass, BaseClass>();
                result.IntDictionary = new Dictionary<int, BaseClass>();
                result.NullableDouble = 100.003;

                if (BenchmarkOptions.Current.IncludeDataSet)
                {
                    result.DataSet = CreateDataset();
                }

                result.NullableDecimal = 3.14M;

                result.HashTableValue.Add(new Class1("0", "hello", Guid.NewGuid()), new Class2("1", "code", "desc"));
                result.HashTableValue.Add(new Class2("0", "hello", "pppp"), new Class1("1", "code", Guid.NewGuid()));

                result.StringDictionary.Add("name1", new Class2("1", "code", "desc"));
                result.StringDictionary.Add("name2", new Class1("1", "code", Guid.NewGuid()));

                result.IntDictionary.Add(1, new Class2("1", "code", "desc"));
                result.IntDictionary.Add(2, new Class1("1", "code", Guid.NewGuid()));

                result.ObjectDictionary.Add(new Class1("0", "hello", Guid.NewGuid()), new Class2("1", "code", "desc"));
                result.ObjectDictionary.Add(new Class2("0", "hello", "pppp"), new Class1("1", "code", Guid.NewGuid()));

                result.ComplexTypeArray = new BaseClass[2];
                result.ComplexTypeArray[0] = new Class1();
                result.ComplexTypeArray[1] = new Class2();
            }


            result.ItemList.Add(new Class1("1", "1", Guid.NewGuid()));
            result.ItemList.Add(new Class2("2", "2", "desc1"));
            result.ItemList.Add(new Class1("3", "3", Guid.NewGuid()));
            result.ItemList.Add(new Class2("4", "4", "desc2"));

            result.SimpleString = "" + DateTime.Now;

            return result;
        }

        /// <summary>
        /// Creates an example data set.
        /// </summary>
        /// <returns></returns>
        internal static DataSet CreateDataset()
        {
            DataSet ds = new DataSet();

            for (int j = 1; j < 3; j++)
            {
                DataTable dt = new DataTable();
                dt.TableName = "Table" + j;
                dt.Columns.Add("col1", typeof(int));
                dt.Columns.Add("col2", typeof(string));
                dt.Columns.Add("col3", typeof(Guid));
                dt.Columns.Add("col4", typeof(string));
                dt.Columns.Add("col5", typeof(bool));
                dt.Columns.Add("col6", typeof(string));
                dt.Columns.Add("col7", typeof(string));
                ds.Tables.Add(dt);

                Random rrr = new Random();

                for (int i = 0; i < 100; i++)
                {
                    DataRow dr = dt.NewRow();

                    dr[0] = rrr.Next(int.MaxValue);
                    dr[1] = "" + rrr.Next(int.MaxValue);
                    dr[2] = Guid.NewGuid();
                    dr[3] = "" + rrr.Next(int.MaxValue);
                    dr[4] = true;
                    dr[5] = "" + rrr.Next(int.MaxValue);
                    dr[6] = "" + rrr.Next(int.MaxValue);

                    dt.Rows.Add(dr);
                }
            }

            return ds;
        }
    }
}
