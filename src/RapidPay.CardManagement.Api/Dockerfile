#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RapidPay.CardManagement.Api/RapidPay.CardManagement.Api.csproj", "RapidPay.CardManagement.Api/"]
COPY ["RapidPay.CardManagement.App/RapidPay.CardManagement.App.csproj", "RapidPay.CardManagement.App/"]
COPY ["RapidPay.CardManagement.Domain/RapidPay.CardManagement.Domain.csproj", "RapidPay.CardManagement.Domain/"]
COPY ["RapidPay.CardManagement.EntityFramewok/RapidPay.CardManagement.EntityFramework.csproj", "RapidPay.CardManagement.EntityFramewok/"]
COPY ["RapidPay.PaymentFees.BackgroundService/RapidPay.PaymentFees.BackgroundService.csproj", "RapidPay.PaymentFees.BackgroundService/"]
RUN dotnet restore "./RapidPay.CardManagement.Api/RapidPay.CardManagement.Api.csproj"
COPY . .
WORKDIR "/src/RapidPay.CardManagement.Api"
RUN dotnet build "./RapidPay.CardManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RapidPay.CardManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RapidPay.CardManagement.Api.dll"]