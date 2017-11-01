# docker build . -t nel/jenkins
# docker run -p 8081:8080 -it nel/jenkins bin/bash
# docker exec -it d27cfc5081fe /bin/bash

FROM ubuntu

ENV JENKINS_HOME /data

RUN apt-get update
RUN apt-get install -y sudo

RUN sudo apt-get install -y software-properties-common python-software-properties debconf-utils
RUN sudo add-apt-repository -y ppa:webupd8team/java
RUN sudo apt-get update
RUN echo "oracle-java8-installer shared/accepted-oracle-license-v1-1 select true" | sudo debconf-set-selections
RUN sudo apt-get install -y oracle-java8-installer

RUN wget -q -O - https://pkg.jenkins.io/debian/jenkins.io.key | sudo apt-key add -
RUN sudo sh -c 'echo deb http://pkg.jenkins.io/debian-stable binary/ > /etc/apt/sources.list.d/jenkins.list'
RUN sudo apt-get update
RUN sudo apt-get install -y jenkins

RUN sudo apt-get install -y git

RUN sudo apt-get install -y curl

RUN curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
RUN sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
RUN sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'
RUN sudo apt-get install -y apt-transport-https
RUN sudo apt-get update
RUN sudo apt-get install -y dotnet-dev-1.1.4

VOLUME [ "/data" ]

EXPOSE 8080

CMD ["java", "-jar", "/usr/share/jenkins/jenkins.war"]
