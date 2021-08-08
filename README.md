# Unturned-AutomatedPermissions
This is a basic server plugin for the game **Unturned**

# Info
This plugin automatically adds a certain group to the player once a played more than a certain amount of time (default 900 seconds -> 15 minutes)
Currently at its first version and maintained well.

# Usage
Build the Project using the Solution File
Copy the built dll into unturnedServer/Rocket/plugins
Start the server
Now a directory of the plugin is created  in which you can alter the XML-serialized configuration file where you can change the time interval.
``<intervalUntilPromotion>900</intervalUntilPromotion>``

# License
**MIT**

# ToDo
- Add configuration field which allows you to change the group to be added
- Be able to change multiple groups over time
- more configuration opportunities
