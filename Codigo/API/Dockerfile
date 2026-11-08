FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY API/MedicaAPI.csproj MedicaAPI/
COPY Web/Core/Core.csproj Web/Core/
COPY Web/Service/Service.csproj Web/Service/
RUN dotnet restore MedicaAPI/MedicaAPI.csproj

COPY API/ API/
COPY Web/Core/ Web/Core/
COPY Web/Service/ Web/Service/

RUN dotnet publish MedicaAPI/MedicaAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedicaAPI.dll"]
