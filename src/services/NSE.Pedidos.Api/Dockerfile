FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./services/NSE.Pedidos.Api/NSE.Pedidos.Api.csproj", "services/NSE.Pedidos.Api/"]
COPY ["./building blocks/NSE.Core/NSE.Core.csproj", "building blocks/NSE.Core/"]
COPY ["./building blocks/NSE.MessageBus/NSE.MessageBus.csproj", "building blocks/NSE.MessageBus/"]
COPY ["./building blocks/NSE.WebApi.Core/NSE.WebApi.Core.csproj", "building blocks/NSE.WebApi.Core/"]
COPY ["./services/NSE.Pedidos.Domain/NSE.Pedidos.Domain.csproj", "services/NSE.Pedidos.Domain/"]
COPY ["./services/NSE.Pedidos.Infra/NSE.Pedidos.Infra.csproj", "services/NSE.Pedidos.Infra/"]

RUN dotnet restore "services/NSE.Pedidos.Api/NSE.Pedidos.Api.csproj"
COPY . .
WORKDIR "services/NSE.Pedidos.Api"
RUN dotnet build "NSE.Pedidos.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NSE.Pedidos.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NSE.Pedidos.Api.dll"]
