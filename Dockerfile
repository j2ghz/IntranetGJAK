FROM microsoft/aspnet:1.0.0-rc1-update1-coreclr

RUN curl -sL https://deb.nodesource.com/setup_5.x | bash -
RUN apt-get update && apt-get install -y nodejs build-essential git-core

COPY . /app
WORKDIR /app
RUN ["dnu", "restore"]
WORKDIR /app/IntranetGJAK
RUN ["npm", "install"]
RUN ./node_modules/.bin/bower install --allow-root
RUN ./node_modules/.bin/gulp

EXPOSE 5000
ENTRYPOINT ["dnx", "-p", "IntranetGJAK/project.json", "web"]
