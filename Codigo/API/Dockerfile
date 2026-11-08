FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Codigo/API/MedicaAPI.csproj", "API/"]
COPY ["Codigo/Web/Core/Core.csproj", "Web/Core/"]
COPY ["Codigo/Web/Service/Service.csproj", "Web/Service/"]
RUN dotnet restore "API/MedicaAPI.csproj"

COPY Codigo/ .
WORKDIR "/src/API"
RUN dotnet publish "MedicaAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedicaAPI.dll"]
