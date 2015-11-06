# Base of your container
FROM microsoft/aspnet:latest
RUN ["ls"]
# Copy the project into folder and then restore packages
COPY . /app
WORKDIR /app
RUN ["ls"]
RUN ["dnu","restore"]

# Open this port in the container
EXPOSE 5000
# Start application
ENTRYPOINT ["dnx","-p","project.json", "web"]
RUN ["tree"]
