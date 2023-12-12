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
            string val = "";

            if (_typeNameCache.TryGetValue(type, out val))
            {
                return val;
            }
            else
            {
                string s = CreateTypeName(type);

                if (String.IsNullOrEmpty(s))
                {
                    throw new SerializationException(String.Format("TypeDescriptor cannot return create a name for type '{0}' (value was empty)", type));
                }

                _typeNameCache.Add(type, s);

                return s;
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
            Type val = null;
            if (_typeCache.TryGetValue(typeName, out val))
            {
                return val;
            }
            else
            {
                Type t = ResolveType(typeName);

                VerifyType(t, typeName);

                _typeCache.Add(typeName, t);
                return t;
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
        private static void VerifyType(Type type, String typeName)
        {
            if (type == null)
            {
                throw new SerializationException(String.Format("TypeDescriptor could not resolve the type with name '{0}'.", typeName));
            }

            // This is only relevant, if we deserialize. On client side however, the class might not be abstract anymore.
            //if (type.IsAbstract)
            //{
            //    throw new SerializationException(String.Format("The type '{0}' is abstract which is not acceptable since it cannot be constructed. (name was '{1}')", type, typeName));
            //}

#if SILVERLIGHT
            if (!type.IsPublic)
            {
                throw new SerializationException(String.Format("The type '{0}' is a non-public type. It must be public for deserialization purposes. (name was '{1}')", type, typeName));
            }
#endif

            // This is only relevant for deserialization...
            //var defaultConstructor = type.GetConstructor(Type.EmptyTypes);

            //if (defaultConstructor == null)
            //{
            //    throw new ApplicationException(String.Format("The type '{0}' is not offering a public default constructor which is mandatory.", type));
            //}
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
        protected virtual String CreateTypeName(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Returns the type for the given type name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        protected virtual Type ResolveType(String typeName)
        {
            return Type.GetType(typeName);
        }

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
