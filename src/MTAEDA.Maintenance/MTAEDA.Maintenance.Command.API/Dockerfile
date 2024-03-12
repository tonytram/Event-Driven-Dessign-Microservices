#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MTAEDA.Maintenance/MTAEDA.Maintenance.Command.API/MTAEDA.Maintenance.Command.API.csproj", "MTAEDA.Maintenance/MTAEDA.Maintenance.Command.API/"]
COPY ["MTAEDA.Maintenance/MTAEDA.Maintenance.Domain/MTAEDA.Maintenance.Domain.csproj", "MTAEDA.Maintenance/MTAEDA.Maintenance.Domain/"]
COPY ["MTAEDA.Maintenance/MTAEDA.Maintenance.Infrastructure/MTAEDA.Maintenance.Infrastructure.csproj", "MTAEDA.Maintenance/MTAEDA.Maintenance.Infrastructure/"]
RUN dotnet restore "./MTAEDA.Maintenance/MTAEDA.Maintenance.Command.API/MTAEDA.Maintenance.Command.API.csproj"
COPY . .
WORKDIR "/src/MTAEDA.Maintenance/MTAEDA.Maintenance.Command.API"
RUN dotnet build "./MTAEDA.Maintenance.Command.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MTAEDA.Maintenance.Command.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MTAEDA.Maintenance.Command.API.dll"]