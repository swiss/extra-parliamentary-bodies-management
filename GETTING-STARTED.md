# Getting Started
The following steps describe how to set up the project locally.

## Set up Docker containers locally
Navigate to ```/etc/docker/``` and execute in a console ```docker-compose up -d```

## Add user secrets
```
{
  "UID" : {
    "Username": "your_uid_user",
    "Password": "your_uid_password"
  },
  "pgsql":{
    "Username": "postgres",
    "Password": "postgres"
  },
  "Sparql": {
    "Username": "your_lindas_user",
    "Password": "your_lindas_password"
  },
  "Post": {
    "Username": "your_post_user",
    "Password": "your_post_password"
  },
  "DocumentService": {
    "ClientId": "your_document_service_client_id",
    "ClientSecret": "your_document_service_secret"
  }
}
```

## Entity Framework
To manipulate and initially create the local database you can use the EF Core tools.<br>
```dotnet tool install --global dotnet-ef```

Create initial database or update to the latest migration.<br>
```dotnet ef database update -s src\Bk.APG.Api -p src\Bk.APG.Infrastructure.DataSource```

Generate new EF migrations using the following command.<br>
```dotnet ef migrations add migration_name_here --context Bk.APG.Infrastructure.DataSource.DataContext --startup-project "../Bk.APG.Api/Bk.APG.Api.csproj"```

## Angular Frontend
Navigate to ```src\Bk.APG.UI```

Install dependencies with ```npm ci```

Start the angular app with ```npm run start```