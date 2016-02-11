# Blade & Soul ACT plugin
This is a preliminary version of an Advanced Combat Tracker plugin for the game Blade and Soul.  

Warning: This software operates by connecting to the game's memory and reading the combat log data.  This action is likely not permitted by the terms of service.  It is unknown at this time whether there is a risk of losing account access from using this software, so please use caution when discussing it in-game, and discontinue use if informed that it is not permitted.

## Installation steps:

1) Download the primary .cs file from here: 
https://github.com/ravahn/BnS_ACT_Plugin/blob/master/BnS_ACT_Plugin/BNS_ACT_Plugin.cs

2) Run ACT as an administrator - this is required to access BnS's game memory

3) Install the file as a new ACT plugin on the Plugins tab

4) Point ACT to the log files in %AppData%\Advanced Combat Tracker\BnSLogs

## Limitations / Notes:
* This is an early alpha - the regex matches are not there for many things, and only self-parsing works for now.
* Only combat data is parsed for the moment
* Zone Names and Player Names are not parsed yet.

## Acknowledgements:
Thanks to Shaid for finding and decoding the chat log structure

Thanks to Aditu, author of Advanced Combat Tracker, for creating such a versitile program.
