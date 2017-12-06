/*From the CodeProject article by Yuri Astrakhan and Sasha Goldshtein.
 * https://www.codeproject.com/Articles/33382/Fast-Native-Structure-Reading-in-C-using-Dynamic-A
**/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace jemalloc.Extensions
{
    public static class TypeBuilderExtensions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodBuilderHelper"/> class
        /// with the specified parameters.
        /// </summary>
        /// <param name="typeBuilder">Associated <see cref="TypeBuilder"/>.</param>
        /// <param name="methodBuilder">A <see cref="MethodBuilder"/></param>
        /// <param name="genericArguments">Generic arguments of the method.</param>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="parameterTypes">The types of the parameters of the method.</param>
        internal static MethodBuilder MethodBuilderHelper(
            TypeBuilder typeBuilder,
            MethodBuilder methodBuilder,
            Type[] genericArguments,
            Type returnType,
            Type[] parameterTypes
            )
        {
            if (methodBuilder == null) throw new ArgumentNullException("methodBuilder");
            if (genericArguments == null) throw new ArgumentNullException("genericArguments");

            var genArgNames = Array.ConvertAll(genericArguments, t => t.Name);

            var genParams = methodBuilder.DefineGenericParameters(genArgNames);

            // Copy parameter constraints.
            //
            List<Type> interfaceConstraints = null;
            for (var i = 0; i < genParams.Length; i++)
            {
                genParams[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);

                foreach (var constraint in genericArguments[i].GetGenericParameterConstraints())
                {
                    if (constraint.IsClass)
                        genParams[i].SetBaseTypeConstraint(constraint);
                    else
                    {
                        if (interfaceConstraints == null)
                            interfaceConstraints = new List<Type>();
                        interfaceConstraints.Add(constraint);
                    }
                }

                if (interfaceConstraints != null && interfaceConstraints.Count != 0)
                {
                    genParams[i].SetInterfaceConstraints(interfaceConstraints.ToArray());
                    interfaceConstraints.Clear();
                }
            }

            // When a method contains a generic parameter we need to replace all
            // generic types from methodInfoDeclaration with local ones.
            //
            for (var i = 0; i < parameterTypes.Length; i++)
                parameterTypes[i] = parameterTypes[i].TranslateGenericParameters(genParams);

            methodBuilder.SetParameters(parameterTypes);
            methodBuilder.SetReturnType(returnType.TranslateGenericParameters(genParams));

            return methodBuilder;
        }

        /// <summary>
        /// Adds a new method to the class, with the given name and method signature.
        /// </summary>
        /// <param name="typeBuilder"/>
        /// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
        /// <param name="attributes">The attributes of the method. </param>
        /// <param name="callingConvention">The <see cref="CallingConventions">calling convention</see> of the method.</param>
        /// <param name="genericArguments">Generic arguments of the method.</param>
        /// <param name="returnType">The return type of the method.</param>
        /// <param name="parameterTypes">The types of the parameters of the method.</param>
        /// <returns>The defined generic method.</returns>
        public static MethodBuilder DefineGenericMethod(
            this TypeBuilder typeBuilder,
            string name,
            MethodAttributes attributes,
            CallingConventions callingConvention,
            Type[] genericArguments,
            Type returnType,
            Type[] parameterTypes)
        {
            return MethodBuilderHelper(typeBuilder,
                                       typeBuilder.DefineMethod(name, attributes, callingConvention), genericArguments,
                                       returnType, parameterTypes);
        }

        /// <summary>
        /// Adds a new method to the class, with the given name and method signature.
        /// </summary>
        /// <param name="typeBuilder"/>
        /// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
        /// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
        /// <param name="attributes">The attributes of the method. </param>
        /// <returns>The defined method.</returns>
        public static MethodBuilder DefineMethod(
            this TypeBuilder typeBuilder,
            string name,
            MethodInfo methodInfoDeclaration,
            MethodAttributes attributes)
        {
            if (methodInfoDeclaration == null) throw new ArgumentNullException("methodInfoDeclaration");

            var pi = methodInfoDeclaration.GetParameters();
            var parameters = new Type[pi.Length];

            for (var i = 0; i < pi.Length; i++)
                parameters[i] = pi[i].ParameterType;

            var method = methodInfoDeclaration.ContainsGenericParameters
                             ? typeBuilder.DefineGenericMethod(
                                   name,
                                   attributes,
                                   methodInfoDeclaration.CallingConvention,
                                   methodInfoDeclaration.GetGenericArguments(),
                                   methodInfoDeclaration.ReturnType,
                                   parameters)
                             : typeBuilder.DefineMethod(
                                   name,
                                   attributes,
                                   methodInfoDeclaration.CallingConvention,
                                   methodInfoDeclaration.ReturnType,
                                   parameters);

            // Compiler overrides methods only for interfaces. We do the same.
            // If we wanted to override virtual methods, then methods should've had
            // MethodAttributes.VtableLayoutMask attribute
            // and the following condition should've been used below:
            // if ((methodInfoDeclaration is FakeMethodInfo) == false)
            //
            if (methodInfoDeclaration.DeclaringType.IsInterface) // todo: && !(methodInfoDeclaration is FakeMethodInfo))
            {
                // OverriddenMethods.Add(methodInfoDeclaration, method.MethodBuilder);
                typeBuilder.DefineMethodOverride(method, methodInfoDeclaration);
            }

            //method.OverriddenMethod = methodInfoDeclaration;

            for (var i = 0; i < pi.Length; i++)
                method.DefineParameter(i + 1, pi[i].Attributes, pi[i].Name);

            return method;
        }

        /// <summary>
        /// Adds a new private method to the class.
        /// </summary>
        /// <param name="typeBuilder"/>
        /// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
        /// <returns>The defined method.</returns>
        public static MethodBuilder DefineMethod(this TypeBuilder typeBuilder, MethodInfo methodInfoDeclaration)
        {
            if (methodInfoDeclaration == null) throw new ArgumentNullException("methodInfoDeclaration");

            var isInterface = methodInfoDeclaration.DeclaringType.IsInterface;

            var name = isInterface
                           ? methodInfoDeclaration.DeclaringType.FullName + "." + methodInfoDeclaration.Name
                           : methodInfoDeclaration.Name;

            var attributes =
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig |
                MethodAttributes.PrivateScope |
                methodInfoDeclaration.Attributes & MethodAttributes.SpecialName;

            if (isInterface)
                attributes |= MethodAttributes.Private;
            else if ((attributes & MethodAttributes.SpecialName) != 0)
                attributes |= MethodAttributes.Public;
            else
                attributes |= methodInfoDeclaration.Attributes &
                              (MethodAttributes.Public | MethodAttributes.Private);

            return typeBuilder.DefineMethod(name, methodInfoDeclaration, attributes);
        }
    }
}