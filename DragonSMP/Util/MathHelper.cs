using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DragonSpire
{
	class MathHelper
	{
		/// <summary>
		/// This method rounds a float toward zero
		/// </summary>
		/// <param name="value">The value to round</param>
		/// <returns>The rounded integer</returns>
		public static int RTZ(float value)
		{
			int rValue;

			if (value < 0)
			{
				rValue = (int)Math.Ceiling(value);

				if (rValue == value) return rValue;

				if (rValue > value) return rValue;
				rValue = (int)Math.Floor(value);
				if (rValue > value) return rValue;

				throw new InvalidOperationException("Cannot round value toward zero!");
			}
			else return (int)Math.Floor(value); //A positive value can always use Floor!
		}
		/// <summary>
		/// This method rounds a double toward zero
		/// </summary>
		/// <param name="value">The double to be rounded</param>
		/// <returns>The rounded integer</returns>
		public static int RTZ(double value)
		{
			int rValue;

			if (value < 0)
			{
				rValue = (int)Math.Ceiling(value);

				if (rValue == value) return rValue;

				if (rValue > value) return rValue;
				rValue = (int)Math.Floor(value);
				if (rValue > value) return rValue;

				throw new InvalidOperationException("Cannot round value toward zero!");
			}
			else return (int)Math.Floor(value); //A positive value can always use Floor!
		}

		/// <summary>
		/// This method rounds a float toward infinity
		/// </summary>
		/// <param name="value">The value to round</param>
		/// <returns>The rounded integer</returns>
		public static int RTI(float value)
		{
			int rValue;

			if (value < 0)
			{
				rValue = (int)Math.Floor(value);

				if(rValue == value) return rValue;

				if (rValue < value) return rValue;
				rValue = (int)Math.Ceiling(value);
				if (rValue < value) return rValue;

				throw new InvalidOperationException("Cannot round value toward infinity!");
			}
			else return (int)Math.Ceiling(value);
		}
		/// <summary>
		/// This method rounds a double toward infinity
		/// </summary>
		/// <param name="value">The double to be rounded</param>
		/// <returns>The rounded integer</returns>
		public static int RTI(double value)
		{
			int rValue;

			if (value < 0)
			{
				rValue = (int)Math.Floor(value);

				if (rValue == value) return rValue;

				if (rValue < value) return rValue;
				rValue = (int)Math.Ceiling(value);
				if (rValue < value) return rValue;

				throw new InvalidOperationException("Cannot round value toward infinity!");
			}
			else return (int)Math.Ceiling(value);
		}
	}
}
