# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy csproj and restore (helps cache)
COPY LibraryManagement.sln ./
COPY src/LibraryManagement.Api/LibraryManagement.Api.csproj src/LibraryManagement.Api/
COPY src/LibraryManagement.Application/LibraryManagement.Application.csproj src/LibraryManagement.Application/
COPY src/LibraryManagement.Infrastructure/LibraryManagement.Infrastructure.csproj src/LibraryManagement.Infrastructure/

RUN dotnet restore src/LibraryManagement.Api/LibraryManagement.Api.csproj

# copy everything and publish
COPY . .
RUN dotnet publish src/LibraryManagement.Api/LibraryManagement.Api.csproj -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "LibraryManagement.Api.dll"]
