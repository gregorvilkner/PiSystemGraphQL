# GraphQL for the PI System

## Summary

This project is a WebAPI for the PI System, like the restful PI WebAPI by OSIsoft, but according to the open GraphQL standard. It utilizes the AF .NET SDK and the GraphQL.NET package. There is only one restful endpoint to POST queries to - and the schema, being part of the thing, can be explored interactively using a client like the GraphiQL electron app.

Here's what's in the box:

- a self-host owin console app that wants to run on the server and eats the AF .NET SDK 4.0 and .NET Framework 4.7
- requires basic auth in the request header from the client side and builds a System.Net.NetworkCredential to access the PI System
- the GraphQL object model includes piSystem, afDatabase, afElement, afAttribute, afElementTemplate
- entrance points include: piSystem(name), afDatabase(path), afElement(path), afElementTemplates(afDb path, names)
- once your past your entrance point you can recursively traverse the AF model anywhere you want
- limitations: for now it's totally read-only. primary use case include dashboards and etl jobs
- limitations: for now it gets afAttribute snapshot values, no timeseries data yet

## App Settings

There's a small appSettings section in the App.config file to take note of:

~~~XML
<appSettings>
  <!--<add key="baseUrl" value="http://*:9090/"/>-->
  <add key="baseUrl" value="https://*:9091/"/>
  <add key="dedicatedPiSystem" value=""/>
</appSettings>
~~~

The baseUrl is straight forward - whatever you want to use on your server. Both, http and https, work.

This project simply uses:

~~~C#
using (Microsoft.Owin.Hosting.WebApp.Start<Startup>(baseUrl))
{
  Process.Start(baseUrl);
  Console.ReadKey();
}

with:

class Startup
{
  public void Configuration(IAppBuilder app)
  {
    app.Map("/api", builder =>
    {
        HttpConfiguration config = new HttpConfiguration();
        config.MapHttpAttributeRoutes();
        app.UseWebApi(config);
    });
  }
}
~~~

The dedicatedPiSystem attribute lets you specify the name of a "default" PI System - good thing to use if there is only one. This allows you to conviniently query for piSystem without specifying a name parameter:

~~~GraphQL
{
  piSystem{
    name
    afDbs{
      name
    }
  }
}
<<vs.>>
{
  {
  piSystem (name: "myPisystemName"){
    name
    afDbs{
      name
    }
  }
}
~~~

## Authentication

This project uses a basic authentication header value to create a System.Net.NetworkCredential to access the PI System. Simply add an Authorization header with the value "Basic abcd1234efgh", where abcd1234efgh is Base64 of domain\username:password.

## GraphQL Schema

It is not the goal of this project to replicate the extensive scopes of awesome functionality of the PI WebAPI or the AF .NET SDK. The schema includes the following simplified types and entrance queries:

### piSystem

~~~GraphQL
type GraphQlPiSystem {
  name: String,
  afDbs(nameFilter: [String]): [GraphQlAfDatabase]
}
~~~

### afDatabase

~~~GraphQL
type GraphQlAfDatabase {
  name: String,
  path: String,
  afElements(nameFilter: [String]): [GraphQlAfElement]
}
~~~

### afElement

~~~GraphQL
type GraphQlAfElement {
  name: String,
  path: String,
  template: String,
  afElements(nameFilter: [String]): [GraphQlAfElement],
  afAttributes(nameFilter: [String]): [GraphQlAfAttribute]
}
~~~

### afElementTemplate

~~~GraphQL
type GraphQlAfElementTemplate {
  name: String,
  afElements(nameFilter: [String]): [GraphQlAfElement],
}
~~~

### afAttribute

~~~GraphQL
type GraphQlAfAttribute {
  name: String,
  value: String,
  timeStamp: String,
  uom: String,
  afAttributes(nameFilter: [String]): [GraphQlAfAttribute]
}
~~~

### Queryies (yes, there's a Hello World in there...)

~~~GraphQL
type Query {
  piSystem(name: String): GraphQlPiSystem
  afDatabase(aAfDatabasePath : String!): GraphQlAfDatabase
  afElement(aAfElementPath: String!): GraphQlAfElement
  afElementTemplates(aAfDatabasePath : String!, nameFilter: [String]): [GraphQlAfElementTemplate]
  hello: String
}
~~~
