# A-Bot

A-Bot is an EVE Online anomaly ratting bot, based on the [Sanderling framework](https://forum.botengine.org/tags/sanderling).

## Features

* **safe**: does not inject into or write to the eve online client. That is why using it with eve online is not detectable.
* **accurate & robust**: uses memory reading to retrieve information about the game state. In contrast to screen scraping, this approach won't be thrown off by a noisy background or non-default UI settings.
* **easy to use**: automatically detects your fitting and comes with reasonable default settings. Thus you can start using it without any setup.
* **monitors local and saves your ship when neutrals or hostiles show up.**


## Get started

Follow these steps to use the bot:

+ Make sure that Microsoft .NET Framework 4.6.1 or newer is installed on your system. This can be downloaded from https://www.microsoft.com/download/details.aspx?id=49982.
+ download the `A-Bot.exe` file of the latest release from the releases section.
+ set the language of the eve online client to english.
+ start an eve online client and login to the game.
+ start the `A-Bot.exe`
+ undock, open probe scanner, overview window and drones window.
+ in the EVE Online client, configure the ship UI as follows
  + enable `Display Module Tooltips`
  + disable `Display Passive Modules`
  + ![](./doc/image/eve.shipui.options.png)
+ wait for the green checkmark to appear in the `Interface` tab header as shown in this screenshot: ![](./doc/image/bot.start.png) (this can take up to 30 seconds)
+ press the button `play` to start the bot.
+ profit.

## Development

If you want to make changes to the bot, these guides on EVE Online bot development might be helpful:
+ An easy way to explore the Sanderling framework, test memory reading and sending input to the game: [A Simpler Way to Make EVE Online Bots](https://forum.botengine.org/t/a-simpler-way-to-make-eve-online-bots/2372)
+ The guides mentioned in the [Sanderling Repository](https://github.com/Arcitectus/Sanderling).

To meet other bot developers and discuss development for EVE Online, see the [BotEngine forum](https://forum.botengine.org/tags/eve-online).
