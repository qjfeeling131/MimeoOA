# Mimeo Office Auto System

It base on the ABP Framework to overwrite, and please refrence the ABP https://github.com/aspnetboilerplate/aspnetboilerplate

## How to create a new special Repository


### Define the special

`public interface IExampleRepository {}
public class ExampleRepository:IExampleRepository{}
`

### Register the special repository to current Module

`
builder.RegisterType<ExampleRepository>().As<IExampleRepository>().InstancePerLifetimeScope();
`

###  Injection the special repository through the constructor

`public UserAppService(IExampleRepository){}
`

## How to create general Repository(Mapping to Table)

Just need to define the class and inherit the Entity base class.

`public class Task:Entity
{
 public Guid Id{get;set;}
 public string Name{get;set;}
}
`
###  Injection the general repository through the constructor

`public UserAppService(IRepository<Task> taskRepository){}
`



