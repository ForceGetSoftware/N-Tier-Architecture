FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app

COPY src/N-Tier.API/*.csproj ./src/N-Tier.API/
COPY src/N-Tier.Application/*.csproj ./src/N-Tier.Application/
COPY src/N-Tier.Core/*.csproj ./src/N-Tier.Core/
COPY src/N-Tier.DataAccess/*.csproj ./src/N-Tier.DataAccess/

WORKDIR /app/src/N-Tier.API
RUN dotnet restore

WORKDIR /app
COPY . .

WORKDIR /app/src/N-Tier.API

RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "N-Tier.API.dll"]
