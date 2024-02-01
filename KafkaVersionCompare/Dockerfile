FROM mcr.microsoft.com/playwright/dotnet:v1.40.0-jammy AS build-env
WORKDIR /App
#EXPOSE 443
#EXPOSE 80

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/playwright/dotnet:v1.40.0-jammy
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "KafkaVersionCompare.dll"]