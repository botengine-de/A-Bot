using Bib3;
using Sanderling.ABot.Bot;
using Sanderling.ABot.Bot.Task;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanderling.ABot.UI
{
	static public class Render
	{
		static IEnumerable<string> RenderBotStepToUITextComponentException(this Exception exception) =>
			null == exception ? null :
			new[]
			{
				"---- Exception ----",
				exception.ToString(),
			};

		static public string RenderBotLeafTaskTypeToString(IBotTask leafTask) =>
			new[]
			{
				leafTask?.Motion == null ? null: "Motion",
				leafTask is DiagnosticTask ? "Diagnostic: \"" + (leafTask as DiagnosticTask)?.MessageText + "\"" : null,
			}.WhereNotDefault().FirstOrDefault();

		static public string RenderTaskPathToUIText(IBotTask[] taskPath) =>
			taskPath.IsNullOrEmpty() ? null :
			RenderBotLeafTaskTypeToString(taskPath?.LastOrDefault()) +
			"(" + string.Join("->", taskPath.Select(taskPathNode => taskPathNode?.GetType()?.Name)) + ")";

		static public string RenderBotStepToUIText(this BotStepResult stepResult) =>
			string.Join(Environment.NewLine, new[]
			{
				stepResult?.Exception?.RenderBotStepToUITextComponentException(),
				new [] { "" },
				stepResult?.OutputListTaskPath?.Select(RenderTaskPathToUIText),
			}.ConcatNullable());

		static public string TimeAgeMilliToUIText(this Int64? ageMilli) =>
			!ageMilli.HasValue ? null :
			(ageMilli / 1000) + " s ago at " + (DateTime.Now - TimeSpan.FromMilliseconds(ageMilli.Value)).ToLongTimeString();

		static public string RenderBotStepToUIText(this PropertyGenTimespanInt64<BotStepResult> stepResultAtTimeMilli) =>
			null == stepResultAtTimeMilli ? null :
			TimeAgeMilliToUIText(Bot.Bot.GetTimeMilli() - stepResultAtTimeMilli?.Begin) +
			Environment.NewLine +
			RenderBotStepToUIText(stepResultAtTimeMilli.Value);
	}
}
