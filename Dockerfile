# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore as distinct layers
COPY ["APIMS_Api/APIMS_Api/APIMS_Api.csproj", "APIMS_Api/APIMS_Api/"]
RUN dotnet restore "APIMS_Api/APIMS_Api/APIMS_Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/source/APIMS_Api/APIMS_Api"
RUN dotnet build "APIMS_Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "APIMS_Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIMS_Api.dll"]
