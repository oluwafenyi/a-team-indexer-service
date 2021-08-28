FROM debian:10

WORKDIR /usr/app/

RUN apt-get update -y && apt-get install dirmngr gnupg apt-transport-https ca-certificates -y &&\
    apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF &&\
    sh -c 'echo "deb https://download.mono-project.com/repo/debian stable-buster main" > /etc/apt/sources.list.d/mono-official-stable.list'\
    && apt-get update -y && apt-get install mono-complete -y

RUN apt-get install wget -y && wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe && cp ./nuget.exe /bin/

ENV nuget="mono nuget.exe"

COPY IndexerService.sln .

COPY ./IndexerService/IndexerService.csproj ./IndexerService/IndexerService.csproj

COPY ./IndexerService/packages.config ./IndexerService/packages.config

COPY ./SearchifyEngine/SearchifyEngine.csproj ./SearchifyEngine/SearchifyEngine.csproj

COPY ./SearchifyEngine/packages.config ./SearchifyEngine/packages.config


RUN $nuget restore IndexerService.sln

COPY . .

RUN #msbuild ./IndexerService/IndexerService.csproj /p:PackageLocation="..\PublishOutput\ZipWebApiApp" /p:DeployOnBuild=true /p:Configuration=Release /p:WebPublishMethod=Package /p:DeployTarget=WebPublish /p:AutoParameterizationWebConfigConnectionStrings=false /p:PackageAsSingleFile=true /p:DeployIisAppPath="Default Web Site" /p:SolutionDir="."

RUN msbuild ./IndexerService/IndexerService.csproj /p:DeployOnBuild=true /p:Configuration=Release 


FROM debian:10 AS deploy

WORKDIR /usr/app/

RUN apt-get update -y && apt-get install dirmngr gnupg apt-transport-https ca-certificates -y &&\
    apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF &&\
    sh -c 'echo "deb https://download.mono-project.com/repo/debian stable-buster main" > /etc/apt/sources.list.d/mono-official-stable.list'\
    && apt-get update -y && apt-get install mono-complete -y

COPY --from=0 /usr/app/IndexerService/ ./

EXPOSE 5050

CMD ["/usr/bin/mono-sgen", "/usr/lib/mono/4.5/xsp4.exe", "--port=5050"]
