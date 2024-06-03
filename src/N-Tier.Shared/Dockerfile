FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App
ARG BUILD_NUMBER
ENV BUILD_NUMBER $BUILD_NUMBER

# Copy everything
COPY ./nuget.config ./
COPY ./ForcegetApplicationBase.Core/ ./
# Build and publish a release
RUN dotnet build --configfile ../../nuget.config -c Release
#RUN dotnet pack  -c Release /p:Version=8.1.36
RUN dotnet pack --configfile ../../nuget.config -c Release /p:Version=7.0.${BUILD_NUMBER}

#RUN dotnet nuget push -s https://devops-baget.forceget.com/v3/index.json ./bin/Release/ForcegetApplicationBase.Core.8.1.36.nupkg --api-key NUGET-SERVER-API-KEY
RUN dotnet nuget push -s https://devops-baget.forceget.com/v3/index.json ./bin/Release/ForcegetApplicationBase.Core.1.0.${BUILD_NUMBER}.nupkg --api-key NUGET-SERVER-API-KEY
