using System;
using System.Runtime.Serialization;

namespace Denobrium.Json.Registry
{
    /// <summary>
    /// Represents a class which describes a type in json using the qualified assembly name.
    /// <seealso cref="DataContractTypeDescriptor"/>
    /// </summary>
    public class JsonTypeDescriptor
    {
        private readonly RegistrySection<Type, string> _typeNameCache;
        private readonly RegistrySection<string, Type> _typeCache;

        /// <summary>
        /// Creates an instance of the type descriptor dictionary.
        /// </summary>
        public JsonTypeDescriptor()
        {
            _typeNameCache = new RegistrySection<Type, string>();
            _typeCache = new RegistrySection<string, Type>();

            Initialize();
        }

        #region Internals

        /// <summary>
        /// Returns the name of the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="SerializationException">If the type does not have a proper name.</exception>
        internal String GetTypeName(Type type)
        {
            if (_typeNameCache.TryGetValue(type, out string name))
            {
                return name;
            }
            else
            {
                string calculatedName = CreateTypeName(type);

                if (String.IsNullOrEmpty(calculatedName))
                {
                    throw new SerializationException(String.Format("TypeDescriptor cannot return create a name for type '{0}' (value was empty)", type));
                }

                _typeNameCache.Add(type, calculatedName);

                return calculatedName;
            }
        }

        /// <summary>
        /// Returns the type for the given name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">If the type name could not be resolved into a proper name.</exception>
        internal Type GetType(String typeName)
        {
            if (_typeCache.TryGetValue(typeName, out Type val))
            {
                return val;
            }
            else
            {
                var type = ResolveType(typeName);

                VerifyType(type, typeName);

                _typeCache.Add(typeName, type);
                return type;
            }
        }

        /// <summary>
        /// Tries to return the type for the given name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">If the type name could not be resolved into a proper name.</exception>
        internal bool TryGetType(String typeName, out Type type)
        {
            if (String.IsNullOrEmpty(typeName)) 
            { 
                type = null; 
                return false; 
            }

            if (_typeCache.TryGetValue(typeName, out type))
            {
                return true;
            }
            else
            {
                type = ResolveType(typeName);

                if (type != null)
                {
                    VerifyType(type, typeName);

                    _typeCache.Add(typeName, type);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Verifies that the resolved type is within expectations or throws an exception.
        /// </summary>
        /// <param name="t"></param>
        /// <exception cref="SerializationException"></exception>
        private static void VerifyType(Type type, String typeName)
        {
            if (type == null)
            {
                throw new SerializationException(String.Format("TypeDescriptor could not resolve the type with name '{0}'.", typeName));
            }
        }

        /// <summary>
        /// Registers an explicit type mapping.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="p"></param>
        internal void Register(Type type, string typeName)
        {
            VerifyType(type, typeName);

            _typeCache.Add(typeName, type);
            _typeNameCache.Add(type, typeName);
        }

        #endregion

        /// <summary>
        /// Initialize the type descriptor.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Returns the expected name for the given type. The result is cached.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual String CreateTypeName(Type type) => type.AssemblyQualifiedName;

        /// <summary>
        /// Returns the type for the given type name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected virtual Type ResolveType(String typeName) => Type.GetType(typeName);

        /// <summary>
        /// Resets the descriptor.
        /// </summary>
        internal void Reset()
        {
            _typeNameCache.Clear();
            _typeCache.Clear();

            Initialize();
        }
    }
}
