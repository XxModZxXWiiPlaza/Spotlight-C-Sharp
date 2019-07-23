using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using Spotlight.Client.Extensions;
using Spotlight.Shared;

namespace Spotlight.Client
{
	public class Main : BaseScript
	{
		private bool _isMovingSpotlight;
		private DateTime _spotlightToggleLastActivated = DateTime.MinValue;
		private const float SpotlightMovementPerTick = 0.05f;

		public Main()
		{
			EventHandlers[Events.ClientAttemptToggleSpotlight] += new Action(OnClientAttemptToggleSpotlight);
            
			Tick += OnTick;
		}

		/// <summary>
		/// Event handler triggered by server when this client tries to toggle a spotlight via the /spotlight command
		/// </summary>
		private void OnClientAttemptToggleSpotlight()
		{
            Ped ped2 = Game.PlayerPed;
            var ped = API.GetPlayerPed(-1);
            var veh = API.GetVehiclePedIsIn(ped, false);
            Vehicle veh2 = ped2.CurrentVehicle;



            if (API.IsPedInAnyVehicle(ped, false))
			{
                if(API.GetVehicleClass(veh) == 18)
                {
                    ToggleSpotlight(veh2);
                }
			}
			else
			{
				Screen.ShowNotification($"~r~ERROR: ~w~You are either out of your vehicle or you are not in a LEO Vehicle.");
			}
		}

		/// <summary>
		/// Turn's the spotlight on or off in the current vehicle
		/// </summary>
		/// <param name="veh"></param>
		private void ToggleSpotlight(Vehicle veh)
		{
			//todo a whitelist of what vehicles have spotlights

			if (veh.IsSpotlightOn())
			{
				veh.IsSpotlightOn(false);
				return;
			}


			veh.SpotlightDirection(Vector3.Zero);
			veh.IsSpotlightOn(true);
		}

		/// <summary>
		/// OnTick method - will render the activated spotlights each tick
		/// </summary>
		/// <returns></returns>
		private async Task OnTick()
		{
			SpotlightControls();

			IEnumerable<Vehicle> vehicles = GetVehiclesForSpotlightHandling();
			var spotlightCounter = 0;

			foreach (Vehicle vehicle in vehicles)
			{
				ProcessSpotlight(vehicle, spotlightCounter++);
			}

			await Task.FromResult(0);
		}

