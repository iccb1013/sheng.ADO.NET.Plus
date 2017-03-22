更好的排版见这里：
http://www.cnblogs.com/sheng_chao/p/6597672.html

=====

目前上传的最新版本有一些新功能特性，还有一些细节调整有兴趣的自己看一下代码。

代码的核心实现简单粗暴，我奉行够用就好，解决问题就好的思路，不会在最初的版本中就考虑上千万上亿数据balabala之类的问题，但是如果我在工作中遇到了这样的场景，我会去升级它并解决这样的问题。

这个组件是我前两年写的，可能和现在流行的 dapper 有一些类似，当时我并不知道有 dapper，如果知道的话可能我就直接使用 dapper了。我写  sheng.ADO.NET.Plus 并不是闲的无聊要造个轮子玩，而是我在自己的项目开发中，切实遇到了一些问题需要解决：使用EF带来的不便和直接使用ADO.NET带来的不便，我需要一个介于两者之间的，高度自由的组件。

=====

 

目前我们所接触到的许多项目开发，大多数都应用了 ORM 技术来实现与数据库的交互，ORM 虽然有诸多好处，但是在实际工作中，特别是在大型项目开发中，容易发现 ORM 存在一些缺点，在复杂场景下，反而容易大大增加开发的复杂度及牺牲灵活度。使用 ORM 不写 SQL 而使数据库交互变得简单易行，是否能够达到预期效果，要画一个问号。

主要问题可能存在于以下几点：

1.大幅度牺牲性能，这里的性能问题不是指什么单表写入100万次的性能对比，而是指基于如EF这样的框架开发的项目的整体开发模式和特点造成的性能低下。

2.虽然隐藏了数据层面的设计，但并没有从根本上降低数据访问复杂度，只是将复杂纬度从一个点（SQL，存储过程）转移到另一个点（代码），以EF为例，最终生成的代码性能与C#书写有很大关系，且难以通过成熟的数据库技术反查性能瓶颈。

3.对于复杂查询，ORM 力不从心，虽然从技术角度说实现肯定都能实现，但是代价是不值的。

 

有朋友认为 ORM 可以使不懂数据库的开发人员也能在开发中轻松实现与数据库的交互，但是，在大型项目中，让不懂数据库的开发人员做这块工作，Are you kidding me?

 

在我自己的项目开发经验中，ORM 还存在以下问题：

1.对于大型项目的开发，表示数据的实体类和数据库层面的持久化设计并非一一对应的关系，使用ORM根据数据库表生成一一对应的实体类模型，并不能完全适用，这是促使我实现自己的增强组件的重要原因之一；

2.在实体类中，需要进行其它编码工作，如额外的属性定义，附加额外的Attribute，部分功能实现和业务操作等，而使用ORM来生成实体类，生成时会覆盖现有实体类而导致项目自身的编码工作丢失；

 

直接使用 ADO.NET 又在很多时候过于繁琐，特别是取值赋值的过程非常冗余又麻烦，那能不能设计一种机制，既能拥有 ORM 所带来的一些便利，又不失 ADO.NET 的高性能和自由度呢？

基于这样的目标，我设计实现了升讯威ADO.NET增强组件。

 

升讯威ADO.NET增强组件  sheng.ADO.NET.Plus 有以下几个特点：

1.支持所有数据库原生操作（基于微软企业库的数据模块，并集成了日志模块，所有数据库操作异常使用企业库写日志）

2.解除与数据库表模型一一对应的关系，由开发人员灵活指定映射关系。

3.支持直接使用SQL语句并根据查询结果在内存中动态映射。

4.支持调用存储过程并根据查询结果动态映射。

5.支持自动化的事务处理，可自动回滚。

6.支持一对多的映射关系，即一个实体类可以映射到多张表。

7.支持自动填充/补全数据实体类中的数据。

8.支持DataSet、DataTable、DataRow多种粒种的内存动态映射。

9.支持简单SQL构造器，支持自动生成简单的无模型映射的SQL语句。

10.支持对实体字段的精细化处理，如将实体对象的任意 Property 标记 JsonAttribute 后，将自动以 Json 格式写入字段。

10.高性能，高灵活性，高可维护性。

  

下面直接从代码示例中看使用效果：

 

现在假定有 User 表，包括四个字段：Id，Name，Age，ExtraInfo。

