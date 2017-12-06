/*From the CodeProject article by Yuri Astrakhan and Sasha Goldshtein.
 * https://www.codeproject.com/Articles/33382/Fast-Native-Structure-Reading-in-C-using-Dynamic-A
**/

using System;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace jemalloc.Extensions
{
    /// <summary>
    /// A wrapper around the <see cref="ILGenerator"/> class.
    /// </summary>
    /// <seealso cref="System.Reflection.Emit.ILGenerator">ILGenerator Class</seealso>
    public static class ILGeneratorExtensions
    {
        #region ILGenerator Methods

        /// <summary>
        /// Begins a catch block.
        /// </summary>
        /// <param name="il"/>
        /// <param name="exceptionType">The Type object that represents the exception.</param>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.BeginCatchBlock(Type)">ILGenerator.BeginCatchBlock Method</seealso>
        public static ILGenerator BeginCatchBlock(this ILGenerator il, Type exceptionType)
        {
            il.BeginCatchBlock(exceptionType);
            return il;
        }

        /// <summary>
        /// Begins an exception block for a filtered exception.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.BeginExceptFilterBlock">ILGenerator.BeginCatchBlock Method</seealso>
        public static ILGenerator BeginExceptFilterBlock(this ILGenerator il)
        {
            il.BeginExceptFilterBlock();
            return il;
        }

        /// <summary>
        /// Begins an exception block for a non-filtered exception.
        /// </summary>
        /// <param name="il"/>
        /// <returns>The label for the end of the block.</returns>
        public static Label BeginExceptionBlock(this ILGenerator il)
        {
            return il.BeginExceptionBlock();
        }

        /// <summary>
        /// Begins an exception fault block in the Microsoft intermediate language (MSIL) stream.
        /// </summary>
        public static ILGenerator BeginFaultBlock(this ILGenerator il)
        {
            il.BeginFaultBlock();
            return il;
        }

        /// <summary>
        /// Begins a finally block in the Microsoft intermediate language (MSIL) instruction stream.
        /// </summary>
        public static ILGenerator BeginFinallyBlock(this ILGenerator il)
        {
            il.BeginFinallyBlock();
            return il;
        }

        /// <summary>
        /// Begins a lexical scope.
        /// </summary>
        public static ILGenerator BeginScope(this ILGenerator il)
        {
            il.BeginScope();
            return il;
        }

        /// <summary>
        /// Declares a local variable.
        /// </summary>
        /// <param name="il"/>
        /// <param name="localType">The Type of the local variable.</param>
        /// <returns>The declared local variable.</returns>
        public static LocalBuilder DeclareLocal(this ILGenerator il, Type localType)
        {
            return il.DeclareLocal(localType);
        }

        /// <summary>
        /// Declares a local variable.
        /// </summary>
        /// <param name="il"/>
        /// <param name="localType">The Type of the local variable.</param>
        /// <param name="pinned">true to pin the object in memory; otherwise, false.</param>
        /// <returns>The declared local variable.</returns>
        public static LocalBuilder DeclareLocal(this ILGenerator il, Type localType, bool pinned)
        {
            return il.DeclareLocal(localType, pinned);
        }

        /// <summary>
        /// Declares a new label.
        /// </summary>
        /// <param name="il"/>
        /// <returns>Returns a new label that can be used as a token for branching.</returns>
        public static Label DefineLabel(this ILGenerator il)
        {
            return il.DefineLabel();
        }

        /// <summary>
        /// Ends an exception block.
        /// </summary>
        public static ILGenerator EndExceptionBlock(this ILGenerator il)
        {
            il.EndExceptionBlock();
            return il;
        }

        /// <summary>
        /// Ends a lexical scope.
        /// </summary>
        public static ILGenerator EndScope(this ILGenerator il)
        {
            il.EndScope();
            return il;
        }

        /// <summary>
        /// Marks the Microsoft intermediate language (MSIL) stream's current position 
        /// with the given label. Due to this method having identical signature with 
        /// the <see cref="ILGenerator.MarkLabel"/>, the name has been changed.
        /// </summary>
        /// <param name="il"/>
        /// <param name="loc">The label for which to set an index.</param>
        public static ILGenerator MarkLabelExt(this ILGenerator il, Label loc)
        {
            il.MarkLabel(loc);
            return il;
        }

        /// <summary>
        /// Marks a sequence point in the Microsoft intermediate language (MSIL) stream.
        /// </summary>
        /// <param name="il"/>
        /// <param name="document">The document for which the sequence point is being defined.</param>
        /// <param name="startLine">The line where the sequence point begins.</param>
        /// <param name="startColumn">The column in the line where the sequence point begins.</param>
        /// <param name="endLine">The line where the sequence point ends.</param>
        /// <param name="endColumn">The column in the line where the sequence point ends.</param>
        public static ILGenerator MarkSequencePoint(this ILGenerator il,
                                                    ISymbolDocumentWriter document,
                                                    int startLine,
                                                    int startColumn,
                                                    int endLine,
                                                    int endColumn)
        {
            il.MarkSequencePoint(document, startLine, startColumn, endLine, endColumn);
            return il;
        }

        /// <summary>
        /// Emits an instruction to throw an exception.
        /// </summary>
        /// <param name="il"/>
        /// <param name="exceptionType">The class of the type of exception to throw.</param>
        public static ILGenerator ThrowException(this ILGenerator il, Type exceptionType)
        {
            il.ThrowException(exceptionType);
            return il;
        }

        /// <summary>
        /// Specifies the namespace to be used in evaluating locals and watches for 
        /// the current active lexical scope.
        /// </summary>
        /// <param name="il"/>
        /// <param name="namespaceName">The namespace to be used in evaluating locals and watches for the current active lexical scope.</param>
        public static ILGenerator UsingNamespace(this ILGenerator il, string namespaceName)
        {
            il.UsingNamespace(namespaceName);
            return il;
        }

        #endregion

        #region Emit Wrappers

        /// <summary>
        /// Adds two values and pushes the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Add"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Add">OpCodes.Add</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator add(this ILGenerator il)
        {
            il.Emit(OpCodes.Add);
            return il;
        }

        /// <summary>
        /// Adds two integers, performs an overflow check, and pushes the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Add_Ovf"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Add_Ovf">OpCodes.Add_Ovf</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator add_ovf(this ILGenerator il)
        {
            il.Emit(OpCodes.Add_Ovf);
            return il;
        }

        /// <summary>
        /// Adds two unsigned integer values, performs an overflow check, and pushes the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Add_Ovf_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Add_Ovf_Un">OpCodes.Add_Ovf_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator add_ovf_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Add_Ovf_Un);
            return il;
        }

        /// <summary>
        /// Computes the bitwise AND of two values and pushes the result onto the evalution stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.And"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.And">OpCodes.And</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator and(this ILGenerator il)
        {
            il.Emit(OpCodes.And);
            return il;
        }

        /// <summary>
        /// Returns an unmanaged pointer to the argument list of the current method
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Arglist"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Arglist">OpCodes.Arglist</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator arglist(this ILGenerator il)
        {
            il.Emit(OpCodes.Arglist);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if two values are equal
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Beq"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Beq">OpCodes.Beq</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator beq(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Beq, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if two values are equal
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Beq_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Beq_S">OpCodes.Beq_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator beq_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Beq_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than or equal to the second value
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Bge"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bge">OpCodes.Bge</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator bge(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bge, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) 
        /// if the first value is greater than or equal to the second value
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Bge_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bge_S">OpCodes.Bge_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator bge_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bge_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if the the first value is greather than the second value,
        /// when comparing unsigned integer values or unordered float values
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Bge_Un"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bge_Un">OpCodes.Bge_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator bge_un(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bge_Un, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if if the the first value is greather than the second value,
        /// when comparing unsigned integer values or unordered float values
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Bge_Un_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bge_Un_S">OpCodes.Bge_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator bge_un_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bge_Un_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than the second value
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Bgt"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bgt">OpCodes.Bgt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator bgt(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bgt, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is greater than the second value
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Bgt_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bgt_S">OpCodes.Bgt_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator bgt_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bgt_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is greater than the second value,
        /// when comparing unsigned integer values or unordered float values
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Bgt_Un"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bgt_Un">OpCodes.Bgt_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator bgt_un(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bgt_Un, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is greater than the second value,
        /// when comparing unsigned integer values or unordered float values
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Bgt_Un_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bgt_Un_S">OpCodes.Bgt_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator bgt_un_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bgt_Un_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is less than or equal to the second value
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ble"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Ble">OpCodes.Ble</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator ble(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Ble, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than or equal to the second value
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ble_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Ble_S">OpCodes.Ble_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator ble_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Ble_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is less than or equal to the second value,
        /// when comparing unsigned integer values or unordered float values
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ble_Un"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Ble_Un">OpCodes.Ble_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator ble_un(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Ble_Un, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than or equal to the second value,
        /// when comparing unsigned integer values or unordered float values
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ble_Un_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Ble_Un_S">OpCodes.Ble_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator ble_un_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Ble_Un_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is less than the second value
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Blt"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Blt">OpCodes.Blt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator blt(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Blt, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than the second value
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Blt_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Blt_S">OpCodes.Blt_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator blt_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Blt_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if the first value is less than the second value,
        /// when comparing unsigned integer values or unordered float values
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Blt_Un"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Blt_Un">OpCodes.Blt_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator blt_un(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Blt_Un, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if the first value is less than the second value,
        /// when comparing unsigned integer values or unordered float values
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Blt_Un_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Blt_Un_S">OpCodes.Blt_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator blt_un_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Blt_Un_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction when two unsigned integer values or unordered float values are not equal
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Bne_Un"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bne_Un">OpCodes.Bne_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator bne_un(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bne_Un, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) 
        /// when two unsigned integer values or unordered float values are not equal
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Bne_Un_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Bne_Un_S">OpCodes.Bne_Un_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator bne_un_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Bne_Un_S, label);
            return il;
        }

        /// <summary>
        /// Converts a value type to an object reference
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Box"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Box">OpCodes.Box</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator box(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Box, type);
            return il;
        }

        /// <summary>
        /// Converts a value type to an object reference if the value is a value type.
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Box">OpCodes.Box</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator boxIfValueType(this ILGenerator il, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.IsValueType ? il.box(type) : il;
        }

        /// <summary>
        /// Unconditionally transfer control to a target instruction
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Br"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Br">OpCodes.Br</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator br(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Br, label);
            return il;
        }

        /// <summary>
        /// Signals the Common Language Infrastructure (CLI) to inform the debugger that a break point has been tripped
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Break"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Break">OpCodes.Break</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator @break(this ILGenerator il)
        {
            il.Emit(OpCodes.Break);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if value is false, a null reference (Nothing in Visual Basic), or zero
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Brfalse"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Brfalse">OpCodes.Brfalse</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator brfalse(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Brfalse, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if value is false, a null reference, or zero
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Brfalse_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Brfalse_S">OpCodes.Brfalse_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator brfalse_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Brfalse_S, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction if value is true, not null, or non-zero
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Brtrue"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Brtrue">OpCodes.Brtrue</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator brtrue(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Brtrue, label);
            return il;
        }

        /// <summary>
        /// Transfers control to a target instruction (short form) if value is true, not null, or non-zero
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Brtrue_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Brtrue_S">OpCodes.Brtrue_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator brtrue_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Brtrue_S, label);
            return il;
        }

        /// <summary>
        /// Unconditionally transfers control to a target instruction (short form)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Br_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Br_S">OpCodes.Br_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator br_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Br_S, label);
            return il;
        }

        /// <summary>
        /// Calls the method indicated by the passed method descriptor
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Call"/>, methodInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Call">OpCodes.Call</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator call(this ILGenerator il, MethodInfo methodInfo)
        {
            il.Emit(OpCodes.Call, methodInfo);
            return il;
        }

        /// <summary>
        /// Calls the method indicated by the passed method descriptor
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Call"/>, constructorInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="constructorInfo">The constructor to be called.</param>
        /// <seealso cref="OpCodes.Call">OpCodes.Call</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator call(this ILGenerator il, ConstructorInfo constructorInfo)
        {
            il.Emit(OpCodes.Call, constructorInfo);
            return il;
        }

        /// <summary>
        /// Calls the method indicated by the passed method descriptor
        /// by calling ILGenerator.EmitCall(<see cref="OpCodes.Call"/>, methodInfo, optionalParameterTypes).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodInfo">The method to be called.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Call">OpCodes.Call</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        public static ILGenerator call(this ILGenerator il, MethodInfo methodInfo, Type[] optionalParameterTypes)
        {
            il.EmitCall(OpCodes.Call, methodInfo, optionalParameterTypes);
            return il;
        }

        /// <summary>
        /// Calls the method indicated by the passed method descriptor
        /// by calling ILGenerator.EmitCall(<see cref="OpCodes.Call"/>, methodInfo, optionalParameterTypes).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <param name="methodName">The name of the method to be called.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Call">OpCodes.Call</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        public static ILGenerator call(this ILGenerator il, Type type, string methodName,
                                       params Type[] optionalParameterTypes)
        {
            if (type == null) throw new ArgumentNullException("type");

            var methodInfo = type.GetMethod(methodName, optionalParameterTypes);

            if (methodInfo == null)
                throw CreateNoSuchMethodException(type, methodName);

            return il.call(methodInfo);
        }

        /// <summary>
        /// Calls the method indicated by the passed method descriptor
        /// by calling ILGenerator.EmitCall(<see cref="OpCodes.Call"/>, methodInfo, optionalParameterTypes).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <param name="methodName">The name of the method to be called.</param>
        /// <param name="flags">A bitmask comprised of one or more <see cref="BindingFlags"/> 
        /// that specify how the search is conducted.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Call">OpCodes.Call</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        public static ILGenerator call(this ILGenerator il, Type type, string methodName, BindingFlags flags,
                                       params Type[] optionalParameterTypes)
        {
            if (type == null) throw new ArgumentNullException("type");

            var methodInfo = type.GetMethod(methodName, flags, null, optionalParameterTypes, null);

            if (methodInfo == null)
                throw CreateNoSuchMethodException(type, methodName);

            return il.call(methodInfo);
        }

        /// <summary>
        /// Calls the method indicated on the evaluation stack (as a pointer to an entry point) 
        /// with arguments described by a calling convention using an unmanaged calling convention
        /// by calling ILGenerator.EmitCalli(<see cref="OpCodes.Calli"/>, <see cref="CallingConvention"/>, Type, Type[]).
        /// </summary>
        /// <param name="il"/>
        /// <param name="unmanagedCallConv">The unmanaged calling convention to be used.</param>
        /// <param name="returnType">The Type of the result.</param>
        /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
        /// <seealso cref="OpCodes.Calli">OpCodes.Calli</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCalli(OpCode,CallingConvention,Type,Type[])">ILGenerator.EmitCalli</seealso>
        public static ILGenerator calli(this ILGenerator il, CallingConvention unmanagedCallConv, Type returnType,
                                        Type[] parameterTypes)
        {
            //il.EmitCalli(OpCodes.Calli, unmanagedCallConv, returnType, parameterTypes,);
            return il;
        }

        /// <summary>
        /// Calls the method indicated on the evaluation stack (as a pointer to an entry point)
        /// with arguments described by a calling convention using a managed calling convention
        /// by calling ILGenerator.EmitCalli(<see cref="OpCodes.Calli"/>, <see cref="CallingConvention"/>, Type, Type[], Type[]).
        /// </summary>
        /// <param name="il"/>
        /// <param name="callingConvention">The managed calling convention to be used.</param>
        /// <param name="returnType">The Type of the result.</param>
        /// <param name="parameterTypes">The types of the required arguments to the instruction.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments for vararg calls.</param>
        /// <seealso cref="OpCodes.Calli">OpCodes.Calli</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCalli(OpCode,CallingConventions,Type,Type[],Type[])">ILGenerator.EmitCalli</seealso>
        public static ILGenerator calli(this ILGenerator il, CallingConventions callingConvention, Type returnType,
                                        Type[] parameterTypes, Type[] optionalParameterTypes)
        {
            il.EmitCalli(OpCodes.Calli, callingConvention, returnType, parameterTypes, optionalParameterTypes);
            return il;
        }

        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Callvirt"/>, methodInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator callvirt(this ILGenerator il, MethodInfo methodInfo)
        {
            il.Emit(OpCodes.Callvirt, methodInfo);
            return il;
        }

        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack
        /// by calling ILGenerator.EmitCall(<see cref="OpCodes.Callvirt"/>, methodInfo, optionalParameterTypes).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodInfo">The method to be called.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        public static ILGenerator callvirt(this ILGenerator il, MethodInfo methodInfo, Type[] optionalParameterTypes)
        {
            il.EmitCall(OpCodes.Callvirt, methodInfo, optionalParameterTypes);
            return il;
        }

        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack
        /// by calling ILGenerator.EmitCall(<see cref="OpCodes.Callvirt"/>, methodInfo, optionalParameterTypes).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodName">The method to be called.</param>
        /// <param name="type">The declaring type of the method.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        public static ILGenerator callvirt(this ILGenerator il, Type type, string methodName,
                                           params Type[] optionalParameterTypes)
        {
            if (type == null) throw new ArgumentNullException("type");

            var methodInfo = type.GetMethod(methodName, optionalParameterTypes);

            if (methodInfo == null)
                throw CreateNoSuchMethodException(type, methodName);

            return il.callvirt(methodInfo);
        }

        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack
        /// by calling ILGenerator.EmitCall(<see cref="OpCodes.Callvirt"/>, methodInfo, optionalParameterTypes).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodName">The method to be called.</param>
        /// <param name="type">The declaring type of the method.</param>
        /// <param name="flags">A bitmask comprised of one or more <see cref="BindingFlags"/> 
        /// that specify how the search is conducted.</param>
        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        public static ILGenerator callvirt(this ILGenerator il, Type type, string methodName, BindingFlags flags,
                                           params Type[] optionalParameterTypes)
        {
            var methodInfo =
                optionalParameterTypes == null
                    ? type.GetMethod(methodName, flags)
                    : type.GetMethod(methodName, flags, null, optionalParameterTypes, null);

            if (methodInfo == null)
                throw CreateNoSuchMethodException(type, methodName);

            return il.callvirt(methodInfo, null);
        }

        /// <summary>
        /// Calls a late-bound method on an object, pushing the return value onto the evaluation stack
        /// by calling ILGenerator.EmitCall(<see cref="OpCodes.Callvirt"/>, methodInfo, optionalParameterTypes).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodName">The method to be called.</param>
        /// <param name="type">The declaring type of the method.</param>
        /// <param name="flags">A bitmask comprised of one or more <see cref="BindingFlags"/> 
        /// that specify how the search is conducted.</param>
        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
        public static ILGenerator callvirt(this ILGenerator il, Type type, string methodName, BindingFlags flags)
        {
            return il.callvirt(type, methodName, flags, null);
        }

//        /// <summary>
//        /// Calls ILGenerator.EmitCall(<see cref="OpCodes.Callvirt"/>, methodInfo, optionalParameterTypes) that
//        /// calls a late-bound method on an object, pushing the return value onto the evaluation stack.
//        /// </summary>
//        /// <param name="il"/>
//        /// <param name="methodName">The non-generic method to be called.</param>
//        /// <param name="type">The declaring type of the method.</param>
//        /// <param name="optionalParameterTypes">The types of the optional arguments if the method is a varargs method.</param>
//        /// <seealso cref="OpCodes.Callvirt">OpCodes.Callvirt</seealso>
//        /// <seealso cref="System.Reflection.Emit.ILGenerator.EmitCall(OpCode,MethodInfo,Type[])">ILGenerator.EmitCall</seealso>
//        public static ILGenerator callvirtNoGenerics(this ILGenerator il, Type type, string methodName, params Type[] optionalParameterTypes)
//        {
//            MethodInfo methodInfo = type.GetMethod(
//                methodName,
//                BindingFlags.Instance | BindingFlags.Public,
//                GenericBinder.NonGeneric,
//                optionalParameterTypes, null);
//
//            if (methodInfo == null)
//                throw CreateNoSuchMethodException(type, methodName);
//
//            return il.callvirt(methodInfo);
//        }

        /// <summary>
        /// Attempts to cast an object passed by reference to the specified class
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Castclass"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Castclass">OpCodes.Castclass</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator castclass(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Castclass, type);
            return il;
        }

        /// <summary>
        /// Attempts to cast an object passed by reference to the specified class 
        /// or to unbox if the type is a value type.
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        public static ILGenerator castType(this ILGenerator il, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.IsValueType ? il.unbox_any(type) : il.castclass(type);
        }

        /// <summary>
        /// Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack;
        /// otherwise 0 (int32) is pushed onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ceq"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ceq">OpCodes.Ceq</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ceq(this ILGenerator il)
        {
            il.Emit(OpCodes.Ceq);
            return il;
        }

        /// <summary>
        /// Compares two values. If the first value is greater than the second,
        /// the integer value 1 (int32) is pushed onto the evaluation stack;
        /// otherwise 0 (int32) is pushed onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Cgt"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Cgt">OpCodes.Cgt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator cgt(this ILGenerator il)
        {
            il.Emit(OpCodes.Cgt);
            return il;
        }

        /// <summary>
        /// Compares two unsigned or unordered values
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Cgt_Un"/>).
        /// If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack;
        /// otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Cgt_Un">OpCodes.Cgt_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator cgt_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Cgt_Un);
            return il;
        }


        /// <summary>
        /// Constrains the type on which a virtual method call is made
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Constrained"/>).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Cgt_Un">OpCodes.Constrained</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator constrained(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Constrained, type);
            return il;
        }

        /// <summary>
        /// Throws <see cref="ArithmeticException"/> if value is not a finite number
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ckfinite"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ckfinite">OpCodes.Ckfinite</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ckfinite(this ILGenerator il)
        {
            il.Emit(OpCodes.Ckfinite);
            return il;
        }

        /// <summary>
        /// Compares two values. If the first value is less than the second,
        /// the integer value 1 (int32) is pushed onto the evaluation stack;
        /// otherwise 0 (int32) is pushed onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Clt"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Clt">OpCodes.Clt</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator clt(this ILGenerator il)
        {
            il.Emit(OpCodes.Clt);
            return il;
        }

        /// <summary>
        /// Compares the unsigned or unordered values value1 and value2
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Clt_Un"/>).
        /// If value1 is less than value2, then the integer value 1 (int32) is pushed onto the evaluation stack;
        /// otherwise 0 (int32) is pushed onto the evaluation stack.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Clt_Un">OpCodes.Clt_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator clt_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Clt_Un);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to natural int
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_I"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_I">OpCodes.Conv_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_i(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_I);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to int8, then extends (pads) it to int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_I1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_I1">OpCodes.Conv_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_i1(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_I1);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to int16, then extends (pads) it to int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_I2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_I2">OpCodes.Conv_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_i2(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_I2);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_I4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_I4">OpCodes.Conv_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_i4(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_I4);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to int64
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_I8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_I8">OpCodes.Conv_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_i8(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_I8);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to the specified type.
        /// </summary>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv(this ILGenerator il, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte: il.conv_i1(); break;
                case TypeCode.Int16: il.conv_i2(); break;
                case TypeCode.Int32: il.conv_i4(); break;
                case TypeCode.Int64: il.conv_i8(); break;

                case TypeCode.Byte: il.conv_u1(); break;
                case TypeCode.Char:
                case TypeCode.UInt16: il.conv_u2(); break;
                case TypeCode.UInt32: il.conv_u4(); break;
                case TypeCode.UInt64: il.conv_u8(); break;

                case TypeCode.Single: il.conv_r4(); break;
                case TypeCode.Double: il.conv_r8(); break;

                default:
                    throw CreateNotExpectedTypeException(type);
            }

            return il;
        }


        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I"/>) that
        /// converts the signed value on top of the evaluation stack to signed natural int,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_I">OpCodes.Conv_Ovf_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_i(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_I);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I1"/>) that
        /// converts the signed value on top of the evaluation stack to signed int8 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_I1">OpCodes.Conv_Ovf_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_i1(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_I1);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I1_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to signed int8 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_I1_Un">OpCodes.Conv_Ovf_I1_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_i1_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_I1_Un);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I2"/>) that
        /// converts the signed value on top of the evaluation stack to signed int16 and extending it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_I2">OpCodes.Conv_Ovf_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_i2(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_I2);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I2_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to signed int16 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_I2_Un">OpCodes.Conv_Ovf_I2_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_i2_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_I2_Un);
            return il;
        }

        /// <summary>
        /// Converts the signed value on top of the evaluation tack to signed int32, throwing <see cref="OverflowException"/> on overflow
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_I4">OpCodes.Conv_Ovf_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_i4(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_I2_Un);
            return il;
        }

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed int32, throwing <see cref="OverflowException"/> on overflow
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I4_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_I4_Un">OpCodes.Conv_Ovf_I4_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_i4_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_I4_Un);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I8"/>) that
        /// converts the signed value on top of the evaluation stack to signed int64,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_I8">OpCodes.Conv_Ovf_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_i8(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_I8);
            return il;
        }

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to signed int64, throwing <see cref="OverflowException"/> on overflow
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I8_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_I8_Un">OpCodes.Conv_Ovf_I8_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_i8_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_I8_Un);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_I_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to signed natural int,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_I_Un">OpCodes.Conv_Ovf_I_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_i_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_I_Un);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U"/>) that
        /// converts the signed value on top of the evaluation stack to unsigned natural int,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_U">OpCodes.Conv_Ovf_U</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_u(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_U);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U1"/>) that
        /// converts the signed value on top of the evaluation stack to unsigned int8 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_U1">OpCodes.Conv_Ovf_U1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_u1(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_U1);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U1_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to unsigned int8 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_U1_Un">OpCodes.Conv_Ovf_U1_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_u1_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_U1_Un);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U2"/>) that
        /// converts the signed value on top of the evaluation stack to unsigned int16 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_U2">OpCodes.Conv_Ovf_U2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_u2(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_U2);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U2_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to unsigned int16 and extends it to int32,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_U2_Un">OpCodes.Conv_Ovf_U2_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_u2_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_U2_Un);
            return il;
        }

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned int32, throwing <see cref="OverflowException"/> on overflow
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_U4">OpCodes.Conv_Ovf_U4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_u4(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_U4);
            return il;
        }

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned int32, throwing <see cref="OverflowException"/> on overflow
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U4_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_U4_Un">OpCodes.Conv_Ovf_U4_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_u4_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_U4_Un);
            return il;
        }

        /// <summary>
        /// Converts the signed value on top of the evaluation stack to unsigned int64, throwing <see cref="OverflowException"/> on overflow
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_U8">OpCodes.Conv_Ovf_U8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_u8(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_U8);
            return il;
        }

        /// <summary>
        /// Converts the unsigned value on top of the evaluation stack to unsigned int64, throwing <see cref="OverflowException"/> on overflow
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U8_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_U8_Un">OpCodes.Conv_Ovf_U8_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_u8_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_U8_Un);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Conv_Ovf_U_Un"/>) that
        /// converts the unsigned value on top of the evaluation stack to unsigned natural int,
        /// throwing <see cref="OverflowException"/> on overflow.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_Ovf_U_Un">OpCodes.Conv_Ovf_U_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_ovf_u_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_Ovf_U_Un);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to float32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_R4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_R4">OpCodes.Conv_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_r4(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_R4);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to float64
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_R8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_R8">OpCodes.Conv_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_r8(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_R8);
            return il;
        }

        /// <summary>
        /// Converts the unsigned integer value on top of the evaluation stack to float32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_R_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_R_Un">OpCodes.Conv_R_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_r_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_R_Un);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned natural int, and extends it to natural int
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_U"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_U">OpCodes.Conv_U</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_u(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_U);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int8, and extends it to int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_U1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_U1">OpCodes.Conv_U1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_u1(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_U1);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int16, and extends it to int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_U2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_U2">OpCodes.Conv_U2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_u2(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_U2);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int32, and extends it to int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_U4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_U4">OpCodes.Conv_U4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_u4(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_U4);
            return il;
        }

        /// <summary>
        /// Converts the value on top of the evaluation stack to unsigned int64, and extends it to int64
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Conv_U8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Conv_U8">OpCodes.Conv_U8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator conv_u8(this ILGenerator il)
        {
            il.Emit(OpCodes.Conv_U8);
            return il;
        }

        /// <summary>
        /// Copies a specified number bytes from a source address to a destination address
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Cpblk"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Cpblk">OpCodes.Cpblk</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator cpblk(this ILGenerator il)
        {
            il.Emit(OpCodes.Cpblk);
            return il;
        }

        /// <summary>
        /// Copies the value type located at the address of an object (type &amp;, * or natural int) 
        /// to the address of the destination object (type &amp;, * or natural int)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Cpobj"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Cpobj">OpCodes.Cpobj</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator cpobj(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Cpobj, type);
            return il;
        }

        /// <summary>
        /// Divides two values and pushes the result as a floating-point (type F) or
        /// quotient (type int32) onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Div"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Div">OpCodes.Div</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator div(this ILGenerator il)
        {
            il.Emit(OpCodes.Div);
            return il;
        }

        /// <summary>
        /// Divides two unsigned integer values and pushes the result (int32) onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Div_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Div_Un">OpCodes.Div_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator div_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Div_Un);
            return il;
        }

        /// <summary>
        /// Copies the current topmost value on the evaluation stack, and then pushes the copy onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Dup"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Dup">OpCodes.Dup</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator dup(this ILGenerator il)
        {
            il.Emit(OpCodes.Dup);
            return il;
        }

        /// <summary>
        /// Transfers control from the filter clause of an exception back to
        /// the Common Language Infrastructure (CLI) exception handler
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Endfilter"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Endfilter">OpCodes.Endfilter</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator endfilter(this ILGenerator il)
        {
            il.Emit(OpCodes.Endfilter);
            return il;
        }

        /// <summary>
        /// Transfers control from the fault or finally clause of an exception block back to
        /// the Common Language Infrastructure (CLI) exception handler
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Endfinally"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Endfinally">OpCodes.Endfinally</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator endfinally(this ILGenerator il)
        {
            il.Emit(OpCodes.Endfinally);
            return il;
        }

        /// <summary>
        /// Initializes a specified block of memory at a specific address to a given size and initial value
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Initblk"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Initblk">OpCodes.Initblk</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator initblk(this ILGenerator il)
        {
            il.Emit(OpCodes.Initblk);
            return il;
        }

        /// <summary>
        /// Initializes all the fields of the object at a specific address to a null reference or 
        /// a 0 of the appropriate primitive type
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Initobj"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Initobj">OpCodes.Initobj</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator initobj(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Initobj, type);
            return il;
        }

        /// <summary>
        /// Tests whether an object reference (type O) is an instance of a particular class
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Isinst"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Isinst">OpCodes.Isinst</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator isinst(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Isinst, type);
            return il;
        }

        /// <summary>
        /// Exits current method and jumps to specified method
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Jmp"/>, methodInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodInfo">The method to be jumped.</param>
        /// <seealso cref="OpCodes.Jmp">OpCodes.Jmp</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator jmp(this ILGenerator il, MethodInfo methodInfo)
        {
            il.Emit(OpCodes.Jmp, methodInfo);
            return il;
        }

        /// <summary>
        /// Loads an argument (referenced by a specified index value) onto the stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldarg"/>, short).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Index of the argument that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarg">OpCodes.Ldarg</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator ldarg(this ILGenerator il, short index)
        {
            il.Emit(OpCodes.Ldarg, index);
            return il;
        }

        /// <summary>
        /// Loads an argument onto the stack.
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodBuilder">A <see cref="MethodBuilder"/> of the current method.</param>
        /// <param name="parameterInfo">A <see cref="ParameterInfo"/> representing a parameter.</param>
        /// <param name="box">True, if parameter must be converted to a reference.</param>
        /// <seealso cref="ldarg(ILGenerator,MethodBuilder,ParameterInfo)"/>
        public static ILGenerator ldargEx(this ILGenerator il, MethodBuilder methodBuilder, ParameterInfo parameterInfo, bool box)
        {
            il.ldarg(methodBuilder, parameterInfo);

            Type type = parameterInfo.ParameterType;

            if (parameterInfo.ParameterType.IsByRef)
                type = parameterInfo.ParameterType.GetElementType();

            if (parameterInfo.ParameterType.IsByRef)
            {
                if (type.IsValueType && type.IsPrimitive == false)
                    il.ldobj(type);
                else
                    il.ldind(type);
            }

            if (box)
                il.boxIfValueType(type);

            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg"/>, short) or 
        /// ILGenerator.Emit(<see cref="OpCodes.Ldarg_S"/>, byte) that
        /// loads an argument (referenced by a specified index value) onto the stack.
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Index of the argument that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarg">OpCodes.Ldarg</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator ldarg(this ILGenerator il, int index)
        {
            switch (index)
            {
                case 0: il.ldarg_0(); break;
                case 1: il.ldarg_1(); break;
                case 2: il.ldarg_2(); break;
                case 3: il.ldarg_3(); break;
                default:
                    if (index <= byte.MaxValue)
                        il.ldarg_s((byte) index);
                    else if (index <= short.MaxValue)
                        il.ldarg((short) index);
                    else
                        throw new ArgumentOutOfRangeException("index");

                    break;
            }

            return il;
        }

        /// <summary>
        /// Loads an argument onto the stack.
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodBuilder">A <see cref="MethodBuilder"/> of the current method.</param>
        /// <param name="parameterInfo">A <see cref="ParameterInfo"/> representing a parameter.</param>
        public static ILGenerator ldarg(this ILGenerator il, MethodBuilder methodBuilder, ParameterInfo parameterInfo)
        {
            if (methodBuilder == null) throw new ArgumentNullException("methodBuilder");
            if (parameterInfo == null) throw new ArgumentNullException("parameterInfo");

            return il.ldarg(parameterInfo.Position + (methodBuilder.IsStatic ? 0 : 1));
        }

        /// <summary>
        /// Load an argument address onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldarga"/>, short).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Index of the address addr of the argument that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarga">OpCodes.Ldarga</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator ldarga(this ILGenerator il, short index)
        {
            il.Emit(OpCodes.Ldarga, index);
            return il;
        }

        /// <summary>
        /// Load an argument address, in short form, onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldarga_S"/>, byte).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Index of the address addr of the argument that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarga_S">OpCodes.Ldarga_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        public static ILGenerator ldarga_s(this ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Ldarga_S, index);
            return il;
        }

        /// <summary>
        /// Load an argument address onto the evaluation stack.
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Index of the address addr of the argument that is pushed onto the stack.</param>
        public static ILGenerator ldarga(this ILGenerator il, int index)
        {
            if (index <= byte.MaxValue)
                il.ldarga_s((byte) index);
            else if (index <= short.MaxValue)
                il.ldarga((short) index);
            else
                throw new ArgumentOutOfRangeException("index");

            return il;
        }

        /// <summary>
        /// Loads an argument address onto the stack.
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodBuilder">A <see cref="MethodBuilder"/> of the current method.</param>
        /// <param name="parameterInfo">A <see cref="ParameterInfo"/> representing a parameter.</param>
        public static ILGenerator ldarga(this ILGenerator il, MethodBuilder methodBuilder, ParameterInfo parameterInfo)
        {
            if (methodBuilder == null) throw new ArgumentNullException("methodBuilder");
            if (parameterInfo == null) throw new ArgumentNullException("parameterInfo");

            return il.ldarga(parameterInfo.Position + (methodBuilder.IsStatic ? 0 : 1));
        }


        /// <summary>
        /// Loads the argument at index 0 onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldarg_0"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldarg_0">OpCodes.Ldarg_0</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldarg_0(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
            return il;
        }

        /// <summary>
        /// Loads the argument at index 1 onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldarg_1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldarg_1">OpCodes.Ldarg_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldarg_1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_1);
            return il;
        }

        /// <summary>
        /// Loads the argument at index 2 onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldarg_2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldarg_2">OpCodes.Ldarg_2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldarg_2(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_2);
            return il;
        }

        /// <summary>
        /// Loads the argument at index 3 onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldarg_3"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldarg_3">OpCodes.Ldarg_3</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldarg_3(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_3);
            return il;
        }

        /// <summary>
        /// Loads the argument (referenced by a specified short form index) onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldarg_S"/>, byte).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Index of the argument value that is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldarg_S">OpCodes.Ldarg_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        public static ILGenerator ldarg_s(this ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Ldarg_S, index);
            return il;
        }

        /// <summary>
        /// Pushes a supplied value of type int32 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_0"/> or <see cref="OpCodes.Ldc_I4_1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <param name="b">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_I4">OpCodes.Ldc_I4_0</seealso>
        /// <seealso cref="OpCodes.Ldc_I4">OpCodes.Ldc_I4_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,int)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_bool(this ILGenerator il, bool b)
        {
            il.Emit(b ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            return il;
        }

        /// <summary>
        /// Pushes a supplied value of type int32 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4"/>, int).
        /// </summary>
        /// <param name="il"/>
        /// <param name="num">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_I4">OpCodes.Ldc_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,int)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4(this ILGenerator il, int num)
        {
            il.Emit(OpCodes.Ldc_I4, num);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 0 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_0"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldc_I4_0">OpCodes.Ldc_I4_0</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_0(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_0);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 1 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldc_I4_1">OpCodes.Ldc_I4_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_1);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 2 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldc_I4_2">OpCodes.Ldc_I4_2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_2(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_2);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 3 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_3"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldc_I4_3">OpCodes.Ldc_I4_3</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_3(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_3);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 4 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldc_I4_4">OpCodes.Ldc_I4_4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_4(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_4);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 5 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_5"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldc_I4_5">OpCodes.Ldc_I4_0</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_5(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_5);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 6 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_6"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldc_I4_6">OpCodes.Ldc_I4_6</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_6(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_6);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 7 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_7"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldc_I4_7">OpCodes.Ldc_I4_7</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_7(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_7);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of 8 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldc_I4_8">OpCodes.Ldc_I4_8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_8(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_8);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of -1 onto the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_M1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldc_I4_M1">OpCodes.Ldc_I4_M1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_m1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldc_I4_M1);
            return il;
        }

        /// <summary>
        /// Pushes the integer value of -1 onto the evaluation stack as an int32
        /// by calling the best form of ILGenerator.Emit(Ldc_I4_X).
        /// </summary>
        /// <seealso cref="ldc_i4"/>
        public static ILGenerator ldc_i4_(this ILGenerator il, int num)
        {
            switch (num)
            {
                case -1: il.ldc_i4_m1(); break;
                case 0: il.ldc_i4_0(); break;
                case 1: il.ldc_i4_1(); break;
                case 2: il.ldc_i4_2(); break;
                case 3: il.ldc_i4_3(); break;
                case 4: il.ldc_i4_4(); break;
                case 5: il.ldc_i4_5(); break;
                case 6: il.ldc_i4_6(); break;
                case 7: il.ldc_i4_7(); break;
                case 8: il.ldc_i4_8(); break;
                default:
                    if (num >= sbyte.MinValue && num <= sbyte.MaxValue)
                        il.ldc_i4_s((sbyte) num);
                    else
                        il.ldc_i4(num);

                    break;
            }

            return il;
        }

        /// <summary>
        /// Pushes the supplied int8 value onto the evaluation stack as an int32, short form
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I4_S"/>, byte).
        /// </summary>
        /// <param name="il"/>
        /// <param name="num">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_I4_S">OpCodes.Ldc_I4_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i4_s(this ILGenerator il, sbyte num)
        {
            il.Emit(OpCodes.Ldc_I4_S, num);
            return il;
        }

        /// <summary>
        /// Pushes a supplied value of type int64 onto the evaluation stack as an int64
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_I8"/>, long).
        /// </summary>
        /// <param name="il"/>
        /// <param name="num">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_I8">OpCodes.Ldc_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,long)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_i8(this ILGenerator il, long num)
        {
            il.Emit(OpCodes.Ldc_I8, num);
            return il;
        }

        /// <summary>
        /// Pushes a supplied value of type float32 onto the evaluation stack as type F (float)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_R4"/>, float).
        /// </summary>
        /// <param name="il"/>
        /// <param name="num">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_R4">OpCodes.Ldc_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,float)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_r4(this ILGenerator il, float num)
        {
            il.Emit(OpCodes.Ldc_R4, num);
            return il;
        }

        /// <summary>
        /// Pushes a supplied value of type float64 onto the evaluation stack as type F (float)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldc_R8"/>, double).
        /// </summary>
        /// <param name="il"/>
        /// <param name="num">The value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldc_R8">OpCodes.Ldc_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,double)">ILGenerator.Emit</seealso>
        public static ILGenerator ldc_r8(this ILGenerator il, double num)
        {
            il.Emit(OpCodes.Ldc_R8, num);
            return il;
        }

        /// <summary>
        /// Loads the address of the array element at a specified array index onto the top of the evaluation stack 
        /// as type &amp; (managed pointer)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelema"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Ldelema">OpCodes.Ldelema</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelema(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Ldelema, type);
            return il;
        }

        /// <summary>
        /// Loads the element with type natural int at a specified array index onto the top of the evaluation stack 
        /// as a natural int
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_I"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_I">OpCodes.Ldelem_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_i(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_I);
            return il;
        }

        /// <summary>
        /// Loads the element with type int8 at a specified array index onto the top of the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_I1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_I1">OpCodes.Ldelem_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_i1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_I1);
            return il;
        }

        /// <summary>
        /// Loads the element with type int16 at a specified array index onto the top of the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_I2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_I2">OpCodes.Ldelem_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_i2(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_I2);
            return il;
        }

        /// <summary>
        /// Loads the element with type int32 at a specified array index onto the top of the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_I4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_I4">OpCodes.Ldelem_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_i4(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_I4);
            return il;
        }

        /// <summary>
        /// Loads the element with type int64 at a specified array index onto the top of the evaluation stack as an int64
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_I8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_I8">OpCodes.Ldelem_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_i8(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_I8);
            return il;
        }

        /// <summary>
        /// Loads the element with type float32 at a specified array index onto the top of the evaluation stack as type F (float)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_R4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_R4">OpCodes.Ldelem_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_r4(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_R4);
            return il;
        }

        /// <summary>
        /// Loads the element with type float64 at a specified array index onto the top of the evaluation stack as type F (float)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_R8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_R8">OpCodes.Ldelem_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_r8(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_R8);
            return il;
        }

        /// <summary>
        /// Loads the element containing an object reference at a specified array index 
        /// onto the top of the evaluation stack as type O (object reference)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_Ref"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_Ref">OpCodes.Ldelem_Ref</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_ref(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_Ref);
            return il;
        }

        /// <summary>
        /// Loads the element with type unsigned int8 at a specified array index onto the top of the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_U1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_U1">OpCodes.Ldelem_U1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_u1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_U1);
            return il;
        }

        /// <summary>
        /// Loads the element with type unsigned int16 at a specified array index 
        /// onto the top of the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_U2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_U2">OpCodes.Ldelem_U2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_u2(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_U2);
            return il;
        }

        /// <summary>
        /// Loads the element with type unsigned int32 at a specified array index 
        /// onto the top of the evaluation stack as an int32
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldelem_U4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldelem_U4">OpCodes.Ldelem_U4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldelem_u4(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldelem_U4);
            return il;
        }

        /// <summary>
        /// Finds the value of a field in the object whose reference is currently on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldfld"/>, fieldInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldfld">OpCodes.Ldfld</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator ldfld(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Ldfld, fieldInfo);
            return il;
        }

        /// <summary>
        /// Finds the address of a field in the object whose reference is currently on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldflda"/>, fieldInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldflda">OpCodes.Ldflda</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator ldflda(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Ldflda, fieldInfo);
            return il;
        }

        /// <summary>
        /// Pushes an unmanaged pointer (type natural int) to the native code implementing a specific method 
        /// onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldftn"/>, methodInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Ldftn">OpCodes.Ldftn</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator ldftn(this ILGenerator il, MethodInfo methodInfo)
        {
            il.Emit(OpCodes.Ldftn, methodInfo);
            return il;
        }

        /// <summary>
        /// Loads a value of type natural int as a natural int onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_I"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_I">OpCodes.Ldind_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_i(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_I);
            return il;
        }

        /// <summary>
        /// Loads a value of type int8 as an int32 onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_I1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_I1">OpCodes.Ldind_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_i1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_I1);
            return il;
        }

        /// <summary>
        /// Loads a value of type int16 as an int32 onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_I2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_I2">OpCodes.Ldind_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_i2(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_I2);
            return il;
        }

        /// <summary>
        /// Loads a value of type int32 as an int32 onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_I4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_I4">OpCodes.Ldind_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_i4(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_I4);
            return il;
        }

        /// <summary>
        /// Loads a value of type int64 as an int64 onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_I8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_I8">OpCodes.Ldind_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_i8(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_I8);
            return il;
        }

        /// <summary>
        /// Loads a value of type float32 as a type F (float) onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_R4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_R4">OpCodes.Ldind_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_r4(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_R4);
            return il;
        }

        /// <summary>
        /// Loads a value of type float64 as a type F (float) onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_R8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_R8">OpCodes.Ldind_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_r8(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_R8);
            return il;
        }

        /// <summary>
        /// Loads an object reference as a type O (object reference) onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_Ref"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_Ref">OpCodes.Ldind_Ref</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_ref(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_Ref);
            return il;
        }

        /// <summary>
        /// Loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_U1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_U1">OpCodes.Ldind_U1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_u1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_U1);
            return il;
        }

        /// <summary>
        /// Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_U2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_U2">OpCodes.Ldind_U2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_u2(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_U2);
            return il;
        }

        /// <summary>
        /// Loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldind_U4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldind_U4">OpCodes.Ldind_U4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldind_u4(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldind_U4);
            return il;
        }

        /// <summary>
        /// Loads a value of the type from a supplied address.
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type.</param>
        public static ILGenerator ldind(this ILGenerator il, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte: il.ldind_i1(); break;

                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16: il.ldind_i2(); break;

                case TypeCode.Int32:
                case TypeCode.UInt32: il.ldind_i4(); break;

                case TypeCode.Int64:
                case TypeCode.UInt64: il.ldind_i8(); break;

                case TypeCode.Single: il.ldind_r4(); break;
                case TypeCode.Double: il.ldind_r8(); break;

                default:
                    if (type.IsClass)
                        il.ldind_ref();
                    else if (type.IsValueType)
                        il.stobj(type);
                    else
                        throw CreateNotExpectedTypeException(type);
                    break;
            }

            return il;
        }

        /// <summary>
        /// Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldlen"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldlen">OpCodes.Ldlen</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldlen(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldlen);
            return il;
        }

        /// <summary>
        /// Load an argument address onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldloc"/>, short).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Index of the local variable value pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Ldloc">OpCodes.Ldloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator ldloc(this ILGenerator il, short index)
        {
            il.Emit(OpCodes.Ldloc, index);
            return il;
        }

        /// <summary>
        /// Load an argument address onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldloc"/>, <see cref="LocalBuilder"/>).
        /// </summary>
        /// <param name="il"/>
        /// <param name="localBuilder">Local variable builder.</param>
        /// <seealso cref="OpCodes.Ldloc">OpCodes.Ldloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator ldloc(this ILGenerator il, LocalBuilder localBuilder)
        {
            il.Emit(OpCodes.Ldloc, localBuilder);
            return il;
        }

        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldloca"/>, short).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Index of the local variable.</param>
        /// <seealso cref="OpCodes.Ldloca">OpCodes.Ldloca</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator ldloca(this ILGenerator il, short index)
        {
            il.Emit(OpCodes.Ldloca, index);
            return il;
        }

        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack, short form
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldloca_S"/>, byte).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Index of the local variable.</param>
        /// <seealso cref="OpCodes.Ldloca_S">OpCodes.Ldloca_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        public static ILGenerator ldloca_s(this ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Ldloca_S, index);
            return il;
        }

        /// <summary>
        /// Loads the address of the local variable at a specific index onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldloca"/>, <see cref="LocalBuilder"/>).
        /// </summary>
        /// <param name="il"/>
        /// <param name="local">A <see cref="LocalBuilder"/> representing the local variable.</param>
        /// <seealso cref="OpCodes.Ldloca">OpCodes.Ldloca</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator ldloca(this ILGenerator il, LocalBuilder local)
        {
            il.Emit(OpCodes.Ldloca, local);
            return il;
        }

        /// <summary>
        /// Loads the local variable at index 0 onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldloc_0"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldloc_0">OpCodes.Ldloc_0</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldloc_0(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldloc_0);
            return il;
        }

        /// <summary>
        /// Loads the local variable at index 1 onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldloc_1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldloc_1">OpCodes.Ldloc_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldloc_1(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldloc_1);
            return il;
        }

        /// <summary>
        /// Loads the local variable at index 2 onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldloc_2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldloc_2">OpCodes.Ldloc_2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldloc_2(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldloc_2);
            return il;
        }

        /// <summary>
        /// Loads the local variable at index 3 onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldloc_3"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldloc_3">OpCodes.Ldloc_3</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldloc_3(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldloc_3);
            return il;
        }

        /// <summary>
        /// Loads the local variable at a specific index onto the evaluation stack, short form
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldloc_S"/>, byte).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Index of the local variable.</param>
        /// <seealso cref="OpCodes.Ldloc_S">OpCodes.Ldloc_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        public static ILGenerator ldloc_s(this ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Ldloca_S, index);
            return il;
        }

        /// <summary>
        /// Pushes a null reference (type O) onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldnull"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ldnull">OpCodes.Ldnull</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ldnull(this ILGenerator il)
        {
            il.Emit(OpCodes.Ldnull);
            return il;
        }

        /// <summary>
        /// Copies the value type object pointed to by an address to the top of the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldobj"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Ldobj">OpCodes.Ldobj</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator ldobj(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Ldobj, type);
            return il;
        }

        /// <summary>
        /// Pushes the value of a static field onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldsfld"/>, fieldInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldsfld">OpCodes.Ldsfld</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator ldsfld(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Ldsfld, fieldInfo);
            return il;
        }

        /// <summary>
        /// Pushes the address of a static field onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldsflda"/>, fieldInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldsflda">OpCodes.Ldsflda</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator ldsflda(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Ldsflda, fieldInfo);
            return il;
        }

        /// <summary>
        /// Calls <see cref="ldstr"/> -or- <see cref="ldnull"/>,
        /// if given string is a null reference.
        /// </summary>
        /// <param name="il"/>
        /// <param name="str">The String to be emitted.</param>
        /// <seealso cref="ldstr"/>
        /// <seealso cref="ldnull"/>
        public static ILGenerator ldstrEx(this ILGenerator il, string str)
        {
            return str == null ? il.ldnull() : il.ldstr(str);
        }

        /// <summary>
        /// Pushes a new object reference to a string literal stored in the metadata
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldstr"/>, string).
        /// </summary>
        /// <param name="il"/>
        /// <param name="str">The String to be emitted.</param>
        /// <seealso cref="OpCodes.Ldstr">OpCodes.Ldstr</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator ldstr(this ILGenerator il, string str)
        {
            il.Emit(OpCodes.Ldstr, str);
            return il;
        }

        //        /// <summary>
        //        /// Pushes a new object reference to a string literal stored in the metadata
        //        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldstr"/>, string).
        //        /// </summary>
        //        /// <param name="il"/>
        //        /// <param name="nameOrIndex">The <see cref="nameOrIndex"/> to be emitted.</param>
        //        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        //        public static ILGenerator ldNameOrIndex(this ILGenerator il, NameOrIndexParameter nameOrIndex)
        //        {
        //            return nameOrIndex.ByName
        //                       ? il.ldstr(nameOrIndex.Name).call(typeof(NameOrIndexParameter), "op_Implicit", typeof(string))
        //                       : il.ldc_i4_(nameOrIndex.Index).call(typeof(NameOrIndexParameter), "op_Implicit", typeof(int));
        //        }

        /// <summary>
        /// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldtoken"/>, methodInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Ldtoken">OpCodes.Ldtoken</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator ldtoken(this ILGenerator il, MethodInfo methodInfo)
        {
            il.Emit(OpCodes.Ldtoken, methodInfo);
            return il;
        }

        /// <summary>
        /// Converts a metadata token to its runtime representation, 
        /// pushing it onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldtoken"/>, fieldInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Ldtoken">OpCodes.Ldtoken</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator ldtoken(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Ldtoken, fieldInfo);
            return il;
        }

        /// <summary>
        /// Converts a metadata token to its runtime representation, pushing it onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldtoken"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Ldtoken">OpCodes.Ldtoken</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator ldtoken(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Ldtoken, type);
            return il;
        }

        /// <summary>
        /// Pushes an unmanaged pointer (type natural int) to the native code implementing a particular virtual method 
        /// associated with a specified object onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ldvirtftn"/>, methodInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="methodInfo">The method to be called.</param>
        /// <seealso cref="OpCodes.Ldvirtftn">OpCodes.Ldvirtftn</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,MethodInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator ldvirtftn(this ILGenerator il, MethodInfo methodInfo)
        {
            il.Emit(OpCodes.Ldvirtftn, methodInfo);
            return il;
        }

        /// <summary>
        /// Exits a protected region of code, unconditionally tranferring control to a specific target instruction
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Leave"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label.</param>
        /// <seealso cref="OpCodes.Leave">OpCodes.Leave</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator leave(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Leave, label);
            return il;
        }

        /// <summary>
        /// Exits a protected region of code, unconditionally transferring control to a target instruction (short form)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Leave_S"/>, label).
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label.</param>
        /// <seealso cref="OpCodes.Leave_S">OpCodes.Leave_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator leave_s(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Leave_S, label);
            return il;
        }

        /// <summary>
        /// Allocates a certain number of bytes from the local dynamic memory pool and pushes the address 
        /// (a transient pointer, type *) of the first allocated byte onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Localloc"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Localloc">OpCodes.Localloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator localloc(this ILGenerator il)
        {
            il.Emit(OpCodes.Localloc);
            return il;
        }

        /// <summary>
        /// Pushes a typed reference to an instance of a specific type onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Mkrefany"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Mkrefany">OpCodes.Mkrefany</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator mkrefany(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Mkrefany, type);
            return il;
        }

        /// <summary>
        /// Multiplies two values and pushes the result on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Mul"/>).
        /// (a transient pointer, type *) of the first allocated byte onto the evaluation stack.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Mul">OpCodes.Mul</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator mul(this ILGenerator il)
        {
            il.Emit(OpCodes.Mul);
            return il;
        }

        /// <summary>
        /// Multiplies two integer values, performs an overflow check, 
        /// and pushes the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Mul_Ovf"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Mul_Ovf">OpCodes.Mul_Ovf</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator mul_ovf(this ILGenerator il)
        {
            il.Emit(OpCodes.Mul_Ovf);
            return il;
        }

        /// <summary>
        /// Multiplies two unsigned integer values, performs an overflow check, 
        /// and pushes the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Mul_Ovf_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Mul_Ovf_Un">OpCodes.Mul_Ovf_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator mul_ovf_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Mul_Ovf_Un);
            return il;
        }

        /// <summary>
        /// Negates a value and pushes the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Neg"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Neg">OpCodes.Neg</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator neg(this ILGenerator il)
        {
            il.Emit(OpCodes.Neg);
            return il;
        }

        /// <summary>
        /// Pushes an object reference to a new zero-based, one-dimensional array whose elements 
        /// are of a specific type onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Newarr"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Newarr">OpCodes.Newarr</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator newarr(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Newarr, type);
            return il;
        }

        /// <summary>
        /// Creates a new object or a new instance of a value type,
        /// pushing an object reference (type O) onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Newobj"/>, <see cref="ConstructorInfo"/>).
        /// </summary>
        /// <param name="il"/>
        /// <param name="constructorInfo">A <see cref="ConstructorInfo"/> representing a constructor.</param>
        /// <seealso cref="OpCodes.Newobj">OpCodes.Newobj</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,ConstructorInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator newobj(this ILGenerator il, ConstructorInfo constructorInfo)
        {
            il.Emit(OpCodes.Newobj, constructorInfo);
            return il;
        }

        /// <summary>
        /// Creates a new object or a new instance of a value type,
        /// pushing an object reference (type O) onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Newobj"/>, ConstructorInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A type.</param>
        /// <param name="parameters">An array of System.Type objects representing
        /// the number, order, and type of the parameters for the desired constructor.
        /// -or- An empty array of System.Type objects, to get a constructor that takes
        /// no parameters. Such an empty array is provided by the static field System.Type.EmptyTypes.</param>
        public static ILGenerator newobj(this ILGenerator il, Type type, params Type[] parameters)
        {
            if (type == null) throw new ArgumentNullException("type");

            var ci = type.GetConstructor(parameters);

            return il.newobj(ci);
        }

        /// <summary>
        /// Fills space if opcodes are patched. No meaningful operation is performed although 
        /// a processing cycle can be consumed
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Nop"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Nop">OpCodes.Nop</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator nop(this ILGenerator il)
        {
            il.Emit(OpCodes.Nop);
            return il;
        }

        /// <summary>
        /// Computes the bitwise complement of the integer value on top of the stack 
        /// and pushes the result onto the evaluation stack as the same type
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Not"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Not">OpCodes.Not</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator not(this ILGenerator il)
        {
            il.Emit(OpCodes.Not);
            return il;
        }

        /// <summary>
        /// Compute the bitwise complement of the two integer values on top of the stack and 
        /// pushes the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Or"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Or">OpCodes.Or</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator or(this ILGenerator il)
        {
            il.Emit(OpCodes.Or);
            return il;
        }

        /// <summary>
        /// Removes the value currently on top of the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Pop"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Pop">OpCodes.Pop</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator pop(this ILGenerator il)
        {
            il.Emit(OpCodes.Pop);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Refanytype"/>) that
        /// specifies that the subsequent array address operation performs
        /// no type check at run time, and that it returns a managed pointer
        /// whose mutability is restricted.
        /// </summary>
        /// <seealso cref="OpCodes.Refanytype">OpCodes.Refanytype</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator @readonly(this ILGenerator il)
        {
            il.Emit(OpCodes.Readonly);
            return il;
        }

        /// <summary>
        /// Retrieves the type token embedded in a typed reference
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Refanytype"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Refanytype">OpCodes.Refanytype</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator refanytype(this ILGenerator il)
        {
            il.Emit(OpCodes.Refanytype);
            return il;
        }

        /// <summary>
        /// Retrieves the address (type &amp;) embedded in a typed reference
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Refanyval"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Refanyval">OpCodes.Refanyval</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator refanyval(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Refanyval, type);
            return il;
        }

        /// <summary>
        /// Divides two values and pushes the remainder onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Rem"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Rem">OpCodes.Rem</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator rem(this ILGenerator il)
        {
            il.Emit(OpCodes.Rem);
            return il;
        }

        /// <summary>
        /// Divides two unsigned values and pushes the remainder onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Rem_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Rem_Un">OpCodes.Rem_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator rem_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Rem_Un);
            return il;
        }

        /// <summary>
        /// Returns from the current method, pushing a return value (if present) 
        /// from the caller's evaluation stack onto the callee's evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Ret"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Ret">OpCodes.Ret</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator ret(this ILGenerator il)
        {
            il.Emit(OpCodes.Ret);
            return il;
        }

        /// <summary>
        /// Rethrows the current exception
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Rethrow"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Rethrow">OpCodes.Rethrow</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator rethrow(this ILGenerator il)
        {
            il.Emit(OpCodes.Rethrow);
            return il;
        }

        /// <summary>
        /// Shifts an integer value to the left (in zeroes) by a specified number of bits,
        /// pushing the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Shl"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Shl">OpCodes.Shl</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator shl(this ILGenerator il)
        {
            il.Emit(OpCodes.Shl);
            return il;
        }

        /// <summary>
        /// Shifts an integer value (in sign) to the right by a specified number of bits,
        /// pushing the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Shr"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Shr">OpCodes.Shr</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator shr(this ILGenerator il)
        {
            il.Emit(OpCodes.Shr);
            return il;
        }

        /// <summary>
        /// Shifts an unsigned integer value (in zeroes) to the right by a specified number of bits,
        /// pushing the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Shr_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Shr_Un">OpCodes.Shr_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator shr_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Shr_Un);
            return il;
        }

        /// <summary>
        /// Pushes the size, in bytes, of a supplied value type onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Sizeof"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Sizeof">OpCodes.Sizeof</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator @sizeof(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Sizeof, type);
            return il;
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at a specified index
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Starg"/>, short).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Slot index.</param>
        /// <seealso cref="OpCodes.Starg">OpCodes.Starg</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator starg(this ILGenerator il, short index)
        {
            il.Emit(OpCodes.Starg, index);
            return il;
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at a specified index,
        /// short form
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Starg_S"/>, byte).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Slot index.</param>
        /// <seealso cref="OpCodes.Starg_S">OpCodes.Starg_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,byte)">ILGenerator.Emit</seealso>
        public static ILGenerator starg_s(this ILGenerator il, byte index)
        {
            il.Emit(OpCodes.Starg_S, index);
            return il;
        }

        /// <summary>
        /// Stores the value on top of the evaluation stack in the argument slot at a specified index.
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">Slot index.</param>
        /// <seealso cref="OpCodes.Starg">OpCodes.Starg</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator starg(this ILGenerator il, int index)
        {
            if (index < byte.MaxValue) il.starg_s((byte) index);
            else if (index < short.MaxValue) il.starg((short) index);
            else
                throw new ArgumentOutOfRangeException("index");

            return il;
        }

        /// <summary>
        /// Replaces the array element at a given index with the natural int value 
        /// on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stelem_I"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stelem_I">OpCodes.Stelem_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stelem_i(this ILGenerator il)
        {
            il.Emit(OpCodes.Stelem_I);
            return il;
        }

        /// <summary>
        /// Replaces the array element at a given index with the int8 value on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stelem_I1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stelem_I1">OpCodes.Stelem_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stelem_i1(this ILGenerator il)
        {
            il.Emit(OpCodes.Stelem_I1);
            return il;
        }

        /// <summary>
        /// Replaces the array element at a given index with the int16 value on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stelem_I2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stelem_I2">OpCodes.Stelem_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stelem_i2(this ILGenerator il)
        {
            il.Emit(OpCodes.Stelem_I2);
            return il;
        }

        /// <summary>
        /// Replaces the array element at a given index with the int32 value on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stelem_I4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stelem_I4">OpCodes.Stelem_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stelem_i4(this ILGenerator il)
        {
            il.Emit(OpCodes.Stelem_I4);
            return il;
        }

        /// <summary>
        /// Replaces the array element at a given index with the int64 value on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stelem_I8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stelem_I8">OpCodes.Stelem_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stelem_i8(this ILGenerator il)
        {
            il.Emit(OpCodes.Stelem_I8);
            return il;
        }

        /// <summary>
        /// Replaces the array element at a given index with the float32 value on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stelem_R4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stelem_R4">OpCodes.Stelem_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stelem_r4(this ILGenerator il)
        {
            il.Emit(OpCodes.Stelem_R4);
            return il;
        }

        /// <summary>
        /// Replaces the array element at a given index with the float64 value on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stelem_R8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stelem_R8">OpCodes.Stelem_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stelem_r8(this ILGenerator il)
        {
            il.Emit(OpCodes.Stelem_R8);
            return il;
        }

        /// <summary>
        /// Replaces the array element at a given index with the object ref value (type O)
        /// on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stelem_Ref"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stelem_Ref">OpCodes.Stelem_Ref</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stelem_ref(this ILGenerator il)
        {
            il.Emit(OpCodes.Stelem_Ref);
            return il;
        }

        /// <summary>
        /// Replaces the value stored in the field of an object reference or pointer with a new value
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stfld"/>, <see cref="FieldInfo"/>).
        /// </summary>
        /// <param name="il"/>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Stfld">OpCodes.Stfld</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator stfld(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Stfld, fieldInfo);
            return il;
        }

        /// <summary>
        /// Stores a value of type natural int at a supplied address
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stind_I"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stind_I">OpCodes.Stind_I</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stind_i(this ILGenerator il)
        {
            il.Emit(OpCodes.Stind_I);
            return il;
        }

        /// <summary>
        /// Stores a value of type int8 at a supplied address
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stind_I1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stind_I1">OpCodes.Stind_I1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stind_i1(this ILGenerator il)
        {
            il.Emit(OpCodes.Stind_I1);
            return il;
        }

        /// <summary>
        /// Stores a value of type int16 at a supplied address
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stind_I2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stind_I2">OpCodes.Stind_I2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stind_i2(this ILGenerator il)
        {
            il.Emit(OpCodes.Stind_I2);
            return il;
        }

        /// <summary>
        /// Stores a value of type int32 at a supplied address
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stind_I4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stind_I4">OpCodes.Stind_I4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stind_i4(this ILGenerator il)
        {
            il.Emit(OpCodes.Stind_I4);
            return il;
        }

        /// <summary>
        /// Stores a value of type int64 at a supplied address
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stind_I8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stind_I8">OpCodes.Stind_I8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stind_i8(this ILGenerator il)
        {
            il.Emit(OpCodes.Stind_I8);
            return il;
        }

        /// <summary>
        /// Stores a value of type float32 at a supplied address
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stind_R4"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stind_R4">OpCodes.Stind_R4</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stind_r4(this ILGenerator il)
        {
            il.Emit(OpCodes.Stind_R4);
            return il;
        }

        /// <summary>
        /// Stores a value of type float64 at a supplied address
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stind_R8"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stind_R8">OpCodes.Stind_R8</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stind_r8(this ILGenerator il)
        {
            il.Emit(OpCodes.Stind_R8);
            return il;
        }

        /// <summary>
        /// Stores an object reference value at a supplied address
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stind_Ref"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stind_Ref">OpCodes.Stind_Ref</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stind_ref(this ILGenerator il)
        {
            il.Emit(OpCodes.Stind_Ref);
            return il;
        }

        /// <summary>
        /// Stores a value of the type at a supplied address.
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type.</param>
        public static ILGenerator stind(this ILGenerator il, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte: il.stind_i1(); break;

                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16: il.stind_i2(); break;

                case TypeCode.Int32:
                case TypeCode.UInt32: il.stind_i4(); break;

                case TypeCode.Int64:
                case TypeCode.UInt64: il.stind_i8(); break;

                case TypeCode.Single: il.stind_r4(); break;
                case TypeCode.Double: il.stind_r8(); break;

                default:
                    if (type.IsClass)
                        il.stind_ref();
                    else if (type.IsValueType)
                        il.stobj(type);
                    else
                        throw CreateNotExpectedTypeException(type);
                    break;
            }

            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at a specified index
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stloc"/>, <see cref="LocalBuilder"/>).
        /// </summary>
        /// <param name="il"/>
        /// <param name="local">A local variable.</param>
        /// <seealso cref="OpCodes.Stloc">OpCodes.Stloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,LocalBuilder)">ILGenerator.Emit</seealso>
        public static ILGenerator stloc(this ILGenerator il, LocalBuilder local)
        {
            il.Emit(OpCodes.Stloc, local);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at a specified index
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stloc"/>, short).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">A local variable index.</param>
        /// <seealso cref="OpCodes.Stloc">OpCodes.Stloc</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator stloc(this ILGenerator il, short index)
        {
            if (index >= byte.MinValue && index <= byte.MaxValue)
                return il.stloc_s((byte) index);

            il.Emit(OpCodes.Stloc, index);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index 0
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stloc_0"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stloc_0">OpCodes.Stloc_0</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stloc_0(this ILGenerator il)
        {
            il.Emit(OpCodes.Stloc_0);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index 1
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stloc_1"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stloc_1">OpCodes.Stloc_1</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stloc_1(this ILGenerator il)
        {
            il.Emit(OpCodes.Stloc_1);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index 2
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stloc_2"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stloc_2">OpCodes.Stloc_2</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stloc_2(this ILGenerator il)
        {
            il.Emit(OpCodes.Stloc_2);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index 3
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stloc_3"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Stloc_3">OpCodes.Stloc_3</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator stloc_3(this ILGenerator il)
        {
            il.Emit(OpCodes.Stloc_3);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index (short form)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stloc_S"/>, <see cref="LocalBuilder"/>).
        /// </summary>
        /// <param name="il"/>
        /// <param name="local">A local variable.</param>
        /// <seealso cref="OpCodes.Stloc_S">OpCodes.Stloc_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,LocalBuilder)">ILGenerator.Emit</seealso>
        public static ILGenerator stloc_s(this ILGenerator il, LocalBuilder local)
        {
            il.Emit(OpCodes.Stloc_S, local);
            return il;
        }

        /// <summary>
        /// Pops the current value from the top of the evaluation stack and stores it 
        /// in the local variable list at index (short form)
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stloc_S"/>, byte).
        /// </summary>
        /// <param name="il"/>
        /// <param name="index">A local variable index.</param>
        /// <seealso cref="OpCodes.Stloc_S">OpCodes.Stloc_S</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
        public static ILGenerator stloc_s(this ILGenerator il, byte index)
        {
            switch (index)
            {
                case 0: il.stloc_0(); break;
                case 1: il.stloc_1(); break;
                case 2: il.stloc_2(); break;
                case 3: il.stloc_3(); break;

                default:
                    il.Emit(OpCodes.Stloc_S, index);
                    break;
            }

            return il;
        }

        /// <summary>
        /// Copies a value of a specified type from the evaluation stack into a supplied memory address
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stobj"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Stobj">OpCodes.Stobj</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator stobj(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Stobj, type);
            return il;
        }

        /// <summary>
        /// Replaces the value of a static field with a value from the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Stsfld"/>, fieldInfo).
        /// </summary>
        /// <param name="il"/>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        /// <seealso cref="OpCodes.Stsfld">OpCodes.Stsfld</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,FieldInfo)">ILGenerator.Emit</seealso>
        public static ILGenerator stsfld(this ILGenerator il, FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Stsfld, fieldInfo);
            return il;
        }

        /// <summary>
        /// Subtracts one value from another and pushes the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Sub"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Sub">OpCodes.Sub</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator sub(this ILGenerator il)
        {
            il.Emit(OpCodes.Sub);
            return il;
        }

        /// <summary>
        /// Subtracts one integer value from another, performs an overflow check,
        /// and pushes the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Sub_Ovf"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Sub_Ovf">OpCodes.Sub_Ovf</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator sub_ovf(this ILGenerator il)
        {
            il.Emit(OpCodes.Sub_Ovf);
            return il;
        }

        /// <summary>
        /// Subtracts one unsigned integer value from another, performs an overflow check,
        /// and pushes the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Sub_Ovf_Un"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Sub_Ovf_Un">OpCodes.Sub_Ovf_Un</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator sub_ovf_un(this ILGenerator il)
        {
            il.Emit(OpCodes.Sub_Ovf_Un);
            return il;
        }

        /// <summary>
        /// Implements a jump table
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Switch"/>, label[]).
        /// </summary>
        /// <param name="il"/>
        /// <param name="labels">The array of label objects to which to branch from this location.</param>
        /// <seealso cref="OpCodes.Switch">OpCodes.Switch</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label[])">ILGenerator.Emit</seealso>
        public static ILGenerator @switch(this ILGenerator il, Label[] labels)
        {
            il.Emit(OpCodes.Switch, labels);
            return il;
        }

        /// <summary>
        /// Performs a postfixed method call instruction such that the current method's stack frame 
        /// is removed before the actual call instruction is executed
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Tailcall"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Tailcall">OpCodes.Tailcall</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator tailcall(this ILGenerator il)
        {
            il.Emit(OpCodes.Tailcall);
            return il;
        }

        /// <summary>
        /// Throws the exception object currently on the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Throw"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Throw">OpCodes.Throw</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator @throw(this ILGenerator il)
        {
            il.Emit(OpCodes.Throw);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Unaligned"/>, label) that
        /// indicates that an address currently atop the evaluation stack might not be aligned 
        /// to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, 
        /// initblk, or cpblk instruction.
        /// </summary>
        /// <param name="il"/>
        /// <param name="label">The label to branch from this location.</param>
        /// <seealso cref="OpCodes.Unaligned">OpCodes.Unaligned</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Label)">ILGenerator.Emit</seealso>
        public static ILGenerator unaligned(this ILGenerator il, Label label)
        {
            il.Emit(OpCodes.Unaligned, label);
            return il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Unaligned"/>, long) that
        /// indicates that an address currently atop the evaluation stack might not be aligned 
        /// to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, 
        /// initblk, or cpblk instruction.
        /// </summary>
        /// <param name="il"/>
        /// <param name="addr">An address is pushed onto the stack.</param>
        /// <seealso cref="OpCodes.Unaligned">OpCodes.Unaligned</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,long)">ILGenerator.Emit</seealso>
        public static ILGenerator unaligned(this ILGenerator il, long addr)
        {
            il.Emit(OpCodes.Unaligned, addr);
            return il;
        }

        /// <summary>
        /// Converts the boxed representation of a value type to its unboxed form
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Unbox"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Unbox">OpCodes.Unbox</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator unbox(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Unbox, type);
            return il;
        }

        /// <summary>
        /// Converts the boxed representation of a value type to its unboxed form
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Unbox_Any"/>, type).
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Unbox_Any">OpCodes.Unbox_Any</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator unbox_any(this ILGenerator il, Type type)
        {
            il.Emit(OpCodes.Unbox_Any, type);
            return il;
        }

        /// <summary>
        /// Calls <see cref="unbox_any(ILGenerator, Type)"/> if given type is a value type.
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        /// <seealso cref="OpCodes.Unbox_Any">OpCodes.Unbox</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,Type)">ILGenerator.Emit</seealso>
        public static ILGenerator unboxIfValueType(this ILGenerator il, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.IsValueType ? il.unbox_any(type) : il;
        }

        /// <summary>
        /// Calls ILGenerator.Emit(<see cref="OpCodes.Volatile"/>) that
        /// specifies that an address currently atop the evaluation stack might be volatile, 
        /// and the results of reading that location cannot be cached or that multiple stores 
        /// to that location cannot be suppressed.
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Volatile">OpCodes.Volatile</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator @volatile(this ILGenerator il)
        {
            il.Emit(OpCodes.Volatile);
            return il;
        }

        /// <summary>
        /// Computes the bitwise XOR of the top two values on the evaluation stack, 
        /// pushing the result onto the evaluation stack
        /// by calling ILGenerator.Emit(<see cref="OpCodes.Xor"/>).
        /// </summary>
        /// <param name="il"/>
        /// <seealso cref="OpCodes.Xor">OpCodes.Xor</seealso>
        /// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode)">ILGenerator.Emit</seealso>
        public static ILGenerator xor(this ILGenerator il)
        {
            il.Emit(OpCodes.Xor);
            return il;
        }

        #endregion

        /// <summary>
        /// Loads default value of given type onto the evaluation stack.
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A Type</param>
        public static ILGenerator LoadInitValue(this ILGenerator il, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    il.ldc_i4_0();
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.ldc_i4_0().conv_i8();
                    break;

                case TypeCode.Single:
                case TypeCode.Double:
                    il.ldc_r4(0);
                    break;

                case TypeCode.String:
                    il.ldsfld(typeof (string).GetField("Empty"));
                    break;

                default:
                    if (type.IsClass || type.IsInterface)
                        il.ldnull();
                    else
                        throw CreateNotExpectedTypeException(type);
                    break;
            }

            return il;
        }

        /// <summary>
        /// Loads supplied object value (if possible) onto the evaluation stack.
        /// </summary>
        /// <param name="il"/>
        /// <param name="o">Any object instance or null reference.</param>
        /// <returns>True is a value was loaded, otherwise false.</returns>
        public static bool LoadWellKnownValue(this ILGenerator il, object o)
        {
            if (o == null)
                il.ldnull();
            else
                switch (Type.GetTypeCode(o.GetType()))
                {
                    case TypeCode.Boolean: il.ldc_bool((Boolean)o); break;
                    case TypeCode.Char: il.ldc_i4_((Char)o); break;
                    case TypeCode.Single: il.ldc_r4((Single)o); break;
                    case TypeCode.Double: il.ldc_r8((Double)o); break;
                    case TypeCode.String: il.ldstr((String)o); break;
                    case TypeCode.SByte: il.ldc_i4_((SByte)o); break;
                    case TypeCode.Int16: il.ldc_i4_((Int16)o); break;
                    case TypeCode.Int32: il.ldc_i4_((Int32)o); break;
                    case TypeCode.Int64: il.ldc_i8((Int64)o); break;
                    case TypeCode.Byte: il.ldc_i4_((Byte)o); break;
                    case TypeCode.UInt16: il.ldc_i4_((UInt16)o); break;
                    case TypeCode.UInt32: il.ldc_i4_(unchecked((Int32)(UInt32)o)); break;
                    case TypeCode.UInt64: il.ldc_i8(unchecked((Int64)(UInt64)o)); break;
                    default:
                        return false;
                }

            return true;
        }

        /// <summary>
        /// Initialize parameter with some default value.
        /// </summary>
        /// <param name="il"/>
        /// <param name="parameterInfo">A method parameter.</param>
        /// <param name="index">The parameter index.</param>
        public static ILGenerator Init(this ILGenerator il, ParameterInfo parameterInfo, int index)
        {
            if (parameterInfo == null) throw new ArgumentNullException("parameterInfo");

            var type = parameterInfo.ParameterType.GetUnderlyingType();

            if (parameterInfo.ParameterType.IsByRef)
            {
                type = type.GetElementType();

                return type.IsValueType && type.IsPrimitive == false
                           ? il.ldarg(index).initobj(type)
                           : il.ldarg(index).LoadInitValue(type).stind(type);
            }
            
            return type.IsValueType && type.IsPrimitive == false
                       ? il.ldarga(index).initobj(type)
                       : il.LoadInitValue(type).starg(index);
        }

        /// <summary>
        /// Initialize all output parameters with some default value.
        /// </summary>
        /// <param name="il"/>
        /// <param name="parameters">A method parameters array.</param>
        public static ILGenerator InitOutParameters(this ILGenerator il, ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo pi = parameters[i];

                if (pi.IsOut)
                    il.Init(pi, i + 1);
            }

            return il;
        }

        /// <summary>
        /// Initialize local variable with some default value.
        /// </summary>
        /// <param name="il"/>
        /// <param name="localBuilder">A method local variable.</param>
        public static ILGenerator Init(this ILGenerator il, LocalBuilder localBuilder)
        {
            if (localBuilder == null) throw new ArgumentNullException("localBuilder");

            var type = localBuilder.LocalType;

            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            return type.IsValueType && type.IsPrimitive == false
                       ? il.ldloca(localBuilder).initobj(type)
                       : il.LoadInitValue(type).stloc(localBuilder);
        }

        /// <summary>
        /// Loads a type instance at runtime.
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A type</param>
        public static ILGenerator LoadType(this ILGenerator il, Type type)
        {
            return type == null
                       ? il.ldnull()
                       : il.ldtoken(type).call(typeof (Type), "GetTypeFromHandle", typeof (RuntimeTypeHandle));
        }

        /// <summary>
        /// Loads a field instance at runtime.
        /// </summary>
        /// <param name="il"/>
        /// <param name="fieldInfo">A <see cref="FieldInfo"/> representing a field.</param>
        public static ILGenerator LoadField(this ILGenerator il, FieldInfo fieldInfo)
        {
            return fieldInfo.IsStatic ? il.ldsfld(fieldInfo) : il.ldarg_0().ldfld(fieldInfo);
        }

        /// <summary>
        /// Cast an object passed by reference to the specified type
        /// or unbox a boxed value type.
        /// </summary>
        /// <param name="il"/>
        /// <param name="type">A type</param>
        public static ILGenerator CastFromObject(this ILGenerator il, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return
                type == typeof (object)
                    ? il
                    : (type.IsValueType
                           ? il.unbox_any(type)
                           : il.castclass(type));
        }

        /// <summary>
        /// Cast an object passed by reference to the specified type
        /// or unbox a boxed value type unless <paramref name="expectedType"/>
        /// is a parent of <paramref name="actualType"/>.
        /// </summary>
        /// <param name="il"/>
        /// <param name="expectedType">A type required.</param>
        /// <param name="actualType">A type available.</param>
        public static ILGenerator CastIfNecessary(this ILGenerator il, Type expectedType, Type actualType)
        {
            if (expectedType == null) throw new ArgumentNullException("expectedType");
            if (actualType == null) throw new ArgumentNullException("actualType");

            return
                TypeExtensions.IsSameOrParent(expectedType, actualType)
                    ? il
                    : actualType.IsValueType
                            ? il.unbox_any(expectedType)
                            : il.castclass(expectedType);
        }

        /// <summary>
        /// Increase max stack size by specified delta.
        /// </summary>
        /// <param name="il"/>
        /// <param name="size">Number of bytes to enlarge max stack size.</param>
        public static void AddMaxStackSize(this ILGenerator il, int size)
        {
            // m_maxStackSize isn't public so we need some hacking here.
            //
            var fi = il.GetType().GetField(
                "m_maxStackSize", BindingFlags.Instance | BindingFlags.NonPublic);

            if (fi != null)
            {
                size += (int) fi.GetValue(il);
                fi.SetValue(il, size);
            }
        }

        private static Exception CreateNoSuchMethodException(Type type, string methodName)
        {
            return new InvalidOperationException(
                string.Format("Failed to query type '{0}' for method '{1}'.", type.FullName, methodName));
        }

        private static Exception CreateNotExpectedTypeException(Type type)
        {
            return new ArgumentException(
                string.Format("Type '{0}' is not expected.", type.FullName));
        }
    }
}