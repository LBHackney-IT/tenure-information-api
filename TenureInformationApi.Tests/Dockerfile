FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine

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
RUN apk update \
 && apk upgrade --no-cache \
 && apk add --no-cache openjdk17

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
    /d:sonar.coverageReportPaths="coverage/SonarQube.xml" \
    /d:sonar.dotnet.excludeTestProjects=true \
    /d:sonar.exclusions="**/*.js, **/*.ts, **/*.css"

# Copy everything
COPY . .

# Restore and build tests project
RUN dotnet restore ./TenureInformationApi.Tests/TenureInformationApi.Tests.csproj
RUN dotnet build --no-restore ./TenureInformationApi.Tests/TenureInformationApi.Tests.csproj

CMD dotnet test ./TenureInformationApi.Tests/TenureInformationApi.Tests.csproj --no-build --collect:"XPlat Code Coverage" --results-directory ./coverage \
    && reportgenerator "-reports:./coverage/*/coverage.cobertura.xml" "-targetdir:coverage" "-reporttypes:SonarQube" "-verbosity:Off" \
    && (dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}" > /dev/null)
