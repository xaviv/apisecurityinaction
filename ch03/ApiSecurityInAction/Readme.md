# Api Security In Action: Chapter 3

Using ASP.net core 5 and EF core

## Database In memory for EF Core

Instead of H2 I chose Microsoft.EntityFrameworkCore.InMemory package.
Database is populated after building the host at Main using PrepareDatabase extension method.

## web config

Note web.config file is not added by default, and it is required to remove response headers.

## ASP.net core identity

As EF framework is not used because of Database in memory, custom User, Role and Password stores are needed and explicit User and Role managers need to be declared at Startup.
Book code uses SCrypt as password hasher. By default, Core Identity uses PBKDF2 algorithm with HMAC-SHA256, 128-bit salt, 256-bit subkey and 10k iterations. No need to change it.

## Audit trail

There is not an audit log library at ASP.net core 5. Just for learning purposes, a custom middleware is implemented to store data into the InMemory database.

## Chapter 3:

Check unauthorized access when creating space:
`curl -i -u demo:Password.123 -d "{\"name\": \"test space\", \"owner\": \"demo\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Spaces`

Note book does not wire authenticate process at CreateSpace. Just remove [BasicAuth] from CreateSpace to check owner validation

Now create user demo:
`curl -i -u admin:Admin.1234 -d "{\"username\": \"demo\", \"password\": \"Password.123\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Users`

And create space using the new user:
`curl -i -u demo:Password.123 -d "{\"name\": \"test space\", \"owner\": \"demo\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Spaces`

Dump logs:
`curl -i https://localhost:44364/api/logs`

Attempt to read a message from other space:
`curl -i -u demo:Password.123 https://localhost:44364/api/spaces/1/messages/1`

whereas: 
`curl -i -u admin:Admin.1234 https://localhost:44364/api/spaces/1/messages/1`

Add member:
`curl -i -X POST -u admin:Admin.1234 -H "Content-Type: application/json" -d "{\"userId\": \"demo\", \"perms\":\"r\", \"spaceId\": 1}" https://localhost:44364/api/spaces/1/members`

## Avoiding privilege escalation attacks:

Create users:
`curl -i -d "{\"username\": \"demo2\", \"password\": \"Password.123\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Users`
`curl -i -d "{\"username\": \"evildemo2\", \"password\": \"Password.123\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Users`

Create space for demo:
`curl -i -u demo:Password.123 -d "{\"name\": \"test space\", \"owner\": \"demo\"}" -H  "Content-Type: application/json" https://localhost:44364/api/Spaces`

Add member demo2 with read permissions:
`curl -i -X POST -u demo:Password.123 -H "Content-Type: application/json" -d "{\"userId\": \"demo2\", \"perms\":\"r\", \"spaceId\": 2}" https://localhost:44364/api/spaces/2/members`

Post a message using demo user (will create message number 2):
`curl -i -X POST -u demo:Password.123 -H "Content-Type: application/json" -d "{\"author\": \"demo\", \"text\":\"test message\", \"spaceId\": 2}" https://localhost:44364/api/spaces/2/messages`

Use demo2 (a read-only user) to assign permissions to evildemo2:
`curl -i -X POST -u demo2:Password.123 -H "Content-Type: application/json" -d "{\"userId\": \"evildemo2\", \"perms\":\"rwd\", \"spaceId\": 2}" https://localhost:44364/api/spaces/2/members`

Now evildemo2 has full permissions to space 2, including deleting messages (privilege escalation happened)
`curl -i -X DELETE -u evildemo2:Password.123 https://localhost:44364/api/spaces/2/messages/2`

To fix this vulnerability, just add all policies to SpacesController.AddMember method (comment out Authorize aspects)
