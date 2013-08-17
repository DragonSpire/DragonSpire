using System;

namespace DragonSpire
{
	public struct EntityLocation
	{
		private double _x;
		private double _y;
		private double _stance;
		private double _z;
		private float _pitch;
		private float _yaw;
		private bool _onGround;
		private World _world;

		internal double X
		{
			get { return _x; }
		}
		internal double AX
		{
			get
			{
				return (_x * 32) + 16;
			}
		}
		internal double Y
		{
			get { return _y; }
		}
		internal double AY
		{
			get
			{
				return (_y * 32) + 16;
			}
		}
		internal double Stance
		{
			get { return _stance; }
		}
		internal double Z
		{
			get { return _z; }
		}
		internal double AZ
		{
			get
			{
				return (_z * 32) + 16;
			}
		}
		internal float Pitch
		{
			get { return _pitch; }
		}
		internal float Yaw
		{
			get { return _yaw; }
		}
		internal bool OnGround
		{
			get { return _onGround; }
		}
		internal World world
		{
			get { return _world; }
		}

		internal byte PitchByte
		{
			get
			{
				return FloatToByteFractional(_pitch);
			}
			set
			{
				_pitch = ByteFractionalToFloat(value);
			}
		}
		internal byte YawByte
		{
			get
			{
				return FloatToByteFractional(_yaw);
			}
			set
			{
				_yaw = ByteFractionalToFloat(value);
			}
		}

		internal EntityLocation(World w)
		{
			_onGround = true;
			_x = 0;
			_y = 0;
			_stance = 0;
			_z = 0;
			_pitch = 0;
			_yaw = 90f;
			_world = w;
		}
		internal EntityLocation(EntityLocation pl)
		{
			_onGround = true;
			_x = pl.X;
			_y = pl.Y;
			_stance = pl.Stance;
			_z = pl.Z;
			_pitch = pl.Pitch;
			_yaw = pl.Yaw;
			_world = pl.world;
		}
		internal EntityLocation(double x, double y, double z, World w)
		{
			_onGround = true;
			_x = x;
			_y = y;
			_stance = 0;
			_z = z;
			_pitch = 0;
			_yaw = 90f;
			_world = w;
		}
		internal EntityLocation(double x, double y, double z, float p, float ya, World w)
		{
			_onGround = true;
			_x = x;
			_y = y;
			_stance = 0;
			_z = z;
			_pitch = p;
			_yaw = ya;
			_world = w;
		}

		internal EntityLocation MoveTo(EntityLocation pl)
		{
			_x = pl.X;
			_y = pl.Y;
			_stance = pl.Stance;
			_z = pl.Z;
			_pitch = pl.Pitch;
			_yaw = pl.Yaw;
			_world = pl.world;

			return this;
		}
		internal EntityLocation MoveTo(double x, double y, double stance, double z, bool onGround)
		{
			_x = x;
			_y = y;
			_stance = stance;
			_z = z;
			_onGround = onGround;

			return this;
		}

		internal EntityLocation MoveLook(float yaw, float pitch)
		{
			_yaw = yaw;
			_pitch = pitch;

			return this;
		}

		internal EntityLocation OffsetBy(EntityLocation pl)
		{
			_x += pl.X;
			_y += pl.Y;
			_stance += pl.Stance;
			_z += pl.Z;

			return this;
		}
		internal EntityLocation Rotate(float PitchOffset, float YawOffset)
		{
			_pitch += PitchOffset;
			_yaw += YawOffset;

			return this;
		}

		internal BlockLocation ToBlockLocation()
		{
			return new BlockLocation((int)Math.Floor(X), (byte)Math.Floor(Y), (int)Math.Floor(Z), world);
		}
		internal ChunkLocation ToChunkLocation()
		{
			return new ChunkLocation((int)Math.Floor(X / 16), (int)Math.Floor(Z / 16), world);
		}

		internal EntityLocation GetOffset(EntityLocation pl)
		{
			return new EntityLocation(pl.X - X, pl.Y - Y, pl.Z - Z, world);
		}
		internal float GetDistance(EntityLocation pl) //TODO Verify this is correct, should it be sqrt or cubed-rt?
		{
			pl = GetOffset(pl); //Delta
			return (float)Math.Sqrt(pl.X * pl.X + pl.Y * pl.Y + pl.Z * pl.Z);
		}

		internal static byte FloatToByteFractional(float ToConvert)
		{
			return (byte)(ToConvert * 256.0F / 360.0F);
		}
		internal static float ByteFractionalToFloat(byte ToConvert)
		{
			//TODO verify this, this is not the same as the return method, I highly (almost certainly) believe this is wrong ;)
			return ToConvert * 255;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			EntityLocation PL = (EntityLocation)obj;
			return (GetDistance(PL) < .75); //return true if the players are less than .75 blocks apart
		}
		public static bool operator ==(EntityLocation PL, EntityLocation PL2)
		{
			//When comparing locations, we dont care about pitch / yaw
			return (PL.X == PL2.X && PL.Y == PL2.Y && PL.Z == PL2.Z);
		}
		public static bool operator !=(EntityLocation PL, EntityLocation PL2)
		{
			//When comparing locations, we dont care about pitch / yaw
			return (PL.X != PL2.X || PL.Y != PL2.Y || PL.Z != PL2.Z);
		}
		public override string ToString()
		{
			return _x + " " + _y + " " + _z + " " + _world.Name;
		}
	}
}