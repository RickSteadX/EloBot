# EloBot

## Prerequisites

1. **Install .NET 8**
   - Download the .NET 8 SDK from the [official .NET website](https://dotnet.microsoft.com/download/dotnet/8.0).
   - Follow the installation instructions based on your operating system (Windows, macOS, or Linux).

## Setup Instructions

### 1. Clone the Repository

```
git clone https://github.com/RickSteadX/EloBot
```

Optionally you can download ZIP file or clone using Visual Studio

### 2. Build the Project

Open the folder containing .csproj file
Run the following command to build the project:

```
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true
```
Note: this build command only supports Windows 64-bit
The executable will be located on `{Project}\bin\Release\net8.0\win-x64\publish`

### 3. Update the Database Using EF Core

The database should be ready-to-use, but requires changes in case if you want to change any models.
To modify the database, follow these steps:

- **Add a Migration**:

```
dotnet ef migrations add YourMigrationName
```

- **Update the Database**:

```
dotnet ef database update
```

Make sure to configure your connection string in the `appsettings.json` file as needed before running the migration commands.
Default string is `Data Source=elobot.db`

## Usage

Add the bot token and server ID into `appsettings.json`. The global commands are not supported.
Use /elo to get started.

## License

MIT License

## Contact

Discord: rickstead
