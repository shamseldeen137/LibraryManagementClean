# Build stage for Consumers (worker)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY LibraryManagement.sln ./
COPY src/LibraryManagement.Consumers/LibraryManagement.Consumers.csproj src/LibraryManagement.Consumers/
COPY src/LibraryManagement.Infrastructure/LibraryManagement.Infrastructure.csproj src/LibraryManagement.Infrastructure/

RUN dotnet restore src/LibraryManagement.Consumers/LibraryManagement.Consumers.csproj

COPY . .
RUN dotnet publish src/LibraryManagement.Consumers/LibraryManagement.Consumers.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "LibraryManagement.Consumers.dll"]
