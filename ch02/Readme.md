Using database in memory with EF Core:
dotnet add package Microsoft.EntityFrameworkCore.InMemory

Database is populated after building the host at Main using PrepareDatabase extension method.