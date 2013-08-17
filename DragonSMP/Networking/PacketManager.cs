using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace DragonSpire
{
	class PacketManager
	{
		internal static List<PacketData> DeadPackets = new List<PacketData>(); //Holds Dead PacketData objects for reuse

		internal List<PacketData> Packets = new List<PacketData>(); //Holds PacketData data to loop through
		List<PacketData> WorkingSet = new List<PacketData>(); //Holds the list of PacketData objects that we are currently working with
		List<PacketData> ListHolder = new List<PacketData>(); //Holds an empty list to swap out with the Packets list

		Client Owner; //The client object that this PacketManager belongs to
		internal MCStream mcStream; //The stream that were sending data to

		internal PacketManager(Client owner, NetworkStream stream)
		{
			Owner = owner;
			mcStream = new MCStream(stream, this);

			new Thread(new ThreadStart(SendPacketLoop)).Start();
		}

		void SendPacketLoop()
		{
			while (!Owner.isDisconnected && !Server.shouldShutdown)
			{
				if (Packets.Count == 0)
				{
					Thread.Sleep(10);
					continue;
				}
				lock (Packets)
				{
					WorkingSet = Packets;
					Packets = ListHolder;
				}
				foreach (PacketData PD in WorkingSet)
				{
					if (Owner.isDisconnected || Server.shouldShutdown) return;
					mcStream.WritePacketType(PD.packetType, PD.encrypted);
					mcStream.WriteBytes(PD.data.ToArray(), PD.encrypted);

					DeadPackets.Add(PD);
				}
				WorkingSet.Clear();
				ListHolder = WorkingSet;

				//sup pThread.Sleep(100); //change maybe?
			}
		}

		internal PacketData GetFreshPacket(PacketType PT)
		{
			return GetFreshPacket(PT, mcStream.isEncrypted);
		}
		internal static PacketData GetFreshPacket(PacketType PT, bool encrypted)
		{
			if (DeadPackets.Count == 0) //DeadPackets is empty!
			{
				return new PacketData(PT, encrypted); //Return a new PacketData Object
			}
			else //DeadPackets is NOT Empty
			{
				PacketData pd; //Create a new holder for the PacketData Variable
				lock (DeadPackets) //Lock the DeadPackets list (multi thread catch)
				{
					pd = DeadPackets[0]; //We want the first DeadPackets object
					DeadPackets.RemoveAt(0); //Remove the first DeadPacketsObject
				} //Unload DP List
				if (pd == null)
				{
					return new PacketData(PT, encrypted); //Return a new PacketData Object
				}
				pd.Refresh(PT, encrypted); //Refresh our packet Data object (with the type we have)
				return pd; //Return the PD we got from our DP List
			}
		}
	}
	class PacketData
	{
		internal DateTime timeStamp;
		internal PacketType packetType;
		internal List<byte> data;
		internal bool encrypted;

		internal PacketData(PacketType type, bool enc)
		{
			encrypted = enc;
			packetType = type;
			timeStamp = DateTime.Now;
			data = new List<byte>();
		}
		internal void Refresh(PacketType type, bool enc)
		{
			encrypted = enc;
			timeStamp = DateTime.Now;
			packetType = type;
			data.Clear();
		}

		internal void Add(bool value) { data.AddRange(DBC.GetBytes(value)); }

		internal void Add(byte value) { data.Add(value); }
		internal void Add(sbyte value) { data.Add((byte)value); }

		internal void Add(short value) { data.AddRange(DBC.GetBytes(value)); }
		internal void Add(ushort value) { data.AddRange(DBC.GetBytes(value)); }

		internal void Add(int value) { data.AddRange(DBC.GetBytes(value)); }
		internal void Add(uint value) { data.AddRange(DBC.GetBytes(value)); }

		internal void Add(long value) { data.AddRange(DBC.GetBytes(value)); }
		internal void Add(ulong value) { data.AddRange(DBC.GetBytes(value)); }

		internal void Add(float value) { data.AddRange(DBC.GetBytes(value)); }
		internal void Add(double value) { data.AddRange(DBC.GetBytes(value)); }

		internal void Add(string value) { data.AddRange(DBC.GetBytes(value)); }

		internal void Add(byte[] value) { data.AddRange(value); }
		internal void Add(int[] value) { foreach (int i in value) { Add(i); } }
	}
}
