using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace DragonSpire
{
	class MCStream
	{
		internal bool isEncrypted = false; //Whether or not we have enabled encryption on this stream yet
		Encoding stringEncoder = Encoding.BigEndianUnicode; //String encoder for sending / receiving strings

		PacketManager Owner; //The Client object that this string belongs to

		NetworkStream stream; //The NON-ENCRYPTED Stream
		AesStream Estream; //The Encrypted stream

		internal bool isDataAvailable
		{
			get
			{
				return stream.DataAvailable;
			}
		}

		internal MCStream(NetworkStream inStream, PacketManager owner)
		{
			stream = inStream;
			Owner = owner;
		}
		internal void BeginEncryption(byte[] key)
		{
			Estream = new AesStream(stream, key);
			isEncrypted = true;
		}

		#region Writing Methods!
		internal void WriteByte(byte value, bool Encrypted)
		{
			if (Encrypted)
			{
				Estream.WriteByte(value);
			}
			else
			{
				stream.WriteByte(value);
			}
		}
		internal void WriteBytes(byte[] value, bool Encrypted)
		{
			if (Encrypted)
			{
				Estream.Write(value, 0, value.Length);
			}
			else
			{
				stream.Write(value, 0, value.Length);
			}
		}

		internal void WritePacketType(PacketType value, bool Encrypted) { WriteByte((byte)value, Encrypted); }
		//internal void WriteBoolean(bool value) { WriteBytes(DBC.GetBytes(value)); }
		//internal void WriteSByte(sbyte value) { WriteByte((byte)value); }

		//internal void WriteShort(short value) { WriteBytes(DBC.GetBytes(value)); }
		//internal void WriteUShort(ushort value) { WriteBytes(DBC.GetBytes(value)); }

		//internal void WriteInt(int value) { WriteBytes(DBC.GetBytes(value)); }
		//internal void WriteUInt(uint value) { WriteBytes(DBC.GetBytes(value)); }

		//internal void WriteLong(long value) { WriteBytes(DBC.GetBytes(value)); }
		//internal void WriteULong(ulong value) { WriteBytes(DBC.GetBytes(value)); }

		//internal void WriteFloat(float value) { WriteBytes(DBC.GetBytes(value)); }
		//internal void WriteSingle(float value) { WriteBytes(DBC.GetBytes(value)); }
		//internal void WriteDouble(double value) { WriteBytes(DBC.GetBytes(value)); }

		//internal void WriteString(string s) { WriteBytes(DBC.GetBytes(s)); }
		#endregion

		#region Reading Methods
		internal byte ReadByte()
		{
			if (isEncrypted)
			{
				return (byte)Estream.ReadByte();
			}
			else
			{
				return (byte)stream.ReadByte();
			}
		}
		internal byte[] ReadBytes(int count)
		{
			var bytes = new byte[count];

			if (isEncrypted)
			{
				Estream.Read(bytes, 0, count);
			}
			else
			{
				stream.Read(bytes, 0, count);
			}

			return bytes;
		}

		internal bool ReadBool() { return DBC.ToBool(ReadBytes(1)); }
		internal sbyte ReadSByte() { return (sbyte)ReadByte(); }

		internal short ReadShort() { return DBC.ToShort(ReadBytes(2)); }
		internal ushort ReadUShort() { return DBC.ToUShort(ReadBytes(2)); }

		internal int ReadInt() { return DBC.ToInt(ReadBytes(4)); }
		internal uint ReadUInt() { return DBC.ToUInt(ReadBytes(4)); }

		internal long ReadLong() { return DBC.ToLong(ReadBytes(8)); }
		internal ulong ReadULong() { return DBC.ToULong(ReadBytes(8)); }

		internal float ReadFloat() { return DBC.ToFloat(ReadBytes(4)); }
		internal float ReadSingle() { return DBC.ToFloat(ReadBytes(4)); }
		internal double ReadDouble() { return DBC.ToDouble(ReadBytes(8)); }

		internal string ReadString() { return stringEncoder.GetString(ReadBytes(ReadShort() * 2)); }

		internal byte[] ReadByteArray(short length) { return ReadBytes(length); }

		internal SLOT ReadSlot()
		{
			short id = ReadShort();
			if (id == -1) return new SLOT(); //No item here!
			else //We have an item / block
			{
				SLOT slot = new SLOT();
				slot.material = MaterialManager.Materials[id];
				slot.Count = ReadByte();
				slot.Meta = ReadShort();

				short NBTLength = ReadShort();
				if (NBTLength > 0)
				{
					slot.ZippedNBT = ReadByteArray(NBTLength);
				}
				return slot;
			}
		}
		#endregion
	}
}
