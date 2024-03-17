#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 7001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MTAEDA.Equipment/MTAEDA.Equipment.Query.API/MTAEDA.Equipment.Query.API.csproj", "MTAEDA.Equipment/MTAEDA.Equipment.Query.API/"]
COPY ["MTAEDA.Equipment/MTAEDA.Equipment.Infrastructure/MTAEDA.Equipment.Infrastructure.csproj", "MTAEDA.Equipment/MTAEDA.Equipment.Infrastructure/"]
COPY ["MTAEDA.Core/MTAEDA.Core.csproj", "MTAEDA.Core/"]
COPY ["MTAEDA.Equipment/MTAEDA.Equipment.Domain/MTAEDA.Equipment.Domain.csproj", "MTAEDA.Equipment/MTAEDA.Equipment.Domain/"]
RUN dotnet restore "./MTAEDA.Equipment/MTAEDA.Equipment.Query.API/MTAEDA.Equipment.Query.API.csproj"
COPY . .
WORKDIR "/src/MTAEDA.Equipment/MTAEDA.Equipment.Query.API"
RUN dotnet build "./MTAEDA.Equipment.Query.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MTAEDA.Equipment.Query.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MTAEDA.Equipment.Query.API.dll"]