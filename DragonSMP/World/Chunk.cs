using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zlib;

namespace DragonSpire
{
	public class Chunk
	{
		internal Dictionary<int, Player> Players = new Dictionary<int, Player>(); //A list of all PLAYERS in this chunk (Merlin33069 creatorfromhell etc)
		internal Dictionary<int, Mob> Mobs = new Dictionary<int, Mob>(); //A list of all MOBS in this chunk (sheep cows creepers etc)
		internal Dictionary<int, ObjectEntity> Objects = new Dictionary<int, ObjectEntity>(); //A list of all OBJECTS in this chunk (boats minecarts paintings items etc)

		internal Dictionary<BlockLocation, Window> Windows = new Dictionary<BlockLocation, Window>(); //This holds the windows that have been created in the map, a window is not created when a block is set but rather the first time it is opened

		internal World World;
		internal ChunkSection[] ChunkParts = new ChunkSection[16]; //they are all this size and we always want one, so may as well make it default
		internal byte[] BiomeData = new byte[256];
		internal ChunkLocation CL;
		internal bool isPersistant = false; //If true this chunk will not unload (used for Spawn Area for now)
		internal bool isLoaded = false; //We can have this chunk ready to be loaded, but not yet loaded ;)

		/// <summary>
		/// Create a new instance of the Chunk class for storing ChunkSections (which store block data)
		/// </summary>
		/// <param name="World">The world in which this chunk exists</param>
		/// <param name="Point">The ChunkCoordinate of this Chunk</param>
		internal Chunk(World World, ChunkLocation CL)
		{
			this.World = World;
			this.CL = CL;

			for (int i = 0; i < 16; i++)
			{
				ChunkParts[i] = new ChunkSection(i);
			}
		}

		internal void SetBlock(BlockLocation BL, Material mat, byte meta = 0)
		{
			ChunkSection CS = ChunkParts[(int)Math.Floor((double)BL.Y / 16)]; //get the ChunkSection based off the y location

			int x = BL.X % 16;
			int z = BL.Z % 16;

			if (x < 0) x += 16;
			if (z < 0) z += 16;

			int y = BL.Y % 16;

			CS.SetBlock(x, y, z, mat.ID, meta);
		}

		internal Block GetBlock(BlockLocation BL)
		{
			ChunkSection CS = ChunkParts[MathHelper.RTZ(BL.Y / 16)]; //get the ChunkSection based off the y location

			int x = BL.X % 16;
			int z = BL.Z % 16;

			if (x < 0) x += 16;
			if (z < 0) z += 16;

			int y = BL.Y % 16;

			return CS.GetBlock(x, y, z);
		}

		internal byte[] GetData()
		{
			try
			{
				using (MemoryStream ms = new MemoryStream())
				{
					// Write block types
					for (int i = 0; i < 16; i++)
						ms.Write(ChunkParts[i].Blocks, 0, ChunkParts[i].Blocks.Length);

					// Write metadata
					for (int i = 0; i < 16; i++)
						ms.Write(ChunkParts[i].MetaData, 0, ChunkParts[i].MetaData.Length);

					// Write block light
					for (int i = 0; i < 16; i++)
						ms.Write(ChunkParts[i].BlockLight, 0, ChunkParts[i].BlockLight.Length);

					// Write sky light
					for (int i = 0; i < 16; i++)
						ms.Write(ChunkParts[i].SkyLight, 0, ChunkParts[i].SkyLight.Length);

					ms.Write(BiomeData, 0, 256);

					// Compress the data
					return ms.ToArray();
				}
			}
			catch { return null; }
		}
		internal byte[] GetCompressedData()
		{
			return GetData().Compress(CompressionLevel.BestCompression);
		}
		internal static byte[] GetCompressedData(List<Chunk> chunks)
		{
			try
			{
				using (MemoryStream ms = new MemoryStream())
				{
					foreach(Chunk c in chunks)
					{
						byte[] data = c.GetData();
						ms.Write(data, 0, data.Length); 
					}
					return ms.ToArray().Compress(CompressionLevel.BestCompression);
				}
			}
			catch { return null; }
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			Chunk c = obj as Chunk;
			if (c == null) return false;

			return (c.CL == this.CL);
		}
	}
	public static class Extensions
		{
		/// <summary>
		/// Truncates a string to the specified maximum length.
		/// </summary>
		/// <param name="source">The string to be truncated.</param>
		/// <param name="maxLength">The maximum length to truncate the string to.</param>
		/// <returns>The truncated string.</returns>
		public static string Truncate(this string source, int maxLength)
		{
			if (source.Length > maxLength)
				source = source.Substring(0, maxLength);
			return source;
		}

		/// <summary>
		/// Truncates an array to the specified maximum length.
		/// </summary>
		/// <param name="source">The array to be truncated.</param>
		/// <param name="maxLength">The maximum length to truncate the array to.</param>
		/// <returns>The truncated array.</returns>
		public static Array Truncate(this Array source, int maxLength)
		{
			if (source.Length > maxLength)
			{
				Array dest = Array.CreateInstance(source.GetType().GetElementType(), maxLength);
				Array.Copy(source, 0, dest, 0, maxLength);
				return dest;
			}
			return source;
		}

		/// <summary>
		/// Truncates a specified number of elements from the beginning of an array.
		/// </summary>
		/// <param name="source">The array to be truncated.</param>
		/// <param name="count">The number of elements to truncate.</param>
		/// <returns>The truncated array.</returns>
		public static Array TruncateStart(this Array source, int count)
		{
			Array dest = Array.CreateInstance(source.GetType().GetElementType(), source.Length - count);
			Array.Copy(source, count, dest, 0, source.Length - count);
			return dest;
		}

