#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MTAEDA.Scheduling/MTAEDA.Scheduling.Command.API/MTAEDA.Scheduling.Command.API.csproj", "MTAEDA.Scheduling/MTAEDA.Scheduling.Command.API/"]
RUN dotnet restore "MTAEDA.Scheduling/MTAEDA.Scheduling.Command.API/MTAEDA.Scheduling.Command.API.csproj"
COPY . .
WORKDIR "/src/MTAEDA.Scheduling/MTAEDA.Scheduling.Command.API"
RUN dotnet build "MTAEDA.Scheduling.Command.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MTAEDA.Scheduling.Command.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MTAEDA.Scheduling.Command.API.dll"]