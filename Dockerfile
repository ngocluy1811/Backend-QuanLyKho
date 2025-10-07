# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

# Use the official .NET 8 SDK as base image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["FertilizerWarehouseAPI.csproj", "./"]
RUN dotnet restore "FertilizerWarehouseAPI.csproj"

# Copy all source code
COPY . .
RUN dotnet build "FertilizerWarehouseAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "FertilizerWarehouseAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "FertilizerWarehouseAPI.dll"]
