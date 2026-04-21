FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /src

# Copy the entire repository
COPY . .

# Restore dependencies at build time
RUN dotnet restore Message.Api.T2.Tests/Message.Api.T2.Tests.csproj

# Run tests at container startup so Docker Compose services are available
CMD ["dotnet", "test", "Message.Api.T2.Tests/Message.Api.T2.Tests.csproj", "--logger", "console;verbosity=detailed", "--settings", "Message.Api.T2.Tests/Docker.runsettings"]
