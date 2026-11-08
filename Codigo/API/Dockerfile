FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY API/MedicaAPI.csproj MedicaAPI/
COPY Web/Core/Core.csproj Web/Core/
COPY Web/Service/Service.csproj Web/Service/
COPY Web/Util/Util.csproj Web/Util/
RUN dotnet restore API/MedicaAPI.csproj

COPY API/ API/
COPY Web/Core/ Web/Core/
COPY Web/Service/ Web/Service/
COPY Web/Util/ Web/Util/

RUN dotnet publish API/MedicaAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedicaAPI.dll"]
