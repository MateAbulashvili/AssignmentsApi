FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
ARG NUGET_SOURCE_USERNAME
ARG NUGET_SOURCE_TOKEN
ENV NUGET_SOURCE_USERNAME=$NUGET_SOURCE_USERNAME
ENV NUGET_SOURCE_TOKEN=$NUGET_SOURCE_TOKEN
WORKDIR /src
COPY . .
RUN dotnet restore -v n --configfile ./nuget.config "AssignmentsApi/AssignmentsApi.csproj"
WORKDIR "/src/AssignmentsApi"
RUN dotnet build "AssignmentsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AssignmentsApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AssignmentsApi.dll"]