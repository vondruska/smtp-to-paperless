#!/bin/sh

echo "From: <scanner@example.com>
To: <test@test.com>
Subject: Dummy subject
Content-Type: multipart/mixed;
        boundary="XXXXboundary text"

--XXXXboundary text
Content-Type: text/plain

this is the body text

--XXXXboundary text
Content-Type: text/plain;
Content-Disposition: attachment;
        filename="test.txt"

this is the attachment text

--XXXXboundary text--
" | curl -v -T - smtp://localhost:1025 --mail-from scanner@example.com \
--mail-rcpt test2@test.com