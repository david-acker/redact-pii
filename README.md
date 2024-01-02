# Blur PII

🚧 Under Construction 🚧

## Usage

POST to the `/redact-pii` endpoint and providing an image in the form data, using the key "file". The endpoint will redact any PII detected in the image, using solid black rectangles, and return the redacted image as a PNG.

A Postman collection with a request for the endpoint is included in the [samples](/samples) folder for convenience.

## Troubleshooting

### Postman: This request does not have a Content-Type header.

If you see the following error while using Postman, try removing the selected file from the form data and re-adding it. This seems to happen when Postman loses track of the file, after the underlying file is moved or occasionally when closing and reopening Postman.

```
System.InvalidOperationException: This request does not have a Content-Type header. Forms are available from requests with bodies like POSTs and a form Content-Type of either application/x-www-form-urlencoded or multipart/form-data.
```
