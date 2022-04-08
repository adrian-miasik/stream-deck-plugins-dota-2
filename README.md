<p align="center">
  <img src="StreamDeckPluginsDota2/Images/icon@2x.png" width="40">
</p>
<h1 align="center">Stream Deck Plugins - Dota 2</h1>
<p align="center">A suite of Stream Deck plugins created for Dota 2 ⚔️</p>

# Actions
<h2 align="center">Roshan Timer</h2>
<p align="center">
  <img src="StreamDeckPluginsDota2/Images/pluginIcon@2x.png" width="128">
</p>
<p align="center">Keep track of Roshan's respawn time and item drops.</p>

<h2 align="center">Show Top Rune</h2>
<p align="center">
  <img src="StreamDeckPluginsDota2/Images/show-top-rune/pluginAction@2x.png" width="128">
</p>
<p align="center">Quickly position the in-game camera to the top rune.</p>

<h2 align="center">Show Bot Rune</h2>
<p align="center">
  <img src="StreamDeckPluginsDota2/Images/show-bot-rune/pluginAction@2x.png" width="128">
</p>
<p align="center">Quickly position the in-game camera to the bottom rune.</p>

# Installation
1. Install `com.adrian-miasik.sdpdota2.streamDeckPlugin` to your Stream Deck. 
    - Make sure to have the Elgato Stream Deck software installed. 
    - Simple double-clicking this file on Windows will prompt an install.
2. Add the following commands to your Dota 2 launch options: 
    - `-gamestateintegration +exec stream_deck_plugins_dota_2.cfg`

**Important:** Make sure you have nothing currently bound to F13 or 14. If you do, please edit the `stream_deck_plugins_dota_2.cfg` to use unassigned keys.

# Launch Options Explained
- The `-gamestateintegration` line is required to enable this plugin to read the contents of the Dota game state as of [2022 March 11th.](https://dota2.fandom.com/wiki/March_11,_2022_Patch)
- The `+exec stream_deck_plugins_dota_2.cfg` line is required to bind our rune cameras to certain function keys. Please ensure you have no keybindings on F13 or F14.

# Actions Explained
<h2 align="center">Roshan Timer</h2>
<p align="center">
  <img src="StreamDeckPluginsDota2/Images/pluginIcon@2x.png" width="128">
</p>
<p align="center"> Keep track of Roshan's respawn time and item drops.</p>

### How to Use
| Action       | Result                                                      |
|--------------|-------------------------------------------------------------|
| Single Press | Start / Pause / Resume timer                                |
| Long Press   | Restart timer                                               |
| Double Press | Increase Roshan death count (Do this everytime Roshan dies) |

### Timer States
<img src="StreamDeckPluginsDota2/design/roshan-timer-table.png" width="720px">

#### Default
> - Timer has not been started. **Press on Roshan's first death to begin the timer.**

<img src="StreamDeckPluginsDota2/Images/roshan-timer/pluginAction.png" width="64">

---

#### Dead
> - **Dead** when the timer is less than 8 minutes.

<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/dead0.png" width="64">

- First Death
- Roshan has previously dropped: `Aegis of the Immortal`
<br>

<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/dead1.png" width="64">

- Second Death
- Roshan has previously dropped: `Aegis of the Immortal` + `Aghanims Shard` 
<br>

<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/dead2.png" width="64">

- Third Death
- Roshan has previously dropped: `Aegis of the Immortal` + `Cheese` + (`Refresher Shard` OR `Aghanims Blessing`)
<br>

<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/dead3.png" width="64">

- Fourth Death
- Roshan has previously dropped: `Aegis of the Immortal` + `Cheese` + `Aghanims Blessing` + `Refresher Shard`

---

#### Maybe & Alive
> - **Maybe** when the timer is between 8-11 minutes.
> - **Alive** when the timer is 11 minutes or more.

<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/maybe0.png" width="64">
<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/alive0.png" width="64">

- First Death
- Roshan is going to drop: `Aegis of the Immortal` + `Aghanims Shard`
  <br>

<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/maybe1.png" width="64">
<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/alive1.png" width="64">

- Second Death
- Roshan is going to drop: `Aegis of the Immortal` + `Cheese` + (`Refresher Shard` OR `Aghanims Blessing`)
  <br>

<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/maybe2.png" width="64">
<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/alive2.png" width="64">

- Third Death
- Roshan is going to drop: `Aegis of the Immortal` + `Cheese` + `Aghanims Blessing` + `Refresher Shard`
  <br>

<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/maybe3.png" width="64">
<img src="StreamDeckPluginsDota2/Images/roshan-timer/states/alive3.png" width="64">

- Fourth Death
- Roshan is going to drop: `Aegis of the Immortal` + `Cheese` + `Aghanims Blessing` + `Refresher Shard`

# Downloads
**IMPORTANT NOTE: Links coming soon!**
- Elgato Plugin Store (Recommended)
- [Direct Download](StreamDeckPluginsDota2/distribute/com.adrian-miasik.sdpdota2.streamDeckPlugin)

# Author Notes
- If you're looking to build this yourself, make sure to download [.NET Framework 4.7.2 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) and make sure to restore/update your NuGet packages as necessary.

# Contact Us / Support Line
- For inquires related to this specific plugin / repository: `roshan-timer@adrian-miasik.com`
- For inquries related to any of my stream deck plugins: `stream-deck-plugins@adrian-miasik.com`

# Third Party
- Created with BarRaider's [streamdeck-tools](https://github.com/BarRaider/streamdeck-tools) SDK</p>

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
