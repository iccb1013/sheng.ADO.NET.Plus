# sheng.ADO.NET.Plus

cao.silhouette@msn.com

The core implementation of the code is simple and straightforward. I adhere to the principle of "just enough" and "solving the problem," without considering scenarios involving massive data sets (millions or billions of records, etc.) in the initial version. However, if I encounter such a scenario in my work, I will upgrade the system and address the issue.

This component was written around 2015, and the creation of sheng.ADO.NET.Plus wasn’t just about building a tool for fun. It was born out of real problems I encountered in my own project development: the inconvenience of using Entity Framework in certain environments and the limitations of directly using ADO.NET. I needed a component that sits between the two, offering high flexibility.

The main features of the Shengxunwei ADO.NET enhancement component, sheng.ADO.NET.Plus, are as follows:

- Supports all native database operations (based on the Microsoft Enterprise Library data modules, with integrated logging for all database operation exceptions).
- Removes the one-to-one correspondence between the database table model and the entity class, allowing developers to specify mappings flexibly.
- Supports direct SQL queries and dynamic mapping based on query results in memory.
- Supports calling stored procedures with dynamic mapping based on query results.
- Provides automated transaction handling, with automatic rollback.
- Supports one-to-many mapping, allowing an entity class to map to multiple tables.
- Supports automatic filling and completion of data in entity classes.
- Supports various granular memory dynamic mappings for DataSet, DataTable, and DataRow.
- Provides a simple SQL builder, with automatic generation of simple SQL statements for unmapped models.
- Supports detailed handling of entity fields, such as automatically writing a property marked with `JsonAttribute` in JSON format to a database field.
- High performance, high flexibility, and high maintainability.

Now, let’s look at the usage example in the code:

Assume we have a `User` table with four fields: Id, Name, Age, and ExtraInfo.

We define a simple `User` class (other tools can also be used to auto-generate this):

```csharp
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string ExtraInfo { get; set; }
}
```

Initialize the Shengxunwei ADO.NET enhancement component core class `DatabaseWrapper`:

```csharp
private DatabaseWrapper _dataBase = DatabaseUnity.Database;
```

### 1. Simple Data Operations

1. **Insert Data:**

```csharp
public void AddUser(User user)
{
    _dataBase.Insert(user);
}
```

The `Insert` method prototype is:

```csharp
public bool Insert(object obj)
```

The `Insert` method automatically parses the object instance passed, analyzes the object type (e.g., `User`), and its properties, then dynamically maps the `User` table and fields, inserting the data into the table.

2. **Query Data:**

```csharp
public List<User> GetUserList()
{
    return _dataBase.Select<User>();
}
```

The `Select` method works similarly by automatically parsing the object type to get the table and field information and then performing the query and data population.

3. **Update Data:**

```csharp
public void UpdateUser(User user)
{
    _dataBase.Update(user);
}
```

Unlike some ORM frameworks that track object instance changes, the Shengxunwei ADO.NET enhancement component doesn’t use this method, but directly updates the database table based on the submitted object instance.

4. **Delete Data:**

```csharp
public void RemoveUser(User user)
{
    _dataBase.Remove(user);
}
```

It’s important to note that for update and delete operations using the simple method above, the primary key field must be specified in the entity class:

```csharp
public class User
{
    [Key]
    public Guid Id { get; set; }
    // ...
}
```

At this point, we have automated basic database operations.

### 2. Custom Entity Class to Database Table Mapping

In the above simple CRUD operations, the object type is used to derive the type name and properties, but what if the entity class name differs from the database table name, or if the entity properties don’t match the database fields?

This is a common scenario in large projects. For complex database table designs, the business layer might interpret them differently. For example, you might have a user table with product-related information, such as basic and extended info. At the business level, you may want to map these to two different entity objects: one for basic info and another for extended info, where the fields may differ but share some common ones like Id and Name.

As mentioned earlier, the Shengxunwei ADO.NET enhancement component doesn’t impose a strict mapping between entity classes and database tables. The number of fields in the table and properties in the entity class doesn’t need to match. You can simply use attributes to specify whether a property should be mapped.

1. **Table Name Mapping:**

We define two different entity classes:

```csharp
[Table("User")]
public class User_BaseInfo
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

[Table("User")]
public class User_ExtraInfo
{
    [Key]
    public Guid Id { get; set; }
    public string ExtraInfo { get; set; }
}
```

By adding the `TableAttribute` above the class definitions, we can now use the same CRUD operations for instances of `User_BaseInfo` or `User_ExtraInfo`.

2. **Field Mapping in the Database Table:**

This is not a typical field-to-property mapping like other ORMs, but a mapping for result set fields. For example, fields in complex SQL queries or stored procedure result sets might not correspond to the database table’s fields.

For certain scenarios, you might need additional properties in your entity class to store specific information or implement specific functions, which don’t need to be persisted in the database. Or the property names in the entity class might differ from the database field names.

```csharp
[Table("User")]
public class User_ExtraInfo
{
    [Key]
    public Guid Id { get; set; }

    [Column("ExtraInfo")]
    public string Infomation { get; set; }

    [NotMapped]
    public int Count { get; set; }
}
```

Simply add `ColumnAttribute` or `NotMapped` to the properties, and the CRUD operations will work as expected.

3. **Many-to-Many Mapping for Entity Classes to Database Tables:**

This functionality allows us to strongly type the two-dimensional database table (or result set). In most ORM frameworks, with complex database structures, there might be many field definitions, but in our actual business logic, these fields might belong to different logical categories. We may only need to transmit or expose part of the data, not the entire object.

```csharp
public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }

    [Partial]
    public ExtraInfo ExtraInfo { get; set; }
}

public class ExtraInfo
{
    public string ExtraInfo { get; set; }
}
```

Adding the `PartialAttribute` indicates that the `ExtraInfo` property is a part of the current data set’s fields.

### 3. Advanced Operations

1. **Advanced Querying**

The Shengxunwei ADO.NET enhancement component provides additional methods for advanced data querying:

- Basic Query
- Add Extra Query Conditions
- Query via SQL
- Parameterized SQL Queries

2. **Dynamic Mapping with DataSet**

If we receive a `DataSet` from a stored procedure or another source, the component supports dynamic mapping to strongly typed object instances.

3. **Data Population**

You can use the `Fill` method to populate an entity object based on a known condition.

4. **SQL Query Builder**

The component provides an SQL query builder to generate SQL statements, reducing the workload and error-prone nature of writing SQL manually.

5. **Transactions**

The Shengxunwei ADO.NET enhancement component automatically wraps consecutive database operations in a transaction. If any operation fails, it will automatically roll back.

### 4. Native Operations

The Shengxunwei ADO.NET enhancement component also supports native database operations, allowing for simple, efficient, and flexible database operations based on the features mentioned above.

By leveraging dynamic mapping between the entity class and database table, combined with native SQL, stored procedures, custom functions, and views, this approach combines the advantages of ORM automation while fully utilizing the powerful capabilities of native database operations. It simplifies repetitive tasks and automates data mapping and value assignment. Complex business scenarios are handled manually, allowing for high efficiency and quality in the development process.
