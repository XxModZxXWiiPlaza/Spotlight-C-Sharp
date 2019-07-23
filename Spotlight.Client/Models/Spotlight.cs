using CitizenFX.Core;
using Spotlight.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotlight.Client.Models
{
	internal class Spotlight : ISpotlight
	{
		public bool Active { get; set; }

		public Vector3 Start
		{
			get => new Vector3(StartX, StartY, StartZ);
			set
			{
				StartX = value.X;
				StartY = value.Y;
				StartZ = value.Z;
			}
		}

		public float StartX { get; set; }
		public float StartY { get; set; }
		public float StartZ { get; set; }

		public Vector3 End
		{
			get => new Vector3(EndX, EndY, EndZ);
			set
			{
				EndX = value.X;
				EndY = value.Y;
				EndZ = value.Z;
			}
		}

		public float EndX { get; set; }
		public float EndY { get; set; }
		public float EndZ { get; set; }
	}
}