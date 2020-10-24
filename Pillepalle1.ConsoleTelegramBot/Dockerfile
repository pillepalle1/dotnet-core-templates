###############################################################################
# Stage 1: Publish the app

# Based on dotnet-sdk-3.1
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS publisher

# Copy the project file to /src inside the image and restore dependencies
WORKDIR /src
COPY *.csproj .
RUN dotnet restore

# Then copy the source files and release the app in folder /app
COPY . .
RUN dotnet publish --configuration Release --output /app

###############################################################################
# Stage 2: Build an image containing the published app

# Based on dotnet-runtime-3.1 (required to run dotnet core apps)
FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS runtime

# Copy the published app from Stage 1 to the new image
WORKDIR /app
COPY --from=publisher /app/. .

# Entrypoint describes how to launch the bot
ENTRYPOINT [ "dotnet", "Pillepalle1.ConsoleTelegramBot.dll" ]