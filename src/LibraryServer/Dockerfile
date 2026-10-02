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
COPY ["LibraryServer/LibraryServer.csproj", "LibraryServer/"]
COPY ["LibraryBL/LibraryBL.csproj", "LibraryBL/"]
COPY ["LibraryDB/LibraryDB.csproj", "LibraryDB/"]
RUN dotnet restore "LibraryServer/LibraryServer.csproj"
COPY . .
WORKDIR "/src/LibraryServer"
RUN dotnet build "./LibraryServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LibraryServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibraryServer.dll"]
