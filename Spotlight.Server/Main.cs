using CitizenFX.Core;
using System;
using CitizenFX.Core.Native;
using Spotlight.Shared;

namespace Spotlight.Server
{
	public class Main : BaseScript
	{
		public Main()
		{
			EventHandlers["chatMessage"] += new Action<int, string, string>(OnChatCommand);
		}

		/// <summary>
		/// Event handler for FIveM 'chatMessage' event.
		/// 
		/// Handles /spotlight command.
		/// </summary>
		/// <param name="source">Player id of message sender</param>
		/// <param name="name">Player name of message sender</param>
		/// <param name="message">The message content</param>
		private void OnChatCommand(int source, string name, string message)
		{
			string[] args = message.Split(' ');

			// accept a /spotlight command
			if (args[0] == "/spotlight256189129")
			{
				var pl = new PlayerList();
				Player pSrc = pl[source];

				if (pSrc != null)
				{
					TriggerClientEvent(pSrc, Events.ClientAttemptToggleSpotlight);
				}

				API.CancelEvent();
			}
		}
	}
}