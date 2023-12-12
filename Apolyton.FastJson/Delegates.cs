using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apolyton.FastJson
{
    /// <summary>
    /// Represents a serialization handler which returns a string for the given data object.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public delegate string SerializationHandler(object data);

    /// <summary>
    /// Represents a serialization handler which returns a deserialized object for the given data string.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public delegate object DeserializationHandler(string jsonString);

    /// <summary>
    /// Represents a getter handler which is created during runtime.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    internal delegate object GenericSetterHandler(object target, object value);

    /// <summary>
    /// Represents a getter handler which is created during runtime.
    /// </summary>
    internal delegate object GenericGetterHandler(object owner);

    /// <summary>
    /// Represents a getter handler which is created during runtime.
    /// </summary>
    internal delegate object CreateObjectHandler();
}
