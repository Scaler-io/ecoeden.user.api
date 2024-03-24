#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["User.Api/User.Api.csproj", "User.Api/"]
COPY ["Ecoeden.Swagger/Ecoeden.Swagger.csproj", "Ecoeden.Swagger/"]
COPY ["Ecoeden.User.Domain/Ecoeden.User.Domain.csproj", "Ecoeden.User.Domain/"]
COPY ["Ecoeden.User.Application/Ecoeden.User.Application.csproj", "Ecoeden.User.Application/"]
COPY ["Ecoeden.User.Infrastructure/Ecoeden.User.Infrastructure.csproj", "Ecoeden.User.Infrastructure/"]
RUN dotnet restore "User.Api/User.Api.csproj"
COPY . .
WORKDIR "/src/User.Api"
RUN dotnet build "User.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "User.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.Api.dll"]