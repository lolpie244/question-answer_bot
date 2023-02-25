FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

EXPOSE 59890
WORKDIR /source

# copy csproj and restore as distinct layers
COPY franko_bot/*.csproj .
RUN dotnet restore --use-current-runtime

# copy everything else and build app
COPY franko_bot/. .
RUN rm global.json
RUN dotnet publish -c Release --self-contained false --no-restore -o /app

# To enable globalization:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/enable-globalization.md
# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./franko_bot", "--urls", "http://0.0.0.0:59890"]
