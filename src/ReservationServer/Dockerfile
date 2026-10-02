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
COPY ["ReservationServer/ReservationServer.csproj", "ReservationServer/"]
COPY ["ReservationBL/ReservationBL.csproj", "ReservationBL/"]
COPY ["ReservationDB/ReservationDB.csproj", "ReservationDB/"]
RUN dotnet restore "ReservationServer/ReservationServer.csproj"
COPY . .
WORKDIR "/src/ReservationServer"
RUN dotnet build "./ReservationServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReservationServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReservationServer.dll"]
