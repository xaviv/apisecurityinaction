# Api Security In Action: Chapter 2

Using ASP.net core 5 and EF core

## Database In memory for EF Core

Instead of H2 I chose Microsoft.EntityFrameworkCore.InMemory package.
Database is populated after building the host at Main using PrepareDatabase extension method.

## web config

Note web.config file is not added by default, and it is required to remove response headers.

## ASP.net core identity

As EF framework is not used because of Database in memory, custom User, Role and Password stores are needed and explicit User and Role managers need to be declared at Startup.
Book code uses SCrypt as password hasher. By default, Core Identity uses PBKDF2 algorithm with HMAC-SHA256, 128-bit salt, 256-bit subkey and 10k iterations. No need to change it.

## Chapter 2 attacks:

Because of using EF core as ORM, SQL injection attacks are avoided, so this does not return a 500 response code as book says:
`curl -i -d "{\"name\": \"test'space\", \"owner\": \"demo\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Spaces`

Of course this does not work either (it will add that text in owner field):
`curl -i -d "{\"name\": \"test\", \"owner\": \"'); DROP TABLE spaces; --\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Spaces`

Again, using an ORM prevents from this kind of failures:
`curl -i -d "{\"name\": \"test\", \"owner\": \"a really long username that is more than 30 characters long\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Spaces`

XSS exploit (file Resources/xss.html): Modified to use https, the right port, and api at the URL.
This exploit cannot be used with ASP.net CORE, as it does not accept text/plain by default
