using System.Collections.Generic;
using System.Linq;
using Sanderling.Motor;
using BotEngine.Common;
using Sanderling.ABot.Parse;
using Sanderling.Interface.MemoryStruct;

namespace Sanderling.ABot.Bot.Task
{
	public class SaveShipTask : IBotTask
	{
		public Bot Bot;

		public bool AllowRoam;

		static public bool ChatIsClean(WindowChatChannel chatWindow)
		{
			if (null == chatWindow)
				return false;

			if (chatWindow?.ParticipantView?.Scroll?.IsScrollable() ?? true)
				return false;

			var listParticipantNeutralOrEnemy =
				chatWindow?.ParticipantView?.Entry?.Where(participant => participant.IsNeutralOrEnemy())?.ToArray();

			//	we expect own char to show up there as well so there has to be one participant with neutral or enemy flag.
			return 1 == listParticipantNeutralOrEnemy?.Length;
		}

		public IEnumerable<IBotTask> Component
		{
			get
			{
				var memoryMeasurement = Bot?.MemoryMeasurementAtTime?.Value;

				var charIsLocatedInHighsec = 500 < memoryMeasurement?.InfoPanelCurrentSystem?.SecurityLevelMilli;

				var localChatWindow = memoryMeasurement?.WindowChatChannel?.FirstOrDefault(window => window?.Caption?.RegexMatchSuccessIgnoreCase(@"local\s*\[") ?? false);

				if (charIsLocatedInHighsec || ChatIsClean(localChatWindow))
				{
					AllowRoam = true;
					yield break;
				}

				yield return new RetreatTask
				{
					Bot = Bot,
				};
			}
		}

		public MotionParam Motion => null;
	}
}
