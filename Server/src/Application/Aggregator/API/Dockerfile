# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Application/Aggregator/API/API.csproj", "src/Application/Aggregator/API/"]
COPY ["src/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/"]
COPY ["src/Modules/Students/Modules.Students.Persistence/Modules.Students.Persistence.csproj", "src/Modules/Students/Modules.Students.Persistence/"]
COPY ["src/Modules/Students/Modules.Students.Application/Modules.Students.Application.csproj", "src/Modules/Students/Modules.Students.Application/"]
COPY ["src/Modules/Wokrs/Modules.Works.IntegrationEvents/Modules.Works.IntegrationEvents.csproj", "src/Modules/Wokrs/Modules.Works.IntegrationEvents/"]
COPY ["src/Modules/Students/Modules.Students.Domain/Modules.Students.Domain.csproj", "src/Modules/Students/Modules.Students.Domain/"]
COPY ["src/Modules/Students/Modules.Students.IntegrationEvents/Modules.Students.IntegrationEvents.csproj", "src/Modules/Students/Modules.Students.IntegrationEvents/"]
COPY ["src/Modules/Students/Modules.Students.Ifrastructure/Modules.Students.Infrastructure.csproj", "src/Modules/Students/Modules.Students.Ifrastructure/"]
COPY ["src/Modules/Wokrs/Modules.Works.Persistence/Modules.Works.Persistence.csproj", "src/Modules/Wokrs/Modules.Works.Persistence/"]
COPY ["src/Modules/Wokrs/Modules.Works.Application/Modules.Works.Application.csproj", "src/Modules/Wokrs/Modules.Works.Application/"]
COPY ["src/Modules/Wokrs/Modules.Works.Domain/Modules.Works.Domain.csproj", "src/Modules/Wokrs/Modules.Works.Domain/"]
COPY ["src/Modules/Wokrs/Modules.Works.Infrastructure/Modules.Works.Infrastructure.csproj", "src/Modules/Wokrs/Modules.Works.Infrastructure/"]
RUN dotnet restore "./src/Application/Aggregator/API/API.csproj"
COPY . .
WORKDIR "/src/src/Application/Aggregator/API"
RUN dotnet build "./API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]