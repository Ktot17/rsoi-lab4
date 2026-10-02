FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GatewayServer/GatewayServer.csproj", "GatewayServer/"]
COPY ["GatewayBL/GatewayBL.csproj", "GatewayBL/"]
COPY ["GatewayHttpClient/GatewayHttpClient.csproj", "GatewayHttpClient/"]
RUN dotnet restore "GatewayServer/GatewayServer.csproj"
COPY . .
WORKDIR "/src/GatewayServer"
RUN dotnet build "./GatewayServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GatewayServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GatewayServer.dll"]
