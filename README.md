# RhenusCodingChallenge

## Set up and start the backend

### Set up external services

The backend requires an instance of SQL Server to run. Instructions for downloading and installing SQL Server can be found at:
https://learn.microsoft.com/en-us/sql/database-engine/install-windows/install-sql-server?view=sql-server-ver16

### Configure the backend

1. Navigate to the `RhenusCodingChallenge.WebApi` folder.
2. Open the `appsettings.json` file in a text editor.
3. Under `ConnectionStrings`, set the connection strings based on the SQL Server instance you are using.
    1. Under `EventStorageDbContext`, enter the connection string to the SQL Server instance.
4. Save the file.


### Start the backend

The backend can be started by opening a terminal in the `RhenusCodingChallenge.WebApi` folder, and then executing:

    dotnet run

The backend will start and run on http://localhost:8080.