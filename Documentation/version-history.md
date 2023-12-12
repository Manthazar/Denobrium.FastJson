# v1.0: 
Small performance improvements using JsonValue deserialization.
- [NEW] Can now deserialize into a HashSet<>
- [NEW] Allowing to buildup json arrays.
- [NEW] Deserialization into enumeration types is now trying to set an array of the items (instead of exception)
- [FIX] Registry did not always respect property read/ write specifiers 
- [FIX] Crash on deserializing into non-generic Array.
- [FIX] JsonPropertyInfo.Copy copied too not relevant values for Silverlight.
- [FIX] Silverlight property/ field getter for certain types resulted in NullReferenceExceptions
- [CHANGE] JsonParameter.UseGlobalTypes has been removed. 

Other fixes..
# v.93 Release Candidate   

Small performance improvement 5%-10%. 
- [Cancelled] (Cannot declare explicit cast operators on interfaces) IJsonValue.
- [New] Explicit cast operators on JsonPrimitive class 
- [New] JsonParameter is reviewed. The old version misses clear separation of what is serialization option, what is deserialization and what is common. 
- [New] Custom date format support through 'JsonDateTimeOptions.Format' format string (note that automatic conversions are supported). 
- [Fix] JsonPrimitive decimal was using integer parsing. 
- [Fix] JsonPrimitive ToChar was not throwing an exception when local value had more than one character  
- [Fix] IJsonValue implementations now throw the promised NotSupportedException instead of InvalidOperationException. 
- [Fix] BuildUp built Dictionary<,> incorrectly.
- [Change] Setting default UseExtensions to false since it is not required in most of the cases.
- [Change] JsonValue builder will become the default deserializer except for data table and data set objects. 
- [Change] SerializationPolicy is obsolete and to be replaced with MemberStrategy. 
- [Change] Visibility scope of JsonObject, JsonValue, JsonArray corrected down to internal (they are read-only objects). 
- [Change] DateTimeKind specifications (local, Utc, etc) have only an implicit effect on deserialized values, if the string values is zulu time. Otherwise, the kind is set according to the option, but the value is not modified.
- [Change] Type Extensions are now disabled by default as this is an advanced case and has a significant performance impact.  

# v0.92  
First intermediate release before API stability is guaranteed (in v1.0)   

Removing support of xmlignore attribute, it is replaced by DataMember, IgnoreDataMember
- New: JsonPrimitive custom type support. 
- New: Silverlight 5 support.
- New: A SerializationException is thrown when an attempt is done to serialize a class without (visible) properties. Previously, this was rendering to '{}' which will lead to problems on receivers side.
- New: Json.ToJsonBytes returns a byte array representing the json string bytes in the configured encoding.
- New: A date time property can be decorated with the DateTimeOptions attribute which defines the expected kind of date time (utc or not). The deserializer will automatically convert, if appropriate.
- Change: Thread static attribute has been removed from JSON class. It was causing irritations, since each thread had its own default parameters. Any change applied to the parameters needed to be re-set for each worker thread.
- Fix: Can register custom type handler for non default JSON parameters.
- Fix: DateTime deserialization was always converting to local time. Now the field can declare the JsonDateTimeOptions attribute which allows to specify the desired value.
- Fix: If last property is null or empty and SerializeNullValues is true, a final ',' was rendered anyway (ie. { i:1,} where 'j' is nullable and null).
- Increasing unit test depth on JsonRegistry and other related classes.
- Improved error reporting when duplicate data member is detected.

# v0.9 Fork from fastJSON 2.0.9 
Serialization 

- DateTime to UTC is now respecting the kind of the date time (JsonSerializer_DateTimeUtc)
- Byte array was not proper when a List of bytes was serialized (see JsonSerializer_ByteEnumeration)
- Serialization of custom types implementing IList was not considered  
- TimeSpan was wrongly serialized.  

Deserialization

- Deserialization of a number of bytes was failing 
- Wrong date time was returned when given string was in UTC and parameter was set to avoid UTC dates

JsonParameter
- Changing serialization/ deserialization values at runtime could lead to unexpected output.
- Code was changing some properties on its own behalf. 

MyPropInfo   
- CanWrite was always false for fields  
- Fill was never false and unused. Removed.  
- GenericTypes was always null except for dictionaries with the name 'Dictionary'.

Benchmark
- Fixing time measure flaw in benchmark tool.      

# History    
- 28th August 2013: Release of v1.0 (after long testing period).
- 30th January 2013: Release of v0.93
- 4th January 2013: Release of v.92
- 26th November 2012: Release of v.90
- 25th October 2012: Fork of FastJSON. Development started.
