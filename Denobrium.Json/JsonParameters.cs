using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Denobrium.Json.Common;
using Denobrium.Json.Registry;
using Denobrium.Json.Serialization;
using System.Diagnostics;

namespace Denobrium.Json
{
    /// <summary>
    /// Represents the parameters for the json class. Changing instance values at runtime when a (de-)serialization is ongoing on that instance can lead to unexpected outcome. 
    /// Don't do it. 
    /// </summary>
    /// <remarks>
    /// http://www.codeproject.com/Articles/159450/fastJSON
    /// The version over there (2.0.9) could not be taken directly, as its serializer is taking all public properties, disregarding any attribute policy. This is
    /// not good for our case, as we want to return (portions of) data objects.
    /// <para/>
    /// Not thread safe.
    /// </remarks>
    public sealed class JsonParameters
    {
        private readonly JsonDeserializationParameters _deserialization;
        private readonly JsonSerializationParameters _serialization;

        private bool _useFastGuid = true;
        private bool _useUtcDateTime = true;

        private bool _enableAnonymousTypes = false;
        private bool _useTypeExtension = false;

        private Encoding _encoding = Encoding.UTF8;

        internal MemberStrategy _memberStrategy = MemberStrategy.PropertyOptOut;

        /// <summary>
        /// Creates an instance of the json parameters. For optimal performance, you should keep the instance in order to be reused. 
        /// </summary>
        public JsonParameters()
        {
            Registry = new JsonRegistry(this);

            _deserialization = new JsonDeserializationParameters();
            _serialization = new JsonSerializationParameters();
        }

        /// <summary>
        /// Returns the list of members which are concerded by (de-)serialization according to the current default parameters.
        /// </summary>
        public IEnumerable<JsonSerializationInfo> GetSerializationMembers(Type type)
        {
            return Registry.GetPropertiesAndFields(type, null).Values.Select(info => info.ToSerializationInfo()).ToArray();
        }

        /// <summary>
        /// Registers the given type descriptor which handles the type-name / type relationship.
        /// Examples:
        /// <para>
        /// Example type descriptors are:
        /// - <seealso cref="DataContractTypeDescriptor"/>
        /// - <seealso cref="JsonTypeDescriptor"/>
        /// </para>
        /// </summary>
        public void RegisterTypeDescriptor(JsonTypeDescriptor descriptor)
        {
            Guard.ArgumentNotNull(descriptor, "descriptor");

            Registry.TypeDescriptor = descriptor;
        }

        /// <summary>
        /// Registers a custom type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serializer"></param>
        /// <param name="deserializer"></param>
        /// <exception cref="DuplicakeyException">If type is already registered.</exception>
        public void RegisterCustomType(Type type, SerializationHandler serializer, DeserializationHandler deserializer)
        {
            Guard.ArgumentNotNull(type, "type");

            Registry.RegisterCustomType(type, serializer, deserializer);
        }

        /// <summary>
        /// Registers a custom type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serializer"></param>
        /// <param name="deserializer"></param>
        public void RegisterCustomType(ICustomTypeSerializer customTypeSerializer)
        {
            Guard.ArgumentNotNull(customTypeSerializer, "customTypeSerializer");
            Guard.ArgumentNotNull(customTypeSerializer.Type, "customTypeSerializer.Type");

            Registry.RegisterCustomType(customTypeSerializer.Type,
                (customTypeSerializer.CanSerialize ? customTypeSerializer.Serialize : (SerializationHandler)null),
                (customTypeSerializer.CanDeserialize ? customTypeSerializer.Deserialize : (DeserializationHandler)null));

            if (!String.IsNullOrEmpty(customTypeSerializer.TypeName))
            {
                Registry.TypeDescriptor.Register(customTypeSerializer.Type, customTypeSerializer.TypeName);
            }
        }

        /// <summary>
        /// Gets the internal registry.
        /// </summary>
        internal readonly JsonRegistry Registry;

        /// <summary>
        /// Gets the deserialization options.
        /// </summary>
        public JsonDeserializationParameters Deserialization
        {
            get { return _deserialization; }
        }

        /// <summary>
        /// Gets the serialization options.
        /// </summary>
        public JsonSerializationParameters Serialization
        {
            get { return _serialization; }
        }

        /// <summary>
        /// Gets or sets the encoding of (input) json strings.
        /// </summary>
        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        /// <summary>
        /// Use the optimized fast Dataset Schema format (default = True)
        /// </summary>
        [Obsolete("Use Serialization.UseOptimizedDatasetSchema instead")]
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool UseOptimizedDatasetSchema
        {
            get { return Serialization.UseOptimizedDatasetSchema; }
            set { Serialization.UseOptimizedDatasetSchema = value; }
        }

        /// <summary>
        /// Use the fast GUID format. (default = True). The fast format is the base64 encoded byte array of the underlying guid. Otherwise a string representation of it. 
        /// (eg. 12345678-ABCD-ABCD-ABCD-1234567890AB).
        /// </summary>
        public bool UseFastGuid
        {
            get { return _useFastGuid; }
            set { _useFastGuid = value; }
        }

        /// <summary>
        /// Obsolete. Use Serialization.SerializeNullValues instead.
        /// </summary>
        [Obsolete("Use Serialization.SerializeNullValues instead")]
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool SerializeNullValues
        {
            get { return Serialization.SerializeNullValues; }
            set { Serialization.SerializeNullValues = value; }
        }

        /// <summary>
        /// Use the UTC date format (default = True) for DateTime object with Kind=unspecified.
        /// </summary>
        public bool UseUtcDateTime
        {
            get { return _useUtcDateTime; }
            set { _useUtcDateTime = value; }
        }

        /// <summary>
        /// Defines the (de-) serialization member strategy.
        /// </summary>
        public MemberStrategy MemberStrategy
        {
            get { return _memberStrategy; }
            set { _memberStrategy = value; }
        }

        /// <summary>
        /// Gets the currenty type descriptor. 
        /// </summary>
        /// <remarks>
        /// Use RegisterTypeDescriptor to set the value. Never set it while (de-) serialization is ongoing.
        /// </remarks>
        public JsonTypeDescriptor TypeDescriptor
        {
            get { return Registry.TypeDescriptor; }
        }

        /// <summary>
        /// Anonymous types have read only properties. Affects also serialization policy, use extension and using global types.
        /// </summary>
        public bool EnableAnonymousTypes
        {
            get { return _enableAnonymousTypes; }
            set
            {
                _enableAnonymousTypes = value;

                if (_enableAnonymousTypes)
                {
                    Serialization.IncludeReadOnly = true;

                    _memberStrategy = MemberStrategy.PropertyOptOut;
                    _useTypeExtension = false;
                }
            }
        }

        /// <summary>
        /// Enable fastJSON extensions $type (default = True).
        /// </summary>
        public bool UseTypeExtension
        {
            get { return _useTypeExtension; }
            set { _useTypeExtension = value; }
        }
    }
}
