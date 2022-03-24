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

<img src="StreamDeckPluginsDota2/Images/pluginAction.png" width="64">

---

#### Dead
> - **Dead** when the timer is less than 8 minutes.

<img src="StreamDeckPluginsDota2/Images/states/dead0.png" width="64">

- First Death
- Roshan has previously dropped: `Aegis of the Immortal`
<br>

<img src="StreamDeckPluginsDota2/Images/states/dead1.png" width="64">

- Second Death
- Roshan has previously dropped: `Aegis of the Immortal` + `Aghanims Shard` 
<br>

<img src="StreamDeckPluginsDota2/Images/states/dead2.png" width="64">

- Third Death
- Roshan has previously dropped: `Aegis of the Immortal` + `Cheese` + (`Refresher Shard` OR `Aghanims Blessing`)
<br>

<img src="StreamDeckPluginsDota2/Images/states/dead3.png" width="64">

- Fourth Death
- Roshan has previously dropped: `Aegis of the Immortal` + `Cheese` + `Aghanims Blessing` + `Refresher Shard`

---

#### Maybe & Alive
> - **Maybe** when the timer is between 8-11 minutes.
> - **Alive** when the timer is 11 minutes or more.

<img src="StreamDeckPluginsDota2/Images/states/maybe0.png" width="64">
<img src="StreamDeckPluginsDota2/Images/states/alive0.png" width="64">

- First Death
- Roshan is going to drop: `Aegis of the Immortal` + `Aghanims Shard`
  <br>

<img src="StreamDeckPluginsDota2/Images/states/maybe1.png" width="64">
<img src="StreamDeckPluginsDota2/Images/states/alive1.png" width="64">

- Second Death
- Roshan is going to drop: `Aegis of the Immortal` + `Cheese` + (`Refresher Shard` OR `Aghanims Blessing`)
  <br>

<img src="StreamDeckPluginsDota2/Images/states/maybe2.png" width="64">
<img src="StreamDeckPluginsDota2/Images/states/alive2.png" width="64">

- Third Death
- Roshan is going to drop: `Aegis of the Immortal` + `Cheese` + `Aghanims Blessing` + `Refresher Shard`
  <br>

<img src="StreamDeckPluginsDota2/Images/states/maybe3.png" width="64">
<img src="StreamDeckPluginsDota2/Images/states/alive3.png" width="64">

- Fourth Death
- Roshan is going to drop: `Aegis of the Immortal` + `Cheese` + `Aghanims Blessing` + `Refresher Shard`

# Downloads
**IMPORTANT NOTE: Links coming soon!**
- Elgato Plugin Store (Recommended)
- [Direct Download](StreamDeckPluginsDota2/distribute/com.adrian-miasik.sdpdota2.streamDeckPlugin)

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
