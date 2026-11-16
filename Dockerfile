ARG REG=mcr
ARG ORG=microsoft.com

FROM ${REG}.${ORG}/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM ${REG}.${ORG}/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish /app
EXPOSE 8080
ENTRYPOINT ["sh", "-c", "dotnet *.dll"]
