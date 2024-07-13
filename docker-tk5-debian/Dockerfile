FROM debian:latest

RUN apt update
RUN apt install unzip
COPY mvs-tk5.zip /root
RUN chmod +x /root/mvs-tk5.zip
RUN unzip /root/mvs-tk5.zip -d /root
RUN rm /root/mvs-tk5.zip
WORKDIR /root/mvs-tk5
RUN chmod -R +x *
RUN sed -i '$ d' unattended/set_console_mode
RUN unattended/set_console_mode
ENTRYPOINT [ "./mvs" ]