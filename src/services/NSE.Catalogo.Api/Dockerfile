FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./services/NSE.Catalogo.Api/NSE.Catalogo.Api.csproj", "services/NSE.Catalogo.Api/"]
COPY ["./building blocks/NSE.Core/NSE.Core.csproj", "building blocks/NSE.Core/"]
COPY ["./building blocks/NSE.WebApi.Core/NSE.WebApi.Core.csproj", "building blocks/NSE.WebApi.Core/"]
COPY ["./building blocks/NSE.MessageBus/NSE.MessageBus.csproj", "building blocks/NSE.MessageBus/"]

RUN dotnet restore "services/NSE.Catalogo.Api/NSE.Catalogo.Api.csproj"
COPY . .
WORKDIR "services/NSE.Catalogo.Api"
RUN dotnet build "NSE.Catalogo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./NSE.Catalogo.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NSE.Catalogo.Api.dll"]
