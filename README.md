# Simple Onion Architecture

This project is a plugin that installs the basic architectural structure for those who want to develop a project using the oninon architecture.

You can access the plugin [here]((https://marketplace.visualstudio.com/items?itemName=FatihSglm.SimpleOnionArchitectureTemplate)).

# Project structure

The extension creates your project with the name you gave, together with the basic building blocks of onion architecture, Application, Domain, Infrasturucture, Persistence and Presentation Layers.


![image](https://user-images.githubusercontent.com/92210948/234879322-4f5902e2-19b3-45d5-a672-6a1f175a6257.png)


### Application Layer
 
&nbsp; &nbsp; It contains the applied generic Repository pattern separated as Read Write, together with an extension where you can perform Dependency operations in the Application layer.

![image](https://user-images.githubusercontent.com/92210948/234879481-ebb95b80-6235-450b-b17d-bd46a25f3afb.png)

IReadRepository
```csharp

using Microsoft.EntityFrameworkCore.Query;
using Simple_Onion_Architecture1.Domain.Entities.Common;
using System.Linq.Expressions;

namespace Simple_Onion_Architecture1.Application.Abstractions.Repositories.Common
{
    public interface IReadRepository<TEntity> where TEntity : class, IEntity, new()
    {
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>,
                                         IIncludableQueryable<TEntity, object>>? include = null, bool enableTracking = true,
                                         CancellationToken cancellationToken = default);

        Task<IQueryable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                        bool enableTracking = true,
                                        CancellationToken cancellationToken = default);
    }
}
```


IWriteRepository
```csharp
using Simple_Onion_Architecture1.Domain.Entities.Common;

namespace Simple_Onion_Architecture1.Application.Abstractions.Repositories.Common
{
    public interface IWriteRepository<TEntity> where TEntity : class, IEntity, new()
    {
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task Update(TEntity entity);
        Task UpdateRange(IEnumerable<TEntity> entities);
        Task Remove(TEntity entity);
        Task RemoveRange(IEnumerable<TEntity> entities);
        Task<int> SaveChangesAsync();
    }
}
```

IRepository
```csharp

using Simple_Onion_Architecture1.Domain.Entities.Common;

namespace Simple_Onion_Architecture1.Application.Abstractions.Repositories.Common
{
    public interface IRepository<TEntity> : IWriteRepository<TEntity>, IReadRepository<TEntity> where TEntity : class, IEntity, new()
    {
        IQueryable<TEntity> Query { get; }
    }
}


```

Dependency Resolver

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Simple_Onion_Architecture1.Application.Extensions
{
    public static class ApplicationServiceRegistration
    {
        // Bu projede kullanacağınız servisleri IoC mekanizmasına ekleyecek olan fonksiyondur.
        // This is the function that will add the services you will use in this project to the IoC mechanism.
        public static IServiceCollection AddApplicationServiceRegistration(this IServiceCollection services)
        {
            return services;
        }
    }
}

```

### Domain Layer

&nbsp;&nbsp; Apart from the "Base Entity" class where you can define the common properties of the entities you create in the domain layer, there is the "IEntity" interface where you can mark the entities in the Generic Repository.

![image](https://user-images.githubusercontent.com/92210948/234879576-50c1dea8-e0d6-4d78-93de-999c20804b51.png)

BaseEntity
```csharp
namespace Simple_Onion_Architecture1.Domain.Entities.Common
{
    // Burada tüm Entity'ler için geçerli olacak değişkenleri belirtirsiniz, onları buradan miras alarak tekrardan kaçınabilirsiniz.
    // Here you specify variables that will be valid for all Entities, you can avoid duplication by inheriting them from here.

    public abstract class BaseEntity : BaseEntity<int>
    {
    }


    //Burada Entitydeki Id nin tipini kendiniz değiştirebilirsiniz: Guid ,string vb bi şekilde değişken tanımlayabilirsiniz
    //You can change the type of the Id in Entity here by yourself: you can define a variable in various ways such as Guid or string
    public abstract class BaseEntity<TIdentity> : IEntity where TIdentity : struct, IComparable, IComparable<TIdentity>, IEquatable<TIdentity>, IFormattable
    {
        public TIdentity Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
```

IEntity
```csharp

namespace Simple_Onion_Architecture1.Domain.Entities.Common
{
    public interface IEntity
    {
        DateTime CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }
    }
}

```

###  Infrastructure Layer

&nbsp;&nbsp;The Infrastructure layer contains an extension class for Dependency injection, just like the Application layer. Since this layer is used for External service implementation, there is nothing else.

![image](https://user-images.githubusercontent.com/92210948/234880857-39450f67-6dc8-4ce5-a306-81be6202ec99.png)

Dependency Resolver

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Simple_Onion_Architecture1.Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        // Bu projede kullanacağınız servisleri IoC mekanizmasına ekleyecek olan fonksiyondur.
        // This is the function that will add the services you will use in this project to the IoC mechanism.
        public static IServiceCollection AddInfrastructureServiceRegistration(this IServiceCollection services)
        {


            return services;
        }
    }
}
```



### Persistence Layer

&nbsp; &nbsp;Persistence layer is the layer where the repository design pattern interfaces that we created in the Application layer are applied together with ef core. We have developed a function that edits and saves the CreatedDate and UpdatedDate properties that we want to be all entities with IEntity by overriding the SaveChanges method in DbContet, without depending on the user. As in the other layers, a Dependency injection extension class is also available, but there is a code block that automatically adds it to the IoC container for the classes that are suitable for the generic repository pattern you will apply.

![image](https://user-images.githubusercontent.com/92210948/234881345-59421840-8924-43c7-96f9-cca06722e9b9.png)

The reading and writing processes for this project were also written in one piece, if you wish, you can do it by breaking it up.

Repository

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Simple_Onion_Architecture1.Application.Abstractions.Repositories.Common;
using Simple_Onion_Architecture1.Domain.Entities.Common;
using System.Linq.Expressions;

namespace Simple_Onion_Architecture1.Persistence.Concretes.Repositories.Common
{
    public class Repository<TEntity, TContext> : IRepository<TEntity> where TEntity : class, IEntity, new() where TContext : DbContext
    {
        private readonly TContext _context;

        public Repository(TContext context)
        {
            _context = context;
        }
        private DbSet<TEntity> Table => _context.Set<TEntity>();
        public IQueryable<TEntity> Query => Table.AsQueryable();

        #region Read-Metods
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query;
            if (!enableTracking) queryable = Query.AsNoTracking();
            if (include != null) queryable = include(queryable);
            return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<IQueryable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool enableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = Query;
            if (!enableTracking) queryable = queryable.AsNoTracking();
            if (include != null) queryable = include(queryable);
            if (predicate != null) queryable = queryable.Where(predicate);
            if (orderBy != null)
                return await Task.FromResult(orderBy(queryable));
            return await Task.FromResult(queryable);
        }

        #endregion

        #region Write-Metods
        public async Task AddAsync(TEntity entity)
        {
            await Table.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await Table.AddRangeAsync(entities);
        }

        public Task Remove(TEntity entity)
        {
            Table.Remove(entity);
            return Task.CompletedTask;
        }

        public Task RemoveRange(IEnumerable<TEntity> entities)
        {
            Table.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public Task Update(TEntity entity)
        {
            Table.Update(entity);
            return Task.CompletedTask;
        }

        public Task UpdateRange(IEnumerable<TEntity> entities)
        {
            Table.UpdateRange(entities);
            return Task.CompletedTask;
        }
        #endregion

    }
}

```

DbContext

```csharp
using Microsoft.EntityFrameworkCore;
using Simple_Onion_Architecture1.Domain.Entities.Common;

namespace Simple_Onion_Architecture1.Persistence.Context
{
    public class ApplicaitonDbContext : DbContext
    {
        public ApplicaitonDbContext(DbContextOptions options) : base(options)
        {
        }

        // Entityleri db ye kaydederken içerdiği dateTime propertylerini otomatik olarak ekleyen bir metod
        // Not : Eğer PostgreSql kullanacaksanız DateTime yerine DateTime.Utc.Now Kullanmanız gerekiyor

        // A method that automatically adds dateTime properties contained in entities while saving to the database
        // Note: If you're using PostgreSql, you need to use DateTime.Utc.Now instead of DateTime
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var datas = ChangeTracker.Entries<IEntity>();
            foreach (var data in datas)
            {
                _ = data.State switch
                {
                    EntityState.Added => data.Entity.CreatedDate = DateTime.Now,
                    EntityState.Modified => data.Entity.ModifiedDate = DateTime.Now,
                    _ => DateTime.Now
                };
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
```

Dependency Resolver

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simple_Onion_Architecture1.Application.Abstractions.Repositories.Common;
using Simple_Onion_Architecture1.Persistence.Context;
using System.Reflection;

namespace Simple_Onion_Architecture1.Persistence.Extensions
{
    public static class PersistenceServiceRegistration
    {
        // Bu projede kullanacağınız servisleri IoC mekanizmasına ekleyecek olan fonksiyondur.
        // This is the function that will add the services you will use in this project to the IoC mechanism.
        public static IServiceCollection AddPersistenceServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicaitonDbContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("SqlConnection")));
            AddRepositoryToIoC(services, Assembly.GetExecutingAssembly());
            return services;
        }

        // Repository lerin otomatik olarak IoC Container a eklenmesini sağlayan metod
        //Method that enables automatic addition of repositories to IoC Container
        private static IServiceCollection AddRepositoryToIoC(IServiceCollection services, Assembly assembly)
        {
            var reposiories = assembly.GetTypes().Where(x => x.IsAssignableToGenericType(typeof(IRepository<>)) && !x.IsGenericType);
            foreach (var item in reposiories)
            {
                var @interface = item.GetInterfaces().FirstOrDefault(x => !x.IsGenericType) ?? throw new ArgumentNullException();
                services.AddScoped(@interface, item);
            }
            return services;
        }


        //Type in verilen generic türden türeyip türemediğini kontrol eden fonksiyon
        //Function that checks whether a given type is implementing a generic interface
        private static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            return givenType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == genericType) ||
                   givenType.BaseType != null && (givenType.BaseType.IsGenericType && givenType.BaseType.GetGenericTypeDefinition() == genericType ||
                                                  givenType.BaseType.IsAssignableToGenericType(genericType));
        }
    }
}

```

### Presentation layer

&nbsp; &nbsp; In the Presentation layer, extensions written in other layers are added to program.cs and a sample controller has been created so that they are not deleted as an empty folder.
A key was created for the Conneciton string in AppSetting and this key was used in the persistence layer.

![image](https://user-images.githubusercontent.com/92210948/234882401-d39a54ba-2043-488c-af0e-6c42a6586631.png)

Program.cs

```csharp
using Simple_Onion_Architecture1.Application.Extensions;
using Simple_Onion_Architecture1.Infrastructure.Extensions;
using Simple_Onion_Architecture1.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServiceRegistration();
builder.Services.AddPersistenceServiceRegistration(builder.Configuration);
builder.Services.AddInfrastructureServiceRegistration();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

AppSettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "SqlConnection" : ""
  }
}

```



