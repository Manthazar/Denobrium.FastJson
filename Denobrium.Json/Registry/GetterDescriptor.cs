using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apolyton.FastJson.Registry
{
    /// <summary>
    /// Represents brief information about the get aspect of a property or a field. Contains a fast getter handler which can be used to read the values.
    /// </summary>
    /// <remarks>
    /// http://www.codeproject.com/Articles/159450/fastJSON
    /// The version over there (2.0.9) could not be taken directly, as its serializer is taking all public properties, disregarding any attribute policy. This is
    /// not good for our case, as we want to return (portions of) data objects as well.
    /// </remarks>
    internal struct GetterDescriptor
    {
        /// <summary>
        /// The name of the property to which the getter belongs to.
        /// </summary>
        public string Name;

        /// <summary>
        /// The delegate method.
        /// </summary>
        public GenericGetterHandler Getter;

        /// <summary>
        /// The default value of the property. Relevant for value types to be able to quickly detect the 'not-set' case.
        /// </summary>
        public object DefaultValue;

        /// <summary>
        /// The type of the property to which the getter belongs to.
        /// </summary>
        public Type PropertyOrFieldType;
    }
}
