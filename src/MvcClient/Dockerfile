FROM microsoft/aspnet:1.0.0-beta7
MAINTAINER Alex Lee <lilu@mequanta.com>
RUN mkdir -p /usr/src/app/source
WORKDIR /usr/src/app/source
COPY . /usr/src/app/source
RUN dnu restore
RUN dnu build
EXPOSE 5004
CMD ["dnx", "kestrel"]
