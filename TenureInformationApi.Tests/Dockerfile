FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./TenureInformationApi.sln ./
COPY ./TenureInformationApi/TenureInformationApi.csproj ./TenureInformationApi/
COPY ./TenureInformationApi.Tests/TenureInformationApi.Tests.csproj ./TenureInformationApi.Tests/

RUN dotnet restore ./TenureInformationApi/TenureInformationApi.csproj
RUN dotnet restore ./TenureInformationApi.Tests/TenureInformationApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out TenureInformationApi.Tests/TenureInformationApi.Tests.csproj

CMD dotnet test
