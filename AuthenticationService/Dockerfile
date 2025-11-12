# -------- BUILD STAGE --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore
COPY AuthenticationService/AuthenticationService.csproj AuthenticationService/

RUN dotnet restore AuthenticationService/AuthenticationService.csproj

# copy all source and build
COPY . .
WORKDIR /src/AuthenticationService
RUN dotnet publish -c Release -o /app/publish

# -------- RUNTIME STAGE --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80

# Environment configuration
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "AuthenticationService.dll"]
