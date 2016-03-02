# Building Heist Game & Server
by mike tsouris



## Quick Setup:


You need these tools to get started: [all free!!!]

- Unity 3D Game Engine (v5+)
- Visual Studio 2015 Community Edition
- SQL Server 2015 Express
- Redis ( Just need the binaries, easy to get from NuGet )
- MongoDB
- IIS


	
### Server Environment:

1. Set up SQL Server Database
	 - assuming local SQL instance is available through localhost
	 - Run the script located in  \Setup\Database\schema_MSSQL.sql
	 - that should be it!
	 
2. Set up local Redis Service to be available through localhost
	 - Google it
	 
3. Set up local MongoDB service to be available through localhost
	 - Google it
	 
4. Build the server project
	 - The solution should build just fine. If you have problems, make sure that NuGet downloaded all the needed libraries
	 
5. Configure IIS to serve the project directory through "localhost" 
	 - AppPool : integrated / 4.0
	 - Port : 80

	 
### Game Environment:

1. Install Unity 3D 

2. *The Hosts File* : There is a simple configuration file that is NOT already in place. You need to copy the file from 
		"\Setup\Sample Hosts File\hosts.txt"
		and put it in:
			"\Game\Assets\Resources\hosts.txt"		
	
	If you use a host name other than "localhost", you can update the hosts file for the environment you want. 
	
3. Open project with Unity3D
	
4. Run the game!
	
	





