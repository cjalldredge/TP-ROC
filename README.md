# TP-ROC
## Touch Portal Remote OBS Controller

## About TP-ROC

###### What is TP-ROC?
Touch Portal Remote OBS Controller *(better knonwn as TP-ROC)* is an OBS command relay tool originally designed to receive commands from Touch Poral and send them over a LAN to a server application listening on a different machine hosting OBS.

###### Why was TP-ROC created?
I first began developing TP-ROC as a means better manage two instances of OBS I had running. One instance being present on my main PC and another on my streaming PC *(come check me out on Twitch @ [MrTacoJazz](https://www.twitch.tv/mrtacojazz) ;D)*.

This configuration became rather cumbersome as I needed to have two instances of Touch Portal open, one on each PC and one device to connect to each.

This configuration worked well enough for a while but coordinating browser and game source changes on my main PC separate from 'Just Chatting' and 'Gaming' scenes on my streaming PC led to a very clunky looking experience.

## How does it work?

###### ROCClient
The ROCClient acts, a bit, as the heart of the whole operation. This is what gets packaged up and imported in to Touch Portal as part of the plugin.

On launch, Touch Portal will fire up ROCClient. At this point, ROCClient will begin scanning the subnet for services that are listening at the default TP-ROC port 6567.

ROCClient will wait for a second for any potential ROCServer listening to respond at which point it will deliver a list of the known listening sources to Touch Portal. Touch Portal will then update the sources lists for each command with the list ROCClient provided.

From here, ROCClient waits. And waits... very patiently... until Touch Portal hands it something to process. ROCClient will pass the message through it's message handler and gain context for what the message is supposed to mean.

At the moment *(May 24, 2020)*, the only contexts ROCClient is equipped to handle are list changes and actions.

**List Changes** are handled by reading the contents of the list change message and determining which list changed. If it is determined that a source list changed, ROCClient will read which source the user has selected and ask that source for a list of... well... whatever the user wants to set. 

For example, if a user creates a command to set the scene on a remote OBS client, the user will select a source from the inline drop down such as 192.168.1.4. Touch Portal will pass a message to ROCClient which states that the user has selected 192.168.1.4 from the inline drop down of the set scene command. ROCClient will ask the ROCServer at 192.168.1.4 for a list of it's available scenes. Once it has recieved this list, ROCClient will format and send a listChange message to Touch Portal stating that the scenes list inline drop down should be updated with the included scenes.

**Actions** are comparatively simple. They work by receiving an action request from Touch Portal. This request will contain the destination that the request should go to as well as what exactly should happen. ROCClient will then format a message suited for that request, and send it to the appropriate destination. ROCClient will begin waiting for an acknowledgment back from the intended destination to confirm that the request was well received

###### ROCServer
The ROCServer has much less going on. Upon launching ROCServer, it will begin looking for an OBS websocket to connect to.

If it does not find one, ROCServer will retry continuously until it has established connection to one.

Once connection has been established, ROCServer will start listening at the default port of 6567 for commands.

The ROCServer was intentionally built to be rather ambiguous to the source it is receiving data from. Leaving open the possibility of receiving and processing properly formatted from any client wanting to connect.

However, at the moment *(May 24, 2020)*, the only officially supported source of messages is from ROCClient.

**IMPORTANT NOTE**
ROCServer is heavily dependent upon the obs-websocket-dotnet library. I AM NOT THE DEVELOPER OF THIS FANTASTIC LIBRARY. I am merely a user.

I have included the source for this library in this repository as it is vital to the functionality of the ROCServer. 

Please, direct your support for this library to Palakis and his [Repository](https://github.com/Palakis/obs-websocket-dotnet)

## Wow! That's Awesome! But, how do I use it?

Fear not fellow Touch Portal and OBS enthusiast! Documentation on how to setup both the ROCClient and ROCServer is soon to come! We are still in some of the earliest development stages with some very promising functionality in place.

If you are feeling brave and adventurous, have a try at compiling and setting it up on your own!

However, if you would instead like to give it some time to mature, I will have some very easy to follow install and deployment steps soon to come!

## Social

##### Twitch
Come hang out with me on Twitch, I would love to see you all there and chat with you or answer any questions you might have! [MrTacoJazz](https://www.twitch.tv/mrtacojazz)

##### Discord
I also have a Discord server so come hop in there! From there I can keep you posted on the latest developments of TP-ROC and what to expect moving forward! [Discord](https://discord.gg/TpwgRur)