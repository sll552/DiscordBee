
[![Github All Releases](https://img.shields.io/github/downloads/sll552/DiscordBee/total.svg)](https://github.com/sll552/DiscordBee/releases)
[![AppVeyor](https://img.shields.io/appveyor/ci/sll552/DiscordBee.svg)](https://ci.appveyor.com/project/sll552/discordbee)
[![GitHub license](https://img.shields.io/github/license/sll552/DiscordBee.svg)](https://github.com/sll552/DiscordBee/blob/master/LICENSE)

# DiscordBee
MusicBee plugin that updates your Discord status with the currently playing track

## Installation
Just copy all plugin files into your MusicBee Plugins directory (usually "C:\Program Files (x86)\MusicBee\Plugins").
Make sure the "discord-rpc-w32.dll" is present in the Plugins folder, otherwise the plugin won't load.
### Microsoft Store Version of MusicBee
If you are using the Store version of MusicBee please use the "Add Plugin" button in MusicBee -> Settings -> Plugins and select the latest release .zip. It may display an error message (something like "... initialise Method not found ..."), ignore it and restart MusicBee. The Plugin should be loaded now.

## Usage
You need to add MusicBee to Discord so it recognises it as a running "Game".

This can be done by starting MusicBee and then in Discord -> Settings -> Games 
under the "No Game detected" message press "Add it!" and select MusicBee from the dropdown.

The Discord API has a 15s rate limit, so it can take up to 15s for a status change to actually show in Discord.

## Configuration
![Settings](https://i.imgur.com/42bu4Et.png)

You can configure what is displayed in your profile by opening the plugin settings in MusicBee "Edit -> Preferences -> Plugins -> DiscordBee -> Configure".

The settings window is designed after the Discord profile view and has all elements editable with the default values preloaded. You can use all metadata field that MusicBee provides in your custom strings. All valid metadata fields in square brackets will be replaced by the values of the currently playing song.

To see which fields are available press the "Placeholders" button and a window will open containing a table with all fields and their values for the current song.

You can also change which characters are treated as seperators. Seperator characters will be stripped in certain conditions e.g. a field is empty and the seperator would be at the end.

If you are unhappy with your changes, you can always restore the defaults and save again.

## Screenshots
![Small presence](https://i.imgur.com/DUuVlsg.png)
![Profile](https://i.imgur.com/vnBq4rp.png)

## Contributing
Feel free to send pull requests or open issues. The project is a Visual Studio 2017 Solution. 

## Libraries and Assets used
 - [Discord RPC Sharp](https://github.com/Lachee/discord-rpc-csharp)
 - [Icons](https://www.iconfinder.com/iconsets/media-player-long-shadow)
 - [MusicBee Logo](https://github.com/Avik-B/musicbee-website/blob/master/img/mb_icon_touch.png)
