FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.17 AS build
WORKDIR /app
RUN dotnet --version
COPY . .

ARG USERNAME
ARG PASSWORD

RUN dotnet nuget update source Nexus -u $USERNAME -p $PASSWORD --store-password-in-clear-text --configfile nuget.config
RUN dotnet nuget update source NexusHosted -u $USERNAME -p $PASSWORD --store-password-in-clear-text --configfile nuget.config
RUN dotnet nuget update source NexusViaPagCore -u $USERNAME -p $PASSWORD --store-password-in-clear-text --configfile nuget.config
RUN dotnet restore

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.17

RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Template.WebApi.dll"]