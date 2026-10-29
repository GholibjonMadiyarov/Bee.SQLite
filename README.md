# Bee.SQLite

A lightweight static wrapper over System.Data.SQLite. One call — one result object, no exceptions thrown.

## Requirements

- .NET Framework 4.0+
- System.Data.SQLite 1.0.119

## Setup

```csharp
using Bee.SQLite;

SQLite.connectionString = "data source=Company.db;version=3;journal mode=Wal;pooling=True;default timeout=15000";
```

## Methods

| Method | Purpose | Main result |
|---|---|---|
| `select` | All rows | `data` — `List<Dictionary<string, object>>` |
| `selectString` | All rows, values as strings | `data` — `List<Dictionary<string, string>>` |
| `selectRow` | First row | `data`, `read` |
| `selectValue` | First value | `value` (`object`), `read` |
| `selectValueString` | First value as string | `value` (`string`), `read` |
| `selectValueInteger` | First value as int | `value` (`int?`), `read` |
| `insert` | INSERT | `insertedId` / `insertedIds`, `duplicate` |
| `update` | UPDATE | `affectedRowCount` |
| `delete` | DELETE | `affectedRowCount` |
| `query` | Any SQL | `data`, `affectedRowCount`, `duplicate` |

Every result has `execute` (success), `message` and `stackTrace`.
`read` is `false` when the query returned no rows.
`duplicate` is `true` on a UNIQUE or PRIMARY KEY violation.

Methods that take a list of queries (`insert`, `update`, `query`) run them in one transaction: if one fails, all are rolled back. Parameters are passed in the same order; an item may be `null`.

## Examples

### Select

```csharp
var users = SQLite.select("select id, name from users where age > @age",
    new Dictionary<string, object> { { "@age", 18 } });

if (users.execute)
{
    foreach (var row in users.data)
        Console.WriteLine(row["id"] + " " + row["name"]);
}
else
{
    Console.WriteLine(users.message);
}
```

### Single row and value

```csharp
var user = SQLite.selectRow("select * from users where id = @id",
    new Dictionary<string, object> { { "@id", 1 } });

if (user.read)
    Console.WriteLine(user.data["name"]);

int? count = SQLite.selectValueInteger("select count(*) from users").value;
```

### Insert

```csharp
var insert = SQLite.insert("insert into users(name, age) values(@name, @age)",
    new Dictionary<string, object> { { "@name", "Gholibjon" }, { "@age", 29 } });

if (insert.execute)
    Console.WriteLine("New id: " + insert.insertedId);
else if (insert.duplicate)
    Console.WriteLine("Already exists");
```

### Insert in a transaction

```csharp
var queries = new List<string>
{
    "insert into users(name) values(@name)",
    "insert into cities(name) values('Khujand')"
};

var parameters = new List<Dictionary<string, object>>
{
    new Dictionary<string, object> { { "@name", "Gholibjon" } },
    null
};

var insert = SQLite.insert(queries, parameters);
// insert.insertedIds — id of each inserted row
```

### Update and delete

```csharp
var update = SQLite.update("update users set age = @age where id = @id",
    new Dictionary<string, object> { { "@age", 30 }, { "@id", 1 } });

var delete = SQLite.delete("delete from users where id = @id",
    new Dictionary<string, object> { { "@id", 1 } });

Console.WriteLine(update.affectedRowCount + " / " + delete.affectedRowCount);
```

### Query

Runs any statement. Rows are in `data[0]`. To get a new id, use `RETURNING`:

```csharp
var q = SQLite.query("insert into users(name) values(@name) returning id",
    new Dictionary<string, object> { { "@name", "Ali" } });

long id = (long)q.data[0][0]["id"];
```

Several statements in one transaction; `data[i]` holds the rows of query `i`:

```csharp
var q = SQLite.query(new List<string>
{
    "update users set age = age + 1",
    "select count(*) as total from users"
});

Console.WriteLine(q.affectedRowCount);       // changed rows
Console.WriteLine(q.data[1][0]["total"]);    // result of the 2nd query
```

## Encryption

`setPassword` and `changePassword` require the paid [SQLite Encryption Extension (SEE)](https://www.sqlite.org/see). Without it they return `false`.
