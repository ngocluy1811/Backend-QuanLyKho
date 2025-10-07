# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8 SDK as base image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["FertilizerWarehouseAPI/FertilizerWarehouseAPI.csproj", "FertilizerWarehouseAPI/"]
RUN dotnet restore "FertilizerWarehouseAPI/FertilizerWarehouseAPI.csproj"

# Copy all source code
COPY . .
WORKDIR "/src/FertilizerWarehouseAPI"
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
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "FertilizerWarehouseAPI.dll"]
