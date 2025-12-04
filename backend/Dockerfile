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

# Install ca-certificates and OpenSSL for certificate handling
RUN apt-get update && apt-get install -y \
    ca-certificates \
    openssl \
    && rm -rf /var/lib/apt/lists/*

# Configure OpenSSL for better certificate compatibility
RUN echo "[default_conf]" > /etc/ssl/openssl_custom.cnf && \
    echo "ssl_conf = ssl_sect" >> /etc/ssl/openssl_custom.cnf && \
    echo "[ssl_sect]" >> /etc/ssl/openssl_custom.cnf && \
    echo "system_default = system_default_sect" >> /etc/ssl/openssl_custom.cnf && \
    echo "[system_default_sect]" >> /etc/ssl/openssl_custom.cnf && \
    echo "MinProtocol = TLSv1.2" >> /etc/ssl/openssl_custom.cnf && \
    echo "CipherString = DEFAULT@SECLEVEL=1" >> /etc/ssl/openssl_custom.cnf

ENV OPENSSL_CONF=/etc/ssl/openssl_custom.cnf

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
