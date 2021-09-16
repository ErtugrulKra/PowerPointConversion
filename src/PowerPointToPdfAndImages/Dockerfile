#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM ertugrulkara/dotnet-core-50-libreoffice-gs AS base
FROM build AS publish
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/nightly/sdk:5.0-focal AS build
WORKDIR /src
COPY ["PowerPointToPdfAndImages/PowerPointToPdfAndImages.csproj", "PowerPointToPdfAndImages/"]
RUN dotnet restore "PowerPointToPdfAndImages/PowerPointToPdfAndImages.csproj"
COPY . .
WORKDIR "/src/PowerPointToPdfAndImages"
RUN dotnet build "PowerPointToPdfAndImages.csproj" -c Release -o /app/build

FROM build AS publish
RUN apt -y update
RUN apt -y install libreoffice
RUN apt -y install ghostscript


RUN dotnet publish "PowerPointToPdfAndImages.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PowerPointToPdfAndImages.dll"]


LABEL authors="Ertugrul Kara"