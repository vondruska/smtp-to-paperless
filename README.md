# smtp-to-paperless

An SMTP proxy to allow older scanners with rudimentary SMTP capabilities to upload documents into Paperless, a document management system.

Point your scanner to this SMTP server. No SMTP credentials (my scanner doesn't support credentials). No TLS. Any attachments as apart of the email from the scanner is uploaded to Paperless.

> **Warning**
To reiterate: **No** SMTP credentials. **No** TLS. Do **not** run this on an untrusted network, like the public internet.

## Running
Pull the Docker container and expose container port 1025 along with envvars for configuration (see below). Port 1025 allows you to run the container as a non-root user without extending additional permissions. You can use Docker to map port 1025 back to 25 if so desired.

Docker:
```shell
docker run --rm -it -p 1025:1025 -e APP_PAPERLESSBASEURL=https://paperless.example.com -e APP_PAPERLESSUSERNAME=example -e APP_PAPERLESSPASSWORD=hunter2 ghcr.io/vondruska/smtp-to-paperless:main
```

Docker Compose:

```yaml
version: "3"

services:
  smtp-to-paperless:
    image: ghcr.io/vondruska/smtp-to-paperless:main
    environment:
      - APP_PAPERLESSBASEURL=https://paperless.example.com
      - APP_PAPERLESSUSERNAME=example
      - APP_PAPERLESSPASSWORD=hunter2
    ports:
      - 1025:1025
```

## Configuration 

Configuration is done with environment variables or by a settings.json file. 

Environment variables names must be prefixed with "APP_" (case insensitive) then the key below. For example, for the Paperless Base URL, you'll need an envvar key of `APP_PAPERLESSBASEURL`.

A JSON file named `settings.json` will also be read. An example file looks like:

```json
{
  "PaperlessBaseUrl": "https://paperless.example.com"
}
```

If a key exists in both the environment variable and json configuration, the environment variable wins.

| Variable Name         | Description |
| --------------------- | ----------- |
| PaperlessBaseUrl  | Base url for your instance of Paperless. For example, https://paperless.example.com       |
| PaperlessUsername | Username to use to authenticate with Paperless        |
| PaperlessPassword | Password to use to authenticate with Paperless            |