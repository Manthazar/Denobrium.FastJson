using consoletest.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace consoletest
{
    internal static class BenchmarkDataFactory
    {
        internal static BenchmarkDataClass CreateDefaultDataClass()
        {
            var result = new BenchmarkDataClass();

            result.booleanValue = true;
            result.ordinaryDecimal = 3;

            if (BenchmarkOptions.Current.IncludeExotic)
            {
                result.nullableGuid = Guid.NewGuid();
                result.hash = new Hashtable();
                result.bytes = new byte[1024];
                result.stringDictionary = new Dictionary<string, BaseClass>();
                result.objectDictionary = new Dictionary<BaseClass, BaseClass>();
                result.intDictionary = new Dictionary<int, BaseClass>();
                result.nullableDouble = 100.003;

                if (BenchmarkOptions.Current.IncludeDataSet)
                {
                    result.dataset = CreateDataset();
                }

                result.nullableDecimal = 3.14M;

                result.hash.Add(new Class1("0", "hello", Guid.NewGuid()), new Class2("1", "code", "desc"));
                result.hash.Add(new Class2("0", "hello", "pppp"), new Class1("1", "code", Guid.NewGuid()));

                result.stringDictionary.Add("name1", new Class2("1", "code", "desc"));
                result.stringDictionary.Add("name2", new Class1("1", "code", Guid.NewGuid()));

                result.intDictionary.Add(1, new Class2("1", "code", "desc"));
                result.intDictionary.Add(2, new Class1("1", "code", Guid.NewGuid()));

                result.objectDictionary.Add(new Class1("0", "hello", Guid.NewGuid()), new Class2("1", "code", "desc"));
                result.objectDictionary.Add(new Class2("0", "hello", "pppp"), new Class1("1", "code", Guid.NewGuid()));

                result.arrayType = new BaseClass[2];
                result.arrayType[0] = new Class1();
                result.arrayType[1] = new Class2();
            }


            result.items.Add(new Class1("1", "1", Guid.NewGuid()));
            result.items.Add(new Class2("2", "2", "desc1"));
            result.items.Add(new Class1("3", "3", Guid.NewGuid()));
            result.items.Add(new Class2("4", "4", "desc2"));

            result.laststring = "" + DateTime.Now;

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
