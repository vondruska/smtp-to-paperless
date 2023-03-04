# smtp-to-paperless

An SMTP proxy to allow older scanners with rudimentary SMTP capabilities to upload documents into Paperless, a document management system.

Point your scanner to this SMTP server. No SMTP credentials (my scanner doesn't support credentials). No TLS. Any attachments as apart of the email from the scanner is uploaded to Paperless.

> **Warning**
To reiterate: **No** SMTP credentials. **No** TLS. Do **not** run this on an untrusted network, like the public internet.

## Running
Pull the Docker container and expose container port 1025 along with envvars for configuration (see below). Port 1025 allows you to run the container as a non-root user without extending additional permissions. You can use Docker to map port 1025 back to 25 if so desired.

Docker:
```shell
docker run --rm -it -p 1025:1025 -e APP_PAPERLESSBASEURL=https://paperless.example.com -e APP_PAPERLESSUSERNAME=example -e APP_PAPERLESSPASSWORD=hunter2 vondruska/smtp-to-paperless
```

Docker Compose:

```yaml
version: "3"

services:
  smtp-to-paperless:
    image: vondruska/smtp-to-paperless
    environment:
      - APP_PAPERLESSBASEURL=https://paperless.example.com
      - APP_PAPERLESSUSERNAME=example
      - APP_PAPERLESSPASSWORD=hunter2
    ports:
      - 1025:1025
```

## Configuration 

Configuration is done with environment variables

| Variable Name         | Description |
| --------------------- | ----------- |
| APP_PAPERLESSBASEURL  | Base url for your instance of Paperless. For example, https://paperless.example.com       |
| APP_PAPERLESSUSERNAME | Username to use to authenticate with Paperless        |
| APP_PAPERLESSPASSWORD | Password to use to authenticate with Paperless            |