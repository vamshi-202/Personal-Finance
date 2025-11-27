# See https://aka.ms/customizecontainer to learn how to customize your debug container.

# === Stage 1: Base Runtime ===
# Use the ASP.NET runtime image for the final, lean production image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Using a fixed non-root user for security (if available in the image)
# USER $APP_UID 
WORKDIR /app
EXPOSE 8080

# === Stage 2: Build ===
# Use the SDK image for compiling and publishing
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Copy the solution file and both project files first. 
# This is CRUCIAL for resolving project references correctly.
COPY PersonalFinance.sln .
COPY ["PersonalFinance/PersonalFinance.csproj", "PersonalFinance/"]
COPY ["PersonalFinance.Data/PersonalFinance.Data.csproj", "PersonalFinance.Data/"]

# 2. Run dotnet restore against the solution file.
RUN dotnet restore "PersonalFinance.sln"

# 3. Copy all remaining source files into the container.
COPY . .

# 4. Change directory to the web project before building.
WORKDIR "/src/PersonalFinance"

# 5. Build and Publish the main web project to the /app/publish folder.
# We use --no-restore because we did a full restore above.
RUN dotnet publish "PersonalFinance.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# === Stage 3: Final Production ===
# Creates the final, small image based on the runtime
FROM base AS final
WORKDIR /app

# Copy the published output (the compiled DLLs) from the build stage
COPY --from=build /app/publish .

# Set environment variable for the port Render uses (MUST match EXPOSE)
ENV ASPNETCORE_URLS=http://+:8080

# Specify the command to run the application
ENTRYPOINT ["dotnet", "PersonalFinance.dll"]