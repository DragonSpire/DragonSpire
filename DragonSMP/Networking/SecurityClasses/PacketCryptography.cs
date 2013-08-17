using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace DragonSpire
{
	public static class PacketCryptography
	{
		public static class Cryptography
    {
        /// <summary>
        /// Creates a Java-style SHA-1 hash.
        /// </summary>
        public static string JavaHexDigest(byte[] data)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(data);
            bool negative = (hash[0] & 0x80) == 0x80;
            if (negative) // check for negative hashes
                hash = TwosCompliment(hash);
            // Create the string and trim away the zeroes
            string digest = GetHexString(hash).TrimStart('0');
            if (negative)
                digest = "-" + digest;
            return digest;
        }

        private static string GetHexString(byte[] p)
        {
            string result = "";
            for (int i = 0; i < p.Length; i++)
                result += p[i].ToString("X2");
            return result;
        }

        private static byte[] TwosCompliment(byte[] p) // little endian
        {
            int i;
            bool carry = true;
            for (i = p.Length - 1; i >= 0; i--)
            {
                p[i] = unchecked((byte)~p[i]);
                if (carry)
                {
                    carry = p[i] == 0xFF;
                    p[i]++;
                }
            }
            return p;
        }
    }

		public class AsnKeyBuilder
    {
        private static readonly byte[] Zero = new byte[] {0};
        private static readonly byte[] Empty = new byte[] {};

        // PublicKeyInfo (X.509 compatible) message
        /// <summary>
        /// Returns the AsnMessage representing the X.509 PublicKeyInfo.
        /// </summary>
        /// <param name="publicKey">The DSA key to be encoded.</param>
        /// <returns>Returns the AsnType representing the
        /// X.509 PublicKeyInfo.</returns>
        /// <seealso cref="PrivateKeyToPKCS8(DSAParameters)"/>
        /// <seealso cref="PrivateKeyToPKCS8(RSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(RSAParameters)"/>
        public static AsnMessage PublicKeyToX509(DSAParameters publicKey)
        {
            // Value Type cannot be null
            // Debug.Assert(null != publicKey);

            /* *
            * SEQUENCE              // PrivateKeyInfo
            * +- SEQUENCE           // AlgorithmIdentifier
            * |  +- OID             // 1.2.840.10040.4.1
            * |  +- SEQUENCE        // DSS-Params (Optional Parameters)
            * |    +- INTEGER (P)
            * |    +- INTEGER (Q)
            * |    +- INTEGER (G)
            * +- BITSTRING          // PublicKey
            *    +- INTEGER(Y)      // DSAPublicKey Y
            * */

            // DSA Parameters
            AsnType p = CreateIntegerPos(publicKey.P);
            AsnType q = CreateIntegerPos(publicKey.Q);
            AsnType g = CreateIntegerPos(publicKey.G);

            // Sequence - DSA-Params
            AsnType dssParams = CreateSequence(new[] {p, q, g});

            // OID - packed 1.2.840.10040.4.1
            //   { 0x2A, 0x86, 0x48, 0xCE, 0x38, 0x04, 0x01 }
            AsnType oid = CreateOid("1.2.840.10040.4.1");

            // Sequence
            AsnType algorithmID = CreateSequence(new[] {oid, dssParams});

            // Public Key Y
            AsnType y = CreateIntegerPos(publicKey.Y);
            AsnType key = CreateBitString(y);

            // Sequence 'A'
            AsnType publicKeyInfo =
                CreateSequence(new[] {algorithmID, key});

            return new AsnMessage(publicKeyInfo.GetBytes(), "X.509");
        }

        // PublicKeyInfo (X.509 compatible) message
        /// <summary>
        /// Returns the AsnMessage representing the X.509 PublicKeyInfo.
        /// </summary>
        /// <param name="publicKey">The RSA key to be encoded.</param>
        /// <returns>Returns the AsnType representing the
        /// X.509 PublicKeyInfo.</returns>
        /// <seealso cref="PrivateKeyToPKCS8(DSAParameters)"/>
        /// <seealso cref="PrivateKeyToPKCS8(RSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(DSAParameters)"/>
        public static AsnMessage PublicKeyToX509(RSAParameters publicKey)
        {
            // Value Type cannot be null
            // Debug.Assert(null != publicKey);

            /* *
            * SEQUENCE              // PrivateKeyInfo
            * +- SEQUENCE           // AlgorithmIdentifier
            *    +- OID             // 1.2.840.113549.1.1.1
            *    +- Null            // Optional Parameters
            * +- BITSTRING          // PrivateKey
            *    +- SEQUENCE        // RSAPrivateKey
            *       +- INTEGER(N)   // N
            *       +- INTEGER(E)   // E
            * */

            // OID - packed 1.2.840.113549.1.1.1
            //   { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 }
            AsnType oid = CreateOid("1.2.840.113549.1.1.1");
            AsnType algorithmID =
                CreateSequence(new[] {oid, CreateNull()});

            AsnType n = CreateIntegerPos(publicKey.Modulus);
            AsnType e = CreateIntegerPos(publicKey.Exponent);
            AsnType key = CreateBitString(
                CreateSequence(new[] {n, e})
                );

            AsnType publicKeyInfo =
                CreateSequence(new[] {algorithmID, key});

            return new AsnMessage(publicKeyInfo.GetBytes(), "X.509");
        }

        // PKCS #8, Section 6 (PrivateKeyInfo) message
        // !!!!!!!!!!!!!!! Unencrypted !!!!!!!!!!!!!!!
        /// <summary>
        /// Returns AsnMessage representing the unencrypted
        /// PKCS #8 PrivateKeyInfo.
        /// </summary>
        /// <param name="privateKey">The DSA key to be encoded.</param>
        /// <returns>Returns the AsnType representing the unencrypted
        /// PKCS #8 PrivateKeyInfo.</returns>
        /// <seealso cref="PrivateKeyToPKCS8(RSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(DSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(RSAParameters)"/>
        public static AsnMessage PrivateKeyToPKCS8(DSAParameters privateKey)
        {
            // Value Type cannot be null
            // Debug.Assert(null != privateKey);

            /* *
            * SEQUENCE              // PrivateKeyInfo
            * +- INTEGER(0)         // Version (v1998)
            * +- SEQUENCE           // AlgorithmIdentifier
            * |  +- OID             // 1.2.840.10040.4.1
            * |  +- SEQUENCE        // DSS-Params (Optional Parameters)
            * |    +- INTEGER (P)
            * |    +- INTEGER (Q)
            * |    +- INTEGER (G)
            * +- OCTETSTRING        // PrivateKey
            *    +- INTEGER(X)   // DSAPrivateKey X
            * */

            // Version - 0 (v1998)
            AsnType version = CreateInteger(Zero);

            // Domain Parameters
            AsnType p = CreateIntegerPos(privateKey.P);
            AsnType q = CreateIntegerPos(privateKey.Q);
            AsnType g = CreateIntegerPos(privateKey.G);

            AsnType dssParams = CreateSequence(new[] {p, q, g});

            // OID - packed 1.2.840.10040.4.1
            //   { 0x2A, 0x86, 0x48, 0xCE, 0x38, 0x04, 0x01 }
            AsnType oid = CreateOid("1.2.840.10040.4.1");

            // AlgorithmIdentifier
            AsnType algorithmID = CreateSequence(new[] {oid, dssParams});

            // Private Key X
            AsnType x = CreateIntegerPos(privateKey.X);
            AsnType key = CreateOctetString(x);

            // Sequence
            AsnType privateKeyInfo =
                CreateSequence(new[] {version, algorithmID, key});

            return new AsnMessage(privateKeyInfo.GetBytes(), "PKCS#8");
        }

        // PKCS #8, Section 6 (PrivateKeyInfo) message
        // !!!!!!!!!!!!!!! Unencrypted !!!!!!!!!!!!!!!
        /// <summary>
        /// Returns AsnMessage representing the unencrypted
        /// PKCS #8 PrivateKeyInfo.
        /// </summary>
        /// <param name="privateKey">The RSA key to be encoded.</param>
        /// <returns>Returns the AsnType representing the unencrypted
        /// PKCS #8 PrivateKeyInfo.</returns>
        /// <seealso cref="PrivateKeyToPKCS8(DSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(DSAParameters)"/>
        /// <seealso cref="PublicKeyToX509(RSAParameters)"/>
        public static AsnMessage PrivateKeyToPKCS8(RSAParameters privateKey)
        {
            // Value Type cannot be null
            // Debug.Assert(null != privateKey);

            /* *
            * SEQUENCE                  // PublicKeyInfo
            * +- INTEGER(0)             // Version - 0 (v1998)
            * +- SEQUENCE               // AlgorithmIdentifier
            *    +- OID                 // 1.2.840.113549.1.1.1
            *    +- NULL                // Optional Parameters
            * +- OCTETSTRING            // PrivateKey
            *    +- SEQUENCE            // RSAPrivateKey
            *       +- INTEGER(0)       // Version - 0 (v1998)
            *       +- INTEGER(N)
            *       +- INTEGER(E)
            *       +- INTEGER(D)
            *       +- INTEGER(P)
            *       +- INTEGER(Q)
            *       +- INTEGER(DP)
            *       +- INTEGER(DQ)
            *       +- INTEGER(Inv Q)
            * */

            AsnType n = CreateIntegerPos(privateKey.Modulus);
            AsnType e = CreateIntegerPos(privateKey.Exponent);
            AsnType d = CreateIntegerPos(privateKey.D);
            AsnType p = CreateIntegerPos(privateKey.P);
            AsnType q = CreateIntegerPos(privateKey.Q);
            AsnType dp = CreateIntegerPos(privateKey.DP);
            AsnType dq = CreateIntegerPos(privateKey.DQ);
            AsnType iq = CreateIntegerPos(privateKey.InverseQ);

            // Version - 0 (v1998)
            AsnType version = CreateInteger(new byte[] {0});

            // octstring = OCTETSTRING(SEQUENCE(INTEGER(0)INTEGER(N)...))
            AsnType key = CreateOctetString(
                CreateSequence(new[] {version, n, e, d, p, q, dp, dq, iq})
                );

            // OID - packed 1.2.840.113549.1.1.1
            //   { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 }
            AsnType algorithmID = CreateSequence(new[] {CreateOid("1.2.840.113549.1.1.1"), CreateNull()}
                );

            // PrivateKeyInfo
            AsnType privateKeyInfo =
                CreateSequence(new[] {version, algorithmID, key});

            return new AsnMessage(privateKeyInfo.GetBytes(), "PKCS#8");
        }

        /// <summary>
        /// <para>An ordered collection of one or more types.
        /// Returns the AsnType representing an ASN.1 encoded sequence.</para>
        /// <para>If the AsnType is null, an empty sequence (length 0)
        /// is returned.</para>
        /// </summary>
        /// <param name="value">An AsnType consisting of
        /// a single value to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded sequence.</returns>
        /// <seealso cref="CreateSet(AsnType)"/>
        /// <seealso cref="CreateSet(AsnType[])"/> 
        /// <seealso cref="CreateSetOf(AsnType)"/>
        /// <seealso cref="CreateSetOf(AsnType[])"/>
        /// <seealso cref="CreateSequence(AsnType)"/>
        /// <seealso cref="CreateSequence(AsnType[])"/>
        /// <seealso cref="CreateSequenceOf(AsnType)"/>
        /// <seealso cref="CreateSequenceOf(AsnType[])"/>
        public static AsnType CreateSequence(AsnType value)
        {
            // Should be at least 1...
            Debug.Assert(!IsEmpty(value));

            // One or more required
            if (IsEmpty(value))
            {
                throw new ArgumentException("A sequence requires at least one value.");
            }

            // Sequence: Tag 0x30 (16, Universal, Constructed)
            return new AsnType(0x30, value.GetBytes());
        }

        /// <summary>
        /// <para>An ordered collection of one or more types.
        /// Returns the AsnType representing an ASN.1 encoded sequence.</para>
        /// <para>If the AsnType is null, an
        /// empty sequence (length 0) is returned.</para>
        /// </summary>
        /// <param name="values">An array of AsnType consisting of
        /// the values in the collection to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded Set.</returns>
        /// <seealso cref="CreateSet(AsnType)"/>
        /// <seealso cref="CreateSet(AsnType[])"/> 
        /// <seealso cref="CreateSetOf(AsnType)"/>
        /// <seealso cref="CreateSetOf(AsnType[])"/>
        /// <seealso cref="CreateSequence(AsnType)"/>
        /// <seealso cref="CreateSequence(AsnType[])"/>
        /// <seealso cref="CreateSequenceOf(AsnType)"/>
        /// <seealso cref="CreateSequenceOf(AsnType[])"/>
        public static AsnType CreateSequence(AsnType[] values)
        {
            // Should be at least 1...
            Debug.Assert(!IsEmpty(values));

            // One or more required
            if (IsEmpty(values))
            {
                throw new ArgumentException("A sequence requires at least one value.");
            }

            // Sequence: Tag 0x30 (16, Universal, Constructed)
            return new AsnType((0x10 | 0x20), Concatenate(values));
        }

        /// <summary>
        /// <para>An ordered collection zero, one or more types.
        /// Returns the AsnType representing an ASN.1 encoded sequence.</para>
        /// <para>If the AsnType value is null,an
        /// empty sequence (length 0) is returned.</para>
        /// </summary>
        /// <param name="value">An AsnType consisting of
        /// a single value to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded sequence.</returns>
        /// <seealso cref="CreateSet(AsnType)"/>
        /// <seealso cref="CreateSet(AsnType[])"/> 
        /// <seealso cref="CreateSetOf(AsnType)"/>
        /// <seealso cref="CreateSetOf(AsnType[])"/>
        /// <seealso cref="CreateSequence(AsnType)"/>
        /// <seealso cref="CreateSequence(AsnType[])"/>
        /// <seealso cref="CreateSequenceOf(AsnType)"/>
        /// <seealso cref="CreateSequenceOf(AsnType[])"/>
        public static AsnType CreateSequenceOf(AsnType value)
        {
            // From the ASN.1 Mailing List
            if (IsEmpty(value))
            {
                return new AsnType(0x30, Empty);
            }

            // Sequence: Tag 0x30 (16, Universal, Constructed)
            return new AsnType(0x30, value.GetBytes());
        }

        /// <summary>
        /// <para>An ordered collection zero, one or more types.
        /// Returns the AsnType representing an ASN.1 encoded sequence.</para>
        /// <para>If the AsnType array is null or the array is 0 length,
        /// an empty sequence (length 0) is returned.</para>
        /// </summary>
        /// <param name="values">An AsnType consisting of
        /// the values in the collection to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded sequence.</returns>
        /// <seealso cref="CreateSet(AsnType)"/>
        /// <seealso cref="CreateSet(AsnType[])"/> 
        /// <seealso cref="CreateSetOf(AsnType)"/>
        /// <seealso cref="CreateSetOf(AsnType[])"/>
        /// <seealso cref="CreateSequence(AsnType)"/>
        /// <seealso cref="CreateSequence(AsnType[])"/>
        /// <seealso cref="CreateSequenceOf(AsnType)"/>
        /// <seealso cref="CreateSequenceOf(AsnType[])"/>
        public static AsnType CreateSequenceOf(AsnType[] values)
        {
            // From the ASN.1 Mailing List
            if (IsEmpty(values))
            {
                return new AsnType(0x30, Empty);
            }

            // Sequence: Tag 0x30 (16, Universal, Constructed)
            return new AsnType(0x30, Concatenate(values));
        }

        /// <summary>
        /// <para>An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded bit string.</para>
        /// <para>If octets is null or length is 0, an empty (0 length)
        /// bit string is returned.</para>
        /// </summary>
        /// <param name="octets">A MSB (big endian) byte[] representing the
        /// bit string to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded bit string.</returns>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(AsnType[])"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        public static AsnType CreateBitString(byte[] octets)
        {
            // BitString: Tag 0x03 (3, Universal, Primitive)
            return CreateBitString(octets, 0);
        }

        /// <summary>
        /// <para>An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded bit string.</para>
        /// <para>unusedBits is applied to the end of the bit string,
        /// not the start of the bit string. unusedBits must be less than 8
        /// (the size of an octet). Refer to ITU X.680, Section 32.</para>
        /// <para>If octets is null or length is 0, an empty (0 length)
        /// bit string is returned.</para>
        /// </summary>
        /// <param name="octets">A MSB (big endian) byte[] representing the
        /// bit string to be encoded.</param>
        /// <param name="unusedBits">The number of unused trailing binary
        /// digits in the bit string to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded bit string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(AsnType[])"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        public static AsnType CreateBitString(byte[] octets, uint unusedBits)
        {
            if (IsEmpty(octets))
            {
                // Empty octet string
                return new AsnType(0x03, Empty);
            }

            if (!(unusedBits < 8))
            {
                throw new ArgumentException("Unused bits must be less than 8.");
            }

            byte[] b = Concatenate(new[] {(byte)unusedBits}, octets);
            // BitString: Tag 0x03 (3, Universal, Primitive)
            return new AsnType(0x03, b);
        }

        /// <summary>
        /// An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded bit string.
        /// If value is null, an empty (0 length) bit string is
        /// returned.
        /// </summary>
        /// <param name="value">An AsnType object to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded bit string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType[])"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        public static AsnType CreateBitString(AsnType value)
        {
            if (IsEmpty(value))
            {
                return new AsnType(0x03, Empty);
            }

            // BitString: Tag 0x03 (3, Universal, Primitive)
            return CreateBitString(value.GetBytes(), 0x00);
        }

        /// <summary>
        /// An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded bit string.
        /// If value is null, an empty (0 length) bit string is
        /// returned.
        /// </summary>
        /// <param name="values">An AsnType object to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded bit string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        public static AsnType CreateBitString(AsnType[] values)
        {
            if (IsEmpty(values))
            {
                return new AsnType(0x03, Empty);
            }

            // BitString: Tag 0x03 (3, Universal, Primitive)
            return CreateBitString(Concatenate(values), 0x00);
        }

        /// <summary>
        /// <para>An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded bit string.</para>
        /// <para>If octets is null or length is 0, an empty (0 length)
        /// bit string is returned.</para>
        /// <para>If conversion fails, the bit string returned is a partial
        /// bit string. The partial bit string ends at the octet before the
        /// point of failure (it does not include the octet which could
        /// not be parsed, or subsequent octets).</para>
        /// </summary>
        /// <param name="value">A MSB (big endian) byte[] representing the
        /// bit string to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded bit string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        public static AsnType CreateBitString(String value)
        {
            if (IsEmpty(value))
            {
                return CreateBitString(Empty);
            }

            // Any unused bits?
            int lstrlen = value.Length;
            int unusedBits = 8 - (lstrlen%8);
            if (8 == unusedBits)
            {
                unusedBits = 0;
            }

            for (int i = 0; i < unusedBits; i++)
            {
                value += "0";
            }

            // Determine number of octets
            int loctlen = (lstrlen + 7)/8;

            var octets = new List<byte>();
            for (int i = 0; i < loctlen; i++)
            {
                String s = value.Substring(i*8, 8);
                byte b = 0x00;

                try
                {
                    b = Convert.ToByte(s, 2);
                }

                catch (FormatException /*e*/)
                {
                    unusedBits = 0;
                    break;
                }
                catch (OverflowException /*e*/)
                {
                    unusedBits = 0;
                    break;
                }

                octets.Add(b);
            }

            // BitString: Tag 0x03 (3, Universal, Primitive)
            return CreateBitString(octets.ToArray(), (uint)unusedBits);
        }

        /// <summary>
        /// An ordered sequence of zero, one or more octets. Returns
        /// the ASN.1 encoded octet string. If octets is null or length
        /// is 0, an empty (0 length) octet string is returned.
        /// </summary>
        /// <param name="value">A MSB (big endian) byte[] representing the
        /// octet string to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded octet string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        public static AsnType CreateOctetString(byte[] value)
        {
            if (IsEmpty(value))
            {
                // Empty octet string
                return new AsnType(0x04, Empty);
            }

            // OctetString: Tag 0x04 (4, Universal, Primitive)
            return new AsnType(0x04, value);
        }

        /// <summary>
        /// An ordered sequence of zero, one or more octets. Returns
        /// the byte[] representing an ASN.1 encoded octet string.
        /// If octets is null or length is 0, an empty (0 length)
        /// o ctet string is returned.
        /// </summary>
        /// <param name="value">An AsnType object to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded octet string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(String)"/>
        public static AsnType CreateOctetString(AsnType value)
        {
            if (IsEmpty(value))
            {
                // Empty octet string
                return new AsnType(0x04, 0x00);
            }

            // OctetString: Tag 0x04 (4, Universal, Primitive)
            return new AsnType(0x04, value.GetBytes());
        }

        /// <summary>
        /// An ordered sequence of zero, one or more octets. Returns
        /// the byte[] representing an ASN.1 encoded octet string.
        /// If octets is null or length is 0, an empty (0 length)
        /// o ctet string is returned.
        /// </summary>
        /// <param name="values">An AsnType object to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded octet string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(String)"/>
        public static AsnType CreateOctetString(AsnType[] values)
        {
            if (IsEmpty(values))
            {
                // Empty octet string
                return new AsnType(0x04, 0x00);
            }

            // OctetString: Tag 0x04 (4, Universal, Primitive)
            return new AsnType(0x04, Concatenate(values));
        }

        /// <summary>
        /// <para>An ordered sequence of zero, one or more bits. Returns
        /// the AsnType representing an ASN.1 encoded octet string.</para>
        /// <para>If octets is null or length is 0, an empty (0 length)
        /// octet string is returned.</para>
        /// <para>If conversion fails, the bit string returned is a partial
        /// bit string. The partial octet string ends at the octet before the
        /// point of failure (it does not include the octet which could
        /// not be parsed, or subsequent octets).</para>
        /// </summary>
        /// <param name="value">A string representing the
        /// octet string to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded octet string.</returns>
        /// <seealso cref="CreateBitString(byte[])"/>
        /// <seealso cref="CreateBitString(byte[], uint)"/>
        /// <seealso cref="CreateBitString(String)"/>
        /// <seealso cref="CreateBitString(AsnType)"/>
        /// <seealso cref="CreateOctetString(byte[])"/>
        /// <seealso cref="CreateOctetString(AsnType)"/>
        /// <seealso cref="CreateOctetString(AsnType[])"/>
        public static AsnType CreateOctetString(String value)
        {
            if (IsEmpty(value))
            {
                return CreateOctetString(Empty);
            }

            // Determine number of octets
            int len = (value.Length + 255)/256;

            var octets = new List<byte>();
            for (int i = 0; i < len; i++)
            {
                String s = value.Substring(i*2, 2);
                byte b = 0x00;

                try
                {
                    b = Convert.ToByte(s, 16);
                }
                catch (FormatException /*e*/)
                {
                    break;
                }
                catch (OverflowException /*e*/)
                {
                    break;
                }

                octets.Add(b);
            }

            // OctetString: Tag 0x04 (4, Universal, Primitive)
            return CreateOctetString(octets.ToArray());
        }

        /// <summary>
        /// <para>Returns the AsnType representing a ASN.1 encoded
        /// integer. The octets pass through this method are not modified.</para>
        /// <para>If octets is null or zero length, the method returns an
        /// AsnType equivalent to CreateInteger(byte[]{0})..</para>
        /// </summary>
        /// <param name="value">A MSB (big endian) byte[] representing the
        /// integer to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded integer.</returns>
        /// <example>
        /// ASN.1 encoded 0:
        /// <code>CreateInteger(null)</code>
        /// <code>CreateInteger(new byte[]{0x00})</code>
        /// <code>CreateInteger(new byte[]{0x00, 0x00})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded 1:
        /// <code>CreateInteger(new byte[]{0x01})</code>
        /// </example>
        /// <seealso cref="CreateIntegerPos"/>
        /// <seealso cref="CreateIntegerNeg"/>
        public static AsnType CreateInteger(byte[] value)
        {
            // Is it better to add a '0', or silently
            //   drop the Integer? Dropping integers
            //   is probably not te best choice...
            if (IsEmpty(value))
            {
                return CreateInteger(Zero);
            }

            return new AsnType(0x02, value);
        }

        /// <summary>
        /// <para>Returns the AsnType representing a positive ASN.1 encoded
        /// integer. If the high bit of most significant byte is set,
        /// the method prepends a 0x00 to octets before assigning the
        /// value to ensure the resulting integer is interpreted as
        /// positive in the application.</para>
        /// <para>If octets is null or zero length, the method returns an
        /// AsnType equivalent to CreateInteger(byte[]{0})..</para>
        /// </summary>
        /// <param name="value">A MSB (big endian) byte[] representing the
        /// integer to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded positive integer.</returns>
        /// <example>
        /// ASN.1 encoded 0:
        /// <code>CreateIntegerPos(null)</code>
        /// <code>CreateIntegerPos(new byte[]{0x00})</code>
        /// <code>CreateIntegerPos(new byte[]{0x00, 0x00})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded 1:
        /// <code>CreateInteger(new byte[]{0x01})</code>
        /// </example>
        /// <seealso cref="CreateInteger"/>
        /// <seealso cref="CreateIntegerNeg"/>
        public static AsnType CreateIntegerPos(byte[] value)
        {
            byte[] i = null, d = Duplicate(value);

            if (IsEmpty(d))
            {
                d = Zero;
            }

            // Mediate the 2's compliment representation.
            // If the first byte has its high bit set, we will
            // add the additional byte of 0x00
            if (d.Length > 0 && d[0] > 0x7F)
            {
                i = new byte[d.Length + 1];
                i[0] = 0x00;
                Array.Copy(d, 0, i, 1, value.Length);
            }
            else
            {
                i = d;
            }

            // Integer: Tag 0x02 (2, Universal, Primitive)
            return CreateInteger(i);
        }

        /// <summary>
        /// <para>Returns the negative ASN.1 encoded integer. If the high
        /// bit of most significant byte is set, the integer is already
        /// considered negative.</para>
        /// <para>If the high bit of most significant byte
        /// is <bold>not</bold> set, the integer will be 2's complimented
        /// to form a negative integer.</para>
        /// <para>If octets is null or zero length, the method returns an
        /// AsnType equivalent to CreateInteger(byte[]{0})..</para>
        /// </summary>
        /// <param name="value">A MSB (big endian) byte[] representing the
        /// integer to be encoded.</param>
        /// <returns>Returns the negative ASN.1 encoded integer.</returns>
        /// <example>
        /// ASN.1 encoded 0:
        /// <code>CreateIntegerNeg(null)</code>
        /// <code>CreateIntegerNeg(new byte[]{0x00})</code>
        /// <code>CreateIntegerNeg(new byte[]{0x00, 0x00})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded -1 (2's compliment 0xFF):
        /// <code>CreateIntegerNeg(new byte[]{0x01})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded -2 (2's compliment 0xFE):
        /// <code>CreateIntegerNeg(new byte[]{0x02})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded -1:
        /// <code>CreateIntegerNeg(new byte[]{0xFF})</code>
        /// <code>CreateIntegerNeg(new byte[]{0xFF,0xFF})</code>
        /// Note: already negative since the high bit is set.</example>
        /// <example>
        /// ASN.1 encoded -255 (2's compliment 0xFF, 0x01):
        /// <code>CreateIntegerNeg(new byte[]{0x00,0xFF})</code>
        /// </example>
        /// <example>
        /// ASN.1 encoded -255 (2's compliment 0xFF, 0xFF, 0x01):
        /// <code>CreateIntegerNeg(new byte[]{0x00,0x00,0xFF})</code>
        /// </example>
        /// <seealso cref="CreateInteger"/>
        /// <seealso cref="CreateIntegerPos"/>
        public static AsnType CreateIntegerNeg(byte[] value)
        {
            // Is it better to add a '0', or silently
            //   drop the Integer? Dropping integers
            //   is probably not te best choice...
            if (IsEmpty(value))
            {
                return CreateInteger(Zero);
            }

            // No Trimming
            // The byte[] may be that way for a reason
            if (IsZero(value))
            {
                return CreateInteger(value);
            }

            //
            // At this point, we know we have at least 1 octet
            //

            // Is this integer already negative?
            if (value[0] >= 0x80)
                // Pass through with no modifications
            {
                return CreateInteger(value);
            }

            // No need to Duplicate - Compliment2s
            // performs the action
            byte[] c = Compliment2S(value);

            return CreateInteger(c);
        }

        /// <summary>
        /// Returns the AsnType representing an ASN.1 encoded null.
        /// </summary>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded null.</returns>
        public static AsnType CreateNull()
        {
            return new AsnType(0x05, new byte[] {0x00});
        }

        /// <summary>
        /// Removes leading 0x00 octets from the byte[] octets. This
        /// method may return an empty byte array (0 length).
        /// </summary>
        /// <param name="octets">An array of octets to trim.</param>
        /// <returns>A byte[] with leading 0x00 octets removed.</returns>
        public static byte[] TrimStart(byte[] octets)
        {
            if (IsEmpty(octets) || IsZero(octets))
            {
                return new byte[] {};
            }

            byte[] d = Duplicate(octets);

            // Position of the first non-zero value
            int pos = 0;
            foreach (byte b in d)
            {
                if (0 != b)
                {
                    break;
                }
                pos++;
            }

            // Nothing to trim
            if (pos == d.Length)
            {
                return octets;
            }

            // Allocate trimmed array
            var t = new byte[d.Length - pos];

            // Copy
            Array.Copy(d, pos, t, 0, t.Length);

            return t;
        }

        /// <summary>
        /// Removes trailing 0x00 octets from the byte[] octets. This
        /// method may return an empty byte array (0 length).
        /// </summary>
        /// <param name="octets">An array of octets to trim.</param>
        /// <returns>A byte[] with trailing 0x00 octets removed.</returns>
        public static byte[] TrimEnd(byte[] octets)
        {
            if (IsEmpty(octets) || IsZero(octets))
            {
                return Empty;
            }

            byte[] d = Duplicate(octets);

            Array.Reverse(d);

            d = TrimStart(d);

            Array.Reverse(d);

            return d;
        }

        /// <summary>
        /// Returns the AsnType representing an ASN.1 encoded OID.
        /// If conversion fails, the result is a partial conversion
        /// up to the point of failure. If the oid string is null or
        /// not well formed, an empty byte[] is returned.
        /// </summary>
        /// <param name="value">The string representing the object
        /// identifier to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded object identifier.</returns>
        /// <example>The following assigns the encoded AsnType
        /// for a RSA key to oid:
        /// <code>AsnType oid = CreateOid("1.2.840.113549.1.1.1")</code>
        /// </example>
        /// <seealso cref="CreateOid(byte[])"/>
        public static AsnType CreateOid(String value)
        {
            // Punt?
            if (IsEmpty(value))
                return null;

            String[] tokens = value.Split(new[] {' ', '.'});

            // Punt?
            if (IsEmpty(tokens))
                return null;

            // Parsing/Manipulation of the arc value
            UInt64 a = 0;

            // One or more strings are available
            var arcs = new List<UInt64>();

            foreach (String t in tokens)
            {
                // No empty or ill-formed strings...
                if (t.Length == 0)
                {
                    break;
                }

                try
                {
                    a = Convert.ToUInt64(t, CultureInfo.InvariantCulture);
                }
                catch (FormatException /*e*/)
                {
                    break;
                }
                catch (OverflowException /*e*/)
                {
                    break;
                }

                arcs.Add(a);
            }

            // Punt?
            if (0 == arcs.Count)
                return null;

            // Octets to be returned to caller
            var octets = new List<byte>();

            // Guard the case of a small list
            // The list has at least 1 item...    
            if (arcs.Count >= 1)
            {
                a = arcs[0]*40;
            }
            if (arcs.Count >= 2)
            {
                a += arcs[1];
            }
            octets.Add((byte)(a));

            // Add remaining arcs (subidentifiers)
            for (int i = 2; i < arcs.Count; i++)
            {
                // Scratch list builder for this arc
                var temp = new List<byte>();

                // The current arc (subidentifier)
                UInt64 arc = arcs[i];

                // Build the arc (subidentifier) byte array
                // The array is built in reverse (LSB to MSB).
                do
                {
                    // Each entry is formed from the low 7 bits (0x7F).
                    // Set high bit of all entries (0x80) per X.680. We
                    // will unset the high bit of the final byte later.
                    temp.Add((byte)(0x80 | (arc & 0x7F)));
                    arc >>= 7;
                } while (0 != arc);

                // Grab resulting array. Because of the do/while,
                // there is at least one value in the array.
                byte[] t = temp.ToArray();

                // Unset high bit of byte t[0]
                // t[0] will be LSB after the array is reversed.
                t[0] = (byte)(0x7F & t[0]);

                // MSB first...
                Array.Reverse(t);

                // Add to the resulting array
                foreach (byte b in t)
                {
                    octets.Add(b);
                }
            }

            return CreateOid(octets.ToArray());
        }

        /// <summary>
        /// Returns the AsnType representing an ASN.1 encoded OID.
        /// If conversion fails, the result is a partial conversion
        /// (up to the point of failure). If octets is null, an
        /// empty byte[] is returned.
        /// </summary>
        /// <param name="value">The packed byte[] representing the object
        /// identifier to be encoded.</param>
        /// <returns>Returns the AsnType representing an ASN.1
        /// encoded object identifier.</returns>
        /// <example>The following assigns the encoded AsnType for a RSA
        /// key to oid:
        /// <code>// Packed 1.2.840.113549.1.1.1
        /// byte[] rsa = new byte[] { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 };
        /// AsnType = CreateOid(rsa)</code>
        /// </example>
        /// <seealso cref="CreateOid(String)"/>
        public static AsnType CreateOid(byte[] value)
        {
            // Punt...
            if (IsEmpty(value))
            {
                return null;
            }

            // OID: Tag 0x06 (6, Universal, Primitive)
            return new AsnType(0x06, value);
        }

        private static byte[] Compliment2S(byte[] value)
        {
            if (IsEmpty(value))
            {
                return Empty;
            }

            // 2s Compliment of 0 is 0
            if (IsZero(value))
            {
                return Duplicate(value);
            }

            // Make a copy of octet array
            byte[] d = Duplicate(value);

            int carry = 1;
            for (int i = d.Length - 1; i >= 0; i--)
            {
                // Compliment
                d[i] = (byte)~d[i];

                // Add
                int j = d[i] + carry;

                // Write Back
                d[i] = (byte)(j & 0xFF);

                // Determine Next Carry
                if (0x100 == (j & 0x100))
                {
                    carry = 1;
                }
                else
                {
                    carry = 0;
                }
            }

            // Carry Array (we may need to carry out of 'd'
            byte[] c = null;
            if (1 == carry)
            {
                c = new byte[d.Length + 1];

                // Sign Extend....
                c[0] = 0xFF;

                Array.Copy(d, 0, c, 1, d.Length);
            }
            else
            {
                c = d;
            }

            return c;
        }

        private static byte[] Concatenate(AsnType[] values)
        {
            // Nothing in, nothing out
            if (IsEmpty(values))
                return new byte[] {};

            int length = 0;
            foreach (AsnType t in values)
            {
                if (null != t)
                {
                    length += t.GetBytes().Length;
                }
            }

            var cated = new byte[length];

            int current = 0;
            foreach (AsnType t in values)
            {
                if (null != t)
                {
                    byte[] b = t.GetBytes();

                    Array.Copy(b, 0, cated, current, b.Length);
                    current += b.Length;
                }
            }

            return cated;
        }

        private static byte[] Concatenate(byte[] first, byte[] second)
        {
            return Concatenate(new[] {first, second});
        }

        private static byte[] Concatenate(byte[][] values)
        {
            // Nothing in, nothing out
            if (IsEmpty(values))
                return new byte[] {};

            int length = 0;
            foreach (var b in values)
            {
                if (null != b)
                {
                    length += b.Length;
                }
            }

            var cated = new byte[length];

            int current = 0;
            foreach (var b in values)
            {
                if (null != b)
                {
                    Array.Copy(b, 0, cated, current, b.Length);
                    current += b.Length;
                }
            }

            return cated;
        }

        private static byte[] Duplicate(byte[] b)
        {
            if (IsEmpty(b))
            {
                return Empty;
            }

            var d = new byte[b.Length];
            Array.Copy(b, d, b.Length);

            return d;
        }

        private static bool IsZero(byte[] octets)
        {
            if (IsEmpty(octets))
            {
                return false;
            }

            bool allZeros = true;
            for (int i = 0; i < octets.Length; i++)
            {
                if (0 != octets[i])
                {
                    allZeros = false;
                    break;
                }
            }
            return allZeros;
        }

        private static bool IsEmpty(byte[] octets)
        {
            if (null == octets || 0 == octets.Length)
            {
                return true;
            }

            return false;
        }

        private static bool IsEmpty(String s)
        {
            if (null == s || 0 == s.Length)
            {
                return true;
            }

            return false;
        }

        private static bool IsEmpty(String[] strings)
        {
            if (null == strings || 0 == strings.Length)
                return true;

            return false;
        }

        private static bool IsEmpty(AsnType value)
        {
            if (null == value)
            {
                return true;
            }

            return false;
        }

        private static bool IsEmpty(AsnType[] values)
        {
            if (null == values || 0 == values.Length)
                return true;

            return false;
        }

        private static bool IsEmpty(byte[][] arrays)
        {
            if (null == arrays || 0 == arrays.Length)
                return true;

            return false;
        }

        #region Nested type: AsnMessage

        public class AsnMessage
        {
            private readonly String mFormat;
            private readonly byte[] mOctets;

            public AsnMessage(byte[] octets, String format)
            {
                mOctets = octets;
                mFormat = format;
            }

            public int Length
            {
                get
                {
                    if (null == mOctets)
                    {
                        return 0;
                    }
                    return mOctets.Length;
                }
                // set { m_length = value; }
            }

            public byte[] GetBytes()
            {
                if (null == mOctets)
                {
                    return new byte[] {};
                }

                return mOctets;
            }

            public String GetFormat()
            {
                return mFormat;
            }
        }

        #endregion

        #region Nested type: AsnType

        public class AsnType
        {
            // Constructors
            // No default - must specify tag and data

            private readonly byte[] mTag;
            private byte[] mLength;
            private byte[] mOctets;
            private bool mRaw;

            public AsnType(byte tag, byte octet)
            {
                mRaw = false;
                mTag = new[] {tag};
                mOctets = new[] {octet};
            }

            public AsnType(byte tag, byte[] octets)
            {
                mRaw = false;
                mTag = new[] {tag};
                mOctets = octets;
            }

            public AsnType(byte tag, byte[] length, byte[] octets)
            {
                mRaw = true;
                mTag = new[] {tag};
                mLength = length;
                mOctets = octets;
            }

            private bool Raw
            {
                get { return mRaw; }
                set { mRaw = value; }
            }

            // Setters and Getters

            public byte[] Tag
            {
                get
                {
                    if (null == mTag)
                        return Empty;
                    return mTag;
                }
                // set { m_tag = value; }
            }

            public byte[] Length
            {
                get
                {
                    if (null == mLength)
                        return Empty;
                    return mLength;
                }
                // set { m_length = value; }
            }

            public byte[] Octets
            {
                get
                {
                    if (null == mOctets)
                    {
                        return Empty;
                    }
                    return mOctets;
                }
                set { mOctets = value; }
            }

            // Methods
            internal byte[] GetBytes()
            {
                // Created raw by user
                // return the bytes....
                if (mRaw)
                {
                    return Concatenate(
                        new[] {mTag, mLength, mOctets}
                        );
                }

                SetLength();

                // Special case
                // Null does not use length
                if (0x05 == mTag[0])
                {
                    return Concatenate(
                        new[] {mTag, mOctets}
                        );
                }

                return Concatenate(
                    new[] {mTag, mLength, mOctets}
                    );
            }

            private void SetLength()
            {
                if (null == mOctets)
                {
                    mLength = Zero;
                    return;
                }

                // Special case
                // Null does not use length
                if (0x05 == mTag[0])
                {
                    mLength = Empty;
                    return;
                }

                byte[] length = null;

                // Length: 0 <= l < 0x80
                if (mOctets.Length < 0x80)
                {
                    length = new byte[1];
                    length[0] = (byte)mOctets.Length;
                }
                    // 0x80 < length <= 0xFF
                else if (mOctets.Length <= 0xFF)
                {
                    length = new byte[2];
                    length[0] = 0x81;
                    length[1] = (byte)((mOctets.Length & 0xFF));
                }

                    //
                    // We should almost never see these...
                    //

                    // 0xFF < length <= 0xFFFF
                else if (mOctets.Length <= 0xFFFF)
                {
                    length = new byte[3];
                    length[0] = 0x82;
                    length[1] = (byte)((mOctets.Length & 0xFF00) >> 8);
                    length[2] = (byte)((mOctets.Length & 0xFF));
                }

                    // 0xFFFF < length <= 0xFFFFFF
                else if (mOctets.Length <= 0xFFFFFF)
                {
                    length = new byte[4];
                    length[0] = 0x83;
                    length[1] = (byte)((mOctets.Length & 0xFF0000) >> 16);
                    length[2] = (byte)((mOctets.Length & 0xFF00) >> 8);
                    length[3] = (byte)((mOctets.Length & 0xFF));
                }
                    // 0xFFFFFF < length <= 0xFFFFFFFF
                else
                {
                    length = new byte[5];
                    length[0] = 0x84;
                    length[1] = (byte)((mOctets.Length & 0xFF000000) >> 24);
                    length[2] = (byte)((mOctets.Length & 0xFF0000) >> 16);
                    length[3] = (byte)((mOctets.Length & 0xFF00) >> 8);
                    length[4] = (byte)((mOctets.Length & 0xFF));
                }

                mLength = length;
            }

            private byte[] Concatenate(byte[][] values)
            {
                // Nothing in, nothing out
                if (IsEmpty(values))
                    return new byte[] {};

                int length = 0;
                foreach (var b in values)
                {
                    if (null != b) length += b.Length;
                }

                var cated = new byte[length];

                int current = 0;
                foreach (var b in values)
                {
                    if (null != b)
                    {
                        Array.Copy(b, 0, cated, current, b.Length);
                        current += b.Length;
                    }
                }

                return cated;
            }
        };

        #endregion
    }

		public class AsnKeyParser
    {
        private AsnParser parser;

        public AsnKeyParser(String pathname)
        {
            using (BinaryReader reader = new BinaryReader(
                new FileStream(pathname, FileMode.Open, FileAccess.Read)))
            {
                FileInfo info = new FileInfo(pathname);

                parser = new AsnParser(reader.ReadBytes((int)info.Length));
            }
        }

        public AsnKeyParser(byte[] data)
        {
            parser = new AsnParser(data);
        }

        internal static byte[] TrimLeadingZero(byte[] values)
        {
            byte[] r = null;
            if ((0x00 == values[0]) && (values.Length > 1))
            {
                r = new byte[values.Length - 1];
                Array.Copy(values, 1, r, 0, values.Length - 1);
            }
            else
            {
                r = new byte[values.Length];
                Array.Copy(values, r, values.Length);
            }

            return r;
        }

        internal static bool EqualOid(byte[] first, byte[] second)
        {
            if (first.Length != second.Length)
            { return false; }

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                { return false; }
            }

            return true;
        }

        public RSAParameters ParseRSAPublicKey()
        {
            RSAParameters parameters = new RSAParameters();

            // Current value
            byte[] value = null;

            // Sanity Check
            int length = 0;

            // Checkpoint
            int position = parser.CurrentPosition();

            // Ignore Sequence - PublicKeyInfo
            length = parser.NextSequence();
            if (length != parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect Sequence Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture),
                  parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();

            // Ignore Sequence - AlgorithmIdentifier
            length = parser.NextSequence();
            if (length > parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect AlgorithmIdentifier Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture),
                  parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();
            // Grab the OID
            value = parser.NextOID();
            byte[] oid = { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
            if (!EqualOid(value, oid))
            { throw new BerDecodeException("Expected OID 1.2.840.113549.1.1.1", position); }

            // Optional Parameters
            if (parser.IsNextNull())
            {
                parser.NextNull();
                // Also OK: value = parser.Next();
            }
            else
            {
                // Gracefully skip the optional data
                value = parser.Next();
            }

            // Checkpoint
            position = parser.CurrentPosition();

            // Ignore BitString - PublicKey
            length = parser.NextBitString();
            if (length > parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect PublicKey Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture),
                  (parser.RemainingBytes()).ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();

            // Ignore Sequence - RSAPublicKey
            length = parser.NextSequence();
            if (length < parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect RSAPublicKey Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture),
                  parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            parameters.Modulus = TrimLeadingZero(parser.NextInteger());
            parameters.Exponent = TrimLeadingZero(parser.NextInteger());

            Debug.Assert(0 == parser.RemainingBytes());

            return parameters;
        }

        internal RSAParameters ParseRSAPrivateKey()
        {
            RSAParameters parameters = new RSAParameters();

            // Current value
            byte[] value = null;

            // Checkpoint
            int position = parser.CurrentPosition();

            // Sanity Check
            int length = 0;

            // Ignore Sequence - PrivateKeyInfo
            length = parser.NextSequence();
            if (length != parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect Sequence Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();
            // Version
            value = parser.NextInteger();
            if (0x00 != value[0])
            {
                StringBuilder sb = new StringBuilder("Incorrect PrivateKeyInfo Version. ");
                BigInteger v = new BigInteger(value);
                sb.AppendFormat("Expected: 0, Specified: {0}", v.ToString(10));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();

            // Ignore Sequence - AlgorithmIdentifier
            length = parser.NextSequence();
            if (length > parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect AlgorithmIdentifier Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture),
                  parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();

            // Grab the OID
            value = parser.NextOID();
            byte[] oid = { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
            if (!EqualOid(value, oid))
            { throw new BerDecodeException("Expected OID 1.2.840.113549.1.1.1", position); }

            // Optional Parameters
            if (parser.IsNextNull())
            {
                parser.NextNull();
                // Also OK: value = parser.Next();
            }
            else
            {
                // Gracefully skip the optional data
                value = parser.Next();
            }

            // Checkpoint
            position = parser.CurrentPosition();

            // Ignore OctetString - PrivateKey
            length = parser.NextOctetString();
            if (length > parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect PrivateKey Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture),
                  parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();

            // Ignore Sequence - RSAPrivateKey
            length = parser.NextSequence();
            if (length < parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect RSAPrivateKey Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture),
                  parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();
            // Version
            value = parser.NextInteger();
            if (0x00 != value[0])
            {
                StringBuilder sb = new StringBuilder("Incorrect RSAPrivateKey Version. ");
                BigInteger v = new BigInteger(value);
                sb.AppendFormat("Expected: 0, Specified: {0}", v.ToString(10));
                throw new BerDecodeException(sb.ToString(), position);
            }

            parameters.Modulus = TrimLeadingZero(parser.NextInteger());
            parameters.Exponent = TrimLeadingZero(parser.NextInteger());
            parameters.D = TrimLeadingZero(parser.NextInteger());
            parameters.P = TrimLeadingZero(parser.NextInteger());
            parameters.Q = TrimLeadingZero(parser.NextInteger());
            parameters.DP = TrimLeadingZero(parser.NextInteger());
            parameters.DQ = TrimLeadingZero(parser.NextInteger());
            parameters.InverseQ = TrimLeadingZero(parser.NextInteger());

            Debug.Assert(0 == parser.RemainingBytes());

            return parameters;
        }

        internal DSAParameters ParseDSAPublicKey()
        {
            DSAParameters parameters = new DSAParameters();

            // Current value
            byte[] value = null;

            // Current Position
            int position = parser.CurrentPosition();
            // Sanity Checks
            int length = 0;

            // Ignore Sequence - PublicKeyInfo
            length = parser.NextSequence();
            if (length != parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect Sequence Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();

            // Ignore Sequence - AlgorithmIdentifier
            length = parser.NextSequence();
            if (length > parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect AlgorithmIdentifier Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();

            // Grab the OID
            value = parser.NextOID();
            byte[] oid = { 0x2a, 0x86, 0x48, 0xce, 0x38, 0x04, 0x01 };
            if (!EqualOid(value, oid))
            { throw new BerDecodeException("Expected OID 1.2.840.10040.4.1", position); }


            // Checkpoint
            position = parser.CurrentPosition();

            // Ignore Sequence - DSS-Params
            length = parser.NextSequence();
            if (length > parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect DSS-Params Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Next three are curve parameters
            parameters.P = TrimLeadingZero(parser.NextInteger());
            parameters.Q = TrimLeadingZero(parser.NextInteger());
            parameters.G = TrimLeadingZero(parser.NextInteger());

            // Ignore BitString - PrivateKey
            parser.NextBitString();

            // Public Key
            parameters.Y = TrimLeadingZero(parser.NextInteger());

            Debug.Assert(0 == parser.RemainingBytes());

            return parameters;
        }

        internal DSAParameters ParseDSAPrivateKey()
        {
            DSAParameters parameters = new DSAParameters();

            // Current value
            byte[] value = null;

            // Current Position
            int position = parser.CurrentPosition();
            // Sanity Checks
            int length = 0;

            // Ignore Sequence - PrivateKeyInfo
            length = parser.NextSequence();
            if (length != parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect Sequence Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();
            // Version
            value = parser.NextInteger();
            if (0x00 != value[0])
            {
                throw new BerDecodeException("Incorrect PrivateKeyInfo Version", position);
            }

            // Checkpoint
            position = parser.CurrentPosition();

            // Ignore Sequence - AlgorithmIdentifier
            length = parser.NextSequence();
            if (length > parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect AlgorithmIdentifier Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Checkpoint
            position = parser.CurrentPosition();
            // Grab the OID
            value = parser.NextOID();
            byte[] oid = { 0x2a, 0x86, 0x48, 0xce, 0x38, 0x04, 0x01 };
            if (!EqualOid(value, oid))
            { throw new BerDecodeException("Expected OID 1.2.840.10040.4.1", position); }

            // Checkpoint
            position = parser.CurrentPosition();

            // Ignore Sequence - DSS-Params
            length = parser.NextSequence();
            if (length > parser.RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect DSS-Params Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  length.ToString(CultureInfo.InvariantCulture), parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            // Next three are curve parameters
            parameters.P = TrimLeadingZero(parser.NextInteger());
            parameters.Q = TrimLeadingZero(parser.NextInteger());
            parameters.G = TrimLeadingZero(parser.NextInteger());

            // Ignore OctetString - PrivateKey
            parser.NextOctetString();

            // Private Key
            parameters.X = TrimLeadingZero(parser.NextInteger());

            Debug.Assert(0 == parser.RemainingBytes());

            return parameters;
        }
    }

		class AsnParser
    {
        private List<byte> octets;
        private int initialCount;

        public AsnParser(byte[] values)
        {
            octets = new List<byte>(values.Length);
            octets.AddRange(values);

            initialCount = octets.Count;
        }

        internal int CurrentPosition()
        {
            return initialCount - octets.Count;
        }

        internal int RemainingBytes()
        {
            return octets.Count;
        }

        private int GetLength()
        {
            int length = 0;

            // Checkpoint
            int position = CurrentPosition();

            try
            {
                byte b = GetNextOctet();

                if (b == (b & 0x7f)) { return b; }
                int i = b & 0x7f;

                if (i > 4)
                {
                    StringBuilder sb = new StringBuilder("Invalid Length Encoding. ");
                    sb.AppendFormat("Length uses {0} octets",
                      i.ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                while (0 != i--)
                {
                    // shift left
                    length <<= 8;

                    length |= GetNextOctet();
                }
            }
            catch (ArgumentOutOfRangeException ex)
            { throw new BerDecodeException("Error Parsing Key", position, ex); }

            return length;
        }

        internal byte[] Next()
        {
            int position = CurrentPosition();

            try
            {
                byte b = GetNextOctet();

                int length = GetLength();
                if (length > RemainingBytes())
                {
                    StringBuilder sb = new StringBuilder("Incorrect Size. ");
                    sb.AppendFormat("Specified: {0}, Remaining: {1}",
                      length.ToString(CultureInfo.InvariantCulture),
                      RemainingBytes().ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                return GetOctets(length);
            }

            catch (ArgumentOutOfRangeException ex)
            { throw new BerDecodeException("Error Parsing Key", position, ex); }
        }

        internal byte GetNextOctet()
        {
            int position = CurrentPosition();

            if (0 == RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  1.ToString(CultureInfo.InvariantCulture),
                  RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            byte b = GetOctets(1)[0];

            return b;
        }

        internal byte[] GetOctets(int octetCount)
        {
            int position = CurrentPosition();

            if (octetCount > RemainingBytes())
            {
                StringBuilder sb = new StringBuilder("Incorrect Size. ");
                sb.AppendFormat("Specified: {0}, Remaining: {1}",
                  octetCount.ToString(CultureInfo.InvariantCulture),
                  RemainingBytes().ToString(CultureInfo.InvariantCulture));
                throw new BerDecodeException(sb.ToString(), position);
            }

            byte[] values = new byte[octetCount];

            try
            {
                octets.CopyTo(0, values, 0, octetCount);
                octets.RemoveRange(0, octetCount);
            }

            catch (ArgumentOutOfRangeException ex)
            { throw new BerDecodeException("Error Parsing Key", position, ex); }

            return values;
        }

        internal bool IsNextNull()
        {
            return 0x05 == octets[0];
        }

        internal int NextNull()
        {
            int position = CurrentPosition();

            try
            {
                byte b = GetNextOctet();
                if (0x05 != b)
                {
                    StringBuilder sb = new StringBuilder("Expected Null. ");
                    sb.AppendFormat("Specified Identifier: {0}", b.ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                // Next octet must be 0
                b = GetNextOctet();
                if (0x00 != b)
                {
                    StringBuilder sb = new StringBuilder("Null has non-zero size. ");
                    sb.AppendFormat("Size: {0}", b.ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                return 0;
            }

            catch (ArgumentOutOfRangeException ex)
            { throw new BerDecodeException("Error Parsing Key", position, ex); }
        }

        internal bool IsNextSequence()
        {
            return 0x30 == octets[0];
        }

        internal int NextSequence()
        {
            int position = CurrentPosition();

            try
            {
                byte b = GetNextOctet();
                if (0x30 != b)
                {
                    StringBuilder sb = new StringBuilder("Expected Sequence. ");
                    sb.AppendFormat("Specified Identifier: {0}",
                      b.ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                int length = GetLength();
                if (length > RemainingBytes())
                {
                    StringBuilder sb = new StringBuilder("Incorrect Sequence Size. ");
                    sb.AppendFormat("Specified: {0}, Remaining: {1}",
                      length.ToString(CultureInfo.InvariantCulture),
                      RemainingBytes().ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                return length;
            }

            catch (ArgumentOutOfRangeException ex)
            { throw new BerDecodeException("Error Parsing Key", position, ex); }
        }

        internal bool IsNextOctetString()
        {
            return 0x04 == octets[0];
        }

        internal int NextOctetString()
        {
            int position = CurrentPosition();

            try
            {
                byte b = GetNextOctet();
                if (0x04 != b)
                {
                    StringBuilder sb = new StringBuilder("Expected Octet String. ");
                    sb.AppendFormat("Specified Identifier: {0}", b.ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                int length = GetLength();
                if (length > RemainingBytes())
                {
                    StringBuilder sb = new StringBuilder("Incorrect Octet String Size. ");
                    sb.AppendFormat("Specified: {0}, Remaining: {1}",
                      length.ToString(CultureInfo.InvariantCulture),
                      RemainingBytes().ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                return length;
            }

            catch (ArgumentOutOfRangeException ex)
            { throw new BerDecodeException("Error Parsing Key", position, ex); }
        }

        internal bool IsNextBitString()
        {
            return 0x03 == octets[0];
        }

        internal int NextBitString()
        {
            int position = CurrentPosition();

            try
            {
                byte b = GetNextOctet();
                if (0x03 != b)
                {
                    StringBuilder sb = new StringBuilder("Expected Bit String. ");
                    sb.AppendFormat("Specified Identifier: {0}", b.ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                int length = GetLength();

                // We need to consume unused bits, which is the first
                //   octet of the remaing values
                b = octets[0];
                octets.RemoveAt(0);
                length--;

                if (0x00 != b)
                { throw new BerDecodeException("The first octet of BitString must be 0", position); }

                return length;
            }

            catch (ArgumentOutOfRangeException ex)
            { throw new BerDecodeException("Error Parsing Key", position, ex); }
        }

        internal bool IsNextInteger()
        {
            return 0x02 == octets[0];
        }

        internal byte[] NextInteger()
        {
            int position = CurrentPosition();

            try
            {
                byte b = GetNextOctet();
                if (0x02 != b)
                {
                    StringBuilder sb = new StringBuilder("Expected Integer. ");
                    sb.AppendFormat("Specified Identifier: {0}", b.ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                int length = GetLength();
                if (length > RemainingBytes())
                {
                    StringBuilder sb = new StringBuilder("Incorrect Integer Size. ");
                    sb.AppendFormat("Specified: {0}, Remaining: {1}",
                      length.ToString(CultureInfo.InvariantCulture),
                      RemainingBytes().ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                return GetOctets(length);
            }

            catch (ArgumentOutOfRangeException ex)
            { throw new BerDecodeException("Error Parsing Key", position, ex); }
        }

        internal byte[] NextOID()
        {
            int position = CurrentPosition();

            try
            {
                byte b = GetNextOctet();
                if (0x06 != b)
                {
                    StringBuilder sb = new StringBuilder("Expected Object Identifier. ");
                    sb.AppendFormat("Specified Identifier: {0}",
                      b.ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                int length = GetLength();
                if (length > RemainingBytes())
                {
                    StringBuilder sb = new StringBuilder("Incorrect Object Identifier Size. ");
                    sb.AppendFormat("Specified: {0}, Remaining: {1}",
                      length.ToString(CultureInfo.InvariantCulture),
                      RemainingBytes().ToString(CultureInfo.InvariantCulture));
                    throw new BerDecodeException(sb.ToString(), position);
                }

                byte[] values = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    values[i] = octets[0];
                    octets.RemoveAt(0);
                }

                return values;
            }

            catch (ArgumentOutOfRangeException ex)
            { throw new BerDecodeException("Error Parsing Key", position, ex); }
        }
    }

		[Serializable]
		public sealed class BerDecodeException : Exception, ISerializable
    {
        private int m_position;
        public int Position
        { get { return m_position; } }

        public override string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder(base.Message);

                sb.AppendFormat(" (Position {0}){1}",
                  m_position, Environment.NewLine);

                return sb.ToString();
            }
        }

        public BerDecodeException()
            : base() { }

        public BerDecodeException(String message)
            : base(message) { }

        public BerDecodeException(String message, Exception ex)
            : base(message, ex) { }

        public BerDecodeException(String message, int position)
            : base(message) { m_position = position; }

        public BerDecodeException(String message, int position, Exception ex)
            : base(message, ex) { m_position = position; }

        private BerDecodeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { m_position = info.GetInt32("Position"); }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Position", m_position);
        }
    }

		public class BigInteger
		{
			// maximum length of the BigInteger in uint (4 bytes)
			// change this to suit the required level of precision.

			private const int maxLength = 1024;

			// primes smaller than 2000 to test the generated prime number

			public static readonly int[] primesBelow2000 = {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97,
            101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199,
            211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293,
            307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397,
            401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499,
            503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599,
            601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691,
            701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797,
            809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887,
            907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997,
            1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097,
            1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193,
            1201, 1213, 1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297,
            1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381, 1399,
            1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499,
            1511, 1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597,
            1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693, 1697, 1699,
            1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789,
            1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
            1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993, 1997, 1999 };


			private uint[] data = null;             // stores bytes from the Big Integer
			public int dataLength;                 // number of actual chars used


			//***********************************************************************
			// Constructor (Default value for BigInteger is 0
			//***********************************************************************

			public BigInteger()
			{
				data = new uint[maxLength];
				dataLength = 1;
			}


			//***********************************************************************
			// Constructor (Default value provided by long)
			//***********************************************************************

			public BigInteger(long value)
			{
				data = new uint[maxLength];
				long tempVal = value;

				// copy bytes from long to BigInteger without any assumption of
				// the length of the long datatype

				dataLength = 0;
				while (value != 0 && dataLength < maxLength)
				{
					data[dataLength] = (uint)(value & 0xFFFFFFFF);
					value >>= 32;
					dataLength++;
				}

				if (tempVal > 0)         // overflow check for +ve value
				{
					if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
						throw (new ArithmeticException("Positive overflow in constructor."));
				}
				else if (tempVal < 0)    // underflow check for -ve value
				{
					if (value != -1 || (data[dataLength - 1] & 0x80000000) == 0)
						throw (new ArithmeticException("Negative underflow in constructor."));
				}

				if (dataLength == 0)
					dataLength = 1;
			}


			//***********************************************************************
			// Constructor (Default value provided by ulong)
			//***********************************************************************

			public BigInteger(ulong value)
			{
				data = new uint[maxLength];

				// copy bytes from ulong to BigInteger without any assumption of
				// the length of the ulong datatype

				dataLength = 0;
				while (value != 0 && dataLength < maxLength)
				{
					data[dataLength] = (uint)(value & 0xFFFFFFFF);
					value >>= 32;
					dataLength++;
				}

				if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
					throw (new ArithmeticException("Positive overflow in constructor."));

				if (dataLength == 0)
					dataLength = 1;
			}



			//***********************************************************************
			// Constructor (Default value provided by BigInteger)
			//***********************************************************************

			public BigInteger(BigInteger bi)
			{
				data = new uint[maxLength];

				dataLength = bi.dataLength;

				for (int i = 0; i < dataLength; i++)
					data[i] = bi.data[i];
			}


			//***********************************************************************
			// Constructor (Default value provided by a string of digits of the
			//              specified base)
			//
			// Example (base 10)
			// -----------------
			// To initialize "a" with the default value of 1234 in base 10
			//      BigInteger a = new BigInteger("1234", 10)
			//
			// To initialize "a" with the default value of -1234
			//      BigInteger a = new BigInteger("-1234", 10)
			//
			// Example (base 16)
			// -----------------
			// To initialize "a" with the default value of 0x1D4F in base 16
			//      BigInteger a = new BigInteger("1D4F", 16)
			//
			// To initialize "a" with the default value of -0x1D4F
			//      BigInteger a = new BigInteger("-1D4F", 16)
			//
			// Note that string values are specified in the <sign><magnitude>
			// format.
			//
			//***********************************************************************

			public BigInteger(string value, int radix)
			{
				BigInteger multiplier = new BigInteger(1);
				BigInteger result = new BigInteger();
				value = (value.ToUpper()).Trim();
				int limit = 0;

				if (value[0] == '-')
					limit = 1;

				for (int i = value.Length - 1; i >= limit; i--)
				{
					int posVal = (int)value[i];

					if (posVal >= '0' && posVal <= '9')
						posVal -= '0';
					else if (posVal >= 'A' && posVal <= 'Z')
						posVal = (posVal - 'A') + 10;
					else
						posVal = 9999999;       // arbitrary large


					if (posVal >= radix)
						throw (new ArithmeticException("Invalid string in constructor."));
					else
					{
						if (value[0] == '-')
							posVal = -posVal;

						result = result + (multiplier * posVal);

						if ((i - 1) >= limit)
							multiplier = multiplier * radix;
					}
				}

				if (value[0] == '-')     // negative values
				{
					if ((result.data[maxLength - 1] & 0x80000000) == 0)
						throw (new ArithmeticException("Negative underflow in constructor."));
				}
				else    // positive values
				{
					if ((result.data[maxLength - 1] & 0x80000000) != 0)
						throw (new ArithmeticException("Positive overflow in constructor."));
				}

				data = new uint[maxLength];
				for (int i = 0; i < result.dataLength; i++)
					data[i] = result.data[i];

				dataLength = result.dataLength;
			}


			//***********************************************************************
			// Constructor (Default value provided by an array of bytes)
			//
			// The lowest index of the input byte array (i.e [0]) should contain the
			// most significant byte of the number, and the highest index should
			// contain the least significant byte.
			//
			// E.g.
			// To initialize "a" with the default value of 0x1D4F in base 16
			//      byte[] temp = { 0x1D, 0x4F };
			//      BigInteger a = new BigInteger(temp)
			//
			// Note that this method of initialization does not allow the
			// sign to be specified.
			//
			//***********************************************************************

			public BigInteger(byte[] inData)
			{
				dataLength = inData.Length >> 2;

				int leftOver = inData.Length & 0x3;
				if (leftOver != 0)         // length not multiples of 4
					dataLength++;


				if (dataLength > maxLength)
					throw (new ArithmeticException("Byte overflow in constructor."));

				data = new uint[maxLength];

				for (int i = inData.Length - 1, j = 0; i >= 3; i -= 4, j++)
				{
					data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
									 (inData[i - 1] << 8) + inData[i]);
				}

				if (leftOver == 1)
					data[dataLength - 1] = (uint)inData[0];
				else if (leftOver == 2)
					data[dataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
				else if (leftOver == 3)
					data[dataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);


				while (dataLength > 1 && data[dataLength - 1] == 0)
					dataLength--;

				//Console.WriteLine("Len = " + dataLength);
			}


			//***********************************************************************
			// Constructor (Default value provided by an array of bytes of the
			// specified length.)
			//***********************************************************************

			public BigInteger(byte[] inData, int inLen)
			{
				dataLength = inLen >> 2;

				int leftOver = inLen & 0x3;
				if (leftOver != 0)         // length not multiples of 4
					dataLength++;

				if (dataLength > maxLength || inLen > inData.Length)
					throw (new ArithmeticException("Byte overflow in constructor."));


				data = new uint[maxLength];

				for (int i = inLen - 1, j = 0; i >= 3; i -= 4, j++)
				{
					data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
									 (inData[i - 1] << 8) + inData[i]);
				}

				if (leftOver == 1)
					data[dataLength - 1] = (uint)inData[0];
				else if (leftOver == 2)
					data[dataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
				else if (leftOver == 3)
					data[dataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);


				if (dataLength == 0)
					dataLength = 1;

				while (dataLength > 1 && data[dataLength - 1] == 0)
					dataLength--;

				//Console.WriteLine("Len = " + dataLength);
			}


			//***********************************************************************
			// Constructor (Default value provided by an array of unsigned integers)
			//*********************************************************************

			public BigInteger(uint[] inData)
			{
				dataLength = inData.Length;

				if (dataLength > maxLength)
					throw (new ArithmeticException("Byte overflow in constructor."));

				data = new uint[maxLength];

				for (int i = dataLength - 1, j = 0; i >= 0; i--, j++)
					data[j] = inData[i];

				while (dataLength > 1 && data[dataLength - 1] == 0)
					dataLength--;

				//Console.WriteLine("Len = " + dataLength);
			}


			//***********************************************************************
			// Overloading of the typecast operator.
			// For BigInteger bi = 10;
			//***********************************************************************

			public static implicit operator BigInteger(long value)
			{
				return (new BigInteger(value));
			}

			public static implicit operator BigInteger(ulong value)
			{
				return (new BigInteger(value));
			}

			public static implicit operator BigInteger(int value)
			{
				return (new BigInteger((long)value));
			}

			public static implicit operator BigInteger(uint value)
			{
				return (new BigInteger((ulong)value));
			}


			//***********************************************************************
			// Overloading of addition operator
			//***********************************************************************

			public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
			{
				BigInteger result = new BigInteger();

				result.dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

				long carry = 0;
				for (int i = 0; i < result.dataLength; i++)
				{
					long sum = (long)bi1.data[i] + (long)bi2.data[i] + carry;
					carry = sum >> 32;
					result.data[i] = (uint)(sum & 0xFFFFFFFF);
				}

				if (carry != 0 && result.dataLength < maxLength)
				{
					result.data[result.dataLength] = (uint)(carry);
					result.dataLength++;
				}

				while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
					result.dataLength--;


				// overflow check
				int lastPos = maxLength - 1;
				if ((bi1.data[lastPos] & 0x80000000) == (bi2.data[lastPos] & 0x80000000) &&
				   (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
				{
					throw (new ArithmeticException());
				}

				return result;
			}


			//***********************************************************************
			// Overloading of the unary ++ operator
			//***********************************************************************

			public static BigInteger operator ++(BigInteger bi1)
			{
				BigInteger result = new BigInteger(bi1);

				long val, carry = 1;
				int index = 0;

				while (carry != 0 && index < maxLength)
				{
					val = (long)(result.data[index]);
					val++;

					result.data[index] = (uint)(val & 0xFFFFFFFF);
					carry = val >> 32;

					index++;
				}

				if (index > result.dataLength)
					result.dataLength = index;
				else
				{
					while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
						result.dataLength--;
				}

				// overflow check
				int lastPos = maxLength - 1;

				// overflow if initial value was +ve but ++ caused a sign
				// change to negative.

				if ((bi1.data[lastPos] & 0x80000000) == 0 &&
				   (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
				{
					throw (new ArithmeticException("Overflow in ++."));
				}
				return result;
			}


			//***********************************************************************
			// Overloading of subtraction operator
			//***********************************************************************

			public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
			{
				BigInteger result = new BigInteger();

				result.dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

				long carryIn = 0;
				for (int i = 0; i < result.dataLength; i++)
				{
					long diff;

					diff = (long)bi1.data[i] - (long)bi2.data[i] - carryIn;
					result.data[i] = (uint)(diff & 0xFFFFFFFF);

					if (diff < 0)
						carryIn = 1;
					else
						carryIn = 0;
				}

				// roll over to negative
				if (carryIn != 0)
				{
					for (int i = result.dataLength; i < maxLength; i++)
						result.data[i] = 0xFFFFFFFF;
					result.dataLength = maxLength;
				}

				// fixed in v1.03 to give correct datalength for a - (-b)
				while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
					result.dataLength--;

				// overflow check

				int lastPos = maxLength - 1;
				if ((bi1.data[lastPos] & 0x80000000) != (bi2.data[lastPos] & 0x80000000) &&
				   (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
				{
					throw (new ArithmeticException());
				}

				return result;
			}


			//***********************************************************************
			// Overloading of the unary -- operator
			//***********************************************************************

			public static BigInteger operator --(BigInteger bi1)
			{
				BigInteger result = new BigInteger(bi1);

				long val;
				bool carryIn = true;
				int index = 0;

				while (carryIn && index < maxLength)
				{
					val = (long)(result.data[index]);
					val--;

					result.data[index] = (uint)(val & 0xFFFFFFFF);

					if (val >= 0)
						carryIn = false;

					index++;
				}

				if (index > result.dataLength)
					result.dataLength = index;

				while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
					result.dataLength--;

				// overflow check
				int lastPos = maxLength - 1;

				// overflow if initial value was -ve but -- caused a sign
				// change to positive.

				if ((bi1.data[lastPos] & 0x80000000) != 0 &&
				   (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
				{
					throw (new ArithmeticException("Underflow in --."));
				}

				return result;
			}


			//***********************************************************************
			// Overloading of multiplication operator
			//***********************************************************************

			public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
			{
				int lastPos = maxLength - 1;
				bool bi1Neg = false, bi2Neg = false;

				// take the absolute value of the inputs
				try
				{
					if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
					{
						bi1Neg = true; bi1 = -bi1;
					}
					if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
					{
						bi2Neg = true; bi2 = -bi2;
					}
				}
				catch (Exception) { }

				BigInteger result = new BigInteger();

				// multiply the absolute values
				try
				{
					for (int i = 0; i < bi1.dataLength; i++)
					{
						if (bi1.data[i] == 0) continue;

						ulong mcarry = 0;
						for (int j = 0, k = i; j < bi2.dataLength; j++, k++)
						{
							// k = i + j
							ulong val = ((ulong)bi1.data[i] * (ulong)bi2.data[j]) +
										 (ulong)result.data[k] + mcarry;

							result.data[k] = (uint)(val & 0xFFFFFFFF);
							mcarry = (val >> 32);
						}

						if (mcarry != 0)
							result.data[i + bi2.dataLength] = (uint)mcarry;
					}
				}
				catch (Exception)
				{
					throw (new ArithmeticException("Multiplication overflow."));
				}


				result.dataLength = bi1.dataLength + bi2.dataLength;
				if (result.dataLength > maxLength)
					result.dataLength = maxLength;

				while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
					result.dataLength--;

				// overflow check (result is -ve)
				if ((result.data[lastPos] & 0x80000000) != 0)
				{
					if (bi1Neg != bi2Neg && result.data[lastPos] == 0x80000000)    // different sign
					{
						// handle the special case where multiplication produces
						// a max negative number in 2's complement.

						if (result.dataLength == 1)
							return result;
						else
						{
							bool isMaxNeg = true;
							for (int i = 0; i < result.dataLength - 1 && isMaxNeg; i++)
							{
								if (result.data[i] != 0)
									isMaxNeg = false;
							}

							if (isMaxNeg)
								return result;
						}
					}

					throw (new ArithmeticException("Multiplication overflow."));
				}

				// if input has different signs, then result is -ve
				if (bi1Neg != bi2Neg)
					return -result;

				return result;
			}



			//***********************************************************************
			// Overloading of unary << operators
			//***********************************************************************

			public static BigInteger operator <<(BigInteger bi1, int shiftVal)
			{
				BigInteger result = new BigInteger(bi1);
				result.dataLength = shiftLeft(result.data, shiftVal);

				return result;
			}


			// least significant bits at lower part of buffer

			private static int shiftLeft(uint[] buffer, int shiftVal)
			{
				int shiftAmount = 32;
				int bufLen = buffer.Length;

				while (bufLen > 1 && buffer[bufLen - 1] == 0)
					bufLen--;

				for (int count = shiftVal; count > 0; )
				{
					if (count < shiftAmount)
						shiftAmount = count;

					//Console.WriteLine("shiftAmount = {0}", shiftAmount);

					ulong carry = 0;
					for (int i = 0; i < bufLen; i++)
					{
						ulong val = ((ulong)buffer[i]) << shiftAmount;
						val |= carry;

						buffer[i] = (uint)(val & 0xFFFFFFFF);
						carry = val >> 32;
					}

					if (carry != 0)
					{
						if (bufLen + 1 <= buffer.Length)
						{
							buffer[bufLen] = (uint)carry;
							bufLen++;
						}
					}
					count -= shiftAmount;
				}
				return bufLen;
			}


			//***********************************************************************
			// Overloading of unary >> operators
			//***********************************************************************

			public static BigInteger operator >>(BigInteger bi1, int shiftVal)
			{
				BigInteger result = new BigInteger(bi1);
				result.dataLength = shiftRight(result.data, shiftVal);


				if ((bi1.data[maxLength - 1] & 0x80000000) != 0) // negative
				{
					for (int i = maxLength - 1; i >= result.dataLength; i--)
						result.data[i] = 0xFFFFFFFF;

					uint mask = 0x80000000;
					for (int i = 0; i < 32; i++)
					{
						if ((result.data[result.dataLength - 1] & mask) != 0)
							break;

						result.data[result.dataLength - 1] |= mask;
						mask >>= 1;
					}
					result.dataLength = maxLength;
				}

				return result;
			}


			private static int shiftRight(uint[] buffer, int shiftVal)
			{
				int shiftAmount = 32;
				int invShift = 0;
				int bufLen = buffer.Length;

				while (bufLen > 1 && buffer[bufLen - 1] == 0)
					bufLen--;

				//Console.WriteLine("bufLen = " + bufLen + " buffer.Length = " + buffer.Length);

				for (int count = shiftVal; count > 0; )
				{
					if (count < shiftAmount)
					{
						shiftAmount = count;
						invShift = 32 - shiftAmount;
					}

					//Console.WriteLine("shiftAmount = {0}", shiftAmount);

					ulong carry = 0;
					for (int i = bufLen - 1; i >= 0; i--)
					{
						ulong val = ((ulong)buffer[i]) >> shiftAmount;
						val |= carry;

						carry = ((ulong)buffer[i]) << invShift;
						buffer[i] = (uint)(val);
					}

					count -= shiftAmount;
				}

				while (bufLen > 1 && buffer[bufLen - 1] == 0)
					bufLen--;

				return bufLen;
			}


			//***********************************************************************
			// Overloading of the NOT operator (1's complement)
			//***********************************************************************

			public static BigInteger operator ~(BigInteger bi1)
			{
				BigInteger result = new BigInteger(bi1);

				for (int i = 0; i < maxLength; i++)
					result.data[i] = (uint)(~(bi1.data[i]));

				result.dataLength = maxLength;

				while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
					result.dataLength--;

				return result;
			}


			//***********************************************************************
			// Overloading of the NEGATE operator (2's complement)
			//***********************************************************************

			public static BigInteger operator -(BigInteger bi1)
			{
				// handle neg of zero separately since it'll cause an overflow
				// if we proceed.

				if (bi1.dataLength == 1 && bi1.data[0] == 0)
					return (new BigInteger());

				BigInteger result = new BigInteger(bi1);

				// 1's complement
				for (int i = 0; i < maxLength; i++)
					result.data[i] = (uint)(~(bi1.data[i]));

				// add one to result of 1's complement
				long val, carry = 1;
				int index = 0;

				while (carry != 0 && index < maxLength)
				{
					val = (long)(result.data[index]);
					val++;

					result.data[index] = (uint)(val & 0xFFFFFFFF);
					carry = val >> 32;

					index++;
				}

				if ((bi1.data[maxLength - 1] & 0x80000000) == (result.data[maxLength - 1] & 0x80000000))
					throw (new ArithmeticException("Overflow in negation.\n"));

				result.dataLength = maxLength;

				while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
					result.dataLength--;
				return result;
			}


			//***********************************************************************
			// Overloading of equality operator
			//***********************************************************************

			public static bool operator ==(BigInteger bi1, BigInteger bi2)
			{
				return bi1.Equals(bi2);
			}


			public static bool operator !=(BigInteger bi1, BigInteger bi2)
			{
				return !(bi1.Equals(bi2));
			}


			public override bool Equals(object o)
			{
				BigInteger bi = (BigInteger)o;

				if (this.dataLength != bi.dataLength)
					return false;

				for (int i = 0; i < this.dataLength; i++)
				{
					if (this.data[i] != bi.data[i])
						return false;
				}
				return true;
			}


			public override int GetHashCode()
			{
				return this.ToString().GetHashCode();
			}


			//***********************************************************************
			// Overloading of inequality operator
			//***********************************************************************

			public static bool operator >(BigInteger bi1, BigInteger bi2)
			{
				int pos = maxLength - 1;

				// bi1 is negative, bi2 is positive
				if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
					return false;

				// bi1 is positive, bi2 is negative
				else if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
					return true;

				// same sign
				int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
				for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--) ;

				if (pos >= 0)
				{
					if (bi1.data[pos] > bi2.data[pos])
						return true;
					return false;
				}
				return false;
			}


			public static bool operator <(BigInteger bi1, BigInteger bi2)
			{
				int pos = maxLength - 1;

				// bi1 is negative, bi2 is positive
				if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
					return true;

				// bi1 is positive, bi2 is negative
				else if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
					return false;

				// same sign
				int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
				for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--) ;

				if (pos >= 0)
				{
					if (bi1.data[pos] < bi2.data[pos])
						return true;
					return false;
				}
				return false;
			}


			public static bool operator >=(BigInteger bi1, BigInteger bi2)
			{
				return (bi1 == bi2 || bi1 > bi2);
			}


			public static bool operator <=(BigInteger bi1, BigInteger bi2)
			{
				return (bi1 == bi2 || bi1 < bi2);
			}


			//***********************************************************************
			// Private function that supports the division of two numbers with
			// a divisor that has more than 1 digit.
			//
			// Algorithm taken from [1]
			//***********************************************************************

			private static void multiByteDivide(BigInteger bi1, BigInteger bi2,
												BigInteger outQuotient, BigInteger outRemainder)
			{
				uint[] result = new uint[maxLength];

				int remainderLen = bi1.dataLength + 1;
				uint[] remainder = new uint[remainderLen];

				uint mask = 0x80000000;
				uint val = bi2.data[bi2.dataLength - 1];
				int shift = 0, resultPos = 0;

				while (mask != 0 && (val & mask) == 0)
				{
					shift++; mask >>= 1;
				}

				//Console.WriteLine("shift = {0}", shift);
				//Console.WriteLine("Before bi1 Len = {0}, bi2 Len = {1}", bi1.dataLength, bi2.dataLength);

				for (int i = 0; i < bi1.dataLength; i++)
					remainder[i] = bi1.data[i];
				shiftLeft(remainder, shift);
				bi2 = bi2 << shift;

				/*
				Console.WriteLine("bi1 Len = {0}, bi2 Len = {1}", bi1.dataLength, bi2.dataLength);
				Console.WriteLine("dividend = " + bi1 + "\ndivisor = " + bi2);
				for(int q = remainderLen - 1; q >= 0; q--)
						Console.Write("{0:x2}", remainder[q]);
				Console.WriteLine();
				*/

				int j = remainderLen - bi2.dataLength;
				int pos = remainderLen - 1;

				ulong firstDivisorByte = bi2.data[bi2.dataLength - 1];
				ulong secondDivisorByte = bi2.data[bi2.dataLength - 2];

				int divisorLen = bi2.dataLength + 1;
				uint[] dividendPart = new uint[divisorLen];

				while (j > 0)
				{
					ulong dividend = ((ulong)remainder[pos] << 32) + (ulong)remainder[pos - 1];
					//Console.WriteLine("dividend = {0}", dividend);

					ulong q_hat = dividend / firstDivisorByte;
					ulong r_hat = dividend % firstDivisorByte;

					//Console.WriteLine("q_hat = {0:X}, r_hat = {1:X}", q_hat, r_hat);

					bool done = false;
					while (!done)
					{
						done = true;

						if (q_hat == 0x100000000 ||
						   (q_hat * secondDivisorByte) > ((r_hat << 32) + remainder[pos - 2]))
						{
							q_hat--;
							r_hat += firstDivisorByte;

							if (r_hat < 0x100000000)
								done = false;
						}
					}

					for (int h = 0; h < divisorLen; h++)
						dividendPart[h] = remainder[pos - h];

					BigInteger kk = new BigInteger(dividendPart);
					BigInteger ss = bi2 * (long)q_hat;

					//Console.WriteLine("ss before = " + ss);
					while (ss > kk)
					{
						q_hat--;
						ss -= bi2;
						//Console.WriteLine(ss);
					}
					BigInteger yy = kk - ss;

					//Console.WriteLine("ss = " + ss);
					//Console.WriteLine("kk = " + kk);
					//Console.WriteLine("yy = " + yy);

					for (int h = 0; h < divisorLen; h++)
						remainder[pos - h] = yy.data[bi2.dataLength - h];

					/*
					Console.WriteLine("dividend = ");
					for(int q = remainderLen - 1; q >= 0; q--)
							Console.Write("{0:x2}", remainder[q]);
					Console.WriteLine("\n************ q_hat = {0:X}\n", q_hat);
					*/

					result[resultPos++] = (uint)q_hat;

					pos--;
					j--;
				}

				outQuotient.dataLength = resultPos;
				int y = 0;
				for (int x = outQuotient.dataLength - 1; x >= 0; x--, y++)
					outQuotient.data[y] = result[x];
				for (; y < maxLength; y++)
					outQuotient.data[y] = 0;

				while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
					outQuotient.dataLength--;

				if (outQuotient.dataLength == 0)
					outQuotient.dataLength = 1;

				outRemainder.dataLength = shiftRight(remainder, shift);

				for (y = 0; y < outRemainder.dataLength; y++)
					outRemainder.data[y] = remainder[y];
				for (; y < maxLength; y++)
					outRemainder.data[y] = 0;
			}


			//***********************************************************************
			// Private function that supports the division of two numbers with
			// a divisor that has only 1 digit.
			//***********************************************************************

			private static void singleByteDivide(BigInteger bi1, BigInteger bi2,
												 BigInteger outQuotient, BigInteger outRemainder)
			{
				uint[] result = new uint[maxLength];
				int resultPos = 0;

				// copy dividend to reminder
				for (int i = 0; i < maxLength; i++)
					outRemainder.data[i] = bi1.data[i];
				outRemainder.dataLength = bi1.dataLength;

				while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
					outRemainder.dataLength--;

				ulong divisor = (ulong)bi2.data[0];
				int pos = outRemainder.dataLength - 1;
				ulong dividend = (ulong)outRemainder.data[pos];

				//Console.WriteLine("divisor = " + divisor + " dividend = " + dividend);
				//Console.WriteLine("divisor = " + bi2 + "\ndividend = " + bi1);

				if (dividend >= divisor)
				{
					ulong quotient = dividend / divisor;
					result[resultPos++] = (uint)quotient;

					outRemainder.data[pos] = (uint)(dividend % divisor);
				}
				pos--;

				while (pos >= 0)
				{
					//Console.WriteLine(pos);

					dividend = ((ulong)outRemainder.data[pos + 1] << 32) + (ulong)outRemainder.data[pos];
					ulong quotient = dividend / divisor;
					result[resultPos++] = (uint)quotient;

					outRemainder.data[pos + 1] = 0;
					outRemainder.data[pos--] = (uint)(dividend % divisor);
					//Console.WriteLine(">>>> " + bi1);
				}

				outQuotient.dataLength = resultPos;
				int j = 0;
				for (int i = outQuotient.dataLength - 1; i >= 0; i--, j++)
					outQuotient.data[j] = result[i];
				for (; j < maxLength; j++)
					outQuotient.data[j] = 0;

				while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
					outQuotient.dataLength--;

				if (outQuotient.dataLength == 0)
					outQuotient.dataLength = 1;

				while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
					outRemainder.dataLength--;
			}


			//***********************************************************************
			// Overloading of division operator
			//***********************************************************************

			public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
			{
				BigInteger quotient = new BigInteger();
				BigInteger remainder = new BigInteger();

				int lastPos = maxLength - 1;
				bool divisorNeg = false, dividendNeg = false;

				if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
				{
					bi1 = -bi1;
					dividendNeg = true;
				}
				if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
				{
					bi2 = -bi2;
					divisorNeg = true;
				}

				if (bi1 < bi2)
				{
					return quotient;
				}

				else
				{
					if (bi2.dataLength == 1)
						singleByteDivide(bi1, bi2, quotient, remainder);
					else
						multiByteDivide(bi1, bi2, quotient, remainder);

					if (dividendNeg != divisorNeg)
						return -quotient;

					return quotient;
				}
			}


			//***********************************************************************
			// Overloading of modulus operator
			//***********************************************************************

			public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
			{
				BigInteger quotient = new BigInteger();
				BigInteger remainder = new BigInteger(bi1);

				int lastPos = maxLength - 1;
				bool dividendNeg = false;

				if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
				{
					bi1 = -bi1;
					dividendNeg = true;
				}
				if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
					bi2 = -bi2;

				if (bi1 < bi2)
				{
					return remainder;
				}

				else
				{
					if (bi2.dataLength == 1)
						singleByteDivide(bi1, bi2, quotient, remainder);
					else
						multiByteDivide(bi1, bi2, quotient, remainder);

					if (dividendNeg)
						return -remainder;

					return remainder;
				}
			}


			//***********************************************************************
			// Overloading of bitwise AND operator
			//***********************************************************************

			public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
			{
				BigInteger result = new BigInteger();

				int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

				for (int i = 0; i < len; i++)
				{
					uint sum = (uint)(bi1.data[i] & bi2.data[i]);
					result.data[i] = sum;
				}

				result.dataLength = maxLength;

				while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
					result.dataLength--;

				return result;
			}


			//***********************************************************************
			// Overloading of bitwise OR operator
			//***********************************************************************

			public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
			{
				BigInteger result = new BigInteger();

				int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

				for (int i = 0; i < len; i++)
				{
					uint sum = (uint)(bi1.data[i] | bi2.data[i]);
					result.data[i] = sum;
				}

				result.dataLength = maxLength;

				while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
					result.dataLength--;

				return result;
			}


			//***********************************************************************
			// Overloading of bitwise XOR operator
			//***********************************************************************

			public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
			{
				BigInteger result = new BigInteger();

				int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

				for (int i = 0; i < len; i++)
				{
					uint sum = (uint)(bi1.data[i] ^ bi2.data[i]);
					result.data[i] = sum;
				}

				result.dataLength = maxLength;

				while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
					result.dataLength--;

				return result;
			}


			//***********************************************************************
			// Returns max(this, bi)
			//***********************************************************************

			public BigInteger max(BigInteger bi)
			{
				if (this > bi)
					return (new BigInteger(this));
				else
					return (new BigInteger(bi));
			}


			//***********************************************************************
			// Returns min(this, bi)
			//***********************************************************************

			public BigInteger min(BigInteger bi)
			{
				if (this < bi)
					return (new BigInteger(this));
				else
					return (new BigInteger(bi));

			}


			//***********************************************************************
			// Returns the absolute value
			//***********************************************************************

			public BigInteger abs()
			{
				if ((this.data[maxLength - 1] & 0x80000000) != 0)
					return (-this);
				else
					return (new BigInteger(this));
			}


			//***********************************************************************
			// Returns a string representing the BigInteger in base 10.
			//***********************************************************************

			public override string ToString()
			{
				return ToString(10);
			}


			//***********************************************************************
			// Returns a string representing the BigInteger in sign-and-magnitude
			// format in the specified radix.
			//
			// Example
			// -------
			// If the value of BigInteger is -255 in base 10, then
			// ToString(16) returns "-FF"
			//
			//***********************************************************************

			public string ToString(int radix)
			{
				if (radix < 2 || radix > 36)
					throw (new ArgumentException("Radix must be >= 2 and <= 36"));

				string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
				string result = "";

				BigInteger a = this;

				bool negative = false;
				if ((a.data[maxLength - 1] & 0x80000000) != 0)
				{
					negative = true;
					try
					{
						a = -a;
					}
					catch (Exception) { }
				}

				BigInteger quotient = new BigInteger();
				BigInteger remainder = new BigInteger();
				BigInteger biRadix = new BigInteger(radix);

				if (a.dataLength == 1 && a.data[0] == 0)
					result = "0";
				else
				{
					while (a.dataLength > 1 || (a.dataLength == 1 && a.data[0] != 0))
					{
						singleByteDivide(a, biRadix, quotient, remainder);

						if (remainder.data[0] < 10)
							result = remainder.data[0] + result;
						else
							result = charSet[(int)remainder.data[0] - 10] + result;

						a = quotient;
					}
					if (negative)
						result = "-" + result;
				}

				return result;
			}


			//***********************************************************************
			// Returns a hex string showing the contains of the BigInteger
			//
			// Examples
			// -------
			// 1) If the value of BigInteger is 255 in base 10, then
			//    ToHexString() returns "FF"
			//
			// 2) If the value of BigInteger is -255 in base 10, then
			//    ToHexString() returns ".....FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF01",
			//    which is the 2's complement representation of -255.
			//
			//***********************************************************************

			public string ToHexString()
			{
				string result = data[dataLength - 1].ToString("X");

				for (int i = dataLength - 2; i >= 0; i--)
				{
					result += data[i].ToString("X8");
				}

				return result;
			}



			//***********************************************************************
			// Modulo Exponentiation
			//***********************************************************************

			public BigInteger modPow(BigInteger exp, BigInteger n)
			{
				if ((exp.data[maxLength - 1] & 0x80000000) != 0)
					throw (new ArithmeticException("Positive exponents only."));

				BigInteger resultNum = 1;
				BigInteger tempNum;
				bool thisNegative = false;

				if ((this.data[maxLength - 1] & 0x80000000) != 0)   // negative this
				{
					tempNum = -this % n;
					thisNegative = true;
				}
				else
					tempNum = this % n;  // ensures (tempNum * tempNum) < b^(2k)

				if ((n.data[maxLength - 1] & 0x80000000) != 0)   // negative n
					n = -n;

				// calculate constant = b^(2k) / m
				BigInteger constant = new BigInteger();

				int i = n.dataLength << 1;
				constant.data[i] = 0x00000001;
				constant.dataLength = i + 1;

				constant = constant / n;
				int totalBits = exp.bitCount();
				int count = 0;

				// perform squaring and multiply exponentiation
				for (int pos = 0; pos < exp.dataLength; pos++)
				{
					uint mask = 0x01;
					//Console.WriteLine("pos = " + pos);

					for (int index = 0; index < 32; index++)
					{
						if ((exp.data[pos] & mask) != 0)
							resultNum = BarrettReduction(resultNum * tempNum, n, constant);

						mask <<= 1;

						tempNum = BarrettReduction(tempNum * tempNum, n, constant);


						if (tempNum.dataLength == 1 && tempNum.data[0] == 1)
						{
							if (thisNegative && (exp.data[0] & 0x1) != 0)    //odd exp
								return -resultNum;
							return resultNum;
						}
						count++;
						if (count == totalBits)
							break;
					}
				}

				if (thisNegative && (exp.data[0] & 0x1) != 0)    //odd exp
					return -resultNum;

				return resultNum;
			}



			//***********************************************************************
			// Fast calculation of modular reduction using Barrett's reduction.
			// Requires x < b^(2k), where b is the base.  In this case, base is
			// 2^32 (uint).
			//
			// Reference [4]
			//***********************************************************************

			private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
			{
				int k = n.dataLength,
					kPlusOne = k + 1,
					kMinusOne = k - 1;

				BigInteger q1 = new BigInteger();

				// q1 = x / b^(k-1)
				for (int i = kMinusOne, j = 0; i < x.dataLength; i++, j++)
					q1.data[j] = x.data[i];
				q1.dataLength = x.dataLength - kMinusOne;
				if (q1.dataLength <= 0)
					q1.dataLength = 1;


				BigInteger q2 = q1 * constant;
				BigInteger q3 = new BigInteger();

				// q3 = q2 / b^(k+1)
				for (int i = kPlusOne, j = 0; i < q2.dataLength; i++, j++)
					q3.data[j] = q2.data[i];
				q3.dataLength = q2.dataLength - kPlusOne;
				if (q3.dataLength <= 0)
					q3.dataLength = 1;


				// r1 = x mod b^(k+1)
				// i.e. keep the lowest (k+1) words
				BigInteger r1 = new BigInteger();
				int lengthToCopy = (x.dataLength > kPlusOne) ? kPlusOne : x.dataLength;
				for (int i = 0; i < lengthToCopy; i++)
					r1.data[i] = x.data[i];
				r1.dataLength = lengthToCopy;


				// r2 = (q3 * n) mod b^(k+1)
				// partial multiplication of q3 and n

				BigInteger r2 = new BigInteger();
				for (int i = 0; i < q3.dataLength; i++)
				{
					if (q3.data[i] == 0) continue;

					ulong mcarry = 0;
					int t = i;
					for (int j = 0; j < n.dataLength && t < kPlusOne; j++, t++)
					{
						// t = i + j
						ulong val = ((ulong)q3.data[i] * (ulong)n.data[j]) +
									 (ulong)r2.data[t] + mcarry;

						r2.data[t] = (uint)(val & 0xFFFFFFFF);
						mcarry = (val >> 32);
					}

					if (t < kPlusOne)
						r2.data[t] = (uint)mcarry;
				}
				r2.dataLength = kPlusOne;
				while (r2.dataLength > 1 && r2.data[r2.dataLength - 1] == 0)
					r2.dataLength--;

				r1 -= r2;
				if ((r1.data[maxLength - 1] & 0x80000000) != 0)        // negative
				{
					BigInteger val = new BigInteger();
					val.data[kPlusOne] = 0x00000001;
					val.dataLength = kPlusOne + 1;
					r1 += val;
				}

				while (r1 >= n)
					r1 -= n;

				return r1;
			}


			//***********************************************************************
			// Returns gcd(this, bi)
			//***********************************************************************

			public BigInteger gcd(BigInteger bi)
			{
				BigInteger x;
				BigInteger y;

				if ((data[maxLength - 1] & 0x80000000) != 0)     // negative
					x = -this;
				else
					x = this;

				if ((bi.data[maxLength - 1] & 0x80000000) != 0)     // negative
					y = -bi;
				else
					y = bi;

				BigInteger g = y;

				while (x.dataLength > 1 || (x.dataLength == 1 && x.data[0] != 0))
				{
					g = x;
					x = y % x;
					y = g;
				}

				return g;
			}


			//***********************************************************************
			// Populates "this" with the specified amount of random bits
			//***********************************************************************

			public void genRandomBits(int bits, Random rand)
			{
				int dwords = bits >> 5;
				int remBits = bits & 0x1F;

				if (remBits != 0)
					dwords++;

				if (dwords > maxLength)
					throw (new ArithmeticException("Number of required bits > maxLength."));

				for (int i = 0; i < dwords; i++)
					data[i] = (uint)(rand.NextDouble() * 0x100000000);

				for (int i = dwords; i < maxLength; i++)
					data[i] = 0;

				if (remBits != 0)
				{
					uint mask = (uint)(0x01 << (remBits - 1));
					data[dwords - 1] |= mask;

					mask = (uint)(0xFFFFFFFF >> (32 - remBits));
					data[dwords - 1] &= mask;
				}
				else
					data[dwords - 1] |= 0x80000000;

				dataLength = dwords;

				if (dataLength == 0)
					dataLength = 1;
			}


			//***********************************************************************
			// Returns the position of the most significant bit in the BigInteger.
			//
			// Eg.  The result is 0, if the value of BigInteger is 0...0000 0000
			//      The result is 1, if the value of BigInteger is 0...0000 0001
			//      The result is 2, if the value of BigInteger is 0...0000 0010
			//      The result is 2, if the value of BigInteger is 0...0000 0011
			//
			//***********************************************************************

			public int bitCount()
			{
				while (dataLength > 1 && data[dataLength - 1] == 0)
					dataLength--;

				uint value = data[dataLength - 1];
				uint mask = 0x80000000;
				int bits = 32;

				while (bits > 0 && (value & mask) == 0)
				{
					bits--;
					mask >>= 1;
				}
				bits += ((dataLength - 1) << 5);

				return bits;
			}


			//***********************************************************************
			// Probabilistic prime test based on Fermat's little theorem
			//
			// for any a < p (p does not divide a) if
			//      a^(p-1) mod p != 1 then p is not prime.
			//
			// Otherwise, p is probably prime (pseudoprime to the chosen base).
			//
			// Returns
			// -------
			// True if "this" is a pseudoprime to randomly chosen
			// bases.  The number of chosen bases is given by the "confidence"
			// parameter.
			//
			// False if "this" is definitely NOT prime.
			//
			// Note - this method is fast but fails for Carmichael numbers except
			// when the randomly chosen base is a factor of the number.
			//
			//***********************************************************************

			public bool FermatLittleTest(int confidence)
			{
				BigInteger thisVal;
				if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
					thisVal = -this;
				else
					thisVal = this;

				if (thisVal.dataLength == 1)
				{
					// test small numbers
					if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
						return false;
					else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
						return true;
				}

				if ((thisVal.data[0] & 0x1) == 0)     // even numbers
					return false;

				int bits = thisVal.bitCount();
				BigInteger a = new BigInteger();
				BigInteger p_sub1 = thisVal - (new BigInteger(1));
				Random rand = new Random();

				for (int round = 0; round < confidence; round++)
				{
					bool done = false;

					while (!done)        // generate a < n
					{
						int testBits = 0;

						// make sure "a" has at least 2 bits
						while (testBits < 2)
							testBits = (int)(rand.NextDouble() * bits);

						a.genRandomBits(testBits, rand);

						int byteLen = a.dataLength;

						// make sure "a" is not 0
						if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
							done = true;
					}

					// check whether a factor exists (fix for version 1.03)
					BigInteger gcdTest = a.gcd(thisVal);
					if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
						return false;

					// calculate a^(p-1) mod p
					BigInteger expResult = a.modPow(p_sub1, thisVal);

					int resultLen = expResult.dataLength;

					// is NOT prime is a^(p-1) mod p != 1

					if (resultLen > 1 || (resultLen == 1 && expResult.data[0] != 1))
					{
						//Console.WriteLine("a = " + a.ToString());
						return false;
					}
				}

				return true;
			}


			//***********************************************************************
			// Probabilistic prime test based on Rabin-Miller's
			//
			// for any p > 0 with p - 1 = 2^s * t
			//
			// p is probably prime (strong pseudoprime) if for any a < p,
			// 1) a^t mod p = 1 or
			// 2) a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
			//
			// Otherwise, p is composite.
			//
			// Returns
			// -------
			// True if "this" is a strong pseudoprime to randomly chosen
			// bases.  The number of chosen bases is given by the "confidence"
			// parameter.
			//
			// False if "this" is definitely NOT prime.
			//
			//***********************************************************************

			public bool RabinMillerTest(int confidence)
			{
				BigInteger thisVal;
				if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
					thisVal = -this;
				else
					thisVal = this;

				if (thisVal.dataLength == 1)
				{
					// test small numbers
					if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
						return false;
					else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
						return true;
				}

				if ((thisVal.data[0] & 0x1) == 0)     // even numbers
					return false;


				// calculate values of s and t
				BigInteger p_sub1 = thisVal - (new BigInteger(1));
				int s = 0;

				for (int index = 0; index < p_sub1.dataLength; index++)
				{
					uint mask = 0x01;

					for (int i = 0; i < 32; i++)
					{
						if ((p_sub1.data[index] & mask) != 0)
						{
							index = p_sub1.dataLength;      // to break the outer loop
							break;
						}
						mask <<= 1;
						s++;
					}
				}

				BigInteger t = p_sub1 >> s;

				int bits = thisVal.bitCount();
				BigInteger a = new BigInteger();
				Random rand = new Random();

				for (int round = 0; round < confidence; round++)
				{
					bool done = false;

					while (!done)        // generate a < n
					{
						int testBits = 0;

						// make sure "a" has at least 2 bits
						while (testBits < 2)
							testBits = (int)(rand.NextDouble() * bits);

						a.genRandomBits(testBits, rand);

						int byteLen = a.dataLength;

						// make sure "a" is not 0
						if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
							done = true;
					}

					// check whether a factor exists (fix for version 1.03)
					BigInteger gcdTest = a.gcd(thisVal);
					if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
						return false;

					BigInteger b = a.modPow(t, thisVal);

					/*
					Console.WriteLine("a = " + a.ToString(10));
					Console.WriteLine("b = " + b.ToString(10));
					Console.WriteLine("t = " + t.ToString(10));
					Console.WriteLine("s = " + s);
					*/

					bool result = false;

					if (b.dataLength == 1 && b.data[0] == 1)         // a^t mod p = 1
						result = true;

					for (int j = 0; result == false && j < s; j++)
					{
						if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
						{
							result = true;
							break;
						}

						b = (b * b) % thisVal;
					}

					if (result == false)
						return false;
				}
				return true;
			}


			//***********************************************************************
			// Probabilistic prime test based on Solovay-Strassen (Euler Criterion)
			//
			// p is probably prime if for any a < p (a is not multiple of p),
			// a^((p-1)/2) mod p = J(a, p)
			//
			// where J is the Jacobi symbol.
			//
			// Otherwise, p is composite.
			//
			// Returns
			// -------
			// True if "this" is a Euler pseudoprime to randomly chosen
			// bases.  The number of chosen bases is given by the "confidence"
			// parameter.
			//
			// False if "this" is definitely NOT prime.
			//
			//***********************************************************************

			public bool SolovayStrassenTest(int confidence)
			{
				BigInteger thisVal;
				if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
					thisVal = -this;
				else
					thisVal = this;

				if (thisVal.dataLength == 1)
				{
					// test small numbers
					if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
						return false;
					else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
						return true;
				}

				if ((thisVal.data[0] & 0x1) == 0)     // even numbers
					return false;


				int bits = thisVal.bitCount();
				BigInteger a = new BigInteger();
				BigInteger p_sub1 = thisVal - 1;
				BigInteger p_sub1_shift = p_sub1 >> 1;

				Random rand = new Random();

				for (int round = 0; round < confidence; round++)
				{
					bool done = false;

					while (!done)        // generate a < n
					{
						int testBits = 0;

						// make sure "a" has at least 2 bits
						while (testBits < 2)
							testBits = (int)(rand.NextDouble() * bits);

						a.genRandomBits(testBits, rand);

						int byteLen = a.dataLength;

						// make sure "a" is not 0
						if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
							done = true;
					}

					// check whether a factor exists (fix for version 1.03)
					BigInteger gcdTest = a.gcd(thisVal);
					if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
						return false;

					// calculate a^((p-1)/2) mod p

					BigInteger expResult = a.modPow(p_sub1_shift, thisVal);
					if (expResult == p_sub1)
						expResult = -1;

					// calculate Jacobi symbol
					BigInteger jacob = Jacobi(a, thisVal);

					//Console.WriteLine("a = " + a.ToString(10) + " b = " + thisVal.ToString(10));
					//Console.WriteLine("expResult = " + expResult.ToString(10) + " Jacob = " + jacob.ToString(10));

					// if they are different then it is not prime
					if (expResult != jacob)
						return false;
				}

				return true;
			}


			//***********************************************************************
			// Implementation of the Lucas Strong Pseudo Prime test.
			//
			// Let n be an odd number with gcd(n,D) = 1, and n - J(D, n) = 2^s * d
			// with d odd and s >= 0.
			//
			// If Ud mod n = 0 or V2^r*d mod n = 0 for some 0 <= r < s, then n
			// is a strong Lucas pseudoprime with parameters (P, Q).  We select
			// P and Q based on Selfridge.
			//
			// Returns True if number is a strong Lucus pseudo prime.
			// Otherwise, returns False indicating that number is composite.
			//***********************************************************************

			public bool LucasStrongTest()
			{
				BigInteger thisVal;
				if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
					thisVal = -this;
				else
					thisVal = this;

				if (thisVal.dataLength == 1)
				{
					// test small numbers
					if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
						return false;
					else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
						return true;
				}

				if ((thisVal.data[0] & 0x1) == 0)     // even numbers
					return false;

				return LucasStrongTestHelper(thisVal);
			}


			private bool LucasStrongTestHelper(BigInteger thisVal)
			{
				// Do the test (selects D based on Selfridge)
				// Let D be the first element of the sequence
				// 5, -7, 9, -11, 13, ... for which J(D,n) = -1
				// Let P = 1, Q = (1-D) / 4

				long D = 5, sign = -1, dCount = 0;
				bool done = false;

				while (!done)
				{
					int Jresult = BigInteger.Jacobi(D, thisVal);

					if (Jresult == -1)
						done = true;    // J(D, this) = 1
					else
					{
						if (Jresult == 0 && Math.Abs(D) < thisVal)       // divisor found
							return false;

						if (dCount == 20)
						{
							// check for square
							BigInteger root = thisVal.sqrt();
							if (root * root == thisVal)
								return false;
						}

						//Console.WriteLine(D);
						D = (Math.Abs(D) + 2) * sign;
						sign = -sign;
					}
					dCount++;
				}

				long Q = (1 - D) >> 2;

				/*
				Console.WriteLine("D = " + D);
				Console.WriteLine("Q = " + Q);
				Console.WriteLine("(n,D) = " + thisVal.gcd(D));
				Console.WriteLine("(n,Q) = " + thisVal.gcd(Q));
				Console.WriteLine("J(D|n) = " + BigInteger.Jacobi(D, thisVal));
				*/

				BigInteger p_add1 = thisVal + 1;
				int s = 0;

				for (int index = 0; index < p_add1.dataLength; index++)
				{
					uint mask = 0x01;

					for (int i = 0; i < 32; i++)
					{
						if ((p_add1.data[index] & mask) != 0)
						{
							index = p_add1.dataLength;      // to break the outer loop
							break;
						}
						mask <<= 1;
						s++;
					}
				}

				BigInteger t = p_add1 >> s;

				// calculate constant = b^(2k) / m
				// for Barrett Reduction
				BigInteger constant = new BigInteger();

				int nLen = thisVal.dataLength << 1;
				constant.data[nLen] = 0x00000001;
				constant.dataLength = nLen + 1;

				constant = constant / thisVal;

				BigInteger[] lucas = LucasSequenceHelper(1, Q, t, thisVal, constant, 0);
				bool isPrime = false;

				if ((lucas[0].dataLength == 1 && lucas[0].data[0] == 0) ||
				   (lucas[1].dataLength == 1 && lucas[1].data[0] == 0))
				{
					// u(t) = 0 or V(t) = 0
					isPrime = true;
				}

				for (int i = 1; i < s; i++)
				{
					if (!isPrime)
					{
						// doubling of index
						lucas[1] = thisVal.BarrettReduction(lucas[1] * lucas[1], thisVal, constant);
						lucas[1] = (lucas[1] - (lucas[2] << 1)) % thisVal;

						//lucas[1] = ((lucas[1] * lucas[1]) - (lucas[2] << 1)) % thisVal;

						if ((lucas[1].dataLength == 1 && lucas[1].data[0] == 0))
							isPrime = true;
					}

					lucas[2] = thisVal.BarrettReduction(lucas[2] * lucas[2], thisVal, constant);     //Q^k
				}


				if (isPrime)     // additional checks for composite numbers
				{
					// If n is prime and gcd(n, Q) == 1, then
					// Q^((n+1)/2) = Q * Q^((n-1)/2) is congruent to (Q * J(Q, n)) mod n

					BigInteger g = thisVal.gcd(Q);
					if (g.dataLength == 1 && g.data[0] == 1)         // gcd(this, Q) == 1
					{
						if ((lucas[2].data[maxLength - 1] & 0x80000000) != 0)
							lucas[2] += thisVal;

						BigInteger temp = (Q * BigInteger.Jacobi(Q, thisVal)) % thisVal;
						if ((temp.data[maxLength - 1] & 0x80000000) != 0)
							temp += thisVal;

						if (lucas[2] != temp)
							isPrime = false;
					}
				}

				return isPrime;
			}


			//***********************************************************************
			// Determines whether a number is probably prime, using the Rabin-Miller's
			// test.  Before applying the test, the number is tested for divisibility
			// by primes < 2000
			//
			// Returns true if number is probably prime.
			//***********************************************************************

			public bool isProbablePrime(int confidence)
			{
				BigInteger thisVal;
				if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
					thisVal = -this;
				else
					thisVal = this;


				// test for divisibility by primes < 2000
				for (int p = 0; p < primesBelow2000.Length; p++)
				{
					BigInteger divisor = primesBelow2000[p];

					if (divisor >= thisVal)
						break;

					BigInteger resultNum = thisVal % divisor;
					if (resultNum.IntValue() == 0)
					{
						/*
				Console.WriteLine("Not prime!  Divisible by {0}\n",
										  primesBelow2000[p]);
						*/
						return false;
					}
				}

				if (thisVal.RabinMillerTest(confidence))
					return true;
				else
				{
					//Console.WriteLine("Not prime!  Failed primality test\n");
					return false;
				}
			}


			//***********************************************************************
			// Determines whether this BigInteger is probably prime using a
			// combination of base 2 strong pseudoprime test and Lucas strong
			// pseudoprime test.
			//
			// The sequence of the primality test is as follows,
			//
			// 1) Trial divisions are carried out using prime numbers below 2000.
			//    if any of the primes divides this BigInteger, then it is not prime.
			//
			// 2) Perform base 2 strong pseudoprime test.  If this BigInteger is a
			//    base 2 strong pseudoprime, proceed on to the next step.
			//
			// 3) Perform strong Lucas pseudoprime test.
			//
			// Returns True if this BigInteger is both a base 2 strong pseudoprime
			// and a strong Lucas pseudoprime.
			//
			// For a detailed discussion of this primality test, see [6].
			//
			//***********************************************************************

			public bool isProbablePrime()
			{
				BigInteger thisVal;
				if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
					thisVal = -this;
				else
					thisVal = this;

				if (thisVal.dataLength == 1)
				{
					// test small numbers
					if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
						return false;
					else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
						return true;
				}

				if ((thisVal.data[0] & 0x1) == 0)     // even numbers
					return false;


				// test for divisibility by primes < 2000
				for (int p = 0; p < primesBelow2000.Length; p++)
				{
					BigInteger divisor = primesBelow2000[p];

					if (divisor >= thisVal)
						break;

					BigInteger resultNum = thisVal % divisor;
					if (resultNum.IntValue() == 0)
					{
						//Console.WriteLine("Not prime!  Divisible by {0}\n",
						//                  primesBelow2000[p]);

						return false;
					}
				}

				// Perform BASE 2 Rabin-Miller Test

				// calculate values of s and t
				BigInteger p_sub1 = thisVal - (new BigInteger(1));
				int s = 0;

				for (int index = 0; index < p_sub1.dataLength; index++)
				{
					uint mask = 0x01;

					for (int i = 0; i < 32; i++)
					{
						if ((p_sub1.data[index] & mask) != 0)
						{
							index = p_sub1.dataLength;      // to break the outer loop
							break;
						}
						mask <<= 1;
						s++;
					}
				}

				BigInteger t = p_sub1 >> s;

				int bits = thisVal.bitCount();
				BigInteger a = 2;

				// b = a^t mod p
				BigInteger b = a.modPow(t, thisVal);
				bool result = false;

				if (b.dataLength == 1 && b.data[0] == 1)         // a^t mod p = 1
					result = true;

				for (int j = 0; result == false && j < s; j++)
				{
					if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
					{
						result = true;
						break;
					}

					b = (b * b) % thisVal;
				}

				// if number is strong pseudoprime to base 2, then do a strong lucas test
				if (result)
					result = LucasStrongTestHelper(thisVal);

				return result;
			}



			//***********************************************************************
			// Returns the lowest 4 bytes of the BigInteger as an int.
			//***********************************************************************

			public int IntValue()
			{
				return (int)data[0];
			}


			//***********************************************************************
			// Returns the lowest 8 bytes of the BigInteger as a long.
			//***********************************************************************

			public long LongValue()
			{
				long val = 0;

				val = (long)data[0];
				try
				{       // exception if maxLength = 1
					val |= (long)data[1] << 32;
				}
				catch (Exception)
				{
					if ((data[0] & 0x80000000) != 0) // negative
						val = (int)data[0];
				}

				return val;
			}


			//***********************************************************************
			// Computes the Jacobi Symbol for a and b.
			// Algorithm adapted from [3] and [4] with some optimizations
			//***********************************************************************

			public static int Jacobi(BigInteger a, BigInteger b)
			{
				// Jacobi defined only for odd integers
				if ((b.data[0] & 0x1) == 0)
					throw (new ArgumentException("Jacobi defined only for odd integers."));

				if (a >= b) a %= b;
				if (a.dataLength == 1 && a.data[0] == 0) return 0;  // a == 0
				if (a.dataLength == 1 && a.data[0] == 1) return 1;  // a == 1

				if (a < 0)
				{
					if ((((b - 1).data[0]) & 0x2) == 0)       //if( (((b-1) >> 1).data[0] & 0x1) == 0)
						return Jacobi(-a, b);
					else
						return -Jacobi(-a, b);
				}

				int e = 0;
				for (int index = 0; index < a.dataLength; index++)
				{
					uint mask = 0x01;

					for (int i = 0; i < 32; i++)
					{
						if ((a.data[index] & mask) != 0)
						{
							index = a.dataLength;      // to break the outer loop
							break;
						}
						mask <<= 1;
						e++;
					}
				}

				BigInteger a1 = a >> e;

				int s = 1;
				if ((e & 0x1) != 0 && ((b.data[0] & 0x7) == 3 || (b.data[0] & 0x7) == 5))
					s = -1;

				if ((b.data[0] & 0x3) == 3 && (a1.data[0] & 0x3) == 3)
					s = -s;

				if (a1.dataLength == 1 && a1.data[0] == 1)
					return s;
				else
					return (s * Jacobi(b % a1, a1));
			}



			//***********************************************************************
			// Generates a positive BigInteger that is probably prime.
			//***********************************************************************

			public static BigInteger genPseudoPrime(int bits, int confidence, Random rand)
			{
				BigInteger result = new BigInteger();
				bool done = false;

				while (!done)
				{
					result.genRandomBits(bits, rand);
					result.data[0] |= 0x01;        // make it odd

					// prime test
					done = result.isProbablePrime(confidence);
				}
				return result;
			}


			//***********************************************************************
			// Generates a random number with the specified number of bits such
			// that gcd(number, this) = 1
			//***********************************************************************

			public BigInteger genCoPrime(int bits, Random rand)
			{
				bool done = false;
				BigInteger result = new BigInteger();

				while (!done)
				{
					result.genRandomBits(bits, rand);
					//Console.WriteLine(result.ToString(16));

					// gcd test
					BigInteger g = result.gcd(this);
					if (g.dataLength == 1 && g.data[0] == 1)
						done = true;
				}

				return result;
			}


			//***********************************************************************
			// Returns the modulo inverse of this.  Throws ArithmeticException if
			// the inverse does not exist.  (i.e. gcd(this, modulus) != 1)
			//***********************************************************************

			public BigInteger modInverse(BigInteger modulus)
			{
				BigInteger[] p = { 0, 1 };
				BigInteger[] q = new BigInteger[2];    // quotients
				BigInteger[] r = { 0, 0 };             // remainders

				int step = 0;

				BigInteger a = modulus;
				BigInteger b = this;

				while (b.dataLength > 1 || (b.dataLength == 1 && b.data[0] != 0))
				{
					BigInteger quotient = new BigInteger();
					BigInteger remainder = new BigInteger();

					if (step > 1)
					{
						BigInteger pval = (p[0] - (p[1] * q[0])) % modulus;
						p[0] = p[1];
						p[1] = pval;
					}

					if (b.dataLength == 1)
						singleByteDivide(a, b, quotient, remainder);
					else
						multiByteDivide(a, b, quotient, remainder);

					/*
					Console.WriteLine(quotient.dataLength);
					Console.WriteLine("{0} = {1}({2}) + {3}  p = {4}", a.ToString(10),
									  b.ToString(10), quotient.ToString(10), remainder.ToString(10),
									  p[1].ToString(10));
					*/

					q[0] = q[1];
					r[0] = r[1];
					q[1] = quotient; r[1] = remainder;

					a = b;
					b = remainder;

					step++;
				}

				if (r[0].dataLength > 1 || (r[0].dataLength == 1 && r[0].data[0] != 1))
					throw (new ArithmeticException("No inverse!"));

				BigInteger result = ((p[0] - (p[1] * q[0])) % modulus);

				if ((result.data[maxLength - 1] & 0x80000000) != 0)
					result += modulus;  // get the least positive modulus

				return result;
			}


			//***********************************************************************
			// Returns the value of the BigInteger as a byte array.  The lowest
			// index contains the MSB.
			//***********************************************************************

			public byte[] getBytes()
			{
				int numBits = bitCount();

				int numBytes = numBits >> 3;
				if ((numBits & 0x7) != 0)
					numBytes++;

				byte[] result = new byte[numBytes];

				//Console.WriteLine(result.Length);

				int pos = 0;
				uint tempVal, val = data[dataLength - 1];

				if ((tempVal = (val >> 24 & 0xFF)) != 0)
					result[pos++] = (byte)tempVal;
				if ((tempVal = (val >> 16 & 0xFF)) != 0)
					result[pos++] = (byte)tempVal;
				else if (pos > 0)
					pos++;
				if ((tempVal = (val >> 8 & 0xFF)) != 0)
					result[pos++] = (byte)tempVal;
				else if (pos > 0)
					pos++;
				if ((tempVal = (val & 0xFF)) != 0)
					result[pos++] = (byte)tempVal;

				for (int i = dataLength - 2; i >= 0; i--, pos += 4)
				{
					val = data[i];
					result[pos + 3] = (byte)(val & 0xFF);
					val >>= 8;
					result[pos + 2] = (byte)(val & 0xFF);
					val >>= 8;
					result[pos + 1] = (byte)(val & 0xFF);
					val >>= 8;
					result[pos] = (byte)(val & 0xFF);
				}

				return result;
			}


			//***********************************************************************
			// Sets the value of the specified bit to 1
			// The Least Significant Bit position is 0.
			//***********************************************************************

			public void setBit(uint bitNum)
			{
				uint bytePos = bitNum >> 5;             // divide by 32
				byte bitPos = (byte)(bitNum & 0x1F);    // get the lowest 5 bits

				uint mask = (uint)1 << bitPos;
				this.data[bytePos] |= mask;

				if (bytePos >= this.dataLength)
					this.dataLength = (int)bytePos + 1;
			}


			//***********************************************************************
			// Sets the value of the specified bit to 0
			// The Least Significant Bit position is 0.
			//***********************************************************************

			public void unsetBit(uint bitNum)
			{
				uint bytePos = bitNum >> 5;

				if (bytePos < this.dataLength)
				{
					byte bitPos = (byte)(bitNum & 0x1F);

					uint mask = (uint)1 << bitPos;
					uint mask2 = 0xFFFFFFFF ^ mask;

					this.data[bytePos] &= mask2;

					if (this.dataLength > 1 && this.data[this.dataLength - 1] == 0)
						this.dataLength--;
				}
			}


			//***********************************************************************
			// Returns a value that is equivalent to the integer square root
			// of the BigInteger.
			//
			// The integer square root of "this" is defined as the largest integer n
			// such that (n * n) <= this
			//
			//***********************************************************************

			public BigInteger sqrt()
			{
				uint numBits = (uint)this.bitCount();

				if ((numBits & 0x1) != 0)        // odd number of bits
					numBits = (numBits >> 1) + 1;
				else
					numBits = (numBits >> 1);

				uint bytePos = numBits >> 5;
				byte bitPos = (byte)(numBits & 0x1F);

				uint mask;

				BigInteger result = new BigInteger();
				if (bitPos == 0)
					mask = 0x80000000;
				else
				{
					mask = (uint)1 << bitPos;
					bytePos++;
				}
				result.dataLength = (int)bytePos;

				for (int i = (int)bytePos - 1; i >= 0; i--)
				{
					while (mask != 0)
					{
						// guess
						result.data[i] ^= mask;

						// undo the guess if its square is larger than this
						if ((result * result) > this)
							result.data[i] ^= mask;

						mask >>= 1;
					}
					mask = 0x80000000;
				}
				return result;
			}


			//***********************************************************************
			// Returns the k_th number in the Lucas Sequence reduced modulo n.
			//
			// Uses index doubling to speed up the process.  For example, to calculate V(k),
			// we maintain two numbers in the sequence V(n) and V(n+1).
			//
			// To obtain V(2n), we use the identity
			//      V(2n) = (V(n) * V(n)) - (2 * Q^n)
			// To obtain V(2n+1), we first write it as
			//      V(2n+1) = V((n+1) + n)
			// and use the identity
			//      V(m+n) = V(m) * V(n) - Q * V(m-n)
			// Hence,
			//      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
			//                   = V(n+1) * V(n) - Q^n * V(1)
			//                   = V(n+1) * V(n) - Q^n * P
			//
			// We use k in its binary expansion and perform index doubling for each
			// bit position.  For each bit position that is set, we perform an
			// index doubling followed by an index addition.  This means that for V(n),
			// we need to update it to V(2n+1).  For V(n+1), we need to update it to
			// V((2n+1)+1) = V(2*(n+1))
			//
			// This function returns
			// [0] = U(k)
			// [1] = V(k)
			// [2] = Q^n
			//
			// Where U(0) = 0 % n, U(1) = 1 % n
			//       V(0) = 2 % n, V(1) = P % n
			//***********************************************************************

			public static BigInteger[] LucasSequence(BigInteger P, BigInteger Q,
													 BigInteger k, BigInteger n)
			{
				if (k.dataLength == 1 && k.data[0] == 0)
				{
					BigInteger[] result = new BigInteger[3];

					result[0] = 0; result[1] = 2 % n; result[2] = 1 % n;
					return result;
				}

				// calculate constant = b^(2k) / m
				// for Barrett Reduction
				BigInteger constant = new BigInteger();

				int nLen = n.dataLength << 1;
				constant.data[nLen] = 0x00000001;
				constant.dataLength = nLen + 1;

				constant = constant / n;

				// calculate values of s and t
				int s = 0;

				for (int index = 0; index < k.dataLength; index++)
				{
					uint mask = 0x01;

					for (int i = 0; i < 32; i++)
					{
						if ((k.data[index] & mask) != 0)
						{
							index = k.dataLength;      // to break the outer loop
							break;
						}
						mask <<= 1;
						s++;
					}
				}

				BigInteger t = k >> s;

				//Console.WriteLine("s = " + s + " t = " + t);
				return LucasSequenceHelper(P, Q, t, n, constant, s);
			}


			//***********************************************************************
			// Performs the calculation of the kth term in the Lucas Sequence.
			// For details of the algorithm, see reference [9].
			//
			// k must be odd.  i.e LSB == 1
			//***********************************************************************

			private static BigInteger[] LucasSequenceHelper(BigInteger P, BigInteger Q,
															BigInteger k, BigInteger n,
															BigInteger constant, int s)
			{
				BigInteger[] result = new BigInteger[3];

				if ((k.data[0] & 0x00000001) == 0)
					throw (new ArgumentException("Argument k must be odd."));

				int numbits = k.bitCount();
				uint mask = (uint)0x1 << ((numbits & 0x1F) - 1);

				// v = v0, v1 = v1, u1 = u1, Q_k = Q^0

				BigInteger v = 2 % n, Q_k = 1 % n,
						   v1 = P % n, u1 = Q_k;
				bool flag = true;

				for (int i = k.dataLength - 1; i >= 0; i--)     // iterate on the binary expansion of k
				{
					//Console.WriteLine("round");
					while (mask != 0)
					{
						if (i == 0 && mask == 0x00000001)        // last bit
							break;

						if ((k.data[i] & mask) != 0)             // bit is set
						{
							// index doubling with addition

							u1 = (u1 * v1) % n;

							v = ((v * v1) - (P * Q_k)) % n;
							v1 = n.BarrettReduction(v1 * v1, n, constant);
							v1 = (v1 - ((Q_k * Q) << 1)) % n;

							if (flag)
								flag = false;
							else
								Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);

							Q_k = (Q_k * Q) % n;
						}
						else
						{
							// index doubling
							u1 = ((u1 * v) - Q_k) % n;

							v1 = ((v * v1) - (P * Q_k)) % n;
							v = n.BarrettReduction(v * v, n, constant);
							v = (v - (Q_k << 1)) % n;

							if (flag)
							{
								Q_k = Q % n;
								flag = false;
							}
							else
								Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
						}

						mask >>= 1;
					}
					mask = 0x80000000;
				}

				// at this point u1 = u(n+1) and v = v(n)
				// since the last bit always 1, we need to transform u1 to u(2n+1) and v to v(2n+1)

				u1 = ((u1 * v) - Q_k) % n;
				v = ((v * v1) - (P * Q_k)) % n;
				if (flag)
					flag = false;
				else
					Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);

				Q_k = (Q_k * Q) % n;


				for (int i = 0; i < s; i++)
				{
					// index doubling
					u1 = (u1 * v) % n;
					v = ((v * v) - (Q_k << 1)) % n;

					if (flag)
					{
						Q_k = Q % n;
						flag = false;
					}
					else
						Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
				}

				result[0] = u1;
				result[1] = v;
				result[2] = Q_k;

				return result;
			}


			//***********************************************************************
			// Tests the correct implementation of the /, %, * and + operators
			//***********************************************************************

			public static void MulDivTest(int rounds)
			{
				Random rand = new Random();
				byte[] val = new byte[64];
				byte[] val2 = new byte[64];

				for (int count = 0; count < rounds; count++)
				{
					// generate 2 numbers of random length
					int t1 = 0;
					while (t1 == 0)
						t1 = (int)(rand.NextDouble() * 65);

					int t2 = 0;
					while (t2 == 0)
						t2 = (int)(rand.NextDouble() * 65);

					bool done = false;
					while (!done)
					{
						for (int i = 0; i < 64; i++)
						{
							if (i < t1)
								val[i] = (byte)(rand.NextDouble() * 256);
							else
								val[i] = 0;

							if (val[i] != 0)
								done = true;
						}
					}

					done = false;
					while (!done)
					{
						for (int i = 0; i < 64; i++)
						{
							if (i < t2)
								val2[i] = (byte)(rand.NextDouble() * 256);
							else
								val2[i] = 0;

							if (val2[i] != 0)
								done = true;
						}
					}

					while (val[0] == 0)
						val[0] = (byte)(rand.NextDouble() * 256);
					while (val2[0] == 0)
						val2[0] = (byte)(rand.NextDouble() * 256);

					Console.WriteLine(count);
					BigInteger bn1 = new BigInteger(val, t1);
					BigInteger bn2 = new BigInteger(val2, t2);


					// Determine the quotient and remainder by dividing
					// the first number by the second.

					BigInteger bn3 = bn1 / bn2;
					BigInteger bn4 = bn1 % bn2;

					// Recalculate the number
					BigInteger bn5 = (bn3 * bn2) + bn4;

					// Make sure they're the same
					if (bn5 != bn1)
					{
						Console.WriteLine("Error at " + count);
						Console.WriteLine(bn1 + "\n");
						Console.WriteLine(bn2 + "\n");
						Console.WriteLine(bn3 + "\n");
						Console.WriteLine(bn4 + "\n");
						Console.WriteLine(bn5 + "\n");
						return;
					}
				}
			}


			//***********************************************************************
			// Tests the correct implementation of the modulo exponential function
			// using RSA encryption and decryption (using pre-computed encryption and
			// decryption keys).
			//***********************************************************************

			public static void RSATest(int rounds)
			{
				Random rand = new Random(1);
				byte[] val = new byte[64];

				// private and public key
				BigInteger bi_e = new BigInteger("a932b948feed4fb2b692609bd22164fc9edb59fae7880cc1eaff7b3c9626b7e5b241c27a974833b2622ebe09beb451917663d47232488f23a117fc97720f1e7", 16);
				BigInteger bi_d = new BigInteger("4adf2f7a89da93248509347d2ae506d683dd3a16357e859a980c4f77a4e2f7a01fae289f13a851df6e9db5adaa60bfd2b162bbbe31f7c8f828261a6839311929d2cef4f864dde65e556ce43c89bbbf9f1ac5511315847ce9cc8dc92470a747b8792d6a83b0092d2e5ebaf852c85cacf34278efa99160f2f8aa7ee7214de07b7", 16);
				BigInteger bi_n = new BigInteger("e8e77781f36a7b3188d711c2190b560f205a52391b3479cdb99fa010745cbeba5f2adc08e1de6bf38398a0487c4a73610d94ec36f17f3f46ad75e17bc1adfec99839589f45f95ccc94cb2a5c500b477eb3323d8cfab0c8458c96f0147a45d27e45a4d11d54d77684f65d48f15fafcc1ba208e71e921b9bd9017c16a5231af7f", 16);

				Console.WriteLine("e =\n" + bi_e.ToString(10));
				Console.WriteLine("\nd =\n" + bi_d.ToString(10));
				Console.WriteLine("\nn =\n" + bi_n.ToString(10) + "\n");

				for (int count = 0; count < rounds; count++)
				{
					// generate data of random length
					int t1 = 0;
					while (t1 == 0)
						t1 = (int)(rand.NextDouble() * 65);

					bool done = false;
					while (!done)
					{
						for (int i = 0; i < 64; i++)
						{
							if (i < t1)
								val[i] = (byte)(rand.NextDouble() * 256);
							else
								val[i] = 0;

							if (val[i] != 0)
								done = true;
						}
					}

					while (val[0] == 0)
						val[0] = (byte)(rand.NextDouble() * 256);

					Console.Write("Round = " + count);

					// encrypt and decrypt data
					BigInteger bi_data = new BigInteger(val, t1);
					BigInteger bi_encrypted = bi_data.modPow(bi_e, bi_n);
					BigInteger bi_decrypted = bi_encrypted.modPow(bi_d, bi_n);

					// compare
					if (bi_decrypted != bi_data)
					{
						Console.WriteLine("\nError at round " + count);
						Console.WriteLine(bi_data + "\n");
						return;
					}
					Console.WriteLine(" <PASSED>.");
				}

			}


			//***********************************************************************
			// Tests the correct implementation of the modulo exponential and
			// inverse modulo functions using RSA encryption and decryption.  The two
			// pseudoprimes p and q are fixed, but the two RSA keys are generated
			// for each round of testing.
			//***********************************************************************

			public static void RSATest2(int rounds)
			{
				Random rand = new Random();
				byte[] val = new byte[64];

				byte[] pseudoPrime1 = {
                        (byte)0x85, (byte)0x84, (byte)0x64, (byte)0xFD, (byte)0x70, (byte)0x6A,
                        (byte)0x9F, (byte)0xF0, (byte)0x94, (byte)0x0C, (byte)0x3E, (byte)0x2C,
                        (byte)0x74, (byte)0x34, (byte)0x05, (byte)0xC9, (byte)0x55, (byte)0xB3,
                        (byte)0x85, (byte)0x32, (byte)0x98, (byte)0x71, (byte)0xF9, (byte)0x41,
                        (byte)0x21, (byte)0x5F, (byte)0x02, (byte)0x9E, (byte)0xEA, (byte)0x56,
                        (byte)0x8D, (byte)0x8C, (byte)0x44, (byte)0xCC, (byte)0xEE, (byte)0xEE,
                        (byte)0x3D, (byte)0x2C, (byte)0x9D, (byte)0x2C, (byte)0x12, (byte)0x41,
                        (byte)0x1E, (byte)0xF1, (byte)0xC5, (byte)0x32, (byte)0xC3, (byte)0xAA,
                        (byte)0x31, (byte)0x4A, (byte)0x52, (byte)0xD8, (byte)0xE8, (byte)0xAF,
                        (byte)0x42, (byte)0xF4, (byte)0x72, (byte)0xA1, (byte)0x2A, (byte)0x0D,
                        (byte)0x97, (byte)0xB1, (byte)0x31, (byte)0xB3,
                };

				byte[] pseudoPrime2 = {
                        (byte)0x99, (byte)0x98, (byte)0xCA, (byte)0xB8, (byte)0x5E, (byte)0xD7,
                        (byte)0xE5, (byte)0xDC, (byte)0x28, (byte)0x5C, (byte)0x6F, (byte)0x0E,
                        (byte)0x15, (byte)0x09, (byte)0x59, (byte)0x6E, (byte)0x84, (byte)0xF3,
                        (byte)0x81, (byte)0xCD, (byte)0xDE, (byte)0x42, (byte)0xDC, (byte)0x93,
                        (byte)0xC2, (byte)0x7A, (byte)0x62, (byte)0xAC, (byte)0x6C, (byte)0xAF,
                        (byte)0xDE, (byte)0x74, (byte)0xE3, (byte)0xCB, (byte)0x60, (byte)0x20,
                        (byte)0x38, (byte)0x9C, (byte)0x21, (byte)0xC3, (byte)0xDC, (byte)0xC8,
                        (byte)0xA2, (byte)0x4D, (byte)0xC6, (byte)0x2A, (byte)0x35, (byte)0x7F,
                        (byte)0xF3, (byte)0xA9, (byte)0xE8, (byte)0x1D, (byte)0x7B, (byte)0x2C,
                        (byte)0x78, (byte)0xFA, (byte)0xB8, (byte)0x02, (byte)0x55, (byte)0x80,
                        (byte)0x9B, (byte)0xC2, (byte)0xA5, (byte)0xCB,
                };


				BigInteger bi_p = new BigInteger(pseudoPrime1);
				BigInteger bi_q = new BigInteger(pseudoPrime2);
				BigInteger bi_pq = (bi_p - 1) * (bi_q - 1);
				BigInteger bi_n = bi_p * bi_q;

				for (int count = 0; count < rounds; count++)
				{
					// generate private and public key
					BigInteger bi_e = bi_pq.genCoPrime(512, rand);
					BigInteger bi_d = bi_e.modInverse(bi_pq);

					Console.WriteLine("\ne =\n" + bi_e.ToString(10));
					Console.WriteLine("\nd =\n" + bi_d.ToString(10));
					Console.WriteLine("\nn =\n" + bi_n.ToString(10) + "\n");

					// generate data of random length
					int t1 = 0;
					while (t1 == 0)
						t1 = (int)(rand.NextDouble() * 65);

					bool done = false;
					while (!done)
					{
						for (int i = 0; i < 64; i++)
						{
							if (i < t1)
								val[i] = (byte)(rand.NextDouble() * 256);
							else
								val[i] = 0;

							if (val[i] != 0)
								done = true;
						}
					}

					while (val[0] == 0)
						val[0] = (byte)(rand.NextDouble() * 256);

					Console.Write("Round = " + count);

					// encrypt and decrypt data
					BigInteger bi_data = new BigInteger(val, t1);
					BigInteger bi_encrypted = bi_data.modPow(bi_e, bi_n);
					BigInteger bi_decrypted = bi_encrypted.modPow(bi_d, bi_n);

					// compare
					if (bi_decrypted != bi_data)
					{
						Console.WriteLine("\nError at round " + count);
						Console.WriteLine(bi_data + "\n");
						return;
					}
					Console.WriteLine(" <PASSED>.");
				}

			}


			//***********************************************************************
			// Tests the correct implementation of sqrt() method.
			//***********************************************************************

			public static void SqrtTest(int rounds)
			{
				Random rand = new Random();
				for (int count = 0; count < rounds; count++)
				{
					// generate data of random length
					int t1 = 0;
					while (t1 == 0)
						t1 = (int)(rand.NextDouble() * 1024);

					Console.Write("Round = " + count);

					BigInteger a = new BigInteger();
					a.genRandomBits(t1, rand);

					BigInteger b = a.sqrt();
					BigInteger c = (b + 1) * (b + 1);

					// check that b is the largest integer such that b*b <= a
					if (c <= a)
					{
						Console.WriteLine("\nError at round " + count);
						Console.WriteLine(a + "\n");
						return;
					}
					Console.WriteLine(" <PASSED>.");
				}
			}
		}
	}
}
