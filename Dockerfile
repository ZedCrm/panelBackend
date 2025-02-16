# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the solution file
COPY *.sln ./

# Copy all project files while maintaining their directory structure
COPY . ./

# Restore dependencies using the solution file
RUN dotnet restore ZCRM.sln



# Build and publish the app
RUN dotnet publish -c Release -o /out

# Use the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory in the runtime image
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build /out .

# Expose the port
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "API.dll"]
