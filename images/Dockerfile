FROM alpine:3.19

RUN apk add --no-cache bash curl unzip

WORKDIR /app

COPY apk/*.apk ./app.apk

RUN echo "APK metadata:" && \
    ls -la app.apk && \
    file app.apk && \
    apk add --no-cache unzip && \
    unzip -l app.apk | head -20

ENTRYPOINT ["/bin/sh", "-c", "echo 'Safe Steps Addis APK container'; ls -la; echo 'Ready for deployment'"]
