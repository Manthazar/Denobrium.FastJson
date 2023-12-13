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
                EnumValue = ProductTypes.Fork,
                IntArray = new int[5] { 1, 2, 3, 4, 5 },

                DecimalValue = 3,
                SimpleString = "" + DateTime.Now,
                NullableDouble = 100.003,
                NullableDecimal = 3.14M,
                NullableGuid = Guid.NewGuid()
            };

            if (BenchmarkOptions.Current.IncludeComplexTypes)
            {
                result.HashTableValue = new Hashtable();
                result.BytesValue = new byte[1024];
                result.SpoonMap = new Dictionary<string, Spoon>();
                result.ProductMap = new Dictionary<string, Product>();
                result.ProductList = new List<Product>();

                if (BenchmarkOptions.Current.IncludeDataSet)
                {
                    result.DataSet = CreateDataset();
                }

                result.SpoonList = new List<Spoon>() {
                        new Spoon ("Silver", "S1", 50),
                        new Spoon ("Gold", "G1", 30),
                        new Spoon("Platinum", "P1",40)
                };

                //result.HashTableValue.Add(new Knife("0", "hello", 5), new Spoon("1", "code", 54));
                //result.HashTableValue.Add(new Spoon("0", "hello", 40), new Knife("1", "code", 6));

                result.SpoonMap.Add("name1", new Spoon("1", "code1", 50));
                result.SpoonMap.Add("name2", new Spoon("2", "code2", 61));

                result.ProductMap.Add("P1", new Knife("0", "knife0", 7));
                result.ProductMap.Add("P2", new Spoon("1", "spoon1", 46));
                result.ProductMap.Add("P3", new Spoon("2", "spoon2", 40));
                result.ProductMap.Add("P4", new Knife("3", "knife3", 9));

                result.KnifeArray = new Knife[2];
                result.KnifeArray[0] = new Knife() { Code = "C1", LevelOfSharpness = 10, Name = "K1" };
                result.KnifeArray[0] = new Knife() { Code = "C2", LevelOfSharpness = 10, Name = "K2" };
                result.KnifeArray[0] = new Knife() { Code = "C3", LevelOfSharpness = 10, Name = "K3" };

                result.ProductList.Add(new Knife("1", "1", 9));
                result.ProductList.Add(new Spoon("2", "2", 34));
                result.ProductList.Add(new Knife("3", "3", 8));
                result.ProductList.Add(new Spoon("4", "4", 54));
            }

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
