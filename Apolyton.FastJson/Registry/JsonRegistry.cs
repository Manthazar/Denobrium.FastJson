using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Apolyton.FastJson.Serialization;

#if DESKTOP
using System.Data;
#endif


namespace Apolyton.FastJson.Registry
{
    /// <summary>
    /// Represents the registry which holds property information being relevant for serialization/ deserialization. 
    /// </summary>
    /// <remarks>
    /// http://www.codeproject.com/Articles/159450/fastJSON
    /// The version over there (2.0.9)could not be taken directly, as its serializer is taking all public properties, disregarding any attribute policy. This is
    /// not good for our case, as we want to return (portions of) data objects as well.
    /// </remarks>
    public sealed class JsonRegistry
    {
        private static readonly Type[] _genericSetterArguments = new Type[2] { typeof(object), typeof(object) };
        private static readonly Type[] _genericGetterArguments = new Type[1] { typeof(object) };

        private readonly JsonParameters _owner;

        private readonly RegistrySection<Type, JsonPropertyInfo> _typeInfo;
        private readonly RegistrySection<Type, CreateObjectHandler> _constructorCache;
        private readonly RegistrySection<Type, IEnumerable<GetterDescriptor>> _gettersCache;
        private readonly RegistrySection<string, RegistrySection<string, JsonPropertyInfo>> _propertycache;

        internal JsonTypeDescriptor TypeDescriptor;
        internal readonly RegistrySection<Type, SerializationHandler> CustomSerializers;
        internal readonly RegistrySection<Type, DeserializationHandler> CustomDeserializer;

        /// <summary>
        /// Creates an instance of the registry
        /// </summary>
        internal JsonRegistry(JsonParameters owner)
        {
            _owner = owner;

            TypeDescriptor = new JsonTypeDescriptor();
            CustomSerializers = new RegistrySection<Type, SerializationHandler>();
            CustomDeserializer = new RegistrySection<Type, DeserializationHandler>();

            _typeInfo = new RegistrySection<Type, JsonPropertyInfo>();
            _constructorCache = new RegistrySection<Type, CreateObjectHandler>();
            _gettersCache = new RegistrySection<Type, IEnumerable<GetterDescriptor>>();
            _propertycache = new RegistrySection<string, RegistrySection<string, JsonPropertyInfo>>();
        }

        /// <summary>
        /// Resets all cached values.
        /// </summary>
        /// <remarks>
        /// Primarily used for unit tests where overlapping cached values 
        /// </remarks>
        internal void Reset()
        {
            TypeDescriptor.Reset();
            CustomSerializers.Clear();
            CustomDeserializer.Clear();

            _typeInfo.Clear();
            _constructorCache.Clear();
            _gettersCache.Clear();
            _propertycache.Clear();
        }

        #region Custom Type Support

        /// <summary>
        /// Registers a custom type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serializer"></param>
        /// <param name="deserializer"></param>
        internal void RegisterCustomType(Type type, SerializationHandler serializer, DeserializationHandler deserializer)
        {
            if (Deserialization.IsTypeInternallyManaged(type))
            {
                throw new ArgumentException("Cannot register a custom serializer for types which are internally managed.");
            }

            if (serializer != null)
            {
                CustomSerializers.Add(type, serializer);
            }

            if (deserializer != null)
            {
                CustomDeserializer.Add(type, deserializer);
            }

            // reset property cache
            _propertycache.Clear();
        }

        /// <summary>
        /// Returns true, if the custom type is registered.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal bool IsCustomTypeRegistered(Type t)
        {
            SerializationHandler s;

            return CustomSerializers.TryGetValue(t, out s);
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Returns the object for the given type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal object CreateInstanceFast(Type t)
        {
            try
            {
                CreateObjectHandler constructorHandler = null;

                if (_constructorCache.TryGetValue(t, out constructorHandler))
                {
                    return constructorHandler();
                }
                else
                {
                    if (t.IsAbstract)
                    {
                        if (!t.IsGenericType && t == typeof(IEnumerable))
                        {
                            return new List<Object>();
                        }
                        else
                        {
                            throw new SerializationException("Cannot create instances of abstract types " + t);
                        }
                    }
                    else if (t.IsArray)
                    {
                        throw new InvalidOperationException("Cannot create constructors for arrays. Size must be known in advance.");
                    }

                    constructorHandler = CreateDefaultConstructorMethod(t);
                    _constructorCache.Add(t, constructorHandler);

                    return constructorHandler();
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Failed to fast create instance for type '{0}' from assembly '{1}'. Does it have a default constructor?",
                    t.FullName, t.AssemblyQualifiedName), ex);
            }
        }

