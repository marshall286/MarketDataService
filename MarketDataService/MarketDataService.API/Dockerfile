# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MarketDataService.API/MarketDataService.API.csproj", "MarketDataService.API/"]
COPY ["MarketDataService.Application/MarketDataService.Application.csproj", "MarketDataService.Application/"]
COPY ["MarketDataService.Domain/MarketDataService.Domain.csproj", "MarketDataService.Domain/"]
COPY ["MarketDataService.Infrastructure/MarketDataService.Infrastructure.csproj", "MarketDataService.Infrastructure/"]
RUN dotnet restore "./MarketDataService.API/MarketDataService.API.csproj"
COPY . .
WORKDIR "/src/MarketDataService.API"
RUN dotnet build "./MarketDataService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MarketDataService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MarketDataService.API.dll"]