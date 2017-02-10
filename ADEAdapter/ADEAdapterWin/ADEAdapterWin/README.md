# ADEventService

### What is it?

ADEventService is basically a Windows service (a collection of services) witch makes it possible for 3. party applications to be notified whenever a change occur i Active Directory. A change may be a creation or deletion of a user, modifying a users cell phone number, adding membership to a security group, etc. And that's about it - ADEventService dosen't do much more!

Well, is not quite true. Actually ADEventService was constructed to be the central hub for notifying any app of changes in AD and therebye making it a important part of a AD centric IDM koncept. It does that by:

- provide ***reliable*** provisionering in real time of AD objects such as users, groups, etc., with the use of EDA/push arcitecture patterns
- allow multiple subscribes to subscribe to events though easy configuration tools
- make integration of 3. party subscribers easy. Everything a 3. party adapter has to do, is to expose a simple REST endpoint, accepting POSTs of AD events (in the form of json formatted data transfer objects).

ADEventService is described in more detail in this document [ADEventService-intro-v4](https://github.com/Gentofte/ADEventService/blob/master/Docs/ADEventService-intro-v4.pdf "https://github.com/Gentofte/ADEventService/blob/master/Docs/ADEventService-intro-v4.pdf") (currently written in danish - sorry).

Also - some kind of drawing, diagrams etc. will be supplied in near future. For now you may have a look at this outdated drawing...

![ADEventService-konceptual-01](http://i.imgur.com/nGvPeMA.jpg)

### Project

The project consists of 2 + 1 VS solutions/projects; ADEventSatellite (ADESATL), ADEventService (ADESVC) and a sample ADE adapter. A short explanation follows:

1. One or more domian joined servers run an instance of ADEventSatellite. This service listen directly to event from AD and store relevant events in a local queue. Whenever possible, events from the local queue is transmitted (downstream) to ADEventService.
1. ADEventService is (typically) installed on a separate domain joined server. ADEventService receives events from satellites and store them in a queue, making sure that the same event from multiple satellites is only stored once in ADEventService.
1. Now, one or more 3. party adapters subscribe to events from ADEventService. Subscription is done by registering the adapter in the ADEventService SubscriptionsConfig-xxx (XML) configuration file.
1. Adapters is installed on (possible) other servers. The project provides a sample adapter, called ADEAdapter, which can be used for testing or as basis for developemnt of 3. party adapter.

ADEventSatellite and ADEventService is supplied as source code and setup.exe installeres. ADEAdapter is supplied as source code only, which can be compiled and run from VS (2015).

### How do I get started?

To get you started, just install everything on a single (domain joined) PC (Windows 10 is recommended) with VS 2015 installed. Do the following steps ...

1. Install the components described below (.NET, OTP, RabbitMQ, etc.). Just follow default instructions on the components websites. Remember to install RabbitMQ managament plugin, as it allows for easy monitoring of queues.
1. Download and run **ADEventSatellite-Setup.exe** from the project [release](https://github.com/Gentofte/ADEventService/releases) tab 
1. Likewise, download and run **ADEventService-Setup.exe**
1. Both services will (by convention) be installed in the C:\NTSYS folder. If you like, you can change this setting during install.
1. In windows service manager, start ADEventService and stop the service again.
1. Download sourcefiles
1. Execute (as admin) the **enable-localhost-acls.bat** file found in the project \Deploy folder.
1. Copy-paste the XML content of the file **ADAdapter-config-stuff.txt** found in the project \Deploy folder into the SubscriptionsConfig-<guid> file found in the \Config folder where ADEventService is installed.
1. Open, compile and **run** the ADEAdapter\ADEAdapterWin solution (found in the project folder)
1. In windows service manager, start ADEventSatellite and ADEventService services
1. Now, find a user in AD - and change the phone number or another attribute. The event should now pop up in the ADEAdapterWin console.

### Deployment

ADEventService is at present build for the Windows OS. Also ADEventService relies of some great 3. party tools, such as the RabbitMQ messaging service ([https://www.rabbitmq.com/](https://www.rabbitmq.com/ "https://www.rabbitmq.com/")) and the windows implementation of the Redis key-value store ([https://github.com/MSOpenTech/redis](https://github.com/MSOpenTech/redis "https://github.com/MSOpenTech/redis")).

To sum up, the following prerequisites is needed to run ADEventServices:

1. Windows server 2008R2+ or Window7+
1. .NET framework 4.6
1. Erlang OTP runtime (download from [https://www.erlang.org/](https://www.erlang.org/ "https://www.erlang.org/"))
1. RabbitMQ - (download from [https://www.rabbitmq.com/](https://www.rabbitmq.com/ "https://www.rabbitmq.com/")) 
1. Redis for windows (download from [https://github.com/MSOpenTech/redis/releases](https://github.com/MSOpenTech/redis/releases "https://github.com/MSOpenTech/redis/releases"))

### License

ADEventService is licensed under GPLv3 license terms

----------

Revision:

First draft, 20170124/SDE