我们定义一个简单的 User 类。（亦可使用其它工具自动生成）。

 

复制代码
public class User
    {
        public Guid Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Age
        {
            get;
            set;
        }

        public string ExtraInfo
        {
            get;
            set;
        }
    }
复制代码
 

初始化升讯威ADO.NET增强组件核心类 DatabaseWrapper，

private DatabaseWrapper _dataBase = DatabaseUnity.Database;
 

一、简单数据操作
 

1.插入一条数据：

public void AddUser(User user)
        {
            _dataBase.Insert(user);
        }
升讯威ADO.NET增强组的 Insert 方法原型是：

public bool Insert(object obj)
Insert 方法会自动解析传入的对象实例，分析对象的类型名称（User）及其所包括的属性（Property），自动实现对User表及各字段的动态映射，将数据插入到表中。

 

2.查询数据

public List<User> GetUserList()
        {
            return _dataBase.Select<User>();
        }
此处原理同上文一样，Select 方法自动解析对象类型，得到表，字段信息，实现数据的查询与填充。

 

3.修改数据

public void UpdateUser(User user)
        {
            _dataBase.Update(user);
        }
有些ORM框架，使用跟踪对象实例的变化的方式，基于特定对象提交数据，但是这种方式的开销非常大，升讯威ADO.NET增强组没有采用这种方式，而是直接根据提交的对象实例，更新数据库表。

 

4.删除数据

public void RemoveUser(User user)
        {
            _dataBase.Remove(user);
        }
 

需要注意的是，使用上文中的简单方式进行修改及删除操作，必须在实体类中指定主键字段：

复制代码
public class User
    {
        [Key]
        public Guid Id
        {
            get;
            set;
        }

       ......
    }
复制代码
 

至此我们实现了基本的数据库操作的自动化。

是不是很熟悉，和Entity Framework很类似是不是？ 

 

二、自定义实体类与数据库表的映射关系
上文中的简单增删改查操作，是根据对象实例得到对象类型从而得到类型名称和属性（Property）集合及他们的名称，那么如果实体类型的名称与数据库表名称并不一样怎么办呢？如果数据实体的属性（Property）与数据库表字段并不一一对应怎么办呢？

在大型项目中，这种情况是经常存在的，对于复杂的数据库表设计，到了业务层，可能会有不同的解释方法，例如我有一张用户表，包含了产品不同维度的信息：基本信息、扩展信息等。到了业务实现层面，我希望展开为两个不同的实体对象进行操作，基本信息对象和扩展信息对象。他们所使用的字段可能不太相同，却又包括了某些共通的字段，如Id，姓名。

如上文所说，升讯威ADO.NET增强组件没有强制的实体类与数据库表的映射关系要求，数据库表中的字段多少与实体类中的属性多少，或者说表中有的，实体类中没有，都没有关系，实体类中有的，通过Attribute标记是否映射即可。

 

1.数据库表名的映射指定

我们定义两个不同的实体类：

复制代码
  [Table("User")]
    public class User_BaseInfo
    {
        [Key]
        public Guid Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Age
        {
            get;
            set;
        }
    }
复制代码
 

复制代码
[Table("User")]
    public class User_ExtraInfo
    {
        [Key]
        public Guid Id
        {
            get;
            set;
        }

        public string ExtraInfo
        {
            get;
            set;
        }
    }
复制代码
 

只需在类型定义前加上 TableAttribute ，对 User_BaseInfo 或 User_ExtraInfo 类的对像实例进行操作，直接使用上文中的增删改查方法即可。至此我们已经开始解除了实体类与数据库表结果的强关联。

 

2.数据库表字段的映射指定

此处严格来讲，并非一般ORM中针对 数据库表字段 的映射，而是针对 结果集字段 的映射。比如说通过复杂SQL，存储过程得到的结果集，根本不是数据库中的表。

在某些场景中，实体类中需要额外定义一些属性，用于存储特定信息或实现特定功能，这些数据并不需要进行持久化存储。或是实体类中的属性名称与数据库表字段名称存在不完全相同的情况，如将一张表映射到多个数据实体后，为了区别描述，以及基于复杂查询（SQL，存储过程）得到的结果集中的字段名。

