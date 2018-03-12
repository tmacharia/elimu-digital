#FROM microsoft/aspnetcore:1.1 AS base
#WORKDIR /app
#EXPOSE 80
#
#FROM microsoft/aspnetcore-build:1.1 AS build
#WORKDIR /src
#COPY *.sln ./
#COPY web/web.csproj web/
#COPY DAL/DAL.csproj DAL/
#COPY Common/Common.csproj Common/
#COPY Services/Services.csproj Services/
#RUN dotnet restore
#COPY . .
#WORKDIR /src/web
#RUN dotnet build -c Release -o /app
#
#FROM build AS publish
#RUN dotnet publish -c Release -o /app
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "web.dll"]

# Use the standard Microsoft ASP.NET Core container
FROM microsoft/aspnetcore:1.1

# Copy our code
WORKDIR /app
COPY bin/Release/netcoreapp1.1/publish .

# Expose port 80 for traffic
ENV ASPNETCORE_URLS http://+:80

EXPOSE 80

# Run the dotnet application against a DLL from within the container
# Publish the application first before building this dockerfile.
ENTRYPOINT ["dotnet","web.dll"]
