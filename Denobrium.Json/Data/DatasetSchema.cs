using System;
using System.Collections.Generic;

namespace Denobrium.Json.Data
{
    /// <summary>
    /// Represents a data model for a data set.
    /// </summary>
    internal sealed class DatasetSchema
    {
        /// <summary>
        /// Gets or sets the columns of the data set.
        /// </summary>
        public List<string> Info { get; set; }

        /// <summary>
        /// Gets or sets the name of the data set.
        /// </summary>
        public string Name { get; set; }
    }
}
