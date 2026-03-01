FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8082



FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV NUGET_HTTP_CACHE_PATH=/root/.local/share/NuGet/v3-cache
ENV NUGET_ENHANCED_MAX_NETWORK_TRY_COUNT=10
ENV NUGET_ENHANCED_NETWORK_RETRY_DELAY_MILLISECONDS=2000

WORKDIR /src
COPY ["TaskService.csproj", "./"]
RUN dotnet restore "TaskService.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "./TaskService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskService.dll"]
