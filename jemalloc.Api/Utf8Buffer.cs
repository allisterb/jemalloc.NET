//B
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Buffers.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Primitives;
using System.Text.Utf8;

namespace jemalloc
{
    [DebuggerDisplay("{ToString()} utf8")]
    public readonly struct Utf8Buffer : IEquatable<Utf8Buffer>, IRetainable, IDisposable
    {
        #region Constructors
        public Utf8Buffer(ReadOnlySpan<byte> utf8Bytes) => buffer = new FixedBuffer<byte>(utf8Bytes);

        public Utf8Buffer(Utf8Span utf8Span) : this(utf8Span.Bytes) { }

        public Utf8Buffer(string utf16String)
        {
            if (utf16String == null)
            {
                throw new ArgumentNullException(nameof(utf16String));
            }

            if (utf16String == string.Empty)
            {
                buffer = new FixedBuffer<byte>();
            }
            else
            {
                byte[] b = Encoding.UTF8.GetBytes(utf16String);
                buffer = new FixedBuffer<byte>(b);
            }
        }

        private Utf8Buffer(byte[] utf8Bytes) => buffer = new FixedBuffer<byte>(utf8Bytes);

        #endregion

        #region Implemented members
        public void Retain() => buffer.Retain();
        public bool Release() => buffer.Release();
        #region Disposer
        void IDisposable.Dispose()
        {
            if (IsRetained)
            {
                throw new InvalidOperationException($"FixedString<[{this.Length}] has outstanding references.");
            }
            else
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
        #endregion
        #endregion

        #region Overidden members
        public override int GetHashCode() => Span.GetHashCode();

        public override string ToString() => Span.ToString();

        public override bool Equals(object obj)
        {
            if (obj is Utf8Buffer)
            {
                return Equals((Utf8Buffer)obj);
            }
            if (obj is string)
            {
                return Equals((string)obj);
            }

            return false;
        }
        #endregion

        #region Properties
        public static Utf8Buffer Empty => s_empty;

        public bool IsEmpty => Bytes.Length == 0;

        public bool IsRetained => buffer.IsRetained;

        public int Length => buffer.Length;

        public ReadOnlySpan<byte> Bytes => buffer.Span;

        internal Utf8Span Span
        {
            get
            {
                ThrowIfInvalid();
                return new Utf8Span(this);
            }
        }

        #endregion

        #region Operators
        public ref byte this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref this.buffer[index];
        }

        public static bool operator ==(Utf8Buffer left, Utf8Buffer right) => left.Equals(right);
        public static bool operator !=(Utf8Buffer left, Utf8Buffer right) => !left.Equals(right);
        public static bool operator ==(Utf8Buffer left, Utf8Span right) => left.Equals(right);
        public static bool operator !=(Utf8Buffer left, Utf8Span right) => !left.Equals(right);
        public static bool operator ==(Utf8Span left, Utf8Buffer right) => right.Equals(left);
        public static bool operator !=(Utf8Span left, Utf8Buffer right) => !right.Equals(left);

        // TODO: do we like all these O(N) operators? 
        public static bool operator ==(Utf8Buffer left, string right) => left.Equals(right);
        public static bool operator !=(Utf8Buffer left, string right) => !left.Equals(right);
        public static bool operator ==(string left, Utf8Buffer right) => right.Equals(left);
        public static bool operator !=(string left, Utf8Buffer right) => !right.Equals(left);

        public static implicit operator ReadOnlySpan<byte>(Utf8Buffer utf8String) => utf8String.Bytes;

        public static implicit operator Utf8Span(Utf8Buffer utf8String) => utf8String.Span;

        public static explicit operator Utf8Buffer(string utf16String) => new Utf8Buffer(utf16String);

        public static explicit operator string(Utf8Buffer utf8String) => utf8String.ToString();
        #endregion

        #region Methods
        public void ThrowIfInvalid() => buffer.ThrowIfInvalid();

        public bool Free() => buffer.Free();

        public bool Equals(Utf8Buffer other) => Bytes.SequenceEqual(other.Bytes);

        public bool Equals(Utf8Span other) => Bytes.SequenceEqual(other.Bytes);

        public bool Equals(string other) => Span.Equals(other);

        public Utf8CodePointEnumerator GetEnumerator() => new Utf8CodePointEnumerator(buffer.Span);

        public int CompareTo(Utf8Buffer other) => Span.CompareTo(other);

        public int CompareTo(string other) => Span.CompareTo(other);

        public int CompareTo(Utf8Span other) => Span.CompareTo(other);

        public bool StartsWith(uint codePoint) => Span.StartsWith(codePoint);

        public bool StartsWith(Utf8Buffer value) => Span.StartsWith(value.Span);

        public bool StartsWith(Utf8Span value) => Span.StartsWith(value);

        public bool EndsWith(Utf8Buffer value) => Span.EndsWith(value.Span);

        public bool EndsWith(Utf8Span value) => Span.EndsWith(value);

        public bool EndsWith(uint codePoint) => Span.EndsWith(codePoint);

        #region Slicing
        // TODO: should Utf8String slicing operations return Utf8Span? 
        // TODO: should we add slicing overloads that take char delimiters?
        // TODO: why do we even have Try versions? If the delimiter is not found, the result should be the original.
        public bool TrySubstringFrom(Utf8Buffer value, out Utf8Buffer result)
        {
            int idx = IndexOf(value);

            if (idx == StringNotFound)
            {
                result = default;
                return false;
            }

            result = Substring(idx);
            return true;
        }

        public bool TrySubstringFrom(uint codePoint, out Utf8Buffer result)
        {
            int idx = IndexOf(codePoint);

            if (idx == StringNotFound)
            {
                result = default;
                return false;
            }

            result = Substring(idx);
            return true;
        }

        public bool TrySubstringTo(Utf8Buffer value, out Utf8Buffer result)
        {
            int idx = IndexOf(value);

            if (idx == StringNotFound)
            {
                result = default;
                return false;
            }

            result = Substring(0, idx);
            return true;
        }

        public bool TrySubstringTo(uint codePoint, out Utf8Buffer result)
        {
            int idx = IndexOf(codePoint);

            if (idx == StringNotFound)
            {
                result = default;
                return false;
            }

            result = Substring(0, idx);
            return true;
        }
        #endregion

        #region Index-based operations
        // TODO: should we even have index based operations?
        // TODO: should we have search (e.g. IndexOf) overlaods that take char?

        public Utf8Buffer Substring(int index) => index == 0 ? this : Substring(index, Bytes.Length - index);

        public Utf8Buffer Substring(int index, int length)
        {
            if (length == 0)
            {
                return Empty;
            }
            if (index == 0 && length == Bytes.Length) return this;

            return new Utf8Buffer(buffer.Span.Slice(index, length));
        }

        public int IndexOf(Utf8Buffer value) => Bytes.IndexOf(value.Bytes);

        public int IndexOf(uint codePoint) => Span.IndexOf(codePoint);

        public int IndexOf(string s) => Span.IndexOf(new Utf8Span(Encoding.UTF8.GetBytes(s)));

        public int LastIndexOf(Utf8Buffer value) => Span.LastIndexOf(value.Span);

        public int LastIndexOf(uint codePoint) => Span.LastIndexOf(codePoint);

        public bool TryFormat(Span<byte> buffer, out int written, StandardFormat format = default, SymbolTable symbolTable = null)
        {
            if (!format.IsDefault) throw new ArgumentOutOfRangeException(nameof(format));
            if (symbolTable == SymbolTable.InvariantUtf8)
            {
                written = Bytes.Length;
                return Bytes.TryCopyTo(buffer);
            }

            return symbolTable.TryEncode(Bytes, buffer, out var consumed, out written);
        }
        #endregion

        /*
        // TODO: unless we change the type of Trim to Utf8Span, this double allocates.
        public FixedUtf8String Trim() => TrimStart().TrimEnd();

        // TODO: implement Utf8String.Trim(uint[])
        public FixedString Trim(uint[] codePoints) => throw new NotImplementedException();

        public FixedString TrimStart()
        {
            Utf8CodePointEnumerator it = GetEnumerator();
            while (it.MoveNext() &&Char.IsWhiteSpace(it.Current)) { }
            return Substring(it.PositionInCodeUnits);
        }

        public FixedUtf8String TrimStart(uint[] codePoints) {
            if (codePoints == null || codePoints.Length == 0) return TrimStart(); // Trim Whitespace

            Utf8CodePointEnumerator it = GetEnumerator();       
            while (it.MoveNext()) {
                if(Array.IndexOf(codePoints, it.Current) == -1){
                    break;
                }
            }

            return Substring(it.PositionInCodeUnits);
        }
        
        // TODO: do we even want this overload? System.String does not have an overload that takes string
        public FixedString TrimStart(FixedString characters)
        {
            if (characters == Empty)
            {
                // Trim Whitespace
                return TrimStart();
            }

            Utf8CodePointEnumerator it = GetEnumerator();
            Utf8CodePointEnumerator itPrefix = characters.GetEnumerator();

            while (it.MoveNext())
            {
                bool found = false;
                // Iterate over prefix set
                while (itPrefix.MoveNext())
                {
                    if (it.Current == itPrefix.Current)
                    {
                        // Character found, don't check further
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // Reached the end, char was not found
                    break;
                }

                itPrefix.Reset();
            }

            return Substring(it.PositionInCodeUnits);
        }

        public FixedString TrimEnd()
        {
            var it = new Utf8CodePointReverseEnumerator(Bytes);
            while (it.MoveNext() && Unicode.IsWhitespace(it.Current))
            {
            }

            return Substring(0, it.PositionInCodeUnits);
        }

        // TODO: implement Utf8String.TrimEnd(uint[])
        public FixedString TrimEnd(uint[] codePoints) => throw new NotImplementedException();

        // TODO: do we even want this overload? System.String does not have an overload that takes string
        public FixedString TrimEnd(FixedString characters)
        {
            if (characters == Empty)
            {
                // Trim Whitespace
                return TrimEnd();
            }

            var it = new Utf8CodePointReverseEnumerator(Bytes);
            Utf8CodePointEnumerator itPrefix = characters.GetEnumerator();

            while (it.MoveNext())
            {
                bool found = false;
                // Iterate over prefix set
                while (itPrefix.MoveNext())
                {
                    if (it.Current == itPrefix.Current)
                    {
                        // Character found, don't check further
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // Reached the end, char was not found
                    break;
                }

                itPrefix.Reset();
            }

            return Substring(0, it.PositionInCodeUnits);
        }
        */

        #region Disposer
        private void Dispose(bool disposing)
        {
            buffer.Free();
        }

        #endregion
        #endregion

        #region Fields
        //private readonly byte[] _buffer;
        private readonly FixedBuffer<byte> buffer;

        private const int StringNotFound = -1;

        static Utf8Buffer s_empty = new Utf8Buffer(string.Empty);
        #endregion

    }
}
