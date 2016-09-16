using NUnit.Framework;
using Sanderling.ABot.Bot.Task;
using Sanderling.ABot.UI;

namespace Sanderling.ABot.Test.Exe
{
	public class BotDiagnosticMessage
	{
		static void AssertStringContainedInString(string expectedString, string containingString)
		{
			if (!(containingString?.Contains(expectedString) ?? false))
				throw new AssertionException("expected string \"" + expectedString + "\" not contained ín string \"" + containingString + "\"");
		}

		[Test]
		public void Diagnostic_Local_Chat_Window_Not_Found()
		{
			var stepResult = new Bot.Bot().Step(new Bot.BotStepInput { });

			AssertStringContainedInString(SaveShipTask.LocalChatWindowNotFoundDiagnosticText, stepResult?.RenderBotStepToUIText());
		}
	}
}
