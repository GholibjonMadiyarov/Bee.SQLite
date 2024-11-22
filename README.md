# Bee.SQLite
A very simple way to work with the database

## Dependencies
	System.Data.SQLite 1.0.117

## Examples

### Select without parameters
```csharp
using Bee.SQLite;

static void Main(string[] args)
{
	SQLite.connectionString = "data source=Company.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000";
    	var select = SQLite.select("select id, name, lastname, age from users");
	
	foreach(var item in select.data)
	{
		Console.WriteLine("id:" + item["id"] + ", name:" + item["name"] + ", lastname:" + item["lastname"] + ", age:" + item["age"]);
	}
}
```

### Select with parameters
```csharp
using Bee.SQLite;

static void Main(string[] args)
{
	SQLite.connectionString = "data source=Company.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000";
	var select = SQLite.select("select id, name, lastname, age from users where id = @user_id", new Dictionary<string, object>{{"@user_id", 1}});

	foreach(var item in select.data)
	{
		Console.WriteLine("id:" + item["id"] + ", name:" + item["name"] + ", lastname:" + item["lastname"] + ", age:" + item["age"]);
	}
}
```

### Sample result check
```csharp
using Bee.SQLite;

static void Main(string[] args)
{
	SQLite.connectionString = "data source=Company.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000";
	var select = SQLite.select("select id, name, lastname, age from users");
	
	if(select.execute)
	{
		foreach(var item in select.data)
		{
			Console.WriteLine("id:" + item["id"] + ", name:" + item["name"] + ", lastname:" + item["lastname"] + ", age:" + item["age"]);
		}
	}
	else
	{
		Console.WriteLine("No result!" + select.message);
	}
}
```

### Insert example
```csharp
using Bee.SQLite;

static void Main(string[] args)
{
	SQLite.connectionString = "data source=Company.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000";
	var insert = SQLite.insert("insert into users(name, lastname, age) values('Gholibjon', 'Madiyarov', 29)");
	
	if(insert.execute)
	{
		Console.WriteLine("Request completed successfully " + insert.message);
	}
	else
	{
		Console.WriteLine("Request failed " + insert.message);
	}
}
```

### Insert example with parameters
```csharp
using Bee.SQLite;

static void Main(string[] args)
{
	SQLite.connectionString = "data source=Company.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000";
	var query = SQLite.insert("insert into users(name, lastname, age) values(@name, @lastname, @age)", new Dictionary<string, object>{{"@name", "Gholibjon"}, {"@lastname", "Madiyarov"}, {"@age", 29}});
	
	if(insert.execute)
	{
		Console.WriteLine("Request completed successfully " + insert.message);
	}
	else
	{
		Console.WriteLine("Request failed " + insert.message);
	}
}
```

### Insert example with transaction
```csharp
using Bee.SQLite;

static void Main(string[] args)
{
	SQLite.connectionString = "data source=Company.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000";
	
	var queries = new List<string>(){
		"insert into users(name, lastname, age) values('Gholibjon', 'Madiyarov', 29)",
		"insert into cities(name, description) values('Hujand', 'This is one of the most civilized and hospitable cities in Central Asia.')",
		"insert into cars(name, description) values('Mercedes Benz', 'One of the most perfect and friendly cars in the world.')"
	};
	
	var insert = SQLite.insert(queries);
	
	if(insert.execute)
	{
		Console.WriteLine("Request completed successfully " + insert.message);
	}
	else
	{
		Console.WriteLine("Request failed " + insert.message);
	}
}
```

### Insert example with transaction, with parameters
```csharp
using Bee.SQLite;

static void Main(string[] args)
{
	SQLite.connectionString = "data source=Company.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000";
	
	var queries = new List<string>(){
		"insert into users(name, lastname, age) values(@name, @lastname, @age)",
		"insert into cities(name, description) values(@name, @description)",
		"insert into cars(name, description) values(@name, @description)"
	};
	
	var parameters = new List<Dictionary<string, object>>{
		new Dictionary<string, object>{{"@name", "Gholibjon"}, {"@lastname", "Madiyarov"}, {"@age", 29}},
		new Dictionary<string, object>{{"@name", "Hujand"}, {"@description", "This is one of the most civilized and hospitable cities in Central Asia."}},
		new Dictionary<string, object>{{"@name", "Mercedes Benz"}, {"@description", "One of the most perfect and friendly cars in the world."}},
	};
	
	var insert = SQLite.insert(queries, parameters);
	
	if(insert.execute)
	{
		Console.WriteLine("Request completed successfully " + insert.message);
	}
	else
	{
		Console.WriteLine("Request failed " + insert.message);
	}
}
```

### Insert example with transaction, with null parameters
```csharp
using Bee.SQLite;

static void Main(string[] args)
{
	SQLite.connectionString = "data source=Company.db;version=3;page size=4096;cache size=10000;journal mode=Wal;pooling=True;legacy format=False;default timeout=15000";
	
	var queries = new List<string>(){
		"insert into users(name, lastname, age) values(@name, @lastname, @age)",
		"insert into cities(name, description) values('Hujand', 'This is one of the most civilized and hospitable cities in Central Asia.')",
		"insert into cars(name, description) values(@name, @description)"
	};
	
	var parameters = new List<Dictionary<string, object>>{
		new Dictionary<string, object>{{"@name", "Gholibjon"}, {"@lastname", "Madiyarov"}, {"@age", 29}},
		null,
		new Dictionary<string, object>{{"@name", "Mercedes Benz"}, {"@description", "One of the most perfect and friendly cars in the world."}},
	};
	
	var insert = SQLite.insert(queries, parameters);
	
	if(insert.execute)
	{
		Console.WriteLine("Request completed successfully " + insert.message);
	}
	else
	{
		Console.WriteLine("Request failed " + insert.message);
	}
}
```
