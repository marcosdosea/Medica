FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MedicaWeb/MedicaWeb.csproj MedicaWeb/
RUN dotnet restore MedicaWeb/MedicaWeb.csproj

COPY . .

RUN dotnet publish MedicaWeb/MedicaWeb.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedicaWeb.dll"]
