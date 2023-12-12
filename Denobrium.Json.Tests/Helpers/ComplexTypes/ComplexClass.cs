using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Denobrium.Json.Tests.Helpers.ComplexTypes;

namespace Denobrium.Json.Tests.Helpers
{
    public class ComplexClass
    {
        public ComplexClass()
        {
        }

        public bool booleanValue { get; set; }
        public DateTime date { get; set; }
        public string multilineString { get; set; }
        public List<BaseClass> items { get; set; }
        public decimal ordinaryDecimal { get; set; }
        public double ordinaryDouble { get; set; }
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

        /// <summary>
        /// Returns a new instance of the object populated with the standard fields plus according to given criteria.
        /// </summary>
        /// <param name="exotic"></param>
        /// <param name="includeDataSet"></param>
        /// <returns></returns>
        public static ComplexClass CreateObject(bool exotic, bool includeDataSet)
        {
            var c = new ComplexClass()
            {
                booleanValue = true,
                ordinaryDecimal = 3,
                items = new List<BaseClass>(),
                date = DateTime.UtcNow,
                multilineString = @"
            AJKLjaskljLA
       ahjksjkAHJKS سلام فارسی
       AJKHSKJhaksjhAHSJKa
       AJKSHajkhsjkHKSJKash
       ASJKhasjkKASJKahsjk
            ",
                ordinaryDouble = 0.001,
                gender = Gender.Female,
                intarray = new int[5] { 1, 2, 3, 4, 5 },
            };

            c.items.Add(new SubClass1("1", "1", Guid.NewGuid()));
            c.items.Add(new SubClass2("2", "2", "desc1"));
            c.items.Add(new SubClass1("3", "3", Guid.NewGuid()));
            c.items.Add(new SubClass2("4", "4", "desc2"));

            c.laststring = "" + DateTime.Now;

            if (exotic)
            {
                c.nullableGuid = Guid.NewGuid();
                c.hash = new Hashtable();
                c.bytes = new byte[1024];
                c.stringDictionary = new Dictionary<string, BaseClass>();
                c.objectDictionary = new Dictionary<BaseClass, BaseClass>();
                c.intDictionary = new Dictionary<int, BaseClass>();
                c.nullableDouble = 100.003;

                c.nullableDecimal = 3.14M;

                c.hash.Add(new SubClass1("0", "hello", Guid.NewGuid()), new SubClass2("1", "code", "desc"));
                c.hash.Add(new SubClass2("0", "hello", "pppp"), new SubClass1("1", "code", Guid.NewGuid()));

                c.stringDictionary.Add("name1", new SubClass2("1", "code", "desc"));
                c.stringDictionary.Add("name2", new SubClass1("1", "code", Guid.NewGuid()));

                c.intDictionary.Add(1, new SubClass2("1", "code", "desc"));
                c.intDictionary.Add(2, new SubClass1("1", "code", Guid.NewGuid()));

                c.objectDictionary.Add(new SubClass1("0", "hello", Guid.NewGuid()), new SubClass2("1", "code", "desc"));
                c.objectDictionary.Add(new SubClass2("0", "hello", "pppp"), new SubClass1("1", "code", Guid.NewGuid()));

                c.arrayType = new BaseClass[2];
                c.arrayType[0] = new SubClass1();
                c.arrayType[1] = new SubClass2();
            }

            if (includeDataSet)
            {
                c.dataset = CreateDataset();
            }

            return c;
        }

        private static DataSet CreateDataset()
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