复制代码
 [Table("User")]
    public class User_ExtraInfo
    {
        [Key]
        public Guid Id
        {
            get;
            set;
        }

        [Column("ExtraInfo")]
        public string Infomation
        {
            get;
            set;
        }

        [NotMapped]
        public int Count
        {
            get;
            set;
        }
    }
复制代码
 

只需在属性定义前加上 ColumnAttribute 或 NotMapped ，使用上文中的增删改查方法即可实现相应的操作。

 

3.实体类对数据库表的多对多映射

此功能用于将二维的数据库表（或结果集）进一步强类型化。

在使用一般ORM框架时，对于复杂的数据库表结构，常常可以见到非常多的字段定义，但在我们的实际业务中，这些字段可能都有不同的逻辑归属，此外，在开发中，我们可能在数据传递，操作的过程中，希望只传递或公开一部分数据，而不是整个对象进行传递。

复制代码
 public class User
    {
        [Key]
        public Guid Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Age
        {
            get;
            set;
        }

        [Partial]
        public ExtraInfo ExtraInfo
        {
            get;
            set;
        }
    }

    public class ExtraInfo
    {
        public string ExtraInfo
        {
            get;
            set;
        }
    }
复制代码
 

只需在对象上加上 PartialAttribute ，表示属性的对象是 当前数据集 的一部分字段所表示的子对象。

PartialAttribute  还提供了 FieldRelationship 用来进一步指定映射关系。

这样我们实现了实体类对数据表（数据集）的多对一映射，那如何实现多对多的映射呢？实际上非常简单，使用SQL，视图，存储过程进行多表查询，结合使用 PartialAttribute 即可。

 

三、进阶操作
1.高级查询

除了上文中提到的基本 Select<T>() 方法外，升讯威ADO.NET增强组件提供了额外的几个进阶方式进行数据查询。

 

a) 基本查询

public List<T> Select<T>() where T : class,new()
上文已展示。

 

b) 附加查询条件

public List<T> Select<T>(Dictionary<string,object> attachedWhere) where T : class,new()
通过 attachedWhere 额外的指定查询条件。Dictionary<string,object> 中的 string 和 object 分别指定字段和字段值。

为什么不使用 lamda？在一些场景中不够灵活。

 

c)通过 SQL 语句进行查询

既然是ADO.NET增强组件，直接使用SQL来操作当然是重头戏。

public List<T> Select<T>(string sql) where T : class
直接编写 SQL 语句进行数据查询，Select 方法可根据返回的结果集和指定的对象类型进行自动映射，返回强类型对象集合。

可以传递任意能够返回结果集的SQL语句，返回的结果集自动与泛型T匹配，泛型T也不一定就是数据库中的表所映射的对象。

 

d) 参数化 SQL 语句查询

public List<T> Select<T>(string sql, List<CommandParameter> parameterList) where T : class
进行参数化的 SQL 语句查询，例如：

List<CommandParameter> parameterList = new List<CommandParameter>();
parameterList.Add(new CommandParameter("@extraInfo", "ABC"));
List<User> userList = _dataBase.Select<User>("SELECT * FROM [User] WHERE ExtraInfo = @extraInfo");
 

2.与内存中的 DataSet 进行动态映射

当我们使用存储过程或其它方式得到一个 DataSet 时，升讯威ADO.NET增强组件支持对其进行动态映射，根据 DataSet 数据集得到强类型的对象实例或对象实例的集合。

RelationalMappingUnity 类提供了以下方法：

public static List<T> Select<T>(DataSet ds) where T : class
将 DataSet 视为一个完整数据源，从中查找指定对象类型所映射的表名进行实例化。

 

public static List<T> Select<T>(DataTable dt) where T : class
使用 DataTable 作为唯一数据集，对指定的对象类型进行实例化。

 

public static T Select<T>(DataRow dr) where T : class
public static object Select(DataRow dr, Type type)
public static object Select(DataRow dr, Type type, Dictionary<string, string> fieldPair)
上面三个方法提供了更细粒度的操作可能，直接从 DataRow 得到一个强类型的对象实例。

 

3.数据填充

很多时候我们需要根据某个已知条件查询得到对象实例，如我们得到 User 的 Id，希望查询数据库表得到 User 对象，在升讯威ADO.NET增强组件中，我们使用 Fill 方法既可。

