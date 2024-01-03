# Redact PII

🚧 Under Construction 🚧

## Usage

POST to the `/redact-pii` endpoint and providing an image in the form data, using the key "file". The endpoint will redact any PII detected in the image, using solid black rectangles, and return the redacted image as a PNG.

A Postman collection with a request for the endpoint is included in the [samples](/samples) folder for convenience.

## Example

<div>
  <img src="/samples/credit-card-input/unredacted.png" alt="unredacted screenshot of credit card form" width="350px" />
  <img src="/samples/credit-card-input/redacted-openai.png" alt="unredacted screenshot of credit card form" width="350px" />
</div>

## PII Detection Providers

The project is currently setup to use Azure's PII Detection service and/or a custom prompt using the OpenAI API with GPT-4. Each PII detection provider can be enabled or disabled via appsettings.

### Prompt used for GPT-4

```
You are detecting personally identifiable information (PII) in the provided text.
List each token or group of tokens in the text that may contain PII (for example: credit card numbers, security codes, names, addresses).
Do not modify or change the text in any way, or add labels.
Exclude labels, descriptive text, other text elements which may refer to or label PII, but are not actually PII themselves (for example: "Card number", "Expiration", "Country").
Also exclude text artifacts, incorrectly extracted text, or miscellaneous text that is unrelated to the PII.
Display each piece of PII as-is with no additional quotes, symbols, or other characters:

{textContent}
```

## Troubleshooting

### Postman: This request does not have a Content-Type header.

If you see the following error while using Postman, try removing the selected file from the form data and re-adding it. This seems to happen when Postman loses track of the file, after the underlying file is moved or occasionally when closing and reopening Postman.

```
System.InvalidOperationException: This request does not have a Content-Type header. Forms are available from requests with bodies like POSTs and a form Content-Type of either application/x-www-form-urlencoded or multipart/form-data.
```
