FROM mono:onbuild
EXPOSE 44002
CMD ["mono", "SelfHost.exe"]
