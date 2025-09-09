# ---------- runtime base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
# roda como usuário não-root recomendado
USER app

# ---------- build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1) Copia só csprojs para cache eficiente do restore
COPY api/api.csproj api/
COPY data/data.csproj data/
COPY model/library.csproj model/
COPY service/service.csproj service/

RUN dotnet restore "api/api.csproj"

# 2) Copia o restante do código
COPY . .

# 3) Build
WORKDIR /src/api
RUN dotnet build "api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ---------- publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ---------- final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# ATENÇÃO: se o assembly não se chama api.dll, ajuste a linha abaixo
ENTRYPOINT ["dotnet", "api.dll"]