		[Obsolete("Learn to use bit operators!", false)]
		public static byte GetBits(this byte data, int index, int count)
		{
			int max = (int)Math.Pow(2, count) - 1;
			return (byte)((data & (max << index)) >> index);
		}

		[Obsolete("Learn to use bit operators!", false)]
		public static byte SetBits(this byte data, int index, int value)
		{
			return (byte)(data | (value << index));
		}

		/// <summary>
		/// Compresses a byte array using Zlib.
		/// </summary>
		/// <param name="bytes">The byte array to be compressed.</param>
		/// <returns>Compressed byte array.</returns>
		public static byte[] Compress(this byte[] bytes)
		{
			return bytes.Compress(CompressionLevel.Default);
		}

		/// <summary>
		/// Compresses a byte array using Zlib.
		/// </summary>
		/// <param name="bytes">The byte array to be compressed.</param>
		/// <param name="level">Amount of compression to use.</param>
		/// <returns>Compressed byte array.</returns>
		public static byte[] Compress(this byte[] bytes, CompressionLevel level)
		{
			return bytes.Compress(level, CompressionType.Zlib);
		}

		/// <summary>
		/// Compresses a byte array using the specified compression type.
		/// </summary>
		/// <param name="bytes">The byte array to be compressed.</param>
		/// <param name="type">Type of compression to use.</param>
		/// <returns>Compressed byte array.</returns>
		public static byte[] Compress(this byte[] bytes, CompressionType type)
		{
			return bytes.Compress(CompressionLevel.Default, type);
		}

		/// <summary>
		/// Compresses a byte array using the specified compression type.
		/// </summary>
		/// <param name="bytes">The byte array to be compressed.</param>
		/// <param name="level">Amount of compression to use.</param>
		/// <param name="type">Type of compression to use.</param>
		/// <returns>Compressed byte array.</returns>
		public static byte[] Compress(this byte[] bytes, CompressionLevel level, CompressionType type)
		{
			using (MemoryStream memory = new MemoryStream())
			{
				switch (type)
				{
					case CompressionType.Zlib:
						using (ZlibStream stream = new ZlibStream(memory, CompressionMode.Compress, level, true))
							stream.Write(bytes, 0, bytes.Length);
						break;
					case CompressionType.GZip:
						using (GZipStream stream = new GZipStream(memory, CompressionMode.Compress, level, true))
							stream.Write(bytes, 0, bytes.Length);
						break;
					default:
						throw new ArgumentException("Unknown compression type.");
				}
				memory.Position = 0;
				bytes = new byte[memory.Length];
				memory.Read(bytes, 0, (int)memory.Length);
			}
			return bytes;
		}

		/// <summary>
		/// Decompresses a byte array using Zlib.
		/// </summary>
		/// <param name="bytes">The byte array to be decompressed.</param>
		/// <returns>Decompressed byte array.</returns>
		public static byte[] Decompress(this byte[] bytes)
		{
			return bytes.Decompress(CompressionType.Zlib);
		}

		/// <summary>
		/// Decompresses a byte array using the specified compression type.
		/// </summary>
		/// <param name="bytes">The byte array to be decompressed.</param>
		/// <param name="type">Type of compression to use.</param>
		/// <returns>Decompressed byte array.</returns>
		public static byte[] Decompress(this byte[] bytes, CompressionType type)
		{
			int size = 4096;
			byte[] buffer = new byte[size];
			using (MemoryStream memory = new MemoryStream())
			{
				using (MemoryStream memory2 = new MemoryStream(bytes))
					switch (type)
					{
						case CompressionType.Zlib:
							using (ZlibStream stream = new ZlibStream(memory2, CompressionMode.Decompress))
							{
								int count = 0;
								while ((count = stream.Read(buffer, 0, size)) > 0)
									memory.Write(buffer, 0, count);
							}
							break;
						case CompressionType.GZip:
							using (GZipStream stream = new GZipStream(memory2, CompressionMode.Decompress))
							{
								int count = 0;
								while ((count = stream.Read(buffer, 0, size)) > 0)
									memory.Write(buffer, 0, count);
							}
							break;
						default:
							throw new ArgumentException("Unknown compression type.");
					}
				return memory.ToArray();
			}
		}

		//public static int[] ToIntArray(this byte[] bytes)
		//{
		//    if (bytes.Length % 4 != 0) throw new Exception("Array length must be divisible by 4.");
		//    int[] ints = new int[bytes.Length / 4];
		//    for (int i = 0; i < bytes.Length; i += 4)
		//        ints[i / 4] = SMP.util.EndianBitConverter.Big.ToInt32(bytes, i);
		//    return ints;
		//}

		//public static byte[] ToByteArray(this int[] ints)
		//{
		//    byte[] bytes = new byte[ints.Length * 4];
		//    for (int i = 0; i < ints.Length; i++)
		//        SMP.util.EndianBitConverter.Big.GetBytes(ints[i]).CopyTo(bytes, i * 4);
		//    return bytes;
		//}

		/// <summary>
		/// Capitalizes a string, duh!
		/// </summary>
		/// <param name="str">String to be capitalized.</param>
		/// <returns>The capitalized string.</returns>
		public static string Capitalize(this string str)
		{
			if (String.IsNullOrEmpty(str))
				return String.Empty;
			char[] a = str.ToCharArray();
			a[0] = char.ToUpper(a[0]);
			return new string(a);
		}
	}
	public enum CompressionType
	{
		Zlib,
		GZip
	}
}