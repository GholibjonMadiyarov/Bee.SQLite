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
| `insert` | INSERT | `insertedId`, `insertedIds`, `duplicate` |
| `update` | UPDATE | `affectedRowCount`, `exception` |
| `delete` | DELETE | `affectedRowCount`, `exception` |
| `query` | Any SQL | `data`, `affectedRowCount`, `insertedId` / `insertedIds`, `duplicate` |

Every result has `execute` (success), `message` and `stackTrace`.
`read` is `false` when the query returned no rows.
`duplicate` is `true` on a UNIQUE or PRIMARY KEY violation.

`update` and `delete` return `execute = false` when no rows match. Use `exception` to tell the cases apart:

| Case | `execute` | `exception` | `message` |
|---|---|---|---|
| Rows changed | `true` | `false` | Request completed successfully |
| No rows matched | `false` | `false` | No rows were changed |
| SQL error | `false` | `true` | Request failed. … |

Methods that take a list of queries (`insert`, `update`, `query`) run them in one transaction. Parameters are passed in the same order; an item may be `null`. All queries are rolled back if:

- a query fails, or
- an INSERT, REPLACE, UPDATE or DELETE changes no rows (this includes `INSERT OR IGNORE` that skips a duplicate).

In the second case `execute` is `false`, and `queryText` shows the query that changed nothing (`insert` and `update` also return its `parameter`).

```csharp
var u = SQLite.update(queries, parameters);

if (!u.execute)
    Console.WriteLine(u.message + ": " + u.queryText);
```

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
// insert.insertedIds[i] — id added by query i (or null)
// insert.insertedId     — the last added id
```

### Update and delete

```csharp
var update = SQLite.update("update users set age = @age where id = @id",
    new Dictionary<string, object> { { "@age", 30 }, { "@id", 1 } });

var delete = SQLite.delete("delete from users where id = @id",
    new Dictionary<string, object> { { "@id", 1 } });

if (update.execute)
    Console.WriteLine("Updated: " + update.affectedRowCount);
else if (!update.exception)
    Console.WriteLine("Nothing to update");
else
    Console.WriteLine(update.message);
```

### Query

Runs any statement. Rows are in `data[0]`. For a query that starts with `INSERT` or `REPLACE` and adds a row, `insertedId` holds the new rowid:

```csharp
var q = SQLite.query("insert into users(name) values(@name)",
    new Dictionary<string, object> { { "@name", "Ali" } });

if (q.execute)
    Console.WriteLine("New id: " + q.insertedId);
```

Several statements in one transaction. Index `i` refers to query `i`: `data[i]` holds its rows, `insertedIds[i]` its new rowid (or `null`):

```csharp
var q = SQLite.query(new List<string>
{
    "update users set age = age + 1",
    "insert into logs(text) values('age updated')",
    "select count(*) as total from users"
});

Console.WriteLine(q.affectedRowCount);       // changed rows in total
Console.WriteLine(q.insertedIds[1]);         // id from the 2nd query
Console.WriteLine(q.data[2][0]["total"]);    // result of the 3rd query
```

## Notes

- **Query type is detected by its first word.** `affectedRowCount`, `insertedId` and the "no rows changed" rollback in `query` work only for queries that start with `INSERT`, `REPLACE`, `UPDATE` or `DELETE`. A query that starts with `WITH` or a comment runs normally, but reports `affectedRowCount = 0` and `insertedId = null`.
- **CREATE, DROP, PRAGMA and SELECT** report `affectedRowCount = 0`.
- **One text, several statements** (`insert ...; insert ...`): all run, `affectedRowCount` is the total, `insertedId` is the last id. With several SELECTs only the first result is returned.
- **Missing parameter:** a parameter used in the SQL but not passed gives `execute = false` (it is not sent as NULL).
- **Parameter types:** `bool` is stored as `1`/`0`; `DateTime` as text `yyyy-MM-dd HH:mm:ss`; `Guid` as BLOB, so search it with a `Guid` value, not a string.
- **`selectValueInteger`:** a value outside the `int` range gives `execute = false`.
- **Locked database:** if another connection holds a write lock longer than `default timeout`, the method returns `execute = false` with "database is locked". Reads keep working in WAL mode.

## Encryption

`setPassword` and `changePassword` require the paid [SQLite Encryption Extension (SEE)](https://www.sqlite.org/see). Without it they return `false`.

- `changePassword` changes the password of the database.
- `setPassword` only checks that the database opens with the password. To use an encrypted database, add `password=...` to `connectionString`.
