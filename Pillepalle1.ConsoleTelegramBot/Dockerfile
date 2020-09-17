FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS publisher
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish --configuration Release --output /app

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS runtime
WORKDIR /app
COPY --from=publisher /app/. .
ENTRYPOINT [ "dotnet", "Pillepalle1.ConsoleTelegramBot.dll" ]