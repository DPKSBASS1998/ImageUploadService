# ���� ������ SDK ��� �������� � ��������� .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# ������������ ������ ���������
WORKDIR /app

# ������� csproj ���� � ���������
COPY ./ImageUploadService.csproj /app/

# ���������� ��������� ����� � ���������
RUN ls -la /app

# ³��������� ���������
RUN dotnet restore /app/ImageUploadService.csproj

# ������� ���� ��� � ���������
COPY . /app/

# �������� �������
RUN dotnet publish /app/ImageUploadService.csproj -c Release -o /app/publish

# ����������� ��������� ����� (� ��������� .NET 8.0)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# ������������ ����������� ��� ����������: curl, ping, telnet, netcat
RUN apt-get update && \
    apt-get install -y curl iputils-ping telnet netcat-openbsd

WORKDIR /app
EXPOSE 5000

# ������� ��������� � ������� ���������
COPY --from=build /app/publish /app

# ������� ���������� ���� �� DLL-�����
ENTRYPOINT ["dotnet", "ImageUploadService.dll"]
