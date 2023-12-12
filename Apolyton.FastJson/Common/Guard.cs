using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Apolyton.FastJson.Common
{
    /// <summary>
    /// A static helper class that includes various parameter checking routines.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if tested value if null.</exception>
        /// <param name="min">The minimum of the int to throw (valid is +1).</param>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        [DebuggerStepThrough]
        public static void ArgumentBigger(int min, int argumentValue, string argumentName)
        {
            if (argumentValue <= min) throw new ArgumentOutOfRangeException(argumentName);
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null.
        /// </summary>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        [DebuggerStepThrough]
        public static void ArgumentNotNull(object argumentValue,
                                           string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null or the filename doesn't exist.
        /// </summary>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        /// <exception cref="ArgumentException">When argumentValue is null.</exception>
        public static void FileExists(String fileName, String argumentName)
        {
            ArgumentNotNull(fileName, "fileName");

            if (!File.Exists(fileName))
            {
                throw new ArgumentException("File not found: " + fileName, "fileName");
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if tested value if null.</exception>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        /// <param name="detailedMessage">The exception message when argumentValue is null.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        [DebuggerStepThrough]
        public static void ArgumentNotNull(object argumentValue, string argumentName, string detailedMessage)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName, detailedMessage);
        }

        /// <summary>
        /// Throws an exception if the tested string argument is null or the empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if string value is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the string is empty</exception>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        /// <exception cref="ArgumentException">When argumentValue is empty.</exception>
        [DebuggerStepThrough]
        public static void ArgumentNotEmpty(ref Guid argumentValue,
                                            string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
            if (argumentValue == Guid.Empty) throw new ArgumentException("Argument must not me empty", argumentName);
        }

        /// <summary>
        /// Throws an exception if the tested string argument is null or the empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if string value is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the string is empty</exception>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        /// <exception cref="ArgumentException">When argumentValue is empty.</exception>
        [DebuggerStepThrough]
        public static void ArgumentNotNullOrEmpty(string argumentValue,
                                                  string argumentName)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
            if (argumentValue.Length == 0) throw new ArgumentException("Argument must not me empty", argumentName);
        }

        /// <summary>
        /// Throws an exception if the tested string argument is null or the empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if string value is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the string is empty</exception>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Name of argument being checked.</param>
        /// <param name="detailedMessage">The exception message when argumentValue is null.</param>
        /// <exception cref="ArgumentNullException">When argumentValue is null.</exception>
        /// <exception cref="ArgumentException">When argumentValue is empty.</exception>
        [DebuggerStepThrough]
        public static void ArgumentNotNullOrEmpty(string argumentValue,
                                                  string argumentName,
                                                  string detailedMessage)
        {
            if (argumentValue == null) throw new ArgumentNullException(argumentName);
            if (argumentValue.Length == 0) throw new ArgumentException(detailedMessage, argumentName);
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentValueType">The type of the value being assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <exception cref="ArgumentNullException">When assignmentTargetType or assignmentValueType is null.</exception>
        /// <exception cref="ArgumentException">When target type is not assignable.</exception>
        [DebuggerStepThrough]
        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName)
        {
            TypeIsAssignable(assignmentTargetType, assignmentValueType, argumentName,
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Types are not assignable",
                    assignmentTargetType,
                    assignmentValueType));
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentValueType">The type of the value being assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="detailedMessage">The detailed exception to throw when the type is not assignables</param>
        /// <exception cref="ArgumentNullException">When assignmentTargetType or assignmentValueType is null.</exception>
        /// <exception cref="ArgumentException">When target type is not assignable.</exception>
        [DebuggerStepThrough]
        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName, string detailedMessage)
        {
            if (assignmentTargetType == null) throw new ArgumentNullException("assignmentTargetType");
            if (assignmentValueType == null) throw new ArgumentNullException("assignmentValueType");

            if (!assignmentTargetType.IsAssignableFrom(assignmentValueType))
            {
                throw new ArgumentException(detailedMessage);
            }
        }

        /// <summary>
        /// Verifies that the two types are equal
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentValueType">The type of the value being assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="detailedMessage">The detailed exception to throw when the type is not assignables</param>
        /// <exception cref="ArgumentNullException">When assignmentTargetType or assignmentValueType is null.</exception>
        /// <exception cref="ArgumentException">When target type is not assignable.</exception>
        [DebuggerStepThrough]
        public static void TypeEquals(Type assignmentTargetType, Type assignmentValueType, string argumentName, string detailedMessage)
        {
            if (assignmentTargetType == null) throw new ArgumentNullException("assignmentTargetType");
            if (assignmentValueType == null) throw new ArgumentNullException("argumentName");

            if (assignmentTargetType.GetType() != assignmentValueType.GetType())
            {
                throw new ArgumentException(argumentName, detailedMessage);
            }
        }
    }
}


