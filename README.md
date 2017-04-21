# ADEventService

### What is it?

ADEventService is basically a Windows service (a collection of services) which makes it possible for 3rd party applications to be notified whenever a change occur in Microsoft Active Directory. A change may be a creation or deletion of a user, modifying a user’s cell phone number, adding membership to a security group, etc. And that's about it - ADEventService doesn’t do much more!

Well, is not quite true. ADEventService was built to be the central hub for notifying any app of changes in AD and thereby making it an important part of a AD centric IDM concept. It does that by:

- provide ***reliable*** provisioning in real time of AD objects such as users, groups, etc., with the use of EDA/push architecture patterns.
- allow multiple subscribes to subscribe to events though easy configuration settings.
- make integration of 3rd party subscribers easy. Everything a 3rd party adapter must do, is to expose a simple REST endpoint, accepting POSTs of json formatted AD events.

ADEventService is described in more detail in this document [ADEventService-intro-v4](https://github.com/Gentofte/ADEventService/blob/master/Docs/ADEventService-intro-v4.pdf "https://github.com/Gentofte/ADEventService/blob/master/Docs/ADEventService-intro-v4.pdf") (currently written in Danish only - sorry for that).

More docs, diagrams, drawings etc. will be supplied in the future. For now, you may have a look at this somewhat outdated drawing...

![ADEventService-konceptual-01](http://i.imgur.com/nGvPeMA.jpg)

### Project

The project consists of 2 + 2 VS solutions/projects; ADEventSatellite (ADESATL), ADEventService (ADESVC) and a two sample ADx adapter. A short explanation follows:

1. One or more domain joined servers run an instance of ADEventSatellite. ADEventSatellite listen to events directly from AD and store events in a local queue whenever they occur. Whenever possible (i.e. the satellite is connected OK to eventservice) events from the local queue is transmitted (downstream) to ADEventService.
1. ADEventService is (typically) installed on a separate server. ADEventService receives events from satellites and store them in a local queue and makes sure that the same event from multiple satellites is only stored once in ADEventService.
1. Now, one or more 3rd party adapters subscribe to events from ADEventService. Subscription is done by registering the adapter in the ADEventService SubscriptionsConfig-xxx (XML) configuration file.
1. Adapters (ADx) is installed on (possible) other servers. The project provides two sample adapters called ADxSimpleAdapterWin and ADEAdapterWin. The former is as lightweight adapter, whereas the latter also do local persistence of events before processing them. Both is implemented as classic Windows form apps, so it's possible to see what is going on - in contrast to ADEventSatellite and ADEventServices GUI free Windows services. OWIN is used to enable REST based endpoints in all services/components.

ADEventSatellite, ADEventService and ADxSimpleAdapterWin is supplied as setup.exe installers, so you can get up to speed with building your own adapters or just see how the concept works. ADxAdapter is supplied as source code only which can be compiled and run from VS (2015).

### How do I get started?

To get you started, just install everything on a single (domain joined) PC (Windows 10 is recommended) with VS 2015 installed. Do the following steps ...

1. Install the components described below (.NET, OTP, RabbitMQ, etc.). Just follow default instructions on the components websites. Remember to install RabbitMQ managament plugin, as it allows for easy monitoring of queues. **Note! If you skip this part - none of the services will run**
1. Download and run **ADEventSatellite-Setup.exe** from the project [release](https://github.com/Gentofte/ADEventService/releases) tab 
1. Likewise, download and run **ADEventService-Setup.exe**
1. Also, download and run (the sample adapter) **ADxSimpleAdapterWin-Setup.exe** 
1. All services will (by convention) be installed in the **C:\NTSYS\bin** folder. If you like, you can change this setting during installation.
1. Execute (as admin) the **enable-localhost-acls.bat** file found in the project \Deploy folder. Due to Windows security settings, you must enable REST API calls from one service to another, if they are supposed to run on the same machine (localhost). The batch file take care of this using ADx standard port settings.
1. Now start ADxSimpleAdapterWin.exe found in C:\NTVOL1\bin\ADxSimpleAdapterWin.
1. In windows service manager, start ADEventSatellite and ADEventService services
1. Now, find a user in AD - and change the phone number or another attribute. The event should now pop up in the ADxSimpleAdapterWin console.

### Deployment

ADEventService is at present build for the Windows OS. ADEventService relies of some great 3rd party tools, such as the RabbitMQ messaging service ([https://www.rabbitmq.com/](https://www.rabbitmq.com/ "https://www.rabbitmq.com/")) and the windows implementation of the Redis key-value store ([https://github.com/MSOpenTech/redis](https://github.com/MSOpenTech/redis "https://github.com/MSOpenTech/redis")).

To sum up, the following prerequisites is needed to run ADEventServices:

1. Windows server 2008R2+ or Window7+
1. .NET framework 4.6
1. Erlang OTP runtime (download from [https://www.erlang.org/](https://www.erlang.org/ "https://www.erlang.org/"))
1. RabbitMQ - (download from [https://www.rabbitmq.com/](https://www.rabbitmq.com/ "https://www.rabbitmq.com/")) 
1. Redis for windows (download from [https://github.com/MSOpenTech/redis/releases](https://github.com/MSOpenTech/redis/releases "https://github.com/MSOpenTech/redis/releases"))

### Development of adapters

For a rather rough introduction of building ADESVC adapters, check out the document Short-intro-to-adapter-dev.docx located in the Docs folder.

### License

ADEventService is licensed under the GPLv3 license.

----------

Revision:

- Sample adapter added, bugfixes and enhancements, 20170414/SDE
- First draft, 20170124/SDE