        /// <summary>
        /// Handles all keybind controls for the spotlight
        ///   Toggle spotlight:  double-tap handbrake
        ///   Aim spotlight:  hold handbrake, press W/A/S/D
        /// 
        /// Requires the vehicle to be stopped in order to aim the spotlight
        /// </summary>
        private void SpotlightControls() //todo controller-friendly control binds
        {
            if (_isMovingSpotlight)
            {
                return;
            }

            Ped ped2 = Game.PlayerPed;
            var ped = API.GetPlayerPed(-1);
            var veh = API.GetVehiclePedIsIn(ped, false);
            Vehicle veh2 = ped2.CurrentVehicle;

            //double-click of the handbrake will toggle the spotlight
            if (API.IsPedInAnyVehicle(ped, false))
            {
                if (API.GetVehicleClass(veh) == 18)
                {
                    if (Game.IsControlJustPressed(27, Control.ReplaySave))
            {
               
                        DateTime now = DateTime.UtcNow;
                        TimeSpan diff = now - _spotlightToggleLastActivated;
                        if (diff.TotalMilliseconds < 1000)
                        {
                            _spotlightToggleLastActivated = DateTime.MinValue;
                            ToggleSpotlight(veh2);
                        }
                        else
                        {
                            _spotlightToggleLastActivated = now;
                        }
                    }

                    if (veh2 == null || !veh2.IsStopped)
                    {
                        return;
                    }

                    if (Game.IsControlPressed(27, Control.VehicleHandbrake))
                    {
                        _isMovingSpotlight = true;

                        if (Game.IsControlPressed(27, Control.VehicleAccelerate))
                        {
                            Vector3 vector = veh2.SpotlightDirection();
                            vector.Z += SpotlightMovementPerTick;
                            veh2.SpotlightDirection(vector);
                        }

                        if (Game.IsControlPressed(27, Control.VehicleBrake))
                        {
                            Vector3 vector = veh2.SpotlightDirection();
                            vector.Z -= SpotlightMovementPerTick;
                            veh2.SpotlightDirection(vector);
                        }

                        if (Game.IsControlPressed(27, Control.MoveLeftOnly))
                        {
                            Vector3 vector = veh2.SpotlightDirection();
                            float heading = LocalPlayer.Character.Heading;
                            if (heading > 90 && heading < 270)
                            {
                                vector.X += SpotlightMovementPerTick;
                            }
                            else
                            {
                                vector.X -= SpotlightMovementPerTick;
                            }



                            veh2.SpotlightDirection(vector);
                        }

                        if (Game.IsControlPressed(27, Control.MoveRightOnly))
                        {
                            Vector3 vector = veh2.SpotlightDirection();
                            float heading = LocalPlayer.Character.Heading;
                            if (heading > 90 && heading < 270)
                            {
                                vector.X -= SpotlightMovementPerTick;
                            }
                            else
                            {
                                vector.X += SpotlightMovementPerTick;
                            }



                            veh2.SpotlightDirection(vector);
                        }

                        _isMovingSpotlight = false;
                    }
                }
            }
        }
		/// <summary>
		/// Gathers a list of vehicles to be evaluated for potentially having a spotlight.
		/// 
		/// This list is created, for seemingly lack of a better method, by compiling all players' current
		/// and previous vehicles.
		/// </summary>
		/// <returns>List of vehicles</returns>
		private IEnumerable<Vehicle> GetVehiclesForSpotlightHandling()
		{
			var vehicles = new HashSet<Vehicle>();

			var pl = new PlayerList();
			foreach (Player player in pl)
			{
				Ped ped = player.Character;

				if (ped.CurrentVehicle != null)
				{
					vehicles.Add(ped.CurrentVehicle);
				}

				if (ped.LastVehicle != null)
				{
					vehicles.Add(ped.LastVehicle);
				}
			}

			return vehicles;
		}

		/// <summary>
		/// Checks a single vehicle and renders the spotlight if appropriate.
		/// </summary>
		/// <param name="vehicle">The vehicle that might have a spotlight</param>
		/// <param name="counter">A unique counter per spotlight across all vehicles</param>
		private void ProcessSpotlight(Vehicle vehicle, int counter)
		{
			//dynamic abort conditions that need checked each time
			if (!Entity.Exists(vehicle) || !vehicle.IsRendered || !vehicle.IsDriveable)
			{
				return;
			}

			if (!vehicle.IsSpotlightOn())
			{
				return;
			}

			Vector3 direction = vehicle.SpotlightDirection(true);
			Vector3 position = vehicle.Position;

			//todo tweak these adjustments
			position = position + vehicle.ForwardVector *
			           1f; // move forward 1 unit to avoid getting shadows from the vehicle itself
			position.Z += 1f;

			RenderSpotlight(position, direction, counter);
		}

		/// <summary>
		/// Renders a spotlight instance for this client
		/// </summary>
		/// <param name="position">The origin of the spotlight</param>
		/// <param name="direction">Directional (aim) vector</param>
		/// <param name="counter">A unique counter per spotlight across all vehicles</param>
		private void RenderSpotlight(Vector3 position, Vector3 direction, int counter)
		{
			Function.Call(Hash._DRAW_SPOT_LIGHT_WITH_SHADOW,
				position.X, position.Y, position.Z, /* position vector */
				direction.X, direction.Y, direction.Z, /* direction vector */
				255, 175, 110, /* color rgb -- 255/170/110 is incandescent-like */
				100.0f, /* max ilumination distance */
			    50.0f, /* brightness */
				0.0f, /* roundness */
				14.0f, /* radius factor (beam angle) */
				6.0f, /* falloff - greater number = more intensity lost with distance */
				counter /* shadow id (unique per light) */
			);
		}
	}
}