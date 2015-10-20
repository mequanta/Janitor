FROM mono:4.0.0
MAINTAINER Alex Lee <lilu@mequanta.com>
RUN mkdir -p /usr/src/app/source
WORKDIR /usr/src/app/source
COPY . /usr/src/app/source
RUN nuget restore -NonInteractive
RUN xbuild /property:Configuration=Debug
EXPOSE 44002
CMD ["mono", "Janitor/bin/Debug/Janitor.exe"]
