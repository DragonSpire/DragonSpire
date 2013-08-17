using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{
	public static class DBC
	{
		#region ConvertToSend
		public static byte[] GetBytes(bool value)
		{
			return BitConverter.GetBytes(value);
		}

		public static byte[] GetBytes(short value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return bytes;

			//List<byte> bytes = new List<byte>();

			//bytes.Add((byte)((value & 0xFF00) >> 8));
			//bytes.Add((byte)(value & 0xFF));

			//return bytes.ToArray();
		}
		public static byte[] GetBytes(ushort value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return bytes;

			//List<byte> bytes = new List<byte>();

			//bytes.Add((byte)((value & 0xFF00) >> 8));
			//bytes.Add((byte)(value & 0xFF));

			//return bytes.ToArray();
		}

		public static byte[] GetBytes(int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return bytes;

			//List<byte> bytes = new List<byte>();

			//bytes.Add((byte)((value & 0xFF000000) >> 24));
			//bytes.Add((byte)((value & 0xFF0000) >> 16));
			//bytes.Add((byte)((value & 0xFF00) >> 8));
			//bytes.Add((byte)(value & 0xFF));

			//return bytes.ToArray();
		}
		public static byte[] GetBytes(uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return bytes;

			//List<byte> bytes = new List<byte>();

			//bytes.Add((byte)((value & 0xFF000000) >> 24));
			//bytes.Add((byte)((value & 0xFF0000) >> 16));
			//bytes.Add((byte)((value & 0xFF00) >> 8));
			//bytes.Add((byte)(value & 0xFF));

			//return bytes.ToArray();
		}

		public static byte[] GetBytes(long value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return bytes;

			//ulong Uvalue = BitConverter.ToUInt64(BitConverter.GetBytes(value), 0); //We need to convert it to a ulong to properly convert this value
			//return GetBytes(Uvalue);
		}
		public static byte[] GetBytes(ulong value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return bytes;

			//List<byte> bytes = new List<byte>();

			//bytes.Add((byte)((value & 0xFF00000000000000) >> 56));
			//bytes.Add((byte)((value & 0xFF000000000000) >> 48));
			//bytes.Add((byte)((value & 0xFF0000000000) >> 40));
			//bytes.Add((byte)((value & 0xFF00000000) >> 32));
			//bytes.Add((byte)((value & 0xFF000000) >> 24));
			//bytes.Add((byte)((value & 0xFF0000) >> 16));
			//bytes.Add((byte)((value & 0xFF00) >> 8));
			//bytes.Add((byte)(value & 0xFF));

			//return bytes.ToArray();
		}

		public static byte[] GetBytes(float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return bytes;

			//return GetBytes(BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
		}
		public static byte[] GetBytes(double value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Reverse(bytes);
			return bytes;

			//return GetBytes(BitConverter.ToInt64(BitConverter.GetBytes(value), 0));
		}

		public static byte[] GetBytes(string value)
		{
			List<byte> bytes = new List<byte>();

			if (value.Length == 0)
			{
				bytes.AddRange(GetBytes((ushort)0));
				return bytes.ToArray();
			}

			bytes.AddRange(GetBytes((ushort)value.Length));
			bytes.AddRange(Encoding.BigEndianUnicode.GetBytes(value));

			return bytes.ToArray();
		}
		#endregion
		#region ConvertFromClient
		public static bool ToBool(byte[] value)
		{
			if (value.Length != 1) throw new ArgumentOutOfRangeException("Byte arrays passed to bool can only have a length of 1, this one has a length of " + value.Length);
			return BitConverter.ToBoolean(value, 0);
		}

		public static short ToShort(byte[] value)
		{
			if (value.Length != 2) throw new ArgumentOutOfRangeException("Byte arrays passed to ToShort can only have a length of 2, this one has a length of " + value.Length);
			Array.Reverse(value);
			return BitConverter.ToInt16(value, 0);
		}
		public static ushort ToUShort(byte[] value)
		{
			if (value.Length != 2) throw new ArgumentOutOfRangeException("Byte arrays passed to ToUShort can only have a length of 2, this one has a length of " + value.Length);
			Array.Reverse(value);
			return BitConverter.ToUInt16(value, 0);
		}

		public static int ToInt(byte[] value)
		{
			if (value.Length != 4) throw new ArgumentOutOfRangeException("Byte arrays passed to ToInt can only have a length of 4, this one has a length of " + value.Length);
			Array.Reverse(value);
			return BitConverter.ToInt32(value, 0);
		}
		public static uint ToUInt(byte[] value)
		{
			if (value.Length != 4) throw new ArgumentOutOfRangeException("Byte arrays passed to ToUint can only have a length of 4, this one has a length of " + value.Length);
			Array.Reverse(value);
			return BitConverter.ToUInt32(value, 0);
		}

		public static long ToLong(byte[] value)
		{
			if (value.Length != 8) throw new ArgumentOutOfRangeException("Byte arrays passed to ToLong can only have a length of 8, this one has a length of " + value.Length);
			Array.Reverse(value);
			return BitConverter.ToInt64(value, 0);
		}
		public static ulong ToULong(byte[] value)
		{
			if (value.Length != 8) throw new ArgumentOutOfRangeException("Byte arrays passed to ToULong can only have a length of 8, this one has a length of " + value.Length);
			Array.Reverse(value);
			return BitConverter.ToUInt64(value, 0);
		}

		public static float ToFloat(byte[] value)
		{
			if (value.Length != 4) throw new ArgumentOutOfRangeException("Byte arrays passed to ToFloat can only have a length of 4, this one has a length of " + value.Length);
			Array.Reverse(value);
			return BitConverter.ToSingle(value, 0);
		}
		public static float ToSingle(byte[] value)
		{
			if (value.Length != 4) throw new ArgumentOutOfRangeException("Byte arrays passed to ToSingle can only have a length of 4, this one has a length of " + value.Length);
			Array.Reverse(value);
			return BitConverter.ToSingle(value, 0);
		}
		public static double ToDouble(byte[] value)
		{
			if (value.Length != 8) throw new ArgumentOutOfRangeException("Byte arrays passed to ToDouble can only have a length of 8, this one has a length of " + value.Length);
			Array.Reverse(value);
			return BitConverter.ToDouble(value, 0);
		}
		#endregion
	}
}
