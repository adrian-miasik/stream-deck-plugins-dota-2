<h1 align="center">Stream Deck Plugins - Dota 2</h1>
<p align="center">
  <img src="StreamDeckPluginsDota2/images/pluginIcon@2x.png" width="64">
</p>

## About
SDP-D2 is a suite of Elgato Stream Deck plugins/actions created for Valve's MOBA: Dota 2 ⚔️. Capable of reading the Dota 2 game state via Valve's Game State Integration (GSI). Streamlining your Dota experience with helpful tools and widgets.


**Platforms**: Windows

**Version**:  1.3.1

**Author**:  **[`Adrian Miasik`](https://adrian-miasik.com)**

**Contributor(s)**: [adrian-miasik](https://github.com/adrian-miasik), [howiieb](https://github.com/howiieb)
 
Want to help? If you're interested in contributing to the project, please feel free to reach out! `sdp-dota-2@adrian-miasik.com`

# Actions
## Actions
| Name               | Description                                             | Icon                                                                                    | States                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           | Wiki                                                                                           | Game State Integration |
| ------------------ | ------------------------------------------------------- | --------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- | ---------------------- |
| Launch/Quit Game   | Launch/Quit the 'Dota 2' application.                   | <img src="StreamDeckPluginsDota2/images/actions/launch-game@2x.png" width="144">        | <img src="StreamDeckPluginsDota2/images/actions/launch-game@2x.png" width="72"> <img src="StreamDeckPluginsDota2/images/actions/quit-game@2x.png" width="72">                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    |                                                                                                |                      |
| Pause/Resume Match | Pause/Resume the currently running Dota 2 match.        | <img src="StreamDeckPluginsDota2/images/actions/pause-resume-match@2x.png" width="144"> | <img src="StreamDeckPluginsDota2/images/actions/pause-match@2x.png" width="72"> <img src="StreamDeckPluginsDota2/images/actions/resume-match@2x.png" width="72">                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |                                                                                                | Yes                    |
| Display Game Clock | Displays the current game clock time.                   | <img src="StreamDeckPluginsDota2/images/actions/display-game-clock@2x.png" width="144"> | Generated and composed based on game state.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |                                                                                                | Yes                    |
| Roshan Timer       | Keep track of Roshan's respawn time and item drops.     | <img src="StreamDeckPluginsDota2/images/actions/roshan-timer@2x.png" width="144">       | <img src="StreamDeckPluginsDota2/images/actions/dead0.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/dead1.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/dead2.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/dead3.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/maybe0.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/alive0.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/maybe1.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/alive1.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/maybe2.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/alive2.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/maybe3.png" width="64"><img src="StreamDeckPluginsDota2/images/actions/alive3.png" width="64"> | [URL](https://github.com/adrian-miasik/stream-deck-plugins-dota-2/wiki/Roshan-Timer-Explained) |                      |
| Show Top Rune      | Quickly position the in-game camera to the top rune.    | <img src="StreamDeckPluginsDota2/images/actions/show-top-rune@2x.png" width="144">      |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |                                                                                                |                      |
| Show Bot Rune      | Quickly position the in-game camera to the bottom rune. | <img src="StreamDeckPluginsDota2/images/actions/show-bot-rune@2x.png" width="144">      |                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |                                                                                                |                      |    

# Downloads
- [Elgato Plugin Store (Recommended)](https://apps.elgato.com/plugins/com.adrian-miasik.sdpdota2)
- [Direct Download](StreamDeckPluginsDota2/distribute/com.adrian-miasik.sdpdota2.streamDeckPlugin)

# Installation
1. Install `com.adrian-miasik.sdpdota2.streamDeckPlugin` to your Stream Deck.
    - Make sure to have the Elgato Stream Deck software installed.
    - Simple double-clicking this file on Windows will prompt an install.
2. Add the following commands to your Dota 2 launch options:
    - `-gamestateintegration +exec stream_deck_plugins_dota_2.cfg`

# Launch Options Explained
- The `-gamestateintegration` line is required to enable this plugin to read the contents of the Dota game state as of [2022 March 11th.](https://dota2.fandom.com/wiki/March_11,_2022_Patch)
- The `+exec stream_deck_plugins_dota_2.cfg` line is required to bind our cameras actions to certain function keys. Please ensure you have no keybindings on F13, F14, F15, F16.

# Author Notes
- If you're looking to build this yourself, make sure to download [.NET Framework 4.7.2 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) and make sure to restore/update your NuGet packages as necessary.
- Distribution Pipeline:
    1. Build
    2. Navigate to the distribute folder
    3. Run the appropriate `.bat` script (Debug / Release). This will take our built solution and package the files using Elgato's Distribution Tool to create a plugin file. The output will create a `.streamDeckPlugin` file in the same directory.

If you have any questions, feel free to reach out. :)

- Playerbase: 1.7k installs on the Elgato Store
- The SVG assets have been created myself and/or sourced from The Noun Project

# Contact Us / Support Line
- For inquires related to this specific plugin / repository: `sdp-dota-2@adrian-miasik.com`
- For inquries related to any of my stream deck plugins: `stream-deck-plugins@adrian-miasik.com`

# Third Party
- Created with BarRaider's [streamdeck-tools](https://github.com/BarRaider/streamdeck-tools) SDK 👍
- Created with antonpup's [Dota 2 Game State Integration](https://github.com/antonpup/Dota2GSI) interface 👍
- Utilizes michaelnoonan's [inputsimulator](https://github.com/michaelnoonan/inputsimulator) package 👍

# Legal
Copyrights and trademarks are the property of their respective owners.
- Adrian Miasik (Logo)
- Dota 2 (Logo)
- Roshan Spell Block (Skill Art)
- Aegis of the Immortal (Item Art)
- Cheese (Item Art)
- Aghanim's Shard (Item Art)
- Aghanim's Blessing (Item Art)
- Refresher Shard (Item Art)
- Water Rune (Gameplay Update Art)
- Nightstalker: Dark Ascension (Skill Art)
