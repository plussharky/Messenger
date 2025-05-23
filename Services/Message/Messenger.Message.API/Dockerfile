# Базовый образ для рантайма
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Билд-образ
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY Services/Message/Messenger.Messages.Api/Messenger.Messages.Api.csproj Services/Message/Messenger.Messages.Api/
COPY Services/Message/Messenger.Messages.Application/Messenger.Messages.Application.csproj Services/Message/Messenger.Messages.Application/
COPY Services/Message/Messenger.Messages.Domain/Messenger.Messages.Domain.csproj Services/Message/Messenger.Messages.Domain/
COPY Services/Message/Messenger.Messages.Infrastructure/Messenger.Messages.Infrastructure.csproj Services/Message/Messenger.Messages.Infrastructure/

RUN dotnet restore "Services/Message/Messenger.Messages.Api/Messenger.Messages.Api.csproj"

# Копируем все остальные файлы
COPY . .

WORKDIR /src/Services/Message/Messenger.Messages.Api
RUN dotnet build "Messenger.Messages.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Messenger.Messages.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messenger.Messages.Api.dll"]
