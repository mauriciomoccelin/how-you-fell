# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /

# copy csproj and restore as distinct layers
COPY *.sln .
COPY src/HowYouFell.Api.csproj ./src/
COPY test/unit/HowYouFell.Test.Unit.csproj ./test/unit/
RUN dotnet restore

# copy everything and build
COPY . .
RUN cat /src/HowYouFell.Api.csproj
RUN dotnet publish /src/HowYouFell.Api.csproj -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
