#!/bin/ash

#install zip on debian OS, since microsoft/dotnet container doesn't have zip by default
# if [ -f /etc/debian_version ]
# then
  apk update -q
  apk add --no-cache zip
# fi
# check zip version maybe it comes by default

#dotnet restore
dotnet tool install --global Amazon.Lambda.Tools --version 4.0.0


# can check that by default, but since we're using a dotnet image, it should already be set
# (for CI) ensure that the newly-installed tools are on PATH
# if [ -f /etc/debian_version ]
# then
#   export PATH="$PATH:/$(whoami)/.dotnet/tools"
# fi

dotnet restore
dotnet lambda package --configuration release --framework net6.0 --output-package ./bin/release/net6.0/tenure-information-api.zip
