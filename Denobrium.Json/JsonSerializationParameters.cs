namespace Denobrium.Json
{
    /// <summary>
    /// Represents a set of parameters which apply only to serialization.
    /// </summary>
    public sealed class JsonSerializationParameters
    {
        private bool _serializeNullValues = true;
        private bool _includeReadOnly = true;
        private bool _useOptimizedDatasetSchema = true;

        /// <summary>
        /// Gets or set the bool which indicates whether read only properties should be serialized or not (default = True).
        /// </summary>
        public bool IncludeReadOnly
        {
            get => _includeReadOnly; 
            set => _includeReadOnly = value; 
        }

        /// <summary>
        /// Serialize null values to the output (default = True)
        /// </summary>
        public bool SerializeNullValues
        {
            get => _serializeNullValues; 
            set => _serializeNullValues = value; 
        }

        /// <summary>
        /// Use the optimized fast Dataset Schema format (default = True)
        /// </summary>
        public bool UseOptimizedDatasetSchema
        {
            get => _useOptimizedDatasetSchema; 
            set => _useOptimizedDatasetSchema = value;
        }
    }
}
