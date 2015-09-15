FROM mono:4.0.0
MAINTAINER Alex Lee <lilu@mequanta.com>
RUN mkdir -p /usr/src/app/source /usr/src/app/build
WORKDIR /usr/src/app/source
COPY . /usr/src/app/source
RUN nuget restore -NonInteractive
RUN xbuild
EXPOSE 44002
CMD ["mono", "SelfHost/bin/Debug/SelfHost.exe"]
