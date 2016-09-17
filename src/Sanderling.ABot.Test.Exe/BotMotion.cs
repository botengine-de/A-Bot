using BotEngine.Interface;
using Moq;
using NUnit.Framework;
using Sanderling.Interface.MemoryStruct;
using System.Linq;

namespace Sanderling.ABot.Test.Exe
{
	public class BotMotion
	{
		[Test]
		public void Motion_InfoPanel_CurrentSystem_Enable()
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
	}
}
