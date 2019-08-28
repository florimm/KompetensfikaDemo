FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["FutreTechAPI/FutreTechAPI.csproj", "FutreTechAPI/"]
RUN dotnet restore "FutreTechAPI/FutreTechAPI.csproj"
COPY . .
WORKDIR "/src/FutreTechAPI"
RUN dotnet build "FutreTechAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FutreTechAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FutreTechAPI.dll"]