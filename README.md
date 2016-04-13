# Blade & Soul ACT plugin
This is a beta version of an Advanced Combat Tracker plugin for the MMORPG Blade and Soul.  

### NOTE: This project is no longer actively maintained.  Please contact me at Ravahn (at) hotmail.com if you are capable and interested in maintaining it.

Warning: This software operates by connecting to the game's memory and reading the combat log data.  This action is likely not permitted by the terms of service.  It is unknown at this time whether there is a risk of losing account access from using this software, so please use caution when discussing it in-game, and discontinue use if informed that it is not permitted.

## Installation steps:

1) Download the source code file and save it in a folder.  The file is located  here: 

https://raw.githubusercontent.com/ravahn/BnS_ACT_Plugin/master/BnS_ACT_Plugin/BNS_ACT_Plugin.cs

2) Run ACT as an administrator - this is required to access BnS's game memory

3) Go to the Plugins tab and click the Browse button.  Locate the downloaded file and click OK.  Then, click the Add/Enable Plugin button.

4) Run BnS and make sure that the default Combat tab exists, and that all combat text appears there.

## Limitations / Notes:
* This is a beta version - it performs reasonably well, but is missing features such as buff/debuff tracking, evades, blocks, etc.
* Only data on the combat tab (#2) is parsed right now.
* Zone Names are not parsed, so everything is combined into a single 'Blade and Soul' zone.
* The B&S log data sometimes omits the name of the player causing damage.  This will instead be attributed to a combatant with the name Unknown.  When evaluating parses, always check to make sure damage was not lost here.
* The DLL version of the BnS plugin will break with an upcoming ACT update, so please use the source code version only.

## Acknowledgements:
Thanks to Shaid for finding and decoding the chat log structure

Thanks to Aditu, author of Advanced Combat Tracker, for creating such a versatile program.
