# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["api/app.Api.csproj", "api/"]
COPY ["application/app.Application.csproj", "application/"]
COPY ["domain/app.Domain.csproj", "domain/"]
COPY ["infrastructure/app.Infrastructure.csproj", "infrastructure/"]

# Restore dependencies
RUN dotnet restore "api/app.Api.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/api"
RUN dotnet build "app.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "app.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Create directory for SQLite database
RUN mkdir -p /data

# Expose port
EXPOSE 5058

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5058
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "app.Api.dll"]
