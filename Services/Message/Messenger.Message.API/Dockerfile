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
COPY Services/Message/Messenger.Message.API/Messenger.Message.API.csproj Services/Message/Messenger.Message.API/
COPY Services/Message/Messenger.Message.Application/Messenger.Message.Application.csproj Services/Message/Messenger.Message.Application/
COPY Services/Message/Messenger.Message.Domain/Messenger.Message.Domain.csproj Services/Message/Messenger.Message.Domain/
COPY Services/Message/Messenger.Message.Infrastructure/Messenger.Message.Infrastructure.csproj Services/Message/Messenger.Message.Infrastructure/

RUN dotnet restore "Services/Message/Messenger.Message.API/Messenger.Message.API.csproj"

# Копируем все остальные файлы
COPY . .

WORKDIR /src/Services/Message/Messenger.Message.API
RUN dotnet build "Messenger.Message.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Messenger.Message.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Messenger.Message.API.dll"]
