FROM microsoft/dotnet:2.1-sdk AS build-env

ARG RID=linux-x64

WORKDIR /build

COPY Sds.ReactionFileParser.Domain/Sds.ReactionFileParser.Domain.csproj Sds.ReactionFileParser.Domain/
COPY Sds.ReactionFileParser.Tests/Sds.ReactionFileParser.Tests.csproj Sds.ReactionFileParser.Tests/
COPY Nuget.config .

RUN dotnet restore --configfile Nuget.config Sds.ReactionFileParser.Tests/Sds.ReactionFileParser.Tests.csproj

COPY Sds.ReactionFileParser.Domain Sds.ReactionFileParser.Domain
COPY Sds.ReactionFileParser.Tests Sds.ReactionFileParser.Tests

RUN dotnet publish Sds.ReactionFileParser.Tests/Sds.ReactionFileParser.Tests.csproj -r $RID -c Release -o /dist

# Build runtime image
FROM microsoft/dotnet:2.1-sdk

LABEL maintainer="pshenichnov@gmail.com"

WORKDIR /app

RUN apt-get update && apt-get install -y curl
RUN curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /app/wait-for-it.sh && chmod 777 /app/wait-for-it.sh
EXPOSE 8083 
COPY --from=build-env /dist ./

ENTRYPOINT ["dotnet", "vstest", "./Sds.ReactionFileParser.Tests.dll"]
