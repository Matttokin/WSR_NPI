﻿#region Directives
using System;
using VTDev.Libraries.CEXEngine.Numeric;
#endregion

namespace VTDev.Libraries.CEXEngine.Crypto.Cipher.Asymmetric.Encrypt.McEliece.Algebra
{
    /// <summary>
    /// This class implements elements of finite binary fields <c>GF(2^n)</c> using polynomial representation.
    /// <para>For more information on the arithmetic see for example IEEE Standard 1363 or 
    /// <a href="http://www.certicom.com/research/online.html"> Certicom online-tutorial</a>.</para>
    /// </summary>
    internal sealed class GF2nPolynomialElement : GF2nElement
    {
        #region Fields
        // pre-computed Bitmask for fast masking, bitMask[a]=0x1 << a
        private static int[] _bitMask = {0x00000001, 0x00000002, 0x00000004,
        0x00000008, 0x00000010, 0x00000020, 0x00000040, 0x00000080,
        0x00000100, 0x00000200, 0x00000400, 0x00000800, 0x00001000,
        0x00002000, 0x00004000, 0x00008000, 0x00010000, 0x00020000,
        0x00040000, 0x00080000, 0x00100000, 0x00200000, 0x00400000,
        0x00800000, 0x01000000, 0x02000000, 0x04000000, 0x08000000,
        0x10000000, 0x20000000, 0x40000000, unchecked((int)0x80000000), 0x00000000};

        // the used GF2Polynomial which stores the coefficients
        private GF2Polynomial polynomial;
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new random GF2nPolynomialElement using the given field and source of randomness
        /// </summary>
        /// 
        /// <param name="Gf">The GF2nField to use</param>
        /// <param name="Rnd">The source of randomness</param>
        public GF2nPolynomialElement(GF2nPolynomialField Gf, Random Rnd)
        {
            mField = Gf;
            mDegree = mField.Degree;
            polynomial = new GF2Polynomial(mDegree);
            Randomize(Rnd);
        }

        /// <summary>
        /// Creates a new GF2nPolynomialElement using the given field and Bitstring
        /// </summary>
        /// 
        /// <param name="Gf">The GF2nPolynomialField to use</param>
        /// <param name="Gp">The desired value as Bitstring</param>
        public GF2nPolynomialElement(GF2nPolynomialField Gf, GF2Polynomial Gp)
        {
            mField = Gf;
            mDegree = mField.Degree;
            polynomial = new GF2Polynomial(Gp);
            polynomial.ExpandN(mDegree);
        }

        /// <summary>
        /// Creates a new GF2nPolynomialElement using the given field <c>f</c> and byte[] <c>os</c> as value. 
        /// <para>The conversion is done according to 1363.</para>
        /// </summary>
        /// 
        /// <param name="Gf">The GF2nField to use</param>
        /// <param name="Os">The octet string to assign to this GF2nPolynomialElement</param>
        public GF2nPolynomialElement(GF2nPolynomialField Gf, byte[] Os)
        {
            mField = Gf;
            mDegree = mField.Degree;
            polynomial = new GF2Polynomial(mDegree, Os);
            polynomial.ExpandN(mDegree);
        }

