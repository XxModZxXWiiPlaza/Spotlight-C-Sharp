using System;
using CitizenFX.Core;

namespace Spotlight.Client.Extensions
{
	internal static class VehicleExtensions
	{
		private const string Decorator_SpotlightIsOn = "Spotlight_IsOn";
		private const string Decorator_SpotlightDirX = "Spotlight_DirX";
		private const string Decorator_SpotlightDirY = "Spotlight_DirY";
		private const string Decorator_SpotlightDirZ = "Spotlight_DirZ";
		private static readonly Vector3 SpotlightDirectionMins = new Vector3(-.6f, -.6f, -.6f);
		private static readonly Vector3 SpotlightDirectionMaxs = new Vector3(.6f, .6f, .6f);

		static VehicleExtensions()
		{
			EntityDecoration.RegisterProperty(Decorator_SpotlightIsOn, DecorationType.Bool);
			EntityDecoration.RegisterProperty(Decorator_SpotlightDirX, DecorationType.Float);
			EntityDecoration.RegisterProperty(Decorator_SpotlightDirY, DecorationType.Float);
			EntityDecoration.RegisterProperty(Decorator_SpotlightDirZ, DecorationType.Float);
		}

		/// <summary>
		/// Gets the state of this vehicle's spotlight
		/// </summary>
		/// <param name="vehicle"></param>
		/// <returns></returns>
		internal static bool IsSpotlightOn(this Vehicle vehicle)
		{
			return vehicle.HasDecor(Decorator_SpotlightIsOn) &&
			       vehicle.GetDecor<bool>(Decorator_SpotlightIsOn);
		}

		/// <summary>
		/// Sets the state of this vehicle's spotlight
		/// </summary>
		/// <param name="vehicle"></param>
		/// <param name="isOn"></param>
		internal static void IsSpotlightOn(this Vehicle vehicle, bool isOn)
		{
			vehicle.SetDecor(Decorator_SpotlightIsOn, isOn);
		}

		/// <summary>
		/// Gets the directional vector of this vehicle's spotlight.
		/// </summary>
		/// <param name="vehicle"></param>
		/// <param name="convertToWorldVector">If true, yields a world-relative directional vector instead of vehicle-relative</param>
		/// <returns></returns>
		internal static Vector3 SpotlightDirection(this Vehicle vehicle, bool convertToWorldVector = false)
		{
			Vector3 direction;
			try
			{
				direction = new Vector3(vehicle.GetDecor<float>(Decorator_SpotlightDirX),
					vehicle.GetDecor<float>(Decorator_SpotlightDirY),
					vehicle.GetDecor<float>(Decorator_SpotlightDirZ));
			}
			catch (EntityDecorationUnregisteredPropertyException)
			{
				vehicle.SpotlightDirection(Vector3.Zero);
				direction = Vector3.Zero;
			}

			//convert vehicle-relative direction into coordinate-relative
			if (convertToWorldVector)
			{
				//todo this doesn't work properly
				direction = Vector3.Add(direction, vehicle.ForwardVector);
				direction.Normalize();
			}

			return direction;
		}

		/// <summary>
		/// Sets the directional vector of this vehicle's spotlight.
		/// </summary>
		/// <param name="vehicle"></param>
		/// <param name="direction">The vehicle-relative vector (e.g. 0,0,0 is straight out the front). Do not provide a world-relative vector.</param>
		internal static void SpotlightDirection(this Vehicle vehicle, Vector3 direction)
		{
			direction = NormalizeDirectionalVector(direction);
			vehicle.SetDecor(Decorator_SpotlightDirX, direction.X);
			vehicle.SetDecor(Decorator_SpotlightDirY, direction.Y);
			vehicle.SetDecor(Decorator_SpotlightDirZ, direction.Z);

		}

		/// <summary>
		/// Normalizes the directional vector by handling wrap-around and applies movement limits.
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		private static Vector3 NormalizeDirectionalVector(Vector3 vector)
		{
			for (var index = 0; index <= 2; index++)
			{
				float val = vector[index];

				if (val < -1 || val > 1)
				{
					var baseval = (int) val;
					float remainder = val - baseval;
					float newval = (1 - Math.Abs(remainder)) * (Math.Sign(val) * -1);
					vector[index] = newval;
				}
			}

			//apply movement limits
			vector = Vector3.Clamp(vector, SpotlightDirectionMins, SpotlightDirectionMaxs);

			return vector;
		}
	}
}