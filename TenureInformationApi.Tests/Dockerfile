FROM mcr.microsoft.com/dotnet/sdk:6.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'
ENV Sns_LocalMode='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

WORKDIR /app

# Install and run sonar cloud scanner
RUN apt-get update && apt-get install -y openjdk-17-jdk
RUN dotnet tool install --global dotnet-sonarscanner --version 5.6.0
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"LBHackney-IT_tenure-api" /o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}"

# Copy csproj and restore as distinct layers
COPY ./TenureInformationApi.sln ./
COPY ./TenureInformationApi/TenureInformationApi.csproj ./TenureInformationApi/
COPY ./TenureInformationApi.Tests/TenureInformationApi.Tests.csproj ./TenureInformationApi.Tests/
COPY /nuget.config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./TenureInformationApi/TenureInformationApi.csproj
RUN dotnet restore ./TenureInformationApi.Tests/TenureInformationApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c Release -o out TenureInformationApi/TenureInformationApi.csproj
RUN dotnet build -c debug -o out TenureInformationApi.Tests/TenureInformationApi.Tests.csproj

CMD dotnet test
RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