public bool Fill<T>(object obj) where T : class,new()
复制代码
public User GetUser(Guid id)
        {
            User user = new User();
            user.Id = id;
            if (_dataBase.Fill<User>(user))
                return user;
            else
                return null;
        }
复制代码
Fill 方法返回一个 bool 值，表示是否成功查询并填充了数据。

Fill 方法也有一个高阶重载，可以额外指定查询条件：

public bool Fill<T>(object obj, Dictionary<string, object> attachedWhere) where T : class,new()
 

4.SQL 语句构造器

有时，我们希望直接通过 SQL 语句实现对数据库表的简单操作，升讯威ADO.NET增强组件提供了一个 SQL 语句构造器，帮助生成 SQL 语句，可以减轻开发人员编写 SQL 语句的工作量和出错的可能性，提高软件工程的质量。

复制代码
 public void AddUser(User user)
        {
            SqlStructureBuild sqlStructureBuild = new SqlStructureBuild();
            sqlStructureBuild.Type = SqlExpressionType.Insert;
            sqlStructureBuild.Table = "User";
            sqlStructureBuild.AddParameter("Id", user.Id);
            sqlStructureBuild.AddParameter("Name", user.Name);
            sqlStructureBuild.AddParameter("Age", user.Age);

            SqlExpression sqlExpression = sqlStructureBuild.GetSqlExpression();
            _dataBase.ExcuteSqlExpression(sqlExpression);
        }
复制代码
ExcuteSqlExpression 方法在执行 SQL 构造器生成的 SqlExpression 对象时，使用的是参数化，强类型的方法进行的。

 

5.事务

对于连续的数据库操作，升讯威ADO.NET增强组件自动封装为一个事务进行执行，如果执行失败，将自动回滚。

a) 连续写入操作

非常简单，直接使用 Insert 方法插入一个对象集合既可，方法原型如下：

public void InsertList(List<object> objList)
连接的写入操作时，并不要求传入的参数是同样类型的，也就是说可以传入多个不同类似的实体对象，如同时传入User和Order，升讯威ADO.NET增强组件也会将其封装为事务执行，要么全部写入成功，要么回滚。

 

b) 复杂复合操作

对于相对复杂的数据库事务操作，可使用 SQL 语句构造器，分别构造 SqlExpression 对象，将其按执行顺序放入集合中，通过 ExcuteSqlExpression 执行即可。

public void ExcuteSqlExpression(List<SqlExpression> sqlExpressionList)
这种方式执行的多个 SqlExpression 对象，亦封装为事务进行执行。

 

四、原生操作
升讯威ADO.NET增强组件支持对数据库进行原生操作，在此基础之上，结合上述功能，实现简单高效高灵活性的数据库操作。

复制代码
public int ExecuteNonQuery(string commandText)
public int ExecuteNonQuery(string commandText, List<CommandParameter> parameterList)
public int ExecuteNonQuery(CommandType commandType, string commandText, List<CommandParameter> parameterList)
public object ExecuteScalar(string commandText)
public object ExecuteScalar(string commandText, List<CommandParameter> parameterList)
public object ExecuteScalar(CommandType commandType, string commandText, List<CommandParameter> parameterList)
public DataSet ExecuteDataSet(string commandText)
public DataSet ExecuteDataSet(string commandText, string tableName)
public DataSet ExecuteDataSet(CommandType commandType, string commandText, string tableName)
public DataSet ExecuteDataSet(string commandText, List<CommandParameter> parameterList, string tableName)
public DataSet ExecuteDataSet(CommandType commandType, string commandText,List<CommandParameter> parameterList, string tableName)
复制代码
 

综上所述，升讯威ADO.NET增强组件强调的并非实体类与数据库表结构的强关联，而是通过与内存数据集的动态映射，将数据库操作时大量的重复劳动自动化，对于复杂数据库操作，继续使用原生 SQL，存储过程，自定义函数，视图等。

这种方式结合了 ORM 自动化的优点，又充分利用了数据库原生操作的强大功能，使数据层的开发轻松，高效，高质量。将简单的，重复的体力劳动，交由程序自动化处理，复杂业务场景由人工处理，并将数据映射，取/赋值等重复劳动，自动化处理。

 

以上设计实现难免存在考虑不周的情况，希望和大家多多交流。

欢迎加我QQ交流探讨，共同学习：279060597，另外我在南京，有南京的朋友吗？
