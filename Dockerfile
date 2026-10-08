FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY hotelbooking/hotelbooking.csproj hotelbooking/
RUN dotnet restore hotelbooking/hotelbooking.csproj

COPY hotelbooking/ hotelbooking/
RUN dotnet publish hotelbooking/hotelbooking.csproj -c Release -o /app/publish

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "hotelbooking.dll"]