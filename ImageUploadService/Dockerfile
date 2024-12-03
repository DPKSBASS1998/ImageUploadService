# Вибір образу SDK для побудови з підтримкою .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Встановлюємо робочу директорію
WORKDIR /app

# Копіюємо csproj файл у контейнер
COPY ./ImageUploadService.csproj /app/

# Перевіряємо наявність файлів у контейнері
RUN ls -la /app

# Відновлюємо залежності
RUN dotnet restore /app/ImageUploadService.csproj

# Копіюємо весь код у контейнер
COPY . /app/

# Публікуємо додаток
RUN dotnet publish /app/ImageUploadService.csproj -c Release -o /app/publish

# Налаштовуємо фінальний образ (з підтримкою .NET 8.0)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Встановлюємо інструменти для діагностики: curl, ping, telnet, netcat
RUN apt-get update && \
    apt-get install -y curl iputils-ping telnet netcat-openbsd

WORKDIR /app
EXPOSE 5000

# Копіюємо публікацію в кінцевий контейнер
COPY --from=build /app/publish /app

# Вказуємо правильний шлях до DLL-файлу
ENTRYPOINT ["dotnet", "ImageUploadService.dll"]
