Dialed In
Created by Group 11 - Marc Beitchman, Lauren Cohen, Bojana Duke, & Jim Griffin

=== CONTENTS ===

This Visual Studio solution contains the following projects:

* DialedIn - Defines the application that runs on the phone client.
* NotificationServer - Defines the WCF web service that provides push notifications to the client.
* NotificationSenderUtility - A utility helper library for the NotificationServer project.

=== SETUP ===

Pre-requisites:
- Supported OSes - Windows Vista, Windows Server 2008 R2, Windows 7
- Windows Phone Developer Tools (http://create.msdn.com/en-us/home/getting_started)

* Run the following in a command prompt (as Administrator) in order to allow access to the port used by the NotificationServer.
	netsh http add urlacl url=http://+:8000/RegistratorService/ user=DOMAIN\user
* Open the solution in Visual Studio and press F5 to compile and run.

=== KNOWN ISSUES ==

* NotificationServer may launch partially off the top of the screen. To bring the full UI in to view, expand the bottom to the bottom of the screen.
* Users may experience timing issues (especially on older computers) connecting to the Push Notification service when attempting to launch and debug both NotificationServer and DialedIn at the same time in Visual Studio. Workaround is to publish NotificationServer and run as a standalone application from Visual Studio:
	- Right click on NotificationServer in the Solution Explorer and click "Publish..."
	- Follow the Publishing Wizard
	- Install and run the created NotificationServer application
	- Right click on DialedIn in the Solution Explorer and click "Deploy"