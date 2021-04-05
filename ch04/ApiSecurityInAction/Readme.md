# Api Security In Action: Chapter 4

Session cookie authentication

## Chapter 4:

UseStaticFiles must be placed before the explicit security headers.

## Basic authentication in web browsers

Create user test for basic authentication:
`curl -i -u admin:Admin.1234 -d "{\"username\": \"test\", \"password\": \"Password.123\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Users`

And connect to https://localhost:44364/natter.html

## Token-based authentication

AddDistributedMemoryCache, AddSession and UseSession are added at Startup. In order to emulate Spark behavior, I added a JSESSIONID header to the response at ITokenStore.

Create user test (if not created before):
`curl -i -u admin:Admin.1234 -d "{\"username\": \"test\", \"password\": \"Password.123\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Users`

Create session:
`curl -i -u test:Password.123 -H "Content-Type: application/json" -X POST -d '' https://localhost:44364/api/sessions`