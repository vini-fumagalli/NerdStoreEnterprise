FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./services/NSE.Pagamentos.Api/NSE.Pagamentos.Api.csproj", "services/NSE.Pagamentos.Api/"]
COPY ["./building blocks/NSE.Core/NSE.Core.csproj", "building blocks/NSE.Core/"]
COPY ["./building blocks/NSE.MessageBus/NSE.MessageBus.csproj", "building blocks/NSE.MessageBus/"]
COPY ["./building blocks/NSE.WebApi.Core/NSE.WebApi.Core.csproj", "building blocks/NSE.WebApi.Core/"]
COPY ["./services/NSE.Pagamento.NerdsPag/NSE.Pagamentos.NerdsPag.csproj", "services/NSE.Pagamentos.NerdsPag/"]

RUN dotnet restore "services/NSE.Pagamentos.Api/NSE.Pagamentos.Api.csproj"
COPY . .
WORKDIR "services/NSE.Pagamentos.Api"
RUN dotnet build "NSE.Pagamentos.Api.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NSE.Pagamentos.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NSE.Pagamentos.Api.dll"]
