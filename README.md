# Blade & Soul ACT plugin
This is a beta version of an Advanced Combat Tracker plugin for the MMORPG Blade and Soul.  

Warning: This software operates by connecting to the game's memory and reading the combat log data.  This action is likely not permitted by the terms of service.  It is unknown at this time whether there is a risk of losing account access from using this software, so please use caution when discussing it in-game, and discontinue use if informed that it is not permitted.

## Installation steps:

1) Download and unzip the BNS_ACT_Plugin zip file from the most recent release:
https://github.com/ravahn/BnS_ACT_Plugin/releases

  Alternately, you can also download the source code file fromhere and use that instead: 
https://github.com/ravahn/BnS_ACT_Plugin/blob/master/BnS_ACT_Plugin/BNS_ACT_Plugin.cs

2) Run ACT as an administrator - this is required to access BnS's game memory

3) Install either the .dll or .cs file as a new ACT plugin on the Plugins tab

4) Run BnS and make sure that the default Combat tab exists, and that all combat actions appear there.

## Limitations / Notes:
* This is a beta version - it performs reasonably well, but is missing features such as buff/debuff tracking, evades, blocks, etc.
* Only data on the combat tab (#2) is parsed right now.
* Zone Names are not parsed, so everything is combined into a single 'Blade and Soul' zone.
* The B&S log data sometimes omits the name of the player causing damage.  This will instead be attributed to a combatant with the name Unknown.  When evaluating parses, always check to make sure damage was not lost here.

## Acknowledgements:
Thanks to Shaid for finding and decoding the chat log structure

Thanks to Aditu, author of Advanced Combat Tracker, for creating such a versatile program.
