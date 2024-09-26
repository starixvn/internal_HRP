#ARG CI_REGISTRY_IMAGE

#FROM $CI_REGISTRY_IMAGE:builder AS deps

#RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

#ENV NUGET_PACKAGES=/src/packages

#COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR /src

COPY ["HRP.Resources/HRP.Resources.csproj", "HRP.Resources/"]
COPY ["HRP.Core/HRP.Core.csproj", "HRP.Core/"]
COPY ["HRP.Models/HRP.Models.csproj", "HRP.Models/"]
COPY ["HRP.Criteria/HRP.Criteria.csproj", "HRP.Criteria/"]
COPY ["HRP.Repositories/HRP.Repositories.csproj", "HRP.Repositories/"]
COPY ["HRP.Services/HRP.Services.csproj", "HRP.Services/"]
COPY ["HRP.Website/HRP.Website.csproj", "HRP.Website/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore -v n -r linux-x64 "HRP.Website/HRP.Website.csproj"

COPY ["HRP.Resources/", "HRP.Resources/"]
COPY ["HRP.Core/", "HRP.Core/"]
COPY ["HRP.Models/", "HRP.Models/"]
COPY ["HRP.Criteria/", "HRP.Criteria/"]
COPY ["HRP.Repositories/", "HRP.Repositories/"]
COPY ["HRP.Services/", "HRP.Services/"]
COPY ["HRP.Website/", "HRP.Website/"]
WORKDIR /src/HRP.Website

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-slim AS base

RUN apt-get update \
  && apt-get install -y --allow-unauthenticated libgdiplus \
  && rm -rf /var/lib/apt/lists/* /var/cache/apt/archives/*

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .
ENV TZ="Asia/Bangkok"

ENTRYPOINT ["dotnet", "HRP.Website.dll"]