# .NET Backend

## Setup .env file

Add a .env file under backend/dig.API with the environment variables needed from .env.example

## Running

Run normally through Visual Studio or Rider.

## DB Migrations

Setup

```bash
dotnet ef migrations add <migration_name>
```

Update DB from new entities

```bash
dotnet ef database update
```