        /// <summary>
        /// Creates a new GF2nPolynomialElement using the given field <c>Gf</c> and int[] <c>Is</c> as value
        /// </summary>
        /// 
        /// <param name="Gf">The GF2nField to use</param>
        /// <param name="Is">The integer string to assign to this GF2nPolynomialElement</param>
        public GF2nPolynomialElement(GF2nPolynomialField Gf, int[] Is)
        {
            mField = Gf;
            mDegree = mField.Degree;
            polynomial = new GF2Polynomial(mDegree, Is);
            polynomial.ExpandN(Gf.Degree);
        }
        /**
         * .
         *
         * @param other t
         */
        /// <summary>
        /// Creates a new GF2nPolynomialElement by cloning the given GF2nPolynomialElement <c>Ge</c>
        /// </summary>
        /// 
        /// <param name="Ge">The GF2nPolynomialElement to clone</param>
        public GF2nPolynomialElement(GF2nPolynomialElement Ge)
        {
            mField = Ge.mField;
            mDegree = Ge.mDegree;
            polynomial = new GF2Polynomial(Ge.polynomial);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the value of this GF2nPolynomialElement in a new Bitstring
        /// </summary>
        /// 
        /// <returns>The GF2nPolynomialElement as a Bitstring</returns>
        private GF2Polynomial GetGF2Polynomial()
        {
            return new GF2Polynomial(polynomial);
        }

        /// <summary>
        /// Calculates the multiplicative inverse of <c>this</c> and returns the result in a new GF2nPolynomialElement
        /// </summary>
        /// 
        /// <returns>Returns <c>this</c>^(-1)</returns>
        public GF2nPolynomialElement InvertEEA()
        {
            if (IsZero())
                throw new ArithmeticException();

            GF2Polynomial b = new GF2Polynomial(mDegree + 32, "ONE");
            b.ReduceN();
            GF2Polynomial c = new GF2Polynomial(mDegree + 32);
            c.ReduceN();
            GF2Polynomial u = GetGF2Polynomial();
            GF2Polynomial v = mField.FieldPolynomial;
            GF2Polynomial h;
            int j;
            u.ReduceN();

            while (!u.IsOne())
            {
                u.ReduceN();
                v.ReduceN();
                j = u.Length - v.Length;
                if (j < 0)
                {
                    h = u;
                    u = v;
                    v = h;
                    h = b;
                    b = c;
                    c = h;
                    j = -j;
                    c.ReduceN(); // this increases the performance
                }

                u.ShiftLeftAddThis(v, j);
                b.ShiftLeftAddThis(c, j);
            }
            b.ReduceN();

            return new GF2nPolynomialElement((GF2nPolynomialField)mField, b);
        }

        /// <summary>
        /// Calculates the multiplicative inverse of <c>this</c> using the modified almost inverse algorithm and returns the result in a new GF2nPolynomialElement
        /// </summary>
        /// 
        /// <returns>Returns <c>this</c>^(-1)</returns>
        public GF2nPolynomialElement InvertMAIA()
        {
            if (IsZero())
            {
                throw new ArithmeticException();
            }
            GF2Polynomial b = new GF2Polynomial(mDegree, "ONE");
            GF2Polynomial c = new GF2Polynomial(mDegree);
            GF2Polynomial u = GetGF2Polynomial();
            GF2Polynomial v = mField.FieldPolynomial;
            GF2Polynomial h;
            while (true)
            {
                while (!u.TestBit(0))
                { // x|u (x divides u)
                    u.ShiftRightThis(); // u = u / x
                    if (!b.TestBit(0))
                    {
                        b.ShiftRightThis();
                    }
                    else
                    {
                        b.AddToThis(mField.FieldPolynomial);
                        b.ShiftRightThis();
                    }
                }

                if (u.IsOne())
                    return new GF2nPolynomialElement((GF2nPolynomialField)mField, b);
                
                u.ReduceN();
                v.ReduceN();

                if (u.Length < v.Length)
                {
                    h = u;
                    u = v;
                    v = h;
                    h = b;
                    b = c;
                    c = h;
                }

                u.AddToThis(v);
                b.AddToThis(c);
            }
        }

        /// <summary>
        /// Calculates the multiplicative inverse of <c>this</c> and returns the result in a new GF2nPolynomialElement
        /// </summary>
        /// 
        /// <returns>Returns <c>this</c>^(-1)</returns>
        public GF2nPolynomialElement InvertSquare()
        {
            GF2nPolynomialElement n;
            GF2nPolynomialElement u;
            int i, j, k, b;

            if (IsZero())
                throw new ArithmeticException();

            // b = (n-1)
            b = mField.Degree - 1;
            // n = a
            n = new GF2nPolynomialElement(this);
            n.polynomial.ExpandN((mDegree << 1) + 32); // increase performance
            n.polynomial.ReduceN();
            // k = 1
            k = 1;

            // for i = (r-1) downto 0 do, r=bitlength(b)
            for (i = BigMath.FloorLog(b) - 1; i >= 0; i--)
            {
                // u = n
                u = new GF2nPolynomialElement(n);
                // for j = 1 to k do
                for (j = 1; j <= k; j++)
                    u.SquareThisPreCalc(); // u = u^2

                // n = nu
                n.MultiplyThisBy(u);
                // k = 2k
                k <<= 1;
                // if b(i)==1
                if ((b & _bitMask[i]) != 0)
                {
                    // n = n^2 * b
                    n.SquareThisPreCalc();
                    n.MultiplyThisBy(this);
                    // k = k+1
                    k += 1;
                }
            }

            // outpur n^2
            n.SquareThisPreCalc();

            return n;
        }

        /// <summary>
        /// Create the one element
        /// </summary>
        /// 
        /// <param name="Gf">The finite field</param>
        /// 
        /// <returns>The one element in the given finite field</returns>
        public static GF2nPolynomialElement One(GF2nPolynomialField Gf)
        {
            GF2Polynomial polynomial = new GF2Polynomial(Gf.Degree, new int[] { 1 });
            return new GF2nPolynomialElement(Gf, polynomial);
        }

        /// <summary>
        /// Calculates <c>this</c> to the power of <c>K</c> and returns the result in a new GF2nPolynomialElement
        /// </summary>
        /// 
        /// <param name="K">The power</param>
        /// 
        /// <returns>Returns <c>this</c>^<c>K</c> in a new GF2nPolynomialElement</returns>
        public GF2nPolynomialElement Power(int K)
        {
            if (K == 1)
                return new GF2nPolynomialElement(this);

            GF2nPolynomialElement result = GF2nPolynomialElement.One((GF2nPolynomialField)mField);
            if (K == 0)
                return result;

            GF2nPolynomialElement x = new GF2nPolynomialElement(this);
            x.polynomial.ExpandN((x.mDegree << 1) + 32); // increase performance
            x.polynomial.ReduceN();

            for (int i = 0; i < mDegree; i++)
            {
                if ((K & (1 << i)) != 0)
                    result.MultiplyThisBy(x);

                x.Square();
            }

            return result;
        }

        /// <summary>
        /// Assign a random value to this GF2nPolynomialElement using the specified source of randomness
        /// </summary>
        /// 
        /// <param name="Rnd">The source of randomness</param>
        private void Randomize(Random Rnd)
        {
            polynomial.ExpandN(mDegree);
            polynomial.Randomize(Rnd);
        }

        /// <summary>
        /// Squares this GF2nPolynomialElement by shifting left its Bitstring and reducing.
        /// <para>This is supposed to be the slowest method. Use SquarePreCalc or SquareMatrix instead.</para>
        /// </summary>
        /// 
        /// <returns></returns>
        public GF2nPolynomialElement SquareBitwise()
        {
            GF2nPolynomialElement result = new GF2nPolynomialElement(this);
            result.SquareThisBitwise();
            result.ReduceThis();
            return result;
        }

        /// <summary>
        /// Squares this GF2nPolynomialElement using GF2nField's squaring matrix.
        /// <para>This is supposed to be fast when using a polynomial (no tri- or pentanomial) as fieldpolynomial.
        /// Use SquarePreCalc when using a tri- or pentanomial as fieldpolynomial instead.</para>
        /// </summary>
        /// 
        /// <returns>Returns <c>this^2</c> (newly created)</returns>
        public GF2nPolynomialElement SquareMatrix()
        {
            GF2nPolynomialElement result = new GF2nPolynomialElement(this);
            result.SquareThisMatrix();
            result.ReduceThis();
            return result;
        }

        /// <summary>
        /// Squares this GF2nPolynomialElement by using precalculated values and reducing.
        /// <para>This is supposed to de fastest when using a trinomial or pentanomial as field polynomial.
        /// Use SquareMatrix when using a ordinary polynomial as field polynomial.</para>
        /// </summary>
        /// 
        /// <returns></returns>
        public GF2nPolynomialElement SquarePreCalc()
        {
            GF2nPolynomialElement result = new GF2nPolynomialElement(this);
            result.SquareThisPreCalc();
            result.ReduceThis();

            return result;
        }

        /// <summary>
        /// Squares this GF2nPolynomialElement by shifting left its Bitstring and reducing.
        /// <para>This is supposed to be the slowest method. Use SquarePreCalc or SquareMatrix instead.</para>
        /// </summary>
        public void SquareThisBitwise()
        {
            polynomial.SquareThisBitwise();
            ReduceThis();
        }

        /// <summary>
        /// Squares this GF2nPolynomialElement using GF2nFields squaring matrix. 
        /// <para>This is supposed to be fast when using a polynomial (no tri- or pentanomial) as fieldpolynomial.
        /// Use SquarePreCalc when using a tri- or pentanomial as fieldpolynomial instead.</para>
        /// </summary>
        public void SquareThisMatrix()
        {
            GF2Polynomial result = new GF2Polynomial(mDegree);
            for (int i = 0; i < mDegree; i++)
            {
                if (polynomial.VectorMult(((GF2nPolynomialField)mField).SquaringMatrix[mDegree - i - 1]))
                    result.SetBit(i);
            }
            polynomial = result;
        }

        /// <summary>
        /// Squares this GF2nPolynomialElement by using precalculated values and reducing.
        /// <para>This is supposed to de fastest when using a tri- or pentanomial as fieldpolynomial.
        /// Use SquareMatrix when using a ordinary polynomial as fieldpolynomial.</para>
        /// </summary>
        public void SquareThisPreCalc()
        {
            polynomial.SquareThisPreCalc();
            ReduceThis();
        }

        /// <summary>
        /// Create the zero element
        /// </summary>
        /// 
        /// <param name="Gf">The finite field</param>
        /// 
        /// <returns>The zero element in the given finite field</returns>
        public static GF2nPolynomialElement Zero(GF2nPolynomialField Gf)
        {
            GF2Polynomial polynomial = new GF2Polynomial(Gf.Degree);
            return new GF2nPolynomialElement(Gf, polynomial);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Compute the sum of this element and <c>Addend</c>.
        /// </summary>
        /// 
        /// <param name="Addend">The addend</param>
        /// 
        /// <returns>Returns <c>this + other</c></returns>
        public override IGFElement Add(IGFElement Addend)
        {
            GF2nPolynomialElement result = new GF2nPolynomialElement(this);
            result.AddToThis(Addend);

            return result;
        }

        /// <summary>
        /// Compute <c>this + addend</c> (overwrite <c>this</c>)
        /// </summary>
        /// 
        /// <param name="Addend">The addend</param>
        public override void AddToThis(IGFElement Addend)
        {
            if (!(Addend is GF2nPolynomialElement))
                throw new Exception();
            if (!mField.Equals(((GF2nPolynomialElement)Addend).mField))
                throw new Exception();

            polynomial.AddToThis(((GF2nPolynomialElement)Addend).polynomial);
        }

        /// <summary>
        /// Assigns the value 'one' to this Polynomial
        /// </summary>
        public override void AssignOne()
        {
            polynomial.AssignOne();
        }

        /// <summary>
        /// Assigns the value 'zero' to this Polynomial
        /// </summary>
        public override void AssignZero()
        {
            polynomial.AssignZero();
        }

        /// <summary>
        /// Creates a new GF2nPolynomialElement by cloning this GF2nPolynomialElement
        /// </summary>
        /// 
        /// <returns>A copy of this element</returns>
        public override Object Clone()
        {
            return new GF2nPolynomialElement(this);
        }

        /// <summary>
        /// Compare this element with another object
        /// </summary>
        /// 
        /// <param name="Obj">The object for comprison</param>
        /// 
        /// <returns>Returns <c>true</c> if the two objects are equal, <c>false</c> otherwise</returns>
        public override bool Equals(Object Obj)
        {
            if (Obj == null || !(Obj is GF2nPolynomialElement))
                return false;
            
            GF2nPolynomialElement otherElem = (GF2nPolynomialElement)Obj;

            if (mField != otherElem.mField)
            {
                if (!mField.FieldPolynomial.Equals(otherElem.mField.FieldPolynomial))
                    return false;
            }

            return polynomial.Equals(otherElem.polynomial);
        }

        /// <summary>
        /// Returns the hash code of this element
        /// </summary>
        /// 
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            return mField.GetHashCode() + polynomial.GetHashCode();
        }

        /// <summary>
        /// Compute the multiplicative inverse of this element
        /// </summary>
        /// 
        /// <returns>Returns <c>this^-1</c> (newly created)</returns>
        public override IGFElement Invert()
        {
            return InvertMAIA();
        }

        /// <summary>
        /// Tests if the GF2nPolynomialElement has 'one' as value
        /// </summary>
        /// 
        /// <returns>Returns true if <c>this</c> equals one (this == 1)</returns>
        public override bool IsOne()
        {
            return polynomial.IsOne();
        }

        /// <summary>
        /// Checks whether this element is zero
        /// </summary>
        /// 
        /// <returns>Returns <c>true</c> if <c>this</c> is the zero element</returns>
        public override bool IsZero()
        {
            return polynomial.IsZero();
        }

        /// <summary>
        /// Increase the element by one
        /// </summary>
        /// 
        /// <returns>Returns <c>this</c> + 'one'</returns>
        public override GF2nElement Increase()
        {
            GF2nPolynomialElement result = new GF2nPolynomialElement(this);
            result.IncreaseThis();

            return result;
        }

        /// <summary>
        /// Increase <c>this</c> polynomial + 'one"
        /// </summary>
        public override void IncreaseThis()
        {
            polynomial.IncreaseThis();
        }

        /// <summary>
        /// Compute the product of this element and <c>factor</c>
        /// </summary>
        /// 
        /// <param name="Factor">he factor</param>
        /// 
        /// <returns>Returns <c>this * factor</c> </returns>
        public override IGFElement Multiply(IGFElement Factor)
        {
            GF2nPolynomialElement result = new GF2nPolynomialElement(this);
            result.MultiplyThisBy(Factor);

            return result;
        }

        /// <summary>
        /// Compute <c>this * factor</c> (overwrite <c>this</c>).
        /// </summary>
        /// 
        /// <param name="Factor">The factor</param>
        public override void MultiplyThisBy(IGFElement Factor)
        {
            if (!(Factor is GF2nPolynomialElement))
                throw new Exception();
            if (!mField.Equals(((GF2nPolynomialElement)Factor).mField))
                throw new Exception();
            if (Equals(Factor))
            {
                SquareThis();
                return;
            }

            polynomial = polynomial.Multiply(((GF2nPolynomialElement)Factor).polynomial);
            ReduceThis();
        }

        /// <summary>
        /// Solves the quadratic equation <c>z^2 + z = this</c> if such a solution exists.
        /// <para>This method returns one of the two possible solutions.
        /// The other solution is <c>z + 1</c>. Use z.Increase() to compute this solution.</para>
        /// </summary>
        /// 
        /// <returns>Returns a GF2nPolynomialElement representing one z satisfying the equation <c>z^2 + z = this</c></returns>
        public override GF2nElement SolveQuadraticEquation()
        {
            if (IsZero())
                return Zero((GF2nPolynomialField)mField);

            if ((mDegree & 1) == 1)
                return HalfTrace();

            // TODO this can be sped-up by precomputation of p and w's
            GF2nPolynomialElement z, w;
            do
            {
                // step 1.
                GF2nPolynomialElement p = new GF2nPolynomialElement(
                    (GF2nPolynomialField)mField, new Random());
                // step 2.
                z = Zero((GF2nPolynomialField)mField);
                w = (GF2nPolynomialElement)p.Clone();
                // step 3.
                for (int i = 1; i < mDegree; i++)
                {
                    // compute z = z^2 + w^2 * this
                    // and w = w^2 + p
                    z.SquareThis();
                    w.SquareThis();
                    z.AddToThis(w.Multiply(this));
                    w.AddToThis(p);
                }
            }
            while (w.IsZero()); // step 4.

            if (!Equals(z.Square().Add(z)))
                throw new Exception();

            // step 5.
            return z;
        }

        /// <summary>
        /// This method is used internally to map the square()-calls within GF2nPolynomialElement to one of the possible squaring methods
        /// </summary>
        /// 
        /// <returns>Returns <c>this^2</c> </returns>
        public override GF2nElement Square()
        {
            return SquarePreCalc();
        }

        /// <summary>
        /// Compute the square root of this element and return the result in a new GF2nPolynomialElement
        /// </summary>
        /// 
        /// <returns>Returns <c>this^1/2</c> (newly created)</returns>
        public override GF2nElement SquareRoot()
        {
            GF2nPolynomialElement result = new GF2nPolynomialElement(this);
            result.SquareRootThis();

            return result;
        }

        /// <summary>
        /// Compute the square root of this element
        /// </summary>
        public override void SquareRootThis()
        {
            // increase performance
            polynomial.ExpandN((mDegree << 1) + 32);
            polynomial.ReduceN();

            for (int i = 0; i < mField.Degree - 1; i++)
                SquareThis();
        }

        /// <summary>
        /// This method is used internally to map the square()-calls 
        /// within GF2nPolynomialElement to one of the possible squaring methods
        /// </summary>
        public override void SquareThis()
        {
            SquareThisPreCalc();
        }

        /// <summary>
        /// Checks whether the indexed bit of the bit representation is set
        /// </summary>
        /// 
        /// <param name="Index">The index of the bit to test</param>
        /// 
        /// <returns>Returns <c>true</c> if the indexed bit is set</returns>
        public override bool TestBit(int Index)
        {
            return polynomial.TestBit(Index);
        }

        /// <summary>
        /// Returns whether the rightmost bit of the bit representation is set.
        /// <para>This is needed for data conversion according to 1363.</para>
        /// </summary>
        /// 
        /// <returns>Returns <c>true</c> if the rightmost bit of this element is set</returns>
        public override bool TestRightmostBit()
        {
            return polynomial.TestBit(0);
        }

        /// <summary>
        /// Converts this GF2nPolynomialElement to a byte[] according to 1363
        /// </summary>
        /// 
        /// <returns>Returns a byte[] representing the value of this GF2nPolynomialElement</returns>
        public override byte[] ToByteArray()
        {
            return polynomial.ToByteArray();
        }

        /// <summary>
        /// Converts this GF2nPolynomialElement to an integer according to 1363
        /// </summary>
        /// 
        /// <returns>Returns a BigInteger representing the value of this GF2nPolynomialElement</returns>
        public override BigInteger ToFlexiBigInt()
        {
            return polynomial.ToFlexiBigInt();
        }

        /// <summary>
        /// Returns a string representing this Bitstrings value using hexadecimal radix in MSB-first order
        /// </summary>
        /// 
        /// <returns>Returns a String representing this Bitstrings value</returns>
        public override String ToString()
        {
            return polynomial.ToString(16);
        }

        /// <summary>
        /// Returns a string representing this Bitstrings value using hexadecimal or binary radix in MSB-first order
        /// </summary>
        /// 
        /// <param name="Radix">The radix to use (2 or 16, otherwise 2 is used)</param>
        /// 
        /// <returns>Returns a String representing this Bitstrings value.</returns>
        public override String ToString(int Radix)
        {
            return polynomial.ToString(Radix);
        }

        /// <summary>
        /// Returns the trace of this GF2nPolynomialElement
        /// </summary>
        /// 
        /// <returns>The trace of this GF2nPolynomialElement</returns>
        public override int Trace()
        {
            GF2nPolynomialElement t = new GF2nPolynomialElement(this);
            int i;

            for (i = 1; i < mDegree; i++)
            {
                t.SquareThis();
                t.AddToThis(this);
            }

            if (t.IsOne())
                return 1;
            
            return 0;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Returns the half-trace of this GF2nPolynomialElement
        /// </summary>
        /// 
        /// <returns>Returns a GF2nPolynomialElement representing the half-trace of this GF2nPolynomialElement</returns>
        private GF2nPolynomialElement HalfTrace()
        {
            if ((mDegree & 0x01) == 0)
                throw new Exception();
            
            int i;
            GF2nPolynomialElement h = new GF2nPolynomialElement(this);

            for (i = 1; i <= ((mDegree - 1) >> 1); i++)
            {
                h.SquareThis();
                h.SquareThis();
                h.AddToThis(this);
            }

            return h;
        }

        /// <summary>
        /// Reduce this GF2nPolynomialElement using the pentanomial x^n + x^pc[2] + x^pc[1] + x^pc[0] + 1 as fieldpolynomial.
        /// The coefficients are reduced bit by bit.
        /// </summary>
        private void ReducePentanomialBitwise(int[] Pc)
        {
            int i;
            int k = mDegree - Pc[2];
            int l = mDegree - Pc[1];
            int m = mDegree - Pc[0];

            for (i = polynomial.Length - 1; i >= mDegree; i--)
            {
                if (polynomial.TestBit(i))
                {
                    polynomial.XorBit(i);
                    polynomial.XorBit(i - k);
                    polynomial.XorBit(i - l);
                    polynomial.XorBit(i - m);
                    polynomial.XorBit(i - mDegree);
                }
            }

            polynomial.ReduceN();
            polynomial.ExpandN(mDegree);
        }

        /// <summary>
        /// Reduces this GF2nPolynomialElement modulo the field-polynomial
        /// </summary>
        private void ReduceThis()
        {
            if (polynomial.Length > mDegree)
            { // really reduce ?
                if (((GF2nPolynomialField)mField).IsTrinomial)
                { // fieldpolonomial
                    // is trinomial
                    int tc;
                    try
                    {
                        tc = ((GF2nPolynomialField)mField).Tc;
                    }
                    catch (Exception NATExc)
                    {
                        throw new Exception("GF2nPolynomialElement.Reduce: the field polynomial is not a trinomial!", NATExc);
                    }
                    // do we have to use slow bitwise reduction ?
                    if (((mDegree - tc) <= 32) || (polynomial.Length > (mDegree << 1)))
                    {
                        ReduceTrinomialBitwise(tc);
                        return;
                    }
                    polynomial.ReduceTrinomial(mDegree, tc);

                    return;
                }
                else if (((GF2nPolynomialField)mField).IsPentanomial) // fieldpolynomial is pentanomial
                {
                    int[] pc;
                    try
                    {
                        pc = ((GF2nPolynomialField)mField).Pc;
                    }
                    catch (Exception NATExc)
                    {
                        throw new Exception("GF2nPolynomialElement.Reduce: the field polynomial is not a pentanomial!", NATExc);
                    }
                    // do we have to use slow bitwise reduction ?
                    if (((mDegree - pc[2]) <= 32) || (polynomial.Length > (mDegree << 1)))
                    {
                        ReducePentanomialBitwise(pc);
                        return;
                    }
                    polynomial.ReducePentanomial(mDegree, pc);

                    return;
                }
                else
                { // fieldpolynomial is something else
                    polynomial = polynomial.Remainder(mField.FieldPolynomial);
                    polynomial.ExpandN(mDegree);

                    return;
                }
            }

            if (polynomial.Length < mDegree)
                polynomial.ExpandN(mDegree);
        }

        /// <summary>
        /// Reduce this GF2nPolynomialElement using the trinomial x^n + x^tc + 1 as fieldpolynomial. The coefficients are reduced bit by bit.
        /// </summary>
        private void ReduceTrinomialBitwise(int Tc)
        {
            int i;
            int k = mDegree - Tc;

            for (i = polynomial.Length - 1; i >= mDegree; i--)
            {
                if (polynomial.TestBit(i))
                {
                    polynomial.XorBit(i);
                    polynomial.XorBit(i - k);
                    polynomial.XorBit(i - mDegree);
                }
            }
            polynomial.ReduceN();
            polynomial.ExpandN(mDegree);
        }
        #endregion
    }
}
