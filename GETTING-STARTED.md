# Getting Started
The following steps describe how to set up the project locally.

## Set up Docker containers locally
You can create a docker compose file to run the required services locally.
The following example should be helpful to start.

```
services:
  apg-postgres:
    image: postgres:13.4
    container_name: apg-postgres
    restart: on-failure
    environment:
      POSTGRES_DB: apg
      POSTGRES_USER: your_username_here
      POSTGRES_PASSWORD: your_password_here
      PGDATA: /var/lib/postgresql/data
    volumes:
      - apg-db-data:/var/lib/postgresql/data
    ports:
      - "5442:5432"
    networks:
      - apg-net

  apg-pgadmin:
    image: dpage/pgadmin4:5.6
    container_name: apg-pgadmin
    restart: on-failure
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@admin.com
      PGADMIN_DEFAULT_PASSWORD: your_default_password_here
      PGADMIN_LISTEN_PORT: 80
    ports:
      - "9101:80"
    volumes:
      - apg-pgadmin-data:/var/lib/pgadmin
    networks:
      - apg-net

  apg-mailhog:
    image: mailhog/mailhog:latest
    container_name: apg-mailhog
    restart: on-failure
    ports:
      - "1028:1028"
      - "8038:8025"
    networks:
      - apg-net

  apg-kcpostgres:
    image: postgres:17
    container_name: apg-kcpostgres
    restart: on-failure
    volumes:
      - apg-keycloak-data:/var/lib/postgresql/data
    environment:
      POSTGRES_DB: keycloak
      POSTGRES_USER: your_username_here
      POSTGRES_PASSWORD: your_password_here
    networks:
      - apg-net

  apg-keycloak:
    image: keycloak/keycloak:24.0.1
    container_name: apg-keycloak
    restart: on-failure
    volumes:
      - ./keycloak-realms:/opt/keycloak/data/import
    environment:
      KEYCLOAK_ADMIN: administrator
      KEYCLOAK_ADMIN_PASSWORD: administrator
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://apg-kcpostgres:5432/keycloak
      KC_DB_USERNAME: your_username_here
      KC_DB_PASSWORD: your_password_here
    ports:
      - "9100:8080"
    depends_on:
      - apg-kcpostgres
    networks:
      - apg-net
    command: start-dev --import-realm

  apg-minio:
    container_name: apg-minio
    restart: on-failure
    image: minio/minio:latest
    ports:
      - "9010:9000"
      - "9011:9001"
    command: server /data --console-address ":9001"
    volumes:
      - apg-minio-data:/data
    networks:
      - apg-net

networks:
  apg-net:
    driver: bridge

volumes:
  apg-db-data:
  apg-pgadmin-data:
  apg-keycloak-data:
    driver: local
  apg-minio-data:
```

This docker file contains a reference to a reference to a KeyCloak real export file which will be imported when KeyCloak is initialized.
An example export can be found here: https://github.com/swiss/extra-parliamentary-bodies-management/blob/main/etc/apg-docker/keycloak-realms/bk-apg-realm.json


## Add user secrets

You can add user secrets (see: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to store the credentials to the local services in a secure way.

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
  "Post": {
    "Username": "your_post_user",
    "Password": "your_post_password"
  },
  "DocumentService": {
    "ClientId": "your_document_service_client_id",
    "ClientSecret": "your_document_service_secret",
    "Url": "your_document_service_url",
    "TokenUrl": "your_document_service_token_url"
  },
  "Frontend": {
    "FroalaKey": "your_froala_key",
    "OpenDataStack": {
      "BaseUrl": "your_opendata_stack_base_url",
    }
  },
  "OgdS3": {
    "access_key": "your_ogds3_access_key",
    "secret_access_key": "your_ogds3_secret_access_key",
    "s3_endpoint": "your_ogds3_endpoint",
    "BaseUrl": "your_ogds3_base_url"
  },
  "S3": {
    "access_key": "minioadmin",
    "secret_access_key": "minioadmin"
  },
  "OpenDataStack": {
    "BaseUrl": "your_opendata_stack_base_url"
  },
  "Sparql": {
    "MasterDataProxy": {
      "Address": "your_proxy_address"
    }
  },
  "SparqlTargets": {
    "Targets": {
      "LindasNext": {
        "Username": "your_lindas_next_user",
        "Password": "your_lindas_next_password",
        "Proxy": {
          "Address": "your_proxy_address"
        }
      }
    }
  },
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
