Upcoming Changes 1.0
================================================
- Small performance improvements using JsonValue deserialization.
- [NEW] Can now deserialize into a HashSet<>
- [NEW] Allowing to buildup json arrays.
- [FIX] Registry did not always respect property read/ write specifiers
- [FIX] Crash on deserializing into non-generic Array.
- [FIX] JsonPropertyInfo.Copy copied too not relevant values for Silverlight.
- Other fixes...


================================================
Upcoming Changes 0.95 RC2:
- [NEW] Max serialization depth is now part of parameter.
- [NEW] JsonPrimitive receives a static Convert method which allows to convert simple string representations manually.
- [NEW] Deserialization into enumeration types is now trying to set an array of the items (instead of exception).
- [NEW] JsonPropertyInfo has now a custom display value property which can be adjusted to personal preferences.
- [NEW] JsonPropertyInfo has now a reference to the type which has declared it (DeclaringType).
- [FIX] Silverlight property/ field getter for certain types resulted in NullReferenceExceptions.
- [FIX] GetSerializationMembers returned an enumerable which recreated the item with each iteration.
- [CHANGE] UseGlobalTypes has been removed.

- [PEND] BuildUp can build now primitive values from given dictionaries or NamedValueCollections.
- [PEND] DeepCopy uses JsonValue mechanism.
- [SKIP] ParallelSerializater is added


================================================
Changes 1:
- Small performance improvement 5%-10%. 
- [CANCELLED] (Cannot declare explicit cast operators on interfaces) IJsonValue AsXXXX is obsolete.
- [NEW] Explicit cast operators on JsonPrimitive class
- [NEW] JsonParameter is reviewed. The old version misses clear separation of what is serialization option, what is deserialization and what it common.
- [NEW] Custom date format support through 'JsonDateTimeOptions.Format' format string (note that automatic conversion are not supported).
- [FIX] JsonPrimitive decimal was using integer parsing.
- [FIX] JsonPrimitive ToChar was not throwing an exception when local value had more than one character.
- [FIX] IJsonValue implementations now throw the promissed NotSupportedException instead of InvalidOperationException.
- [FIX] BuildUp built Dictionary<,> incorrectly.

- [CHANGE] Setting default UseExtensions to false since it is not required in most of the cases.
- [CHANGE] JsonValue builder will become the default deserializer except for dataTable and dataset objects.
- [CHANGE] SerializationPolicy is obsolete and to be replaced with MemberStrategy.
- [CHANGE] Visibility scope of JsonObject, JsonValue, JsonArray corrected down to internal (they are read-only objects).
- [CHANGE] DateTimeKind specifications (local, Utc, etc) have only an implicit effect on deserialized values, if the string values is zulu time. Otherwise, the kind is set according to the option, but the value is not modified.
- [CHANGE] Type Extensions are now disabled by default as this is an advanced case and has a significant performance impact.

Known Limitations and Issues
- HashSet in deserialization is not supported (lack of interface to populate the hash set collection).
- DataMember with Name = "$type" and comparable is still allowed and leads potentially to wrong behavior.
- InvalidProgramException when property indexer is defined on class to serialize

Points of interest
- Only public classes should participate on serialization. Serialization may or may not work, depending on security settings and .net version
- All properties should be public get/set for optimal performance. Internal members are also considered, but performance is significally lower.
- Deserialization into non-public types fails with TypeAccessException.



