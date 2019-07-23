using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotlight.Shared.Models
{
	/// <summary>
	/// Interface to be consumed by both Client and Server projects.
	/// 
	/// Each script will have a concrete 'Spotlight' class that implements this interface. 
	/// Each concrete class will have internal 'Start' and 'End' properties that return a Vector3, containing the float values below. 
	/// The 'get' on each prop should construct a Vector3 from the below values, and the 'set' will simply modify the float values. 
	/// This will avoid having to update two sets of variables for the same coordinate
	/// 
	/// Client and Server scripts reference different FX/FiveM DLL's, so the types do not exactly match, which is why it needs to be done this way. 
	/// For a struct like a Vector3, it MAY work to just refernce CitizenFX from the Shared project, 
	/// but I'd rather just be more type-safe (read: OCD) than have to deal with potential issues; FiveM is glitchy enough as it is!
	/// 
	/// The spotlight instances can be sent to/from the server and client with Newtonsoft.Json, 
	/// with each side sending (and expecting to receive) an instance of ISpotlight in a JSON string. 
	/// 
	/// Then the ISpotlight instance can be converted into each project's concrete Spotlight implementation. MAGIC!
	/// </summary>
	public interface ISpotlight
	{
		/// <summary>
		/// Whether the spotlight is on
		/// </summary>
		bool Active { get; set; }

		// TODO: Do we need another property for the player network ID or the vehicle handle?

		// Values of the Spotlight.Start Vector3
		float StartX { get; set; }
		float StartY { get; set; }
		float StartZ { get; set; }

		// Values of the Spotlight.End Vector3
		float EndX { get; set; }
		float EndY { get; set; }
		float EndZ { get; set; }
	}
}