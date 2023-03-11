FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy everything
COPY ./src ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /App
COPY --from=build-env /App/out .
RUN groupadd --gid 7779 smtp-to-paperless \
    && useradd --uid 7779 --gid 7779 -m smtp-to-paperless
USER 7779
ENTRYPOINT ["dotnet", "smtp-to-paperless.dll"]
