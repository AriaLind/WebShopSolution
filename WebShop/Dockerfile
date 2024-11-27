# Use SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env

# Set the working directory inside the container
WORKDIR /App

# Copy the project files into the container
COPY . ./

# Restore the dependencies (via NuGet)
RUN dotnet restore

# Build the project and publish it to the 'out' directory
RUN dotnet publish -c Release -o out

# Runtime image (this is the final image for production)
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff

# Set the working directory inside the container for runtime image
WORKDIR /App

# Copy the published output from the build image to the runtime image
COPY --from=build-env /App/out ./

# Expose port 80 for the app to listen on
EXPOSE 80

# Set environment variables for the application
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80

# Define entry point for the application (run the WebShop.dll)
ENTRYPOINT ["dotnet", "WebShop.dll"]
