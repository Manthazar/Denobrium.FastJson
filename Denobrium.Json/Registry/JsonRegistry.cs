using Denobrium.Json.Common;
using Denobrium.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;


namespace Denobrium.Json.Registry
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
        private static readonly Type[] _genericSetterArguments = [typeof(object), typeof(object)];
        private static readonly Type[] _genericGetterArguments = [typeof(object)];

        private readonly JsonParameters _owner;

        private readonly RegistrySection<Type, JsonPropertyInfo> _typeInfo;
        private readonly RegistrySection<Type, CreateObjectHandler> _constructorCache;
        private readonly RegistrySection<Type, IEnumerable<GetterDescriptor>> _getterCache;
        private readonly RegistrySection<string, RegistrySection<string, JsonPropertyInfo>> _propertycache;

        internal JsonTypeDescriptor TypeDescriptor;
        internal readonly RegistrySection<Type, SerializationHandler> CustomSerializers;
        internal readonly RegistrySection<Type, DeserializationHandler> CustomDeserializer;

        /// <summary>
        /// Creates an instance of the registry
        /// </summary>
        internal JsonRegistry(JsonParameters owner)
        {
            Guard.ArgumentNotNull(owner, nameof(owner));

            _owner = owner;

            TypeDescriptor = new JsonTypeDescriptor();
            CustomSerializers = new RegistrySection<Type, SerializationHandler>();
            CustomDeserializer = new RegistrySection<Type, DeserializationHandler>();

            _typeInfo = new RegistrySection<Type, JsonPropertyInfo>();
            _constructorCache = new RegistrySection<Type, CreateObjectHandler>();
            _getterCache = new RegistrySection<Type, IEnumerable<GetterDescriptor>>();
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
            _getterCache.Clear();
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
            return CustomSerializers.TryGetValue(t, out SerializationHandler s);
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
                if (_constructorCache.TryGetValue(t, out CreateObjectHandler constructorHandler))
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
                var dynMethod = new DynamicMethod("_", t, null);
                ILGenerator ilGen = dynMethod.GetILGenerator();
                ilGen.Emit(OpCodes.Newobj, t.GetConstructor(Type.EmptyTypes));
                ilGen.Emit(OpCodes.Ret);

                return (CreateObjectHandler)dynMethod.CreateDelegate(typeof(CreateObjectHandler));
            }
            else // structs
            {
                var dynMethod = new DynamicMethod("_",
                    MethodAttributes.Public | MethodAttributes.Static,
                    CallingConventions.Standard,
                    typeof(object),
                    null,
                    t, false);

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
            var info = new JsonPropertyInfo
            {
                CanWrite = true,
                PropertyOrFieldType = t,
                IsDictionary = typeof(IDictionary).IsAssignableFrom(t),
                IsValueType = t.IsValueType,
                IsGenericType = t.IsGenericType,
                IsArray = t.IsArray
            };

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
            info.IsHashtable = t == typeof(Hashtable);
            info.IsDataSet = t == typeof(DataSet);
            info.IsDataTable = t == typeof(DataTable);
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

            if (_constructorCache.ContainsKey(t) == false && info.IsClass && !info.IsString && !info.PropertyOrFieldType.IsAbstract && !info.IsArray)
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

        private static Type GetNullableType(Type targetType)
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
            var dynamicSet = new DynamicMethod("_", typeof(object), _genericSetterArguments, type, true);
            var il = dynamicSet.GetILGenerator();

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
            var setMethod = propertyInfo.GetSetMethod();

            if (setMethod == null)
            {
                return null;
            }

            var setter = new DynamicMethod("_", typeof(object), _genericSetterArguments);
            var il = setter.GetILGenerator();

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
            var dynamicGet = new DynamicMethod("_", typeof(object), _genericGetterArguments, type, true);
            var il = dynamicGet.GetILGenerator();

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
            var getMethod = propertyInfo.GetGetMethod();

            if (getMethod == null)
            {
                // Getter is not public
                return null;
            }

            var getter = new DynamicMethod("_", typeof(object), _genericGetterArguments, owningType);
            var il = getter.GetILGenerator();

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
            if (_getterCache.TryGetValue(type, out IEnumerable<GetterDescriptor> result))
            {
                return result;
            }

            var propertiesAndFields = GetPropertiesAndFields(type, null);
            var getters = new List<GetterDescriptor>();

            foreach (JsonPropertyInfo property in propertiesAndFields.Values)
            {
                if (property.Getter == null) { continue; } // Can be for non public getter.

                var descriptor = new GetterDescriptor
                {
                    Name = property.JsonFieldName,
                    Getter = property.Getter,
                    PropertyOrFieldType = property.PropertyOrFieldType,
                    DefaultValue = property.DefaultValue
                };

                getters.Add(descriptor);
            }

            _getterCache.Add(type, getters);

            return getters;
        }

        /// <summary>
        /// Returns the type information (from cache).
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal JsonPropertyInfo GetTypeInfo(Type targetType)
        {
            if (!_typeInfo.TryGetValue(targetType, out JsonPropertyInfo result))
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
            typename ??= type.FullName;

            if (_propertycache.TryGetValue(typename, out RegistrySection<string, JsonPropertyInfo>  cachedDescription))
            {
                return cachedDescription;
            }
            else
            {
                var propertyDescription = new RegistrySection<string, JsonPropertyInfo>();

                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo property in properties)
                {
                    if (!IsSerializationMember(property)) { continue; }
                    if (!property.CanWrite && !_owner.Serialization.IncludeReadOnly) { continue; }

                    var jsonFieldName = GetJsonFieldName(property);
                    var pi = GetTypeInfo(property.PropertyType).Clone();
                    pi.CanWrite = property.CanWrite;
                    pi.JsonFieldName = jsonFieldName;
                    pi.PropertyOrFieldName = property.Name;
                    pi.DeclaringType = property.DeclaringType;

                    if (property.CanRead) { pi.Getter = JsonRegistry.CreateGetMethod(type, property); }
                    else { pi.Getter = null; }

                    if (property.CanWrite) { pi.Setter = JsonRegistry.CreateSetMethod(type, property); }
                    else { pi.Setter = null; }

                    if (pi.IsDateTime && Attribute.IsDefined(property, typeof(JsonDateTimeOptionsAttribute)))
                    {
                        var attr = (JsonDateTimeOptionsAttribute)property.GetCustomAttributes(typeof(JsonDateTimeOptionsAttribute), false).Single();

                        pi.DateTimeKind = attr.Kind;
                        pi.DateTimeFormat = attr.Format;
                    }

                    if (pi.Getter == null && pi.Setter == null)
                    {
                        System.Diagnostics.Trace.WriteLine(String.Format("Member '{0}.{1}' is not properly decorated with DataMember/ IgnoreDataMemberAttribute, no accessible setter or getter found. Skipping.",
                            property.DeclaringType, property.Name));

                        continue;
                    }

                    if (!propertyDescription.TryAdd(jsonFieldName, pi))
                    {
                        throw new SerializationException(String.Format("The DataMember '{0}' is already used on the type '{1}'.", jsonFieldName, type));
                    }

                    _typeInfo[pi.PropertyOrFieldType] = pi;
                }

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    if (!IsSerializationMember(field)) { continue; }

                    var jsonFieldName = GetJsonFieldName(field);
                    var pi = CreateJsonPropertyInfo(field.FieldType).Clone();
                    pi.CanWrite = true;
                    pi.Setter = JsonRegistry.CreateSetField(type, field);
                    pi.Getter = JsonRegistry.CreateGetField(type, field);
                    pi.PropertyOrFieldName = field.Name;
                    pi.JsonFieldName = jsonFieldName;
                    pi.DeclaringType = field.DeclaringType;

                    if (pi.IsDateTime && Attribute.IsDefined(field, typeof(JsonDateTimeOptionsAttribute)))
                    {
                        var attr = (JsonDateTimeOptionsAttribute)field.GetCustomAttributes(typeof(JsonDateTimeOptionsAttribute), false).Single();

                        pi.DateTimeKind = attr.Kind;
                    }

                    if (!propertyDescription.TryAdd(jsonFieldName, pi))
                    {
                        throw new SerializationException(String.Format("The DataMember '{0}' is already used on the type '{1}'.", jsonFieldName, type));
                    }

                    _typeInfo[pi.PropertyOrFieldType] = pi;
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
