# ADEventService

### What is it?

ADEventService is basically a Windows service (a collection of services) witch makes it possible for 3. party applications to be notified whenever a change occur i Active Directory. A change may be a creation or deletion of a user, modifying a users cell phone number, adding membership to a security group, etc. And that's about it - ADEventService dosen't do much more!

Well, is not quite true. Actually ADEventService was constructed to be the central hub for notifying any app of changes in AD and therebye making it a important part of a AD centric IDM koncept. It does that by:

- provide ***reliable*** provisionering of AD objects such as users, groups, etc., with the use of EDA/push arcitecture pattern
- allow multiple subscribes to subscribe to events though easy configuration tools
- make integration of 3. party subscribers easy.

ADEventService is described in more detail in this document [ADEventService-intro-v4](https://github.com/Gentofte/ADEventService/blob/master/Docs/ADEventService-intro-v4.pdf "https://github.com/Gentofte/ADEventService/blob/master/Docs/ADEventService-intro-v4.pdf") (in danish).

### Project

The project consists of 2 + 1 VS solutions/projects; ADEventSatellite (ADESATL), ADEventService (ADESVC) and a sample ADE adapter. A short explanation follows:

1. One or more domian joined servers run an instance of ADEventSatellite. This service listen directly to event from AD and store relevant events in a local queue. Whenever possible events from the local queue is transmitted (downstream) to ADEventService.
1. ADEventService is (typically) installed on a separate domain joined server. ADEventService receives events from satellites and store them in a queue, making sure that the same event form multiple satellites is only stored once in ADEventService.
1. Now, one or more 3. party adapters subscribe to events from ADEventService. Subscription is done by registering the adapter in the ADEventService SubscriptionsConfig-xxx configuration file.
1. Adapters is installed on (possible) other servers. The project provides a sample adapter, called ADEAdapter, which can be used for testing or as basis for developemnt of 3. party adapter.
1. ADEventSatellite and ADEventService is both supplied as source code and setup.exe installeres. ADEAdapter should be compiled and run from VS (2015). To get you started, just install everything on a single (domain joined) PC (Win 8++ recommended)..

### Deployment

ADEventService is at present build for the Windows OS. Also ADEventService relies of some great 3. party tools, such as the RabbitMQ messaging service ([https://www.rabbitmq.com/](https://www.rabbitmq.com/ "https://www.rabbitmq.com/")) and the windows implementation of the Redis key-value store ([https://github.com/MSOpenTech/redis](https://github.com/MSOpenTech/redis "https://github.com/MSOpenTech/redis")).

To sum up, the following prerequisites is needed to run ADEventServices:

1. Windows server 2008R2+ or Window7+
1. .NET framework 4.6
1. Erlang OTP runtime (download from [https://www.erlang.org/](https://www.erlang.org/ "https://www.erlang.org/"))
1. RabbitMQ - (download from [https://www.rabbitmq.com/](https://www.rabbitmq.com/ "https://www.rabbitmq.com/")) 
1. Redis for windows (download from [https://github.com/MSOpenTech/redis/releases](https://github.com/MSOpenTech/redis/releases "https://github.com/MSOpenTech/redis/releases"))

### Adapter development

To get you started, download the 3 setupfiles found in the .\Deploy folder.

### License

ADEventService is free software licensed under GPLv3 license terms

----------

Revision:

First draft, 20170124/SDE