        private static CreateObjectHandler CreateDefaultConstructorMethod(Type t)
        {
            if (t.IsClass)
            {
                DynamicMethod dynMethod = new DynamicMethod("_", t, null);
                ILGenerator ilGen = dynMethod.GetILGenerator();
                ilGen.Emit(OpCodes.Newobj, t.GetConstructor(Type.EmptyTypes));
                ilGen.Emit(OpCodes.Ret);

                return (CreateObjectHandler)dynMethod.CreateDelegate(typeof(CreateObjectHandler));
            }
            else // structs
            {
#if DESKTOP
                DynamicMethod dynMethod = new DynamicMethod("_",
                    MethodAttributes.Public | MethodAttributes.Static,
                    CallingConventions.Standard,
                    typeof(object),
                    null,
                    t, false);
#elif SILVERLIGHT
                DynamicMethod dynMethod = new DynamicMethod("_", typeof(Object), null);
#endif

                ILGenerator ilGen = dynMethod.GetILGenerator();

                var lv = ilGen.DeclareLocal(t);
                ilGen.Emit(OpCodes.Ldloca_S, lv);
                ilGen.Emit(OpCodes.Initobj, t);
                ilGen.Emit(OpCodes.Ldloc_0);
                ilGen.Emit(OpCodes.Box, t);
                ilGen.Emit(OpCodes.Ret);

                return (CreateObjectHandler)dynMethod.CreateDelegate(typeof(CreateObjectHandler));
            }
        }

        private JsonPropertyInfo CreateJsonPropertyInfo(Type t)
        {
            JsonPropertyInfo info = new JsonPropertyInfo();
            info.CanWrite = true;
            info.PropertyOrFieldType = t;
            info.IsDictionary = typeof(IDictionary).IsAssignableFrom(t);
            info.IsValueType = t.IsValueType;
            info.IsGenericType = t.IsGenericType;
            info.IsArray = t.IsArray;

            if (info.IsArray)
            {
                info.GenericItemType = t.GetElementType();
            }
            else if (info.IsGenericType)
            {
                info.IsHashSet = t.GetGenericTypeDefinition() == typeof(HashSet<>);
                info.GenericTypes = t.GetGenericArguments();
                info.GenericItemType = info.GenericTypes[0];
            }

            info.IsByteArray = t == typeof(byte[]);
            info.IsGuid = (t == typeof(Guid) || t == typeof(Guid?));

#if DESKTOP
            info.IsHashtable = t == typeof(Hashtable);
            info.IsDataSet = t == typeof(DataSet);
            info.IsDataTable = t == typeof(DataTable);
#endif
            info.NullableType = GetNullableType(t);
            info.IsEnum = t.IsEnum;
            info.IsDateTime = t == typeof(DateTime) || t == typeof(DateTime?);
            info.IsInt = t == typeof(int) || t == typeof(int?);
            info.IsLong = t == typeof(long) || t == typeof(long?);
            info.IsString = t == typeof(string);
            info.IsBool = t == typeof(bool) || t == typeof(bool?);
            info.IsClass = t.IsClass;

            if (info.IsDictionary && info.GenericTypes != null && info.GenericTypes.Length > 0 && info.GenericTypes[0] == typeof(string))
            {
                info.IsStringDictionary = true;
            }

            if (t.IsValueType)
            {
                info.DefaultValue = Activator.CreateInstance(t);
            }

            if (info.IsClass && !info.IsString && !info.PropertyOrFieldType.IsAbstract && !info.IsArray)
            {
                info.Constructor = CreateDefaultConstructorMethod(t);
                _constructorCache.Add(t, info.Constructor);
            }

            if (IsCustomTypeRegistered(t))
            {
                info.IsCustomType = true;
            }
            return info;
        }

        private Type GetNullableType(Type targetType)
        {
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                return targetType.GetGenericArguments()[0];
            }

            return targetType;
        }

