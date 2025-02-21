# Stage 1: Base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

# Switch to root to install curl and create a user
# since mcr.microsoft.com/dotnet/aspnet:9.0 might be using a non-root user by default
USER root
RUN apt-get update && apt-get install -y curl

# Create a non-root user
ARG APP_UID=1000
RUN useradd -m -u $APP_UID appuser

# Switch to the new non-root user
USER appuser

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy only the API project file from the src folder.
COPY ["ProductManagement.API/ProductManagement.API.csproj", "ProductManagement.API/"]
# Copy the rest of the solution files so that referenced projects are available.
COPY . .
RUN dotnet restore "ProductManagement.API/ProductManagement.API.csproj"
RUN dotnet build "ProductManagement.API/ProductManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProductManagement.API/ProductManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductManagement.API.dll"]
