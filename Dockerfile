# Stage 1: Build the application
FROM ://microsoft.com AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source code and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Run the ASP.NET Core web app
FROM ://microsoft.com AS final
WORKDIR /app

# Copy the compiled app from the build stage
COPY --from=build /app/publish .

# Expose port 8080 (standard for .NET 8 containers)
EXPOSE 8080

# Start the web app (automatically finds your compiled .dll)
ENTRYPOINT ["sh", "-c", "dotnet *.dll"]
