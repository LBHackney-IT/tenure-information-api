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

# Update the system & install java binaries (for sonar)
RUN apt-get update && apt-get install -y openjdk-17-jdk
# Install sonarscanner
RUN dotnet tool install --global dotnet-sonarscanner
# Install report generator
RUN dotnet tool install --global dotnet-reportgenerator-globaltool

ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin \
    /k:"LBHackney-IT_tenure-api" \
    /o:"lbhackney-it" \
    /d:sonar.host.url=https://sonarcloud.io \
    /d:sonar.login="${SONAR_TOKEN}" \
    /d:sonar.coverageReportPaths="coverage/SonarQube.xml"

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

CMD dotnet test ./TenureInformationApi.Tests/TenureInformationApi.Tests.csproj --collect:"XPlat Code Coverage" --results-directory ./coverage \
    && reportgenerator "-reports:./coverage/*/coverage.cobertura.xml" "-targetdir:coverage" "-reporttypes:SonarQube" "-verbosity:Off" \
    && (dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}" > /dev/null)
