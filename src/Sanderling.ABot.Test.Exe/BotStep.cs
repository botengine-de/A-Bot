using BotEngine.Interface;
using Moq;
using NUnit.Framework;
using Sanderling.ABot.Bot.Task;
using Sanderling.ABot.UI;
using Sanderling.Interface.MemoryStruct;
using System.Linq;

namespace Sanderling.ABot.Test.Exe
{
	public class BotStep
	{
		static void AssertStringContainedInString(string expectedString, string containingString)
		{
			if (!(containingString?.Contains(expectedString) ?? false))
				throw new AssertionException("expected string \"" + expectedString + "\" not contained ín string \"" + containingString + "\"");
		}

		[Test]
		public void Bot_Step_Diagnostic_Local_Chat_Window_Not_Found()
		{
			var stepResult = new Bot.Bot().Step(new Bot.BotStepInput { });

			AssertStringContainedInString(SaveShipTask.LocalChatWindowNotFoundDiagnosticText, stepResult?.RenderBotStepToUIText());
		}

		[Test]
		public void Bot_Step_InfoPanel_CurrentSystem_Enable_Click()
		{
			var infoPanelCurrentSystemEnableButtonMock = new Mock<IUIElement>();

			infoPanelCurrentSystemEnableButtonMock
				.Setup(button => button.Id).Returns(4);

			var memoryMeasurementMock = new Mock<IMemoryMeasurement>();

			memoryMeasurementMock
				.Setup(memoryMeasurement => memoryMeasurement.InfoPanelButtonCurrentSystem).Returns(infoPanelCurrentSystemEnableButtonMock.Object);

			var stepResult = new Bot.Bot().Step(new Bot.BotStepInput
			{
				FromProcessMemoryMeasurement = new FromProcessMeasurement<IMemoryMeasurement>(memoryMeasurementMock.Object, 0, 0)
			});

			Assert.That(stepResult?.ListMotion?.FirstOrDefault()?.MotionParam?.MouseListWaypoint?.FirstOrDefault()?.UIElement == infoPanelCurrentSystemEnableButtonMock.Object);
		}

		[Test]
		public void Bot_Step_Retreat()
		{
			var listSurroundingsButton = new Mock<IUIElement>().Object;

			var infoPanelCurrentSystemMock = new Mock<IInfoPanelSystem>();

			infoPanelCurrentSystemMock
				.Setup(infoPanel => infoPanel.ListSurroundingsButton).Returns(listSurroundingsButton);

			var memoryMeasurementMock = new Mock<IMemoryMeasurement>();

			memoryMeasurementMock
				.Setup(memoryMeasurement => memoryMeasurement.InfoPanelCurrentSystem).Returns(infoPanelCurrentSystemMock.Object);

			var stepResult = new Bot.Bot().Step(new Bot.BotStepInput
			{
				FromProcessMemoryMeasurement = new FromProcessMeasurement<IMemoryMeasurement>(memoryMeasurementMock.Object, 0, 0)
			});

			Assert.That(stepResult?.OutputListTaskPath?.Any(taskPath => taskPath?.OfType<RetreatTask>()?.Any() ?? false) ?? false);
			Assert.That(stepResult?.ListMotion?.FirstOrDefault()?.MotionParam?.MouseListWaypoint?.FirstOrDefault()?.UIElement == listSurroundingsButton);
		}
	}
}
