/*Based on the CodeProject article by Yuri Astrakhan and Sasha Goldshtein:
 * https://www.codeproject.com/Articles/33382/Fast-Native-Structure-Reading-in-C-using-Dynamic-A
**/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace jemalloc.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets a value indicating whether a type (or type's element type)
        /// instance can be null in the underlying data store.
        /// </summary>
        /// <param name="type">A <see cref="System.Type"/> instance. </param>
        /// <returns> True, if the type parameter is a closed generic nullable type; otherwise, False.</returns>
        /// <remarks>Arrays of Nullable types are treated as Nullable types.</remarks>
        public static bool IsNullable(this Type type)
        {
            while (type.IsArray)
                type = type.GetElementType();

            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>));
        }

        /// <summary>
        /// Returns the underlying type argument of the specified type.
        /// </summary>
        /// <param name="type">A <see cref="System.Type"/> instance. </param>
        /// <returns><list>
        /// <item>The type argument of the type parameter,
        /// if the type parameter is a closed generic nullable type.</item>
        /// <item>The underlying Type if the type parameter is an enum type.</item>
        /// <item>Otherwise, the type itself.</item>
        /// </list>
        /// </returns>
        public static Type GetUnderlyingType(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (type.IsNullable())
                type = type.GetGenericArguments()[0];

            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            return type;
        }

        /// <summary>
        /// Determines whether the specified types are considered equal.
        /// </summary>
        /// <param name="parent">A <see cref="System.Type"/> instance. </param>
        /// <param name="child">A type possible derived from the <c>parent</c> type</param>
        /// <returns>True, when an object instance of the type <c>child</c>
        /// can be used as an object of the type <c>parent</c>; otherwise, false.</returns>
        /// <remarks>Note that nullable types does not have a parent-child relation to it's underlying type.
        /// For example, the 'int?' type (nullable int) and the 'int' type
        /// aren't a parent and it's child.</remarks>
        public static bool IsSameOrParent(Type parent, Type child)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (child == null) throw new ArgumentNullException("child");

            if (parent == child ||
                child.IsEnum && Enum.GetUnderlyingType(child) == parent ||
                child.IsSubclassOf(parent))
            {
                return true;
            }

            if (parent.IsInterface)
            {
                var interfaces = child.GetInterfaces();

                foreach (var t in interfaces)
                    if (t == parent)
                        return true;
            }

            return false;
        }

        /// <summary>
        /// Recursively checks if the type with all of its members are expressable as a value type that may be cast to a pointer.
        /// Equivalent to what compiler does to check for CS0208 error of this statement:
        ///        fixed (int* p = new int[5]) {}
        /// 
        /// An unmanaged-type is any type that isn’t a reference-type and doesn’t contain reference-type fields 
        /// at any level of nesting. In other words, an unmanaged-type is one of the following:
        ///  * sbyte, byte, short, ushort, int, uint, long, ulong, char, float, double, decimal, or bool.
        ///  * Any enum-type.
        ///  * Any pointer-type.
        ///  * Any user-defined struct-type that contains fields of unmanaged-types only.
        /// 
        /// Strings are not in that list, even though you can use them in structs. 
        /// Fixed-size arrays of unmanaged-types are allowed.
        /// </summary>
        public static void ThrowIfNotUnmanagedType(this Type type)
        {
            ThrowIfNotUnmanagedType(type, new Stack<Type>(4));
        }

        private static void ThrowIfNotUnmanagedType(Type type, Stack<Type> typesStack)
        {
            if ((!type.IsValueType && !type.IsPointer) || type.IsGenericType || type.IsGenericParameter || type.IsArray)
                throw new ArgumentException(String.Format("Type {0} is not an unmanaged type", type.FullName));

            if (!type.IsPrimitive && !type.IsEnum && !type.IsPointer)
                for (var p = type.DeclaringType; p != null; p = p.DeclaringType)
                    if (p.IsGenericTypeDefinition)
                        throw new ArgumentException(
                            String.Format("Type {0} contains a generic type definition declaring type {1}",
                                          type.FullName, p.FullName));

            try
            {
                typesStack.Push(type);

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var f in fields)
                    if (!typesStack.Contains(f.FieldType))
                        ThrowIfNotUnmanagedType(f.FieldType);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    String.Format("Error in subtype of type {0}. See InnerException.", type.FullName), ex);
            }
            finally
            {
                typesStack.Pop();
            }
        }

        /// <summary>
        /// Substitutes the elements of an array of types for the type parameters
        /// of the current generic type definition and returns a Type object
        /// representing the resulting constructed type.
        /// </summary>
        /// <param name="type">A <see cref="System.Type"/> instance.</param>
        /// <param name="typeArguments">An array of types to be substituted for
        /// the type parameters of the current generic type.</param>
        /// <returns>A Type representing the constructed type formed by substituting
        /// the elements of <paramref name="typeArguments"/> for the type parameters
        /// of the current generic type.</returns>
        /// <seealso cref="System.Type.MakeGenericType"/>
        public static Type TranslateGenericParameters(this Type type, Type[] typeArguments)
        {
            // 'T paramName' case
            //
            if (type.IsGenericParameter)
                return typeArguments[type.GenericParameterPosition];

            // 'List<T> paramName' or something like that.
            //
            if (type.IsGenericType && type.ContainsGenericParameters)
            {
                Type[] genArgs = type.GetGenericArguments();

                for (int i = 0; i < genArgs.Length; ++i)
                    genArgs[i] = TranslateGenericParameters(genArgs[i], typeArguments);

                return type.GetGenericTypeDefinition().MakeGenericType(genArgs);
            }

            // Non-generic type.
            //
            return type;
        }
    }
}