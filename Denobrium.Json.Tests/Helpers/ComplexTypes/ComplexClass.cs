using Denobrium.Json.Tests.Helpers.ComplexTypes;
using System.Collections;
using System.Data;

namespace Denobrium.Json.Tests.Helpers
{
    public class ComplexClass
    {
        public ComplexClass()
        {
        }

        public bool BooleanValue { get; set; }
        public DateTime Date { get; set; }
        public string? MultilineString { get; set; }
        public List<BaseClass>? Items { get; set; }
        public decimal? OrdinaryDecimal { get; set; }
        public double? OrdinaryDouble { get; set; }
        public string? Laststring { get; set; }
        public Gender Gender { get; set; }

        public DataSet? Dataset { get; set; }
        public Dictionary<string, BaseClass>? StringDictionary { get; set; }
        public Dictionary<BaseClass, BaseClass>? ObjectDictionary { get; set; }
        public Dictionary<int, BaseClass>? IntDictionary { get; set; }
        public Guid? NullableGuid { get; set; }
        public decimal? NullableDecimal { get; set; }
        public double? NullableDouble { get; set; }
        public Hashtable? Hash { get; set; }
        public BaseClass[]? ArrayType { get; set; }
        public byte[]? Bytes { get; set; }
        public int[]? IntArray { get; set; }

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
                BooleanValue = true,
                OrdinaryDecimal = 3,
                Items = new List<BaseClass>(),
                Date = DateTime.UtcNow,
                MultilineString = @"
            AJKLjaskljLA
       ahjksjkAHJKS سلام فارسی
       AJKHSKJhaksjhAHSJKa
       AJKSHajkhsjkHKSJKash
       ASJKhasjkKASJKahsjk
            ",
                OrdinaryDouble = 0.001,
                Gender = Gender.Female,
                IntArray = new int[5] { 1, 2, 3, 4, 5 },
            };

            c.Items.Add(new SubClass1("1", "1", Guid.NewGuid()));
            c.Items.Add(new SubClass2("2", "2", "desc1"));
            c.Items.Add(new SubClass1("3", "3", Guid.NewGuid()));
            c.Items.Add(new SubClass2("4", "4", "desc2"));

            c.Laststring = "" + DateTime.Now;

            if (exotic)
            {
                c.NullableGuid = Guid.NewGuid();
                c.Hash = new Hashtable();
                c.Bytes = new byte[1024];
                c.StringDictionary = new Dictionary<string, BaseClass>();
                c.ObjectDictionary = new Dictionary<BaseClass, BaseClass>();
                c.IntDictionary = new Dictionary<int, BaseClass>();
                c.NullableDouble = 100.003;

                c.NullableDecimal = 3.14M;

                c.Hash.Add(new SubClass1("0", "hello", Guid.NewGuid()), new SubClass2("1", "code", "desc"));
                c.Hash.Add(new SubClass2("0", "hello", "pppp"), new SubClass1("1", "code", Guid.NewGuid()));

                c.StringDictionary.Add("name1", new SubClass2("1", "code", "desc"));
                c.StringDictionary.Add("name2", new SubClass1("1", "code", Guid.NewGuid()));

                c.IntDictionary.Add(1, new SubClass2("1", "code", "desc"));
                c.IntDictionary.Add(2, new SubClass1("1", "code", Guid.NewGuid()));

                c.ObjectDictionary.Add(new SubClass1("0", "hello", Guid.NewGuid()), new SubClass2("1", "code", "desc"));
                c.ObjectDictionary.Add(new SubClass2("0", "hello", "pppp"), new SubClass1("1", "code", Guid.NewGuid()));

                c.ArrayType = new BaseClass[2];
                c.ArrayType[0] = new SubClass1();
                c.ArrayType[1] = new SubClass2();
            }

            if (includeDataSet)
            {
                c.Dataset = CreateDataset();
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
