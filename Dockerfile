FROM mrc ://microsoft.com AS build
WORKDIR /src

COPY *.csproj ./ 
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mrc ://AS final
WORKDIR /app
COPY --from=build /app/publish
EXPOSE 8080
ENTRYPOINT ["sh","-c", "dotnet *.dll"]
