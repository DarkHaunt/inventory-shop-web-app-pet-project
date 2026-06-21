FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["inventory-shop-web-app-pet-project/inventory-shop-web-app-pet-project.csproj", "inventory-shop-web-app-pet-project/"]
RUN dotnet restore "inventory-shop-web-app-pet-project/inventory-shop-web-app-pet-project.csproj"
COPY . .
WORKDIR "/src/inventory-shop-web-app-pet-project"
RUN dotnet build "./inventory-shop-web-app-pet-project.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./inventory-shop-web-app-pet-project.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "inventory-shop-web-app-pet-project.dll"]
