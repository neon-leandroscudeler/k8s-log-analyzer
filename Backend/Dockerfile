FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY K8sLogAnalyzer.sln ./
COPY K8sLogAnalyzer.Api/K8sLogAnalyzer.Api.csproj ./K8sLogAnalyzer.Api/
COPY K8sLogAnalyzer.Application/K8sLogAnalyzer.Application.csproj ./K8sLogAnalyzer.Application/
COPY K8sLogAnalyzer.Infrastructure/K8sLogAnalyzer.Infrastructure.csproj ./K8sLogAnalyzer.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish
WORKDIR /src/K8sLogAnalyzer.Api
RUN dotnet publish -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000

# Run the application
ENTRYPOINT ["dotnet", "K8sLogAnalyzer.Api.dll"]
