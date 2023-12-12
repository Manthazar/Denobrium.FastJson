using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denobrium.Json.Benchmark
{
	public static partial class Benchmarks
	{
        #region [   other tests  ]

        /*
		private static void systemweb_serialize()
		{
			Console.WriteLine();
			Console.Write("msjson serialize");
			colclass c = CreateObject();
			var sws = new System.Web.Script.Serialization.JavaScriptSerializer();
			for (int pp = 0; pp < tcount; pp++)
			{
				DateTime st = DateTime.Now;
				colclass deserializedStore = null;
				string jsonText = null;

				//jsonText =sws.Serialize(c);
				//Console.WriteLine(" size = " + jsonText.Length);
				for (int i = 0; i < count; i++)
				{
					jsonText =sws.Serialize(c);
					//deserializedStore = (colclass)sws.DeserializeObject(jsonText);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

//		private static void stack_serialize()
//		{
//			Console.WriteLine();
//			Console.Write("stack serialize");
//			colclass c = CreateObject();
//			for (int pp = 0; pp < 5; pp++)
//			{
//				DateTime st = DateTime.Now;
//				string jsonText = null;
//
//				for (int i = 0; i < count; i++)
//				{
//					jsonText = ServiceStack.Text.JsonSerializer.SerializeToString(c);
//				}
//				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
//			}
//		}		

		private static void systemweb_deserialize()
//		{
//			Console.WriteLine();
//			Console.Write("fastjson deserialize");
//			colclass c = CreateObject();
//			var sws = new System.Web.Script.Serialization.JavaScriptSerializer();
//			for (int pp = 0; pp < tcount; pp++)
//			{
//				DateTime st = DateTime.Now;
//				colclass deserializedStore = null;
//				string jsonText = null;
//
//				jsonText =sws.Serialize(c);
//				//Console.WriteLine(" size = " + jsonText.Length);
//				for (int i = 0; i < count; i++)
//				{
//					deserializedStore = (colclass)sws.DeserializeObject(jsonText);
//				}
//				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
//			}
//		}

		private static void jsonnet4_deserialize()
		{
			Console.WriteLine();
			Console.Write("json.net4 deserialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c;
				colclass deserializedStore = null;
				string jsonText = null;
				c = Tests.mytests.CreateObject();
				var s = new Newtonsoft.Json.JsonSerializerSettings();
				s.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
				jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(c, Newtonsoft.Json.Formatting.Indented, s);
				for (int i = 0; i < count; i++)
				{
					deserializedStore = (colclass)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonText, typeof(colclass), s);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void jsonnet4_serialize()
		{
			Console.WriteLine();
			Console.Write("json.net4 serialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c = Tests.mytests.CreateObject();
				Newtonsoft.Json.JsonSerializerSettings s = null;
				string jsonText = null;
				s = new Newtonsoft.Json.JsonSerializerSettings();
				s.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;

				for (int i = 0; i < count; i++)
				{
					jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(c, Newtonsoft.Json.Formatting.Indented, s);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void stack_deserialize()
		{
			Console.WriteLine();
			Console.Write("stack deserialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c;
				colclass deserializedStore = null;
				string jsonText = null;
				c = Tests.mytests.CreateObject();
				jsonText = ServiceStack.Text.JsonSerializer.SerializeToString(c);
				for (int i = 0; i < count; i++)
				{
					deserializedStore = ServiceStack.Text.JsonSerializer.DeserializeFromString<colclass>(jsonText);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void jsonnet_deserialize()
		{
			Console.WriteLine();
			Console.Write("json.net deserialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c;
				colclass deserializedStore = null;
				string jsonText = null;
				c = Tests.mytests.CreateObject();
				var s = new json.net.JsonSerializerSettings();
				s.TypeNameHandling = json.net.TypeNameHandling.All;
				jsonText = json.net.JsonConvert.SerializeObject(c, json.net.Formatting.Indented, s);
				for (int i = 0; i < count; i++)
				{
					deserializedStore = (colclass)json.net.JsonConvert.DeserializeObject(jsonText, typeof(colclass), s);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void jsonnet_serialize()
		{
			Console.WriteLine();
			Console.Write("json.net serialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c = Tests.mytests.CreateObject();
				json.net.JsonSerializerSettings s = null;
				string jsonText = null;
				s = new json.net.JsonSerializerSettings();
				s.TypeNameHandling = json.net.TypeNameHandling.All;

				for (int i = 0; i < count; i++)
				{
					jsonText = json.net.JsonConvert.SerializeObject(c, json.net.Formatting.Indented, s);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void litjson_deserialize()
		{
			Console.WriteLine();
			Console.Write("litjson deserialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c;
				colclass deserializedStore = null;
				string jsonText = null;
				c = Tests.mytests.CreateObject();
				jsonText = BizFX.Common.JSON.JsonMapper.ToJson(c);
				for (int i = 0; i < count; i++)
				{
					deserializedStore = (colclass)BizFX.Common.JSON.JsonMapper.ToObject(jsonText);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void litjson_serialize()
		{
			Console.WriteLine();
			Console.Write("litjson serialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c;
				string jsonText = null;
				c = Tests.mytests.CreateObject();
				for (int i = 0; i < count; i++)
				{
					jsonText = BizFX.Common.JSON.JsonMapper.ToJson(c);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		 */

        #endregion
	}
}