        /// <summary>
        /// Creates a generic setter for the given field.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static GenericSetterHandler CreateSetField(Type type, FieldInfo fieldInfo)
        {
#if DESKTOP
            DynamicMethod dynamicSet = new DynamicMethod("_", typeof(object), _genericSetterArguments, type, true);
#elif SILVERLIGHT
            DynamicMethod dynamicSet = new DynamicMethod("_", typeof(object), _genericSetterArguments);
#endif
            ILGenerator il = dynamicSet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
                if (fieldInfo.FieldType.IsClass)
                    il.Emit(OpCodes.Castclass, fieldInfo.FieldType);
                else
                    il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);
            }
            return (GenericSetterHandler)dynamicSet.CreateDelegate(typeof(GenericSetterHandler));
        }

        /// <summary>
        /// Creates a generic setter for the given property.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static GenericSetterHandler CreateSetMethod(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo setMethod = propertyInfo.GetSetMethod();

            if (setMethod == null)
            {
                return null;
            }

            DynamicMethod setter = new DynamicMethod("_", typeof(object), _genericSetterArguments);
            ILGenerator il = setter.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);

                if (propertyInfo.PropertyType.IsClass)
                {
                    il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                }
                else
                {
                    il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                }

                il.EmitCall(OpCodes.Call, setMethod, null);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                il.Emit(OpCodes.Ldarg_1);

                if (propertyInfo.PropertyType.IsClass)
                {
                    il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                }
                else
                {
                    il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                }

                il.EmitCall(OpCodes.Callvirt, setMethod, null);
                il.Emit(OpCodes.Ldarg_0);
            }

            il.Emit(OpCodes.Ret);

            return (GenericSetterHandler)setter.CreateDelegate(typeof(GenericSetterHandler));
        }

        /// <summary>
        /// Creates a generic getter for the given field.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private static GenericGetterHandler CreateGetField(Type type, FieldInfo fieldInfo)
        {
#if DESKTOP
            DynamicMethod dynamicGet = new DynamicMethod("_", typeof(object), _genericGetterArguments, type, true);
#elif SILVERLIGHT
            DynamicMethod dynamicGet = new DynamicMethod("_", typeof(object), _genericGetterArguments);
#endif
            ILGenerator il = dynamicGet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldfld, fieldInfo);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldInfo);
            }

            if (fieldInfo.FieldType.IsValueType)
            {
                il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetterHandler)dynamicGet.CreateDelegate(typeof(GenericGetterHandler));
        }

        /// <summary>
        /// Creates a generic getter for the given property.
        /// </summary>
        /// <param name="owningType"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static GenericGetterHandler CreateGetMethod(Type owningType, PropertyInfo propertyInfo)
        {
            MethodInfo getMethod = propertyInfo.GetGetMethod();

            if (getMethod == null)
            {
                // Getter is not public
                return null;
            }

#if DESKTOP
            DynamicMethod getter = new DynamicMethod("_", typeof(object), _genericGetterArguments, owningType);
#elif SILVERLIGHT
            if (propertyInfo.GetSetMethod() == null || owningType.IsNotPublic)
            {
                // In Silverlight the rules are a bit odd. If the setter is not public or the owning type, our nice generic 
                // getter will fail due to a type access exception.

                return new GenericGetterHandler(owmer => getMethod.Invoke(owmer, null));
            }

            DynamicMethod getter = new DynamicMethod("_", typeof(object), _genericGetterArguments);
#endif

            ILGenerator il = getter.GetILGenerator();

            if (!owningType.IsClass) // structs
            {
                var lv = il.DeclareLocal(owningType);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, owningType);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.EmitCall(OpCodes.Call, getMethod, null);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                il.EmitCall(OpCodes.Callvirt, getMethod, null);
            }

            if (propertyInfo.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetterHandler)getter.CreateDelegate(typeof(GenericGetterHandler));
        }

        #endregion

        /// <summary>
        /// Returns the getters for the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal IEnumerable<GetterDescriptor> GetGetters(Type type)
        {
            IEnumerable<GetterDescriptor> result = null;
            if (_gettersCache.TryGetValue(type, out result))
            {
                return result;
            }

            var propertiesAndFields = GetPropertiesAndFields(type, null);

            List<GetterDescriptor> getters = new List<GetterDescriptor>();

            foreach (JsonPropertyInfo property in propertiesAndFields.Values)
            {
                if (property.Getter == null) { continue; } // Can be for non public getter.

                GetterDescriptor gg = new GetterDescriptor();
                gg.Name = property.JsonFieldName;
                gg.Getter = property.Getter;
                gg.PropertyOrFieldType = property.PropertyOrFieldType;
                gg.DefaultValue = property.DefaultValue;

                getters.Add(gg);
            }

            _gettersCache.Add(type, getters);

            return getters;
        }

        /// <summary>
        /// Returns the type information (from cache).
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal JsonPropertyInfo GetTypeInfo(Type targetType)
        {
            JsonPropertyInfo result;

            if (!_typeInfo.TryGetValue(targetType, out result))
            {
                result = CreateJsonPropertyInfo(targetType);

                _typeInfo.Add(targetType, result);
            }

            return result;
        }

        /// <summary>
        /// Returns the properties for the given types which participate in serialization.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typename"></param>
        /// <returns></returns>
        internal RegistrySection<string, JsonPropertyInfo> GetPropertiesAndFields(Type type, string typename)
        {
            RegistrySection<string, JsonPropertyInfo> propertyDescription = null;

            if (typename == null) { typename = type.FullName; }

            if (_propertycache.TryGetValue(typename, out propertyDescription))
            {
                return propertyDescription;
            }
            else
            {
                propertyDescription = new RegistrySection<string, JsonPropertyInfo>();

                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo property in properties)
                {
                    if (!IsSerializationMember(property)) { continue; }
                    if (!property.CanWrite && !_owner.Serialization.IncludeReadOnly) { continue; }

                    String jsonFieldName = GetJsonFieldName(property);
                    JsonPropertyInfo d = GetTypeInfo(property.PropertyType).Clone();
                    d.CanWrite = property.CanWrite;
                    d.JsonFieldName = jsonFieldName;
                    d.PropertyOrFieldName = property.Name;
                    d.DeclaringType = property.DeclaringType;

                    if (property.CanRead) { d.Getter = JsonRegistry.CreateGetMethod(type, property); }
                    else { d.Getter = null; }

                    if (property.CanWrite) { d.Setter = JsonRegistry.CreateSetMethod(type, property); }
                    else { d.Setter = null; }

                    if (d.IsDateTime && Attribute.IsDefined(property, typeof(JsonDateTimeOptionsAttribute)))
                    {
                        var attr = (JsonDateTimeOptionsAttribute)property.GetCustomAttributes(typeof(JsonDateTimeOptionsAttribute), false).Single();

                        d.DateTimeKind = attr.Kind;
                        d.DateTimeFormat = attr.Format;
                    }
#if DESKTOP
                    if (d.Getter == null && d.Setter == null)
                    {
                        System.Diagnostics.Trace.WriteLine(String.Format("Member '{0}.{1}' is not properly decorated with DataMember/ IgnoreDataMemberAttribute, no accessible setter or getter found. Skipping.",
                            property.DeclaringType, property.Name));

                        continue;
                    }
#endif

                    if (!propertyDescription.TryAdd(jsonFieldName, d))
                    {
                        throw new SerializationException(String.Format("The DataMember '{0}' is already used on the type '{1}'.", jsonFieldName, type));
                    }

                    _typeInfo[d.PropertyOrFieldType] = d;
                }

                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    if (!IsSerializationMember(field)) { continue; }

                    String jsonFieldName = GetJsonFieldName(field);
                    JsonPropertyInfo d = CreateJsonPropertyInfo(field.FieldType).Clone();
                    d.CanWrite = true;
                    d.Setter = JsonRegistry.CreateSetField(type, field);
                    d.Getter = JsonRegistry.CreateGetField(type, field);
                    d.PropertyOrFieldName = field.Name;
                    d.JsonFieldName = jsonFieldName;
                    d.DeclaringType = field.DeclaringType;

                    if (d.IsDateTime && Attribute.IsDefined(field, typeof(JsonDateTimeOptionsAttribute)))
                    {
                        var attr = (JsonDateTimeOptionsAttribute)field.GetCustomAttributes(typeof(JsonDateTimeOptionsAttribute), false).Single();

                        d.DateTimeKind = attr.Kind;
                    }

                    if (!propertyDescription.TryAdd(jsonFieldName, d))
                    {
                        throw new SerializationException(String.Format("The DataMember '{0}' is already used on the type '{1}'.", jsonFieldName, type));
                    }

                    _typeInfo[d.PropertyOrFieldType] = d;
                }

                _propertycache.Add(typename, propertyDescription);

                return propertyDescription;
            }
        }

        private static String GetJsonFieldName(MemberInfo memberInfo)
        {
            var dataMemberAttribute = (DataMemberAttribute)memberInfo.GetCustomAttributes(typeof(DataMemberAttribute), false).SingleOrDefault();

            if (dataMemberAttribute != null)
            {
                return (!String.IsNullOrEmpty(dataMemberAttribute.Name)) ? dataMemberAttribute.Name : memberInfo.Name;
            }
            else
            {
                return memberInfo.Name;
            }
        }

        private bool IsSerializationMember(MemberInfo memberInfo)
        {
            object[] attributes = memberInfo.GetCustomAttributes(false);

            if (_owner._memberStrategy.HasFlag(MemberStrategy.PropertyOptIn))
            {
                if (attributes.OfType<DataMemberAttribute>().Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (_owner._memberStrategy.HasFlag(MemberStrategy.PropertyOptOut))
            {
                if (attributes.OfType<IgnoreDataMemberAttribute>().Any())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }
    }
}
