# Api Security In Action: Chapter 4

Session cookie authentication

## Chapter 4:

UseStaticFiles must be placed before the explicit security headers.

## 4.1 Authentication in web browsers

Create user test for basic authentication:
`curl -i -u admin:Admin.1234 -d "{\"username\": \"test\", \"password\": \"Password.123\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Users`

## 4.2 Token-based authentication