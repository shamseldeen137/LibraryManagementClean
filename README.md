# LibraryManagementClean

Normal .NET Clean Architecture API for library management.

## Stack

- ASP.NET Core Web API
- MongoDB for persistence
- Redis for distributed caching
- RabbitMQ for domain/event publishing
- Kafka as an alternative event publisher
- Strategy pattern for selecting RabbitMQ or Kafka publishing
- JWT authentication with ASP.NET Core Identity password hashing

## Run

```powershell
docker compose up -d
dotnet run --project src/LibraryManagement.Api/LibraryManagement.Api.csproj
```

Swagger: `https://localhost:7045/swagger`

Postman files:

- `postman/LibraryManagementClean.postman_collection.json`
- `postman/LibraryManagementClean.local.postman_environment.json`

Run the message consumers in a separate terminal:

```powershell
dotnet run --project src/LibraryManagement.Consumers/LibraryManagement.Consumers.csproj
```

Register first:

`POST /api/auth/register`

Then login:

`POST /api/auth/login`

Use the returned JWT as a Bearer token for library endpoints.

## Message Publisher Strategy

Set `Messaging:Provider` in `src/LibraryManagement.Api/appsettings.json`:

- `RabbitMq`
- `Kafka`

Both publishers implement the same strategy contract, and the application continues to use `IEventBus`.

The `LibraryManagement.Consumers` worker starts both RabbitMQ and Kafka consumers. RabbitMQ binds the durable
`library.events.consumer` queue with `#`, so every routing key published to the `library.events` topic exchange
triggers the consumer. Kafka subscribes to the `library-events` topic with the `library-management-consumers`
consumer group.
