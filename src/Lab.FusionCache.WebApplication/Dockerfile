FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MinimalApi.Sample3/MinimalApi.Sample3.csproj", "MinimalApi.Sample3/"]
COPY ["MinimalApi.Repository/MinimalApi.Repository.csproj", "MinimalApi.Repository/"]
COPY ["MinimalApi.Service/MinimalApi.Service.csproj", "MinimalApi.Service/"]
RUN dotnet restore "MinimalApi.Sample3/MinimalApi.Sample3.csproj"
COPY . .
WORKDIR "/src/MinimalApi.Sample3"
RUN dotnet build "MinimalApi.Sample3.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MinimalApi.Sample3.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinimalApi.Sample3.dll"]
