# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. NEW: Copy the solution file first for context
COPY PersonalFinance.sln . 

# 2. UPDATED: Copy project files using paths relative to the root (Crucial Change!)
COPY ["PersonalFinance/PersonalFinance.csproj", "PersonalFinance/"]
COPY ["PersonalFinance.Data/PersonalFinance.Data.csproj", "PersonalFinance.Data/"]

# 3. UPDATED: Restore dependencies against the entire solution file
RUN dotnet restore "PersonalFinance.sln" 

# 4. UPDATED: Copy all remaining files from the root context
COPY . .
WORKDIR "/src/PersonalFinance"
RUN dotnet build "PersonalFinance.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PersonalFinance.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# IMPORTANT: Render often expects a different port. We'll set the environment variable for 8080
# You may need to change 8080 to 80 if you run into issues, but 8080 is the default.
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PersonalFinance.dll"]