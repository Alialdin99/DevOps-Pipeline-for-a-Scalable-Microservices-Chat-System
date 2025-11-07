# -------- BUILD STAGE --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore
COPY UserService/UserService.csproj UserService/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore UserService/UserService.csproj

# copy all source and build
COPY . .
WORKDIR /src/UserService
RUN dotnet publish -c Release -o /app/publish

# -------- RUNTIME STAGE --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80

# Environment configuration
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "UserService.dll"]
