# GraphQL for the PI System

## Summary

This project is a WebAPI for the PI System, like the restful PI WebAPI by OSIsoft, but according to the open GraphQL standard. It utilizes the AF .NET SDK and the GraphQL.NET package. There is only one restful endpoint to POST queries to - and the schema, being part of the thing, can be explored interactively using a client like the GraphiQL electron app.

Here's what's in the box:

- a self-host owin console app that wants to run on the server and eats the AF .NET SDK 4.0 and .NET Framework 4.7
- requires basic auth in the request header from the client side and builds a System.Net.NetworkCredential to access the PI System
- the GraphQL object model includes piSystem, afDatabase, afElement, afAttribute, piTsValue, afElementTemplate
- entrance points include: piSystem(name), afDatabase(path), afElement(path), afElementTemplates(afDb path, names)
- once you're past your entrance point you can recursively traverse the AF model anywhere you want incl. attribute values, snapshot values, timestamps, and time series data (pretty plot, all values, equal distance interpolated values between start and end datetime)
- limitations: all datetime values are UTC. no time zones here. no daylight savings here. time to grow up...
- limitations: access to a PI System using this codebase is read-only. primary use cases include dashboards and etl jobs

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

## Usage: Using Filters to Limit the Scope of Traversal

The need for filters becomes apparent fairly quickly. One can describe traversals many levels into the hirarchy of an AF Database - and completely crawling through all elements and attributes would be unneccessary. To be more specific about the scope of a traversal, we use filters.

### nameFilter

NameFilters are provided as arrays of strings. The nameFilter feature is available for afDataBases, afElementTemplates, afElements, and afAttributes. A few examples:

~~~GraphQL
// limit USA assets by state (MA, TX, FL) and retrieve Generators' runtime and fuelLevel attributes
{
  afElement (aAfElementPath: "\\\\myPisystemName\\afmodel1\\USA"){
    name
    afElements (nameFilter: ["MA", "TX", "FL"]){
      name
      afElements (nameFilter: ["Generators"]){
        afElements {
          name
          afAttributes (nameFilter: ["runtime", "fuelLevel"]){
            name
            value
            timeStamp
            uof
          }
        }
      }
    }
  }
}
~~~

### attributeValueFilter

We encounter the need for this, because we often name afElements with GUIDs to allow duplicate asset names. Asset names are then burried in attributes. To filter for elements by attribute values we use attributeValueFilter, as array of strings, in the format attributeName=value. For instance:

~~~GraphQL
// retrieve Generators by template and by manufacturer attribute
{
  afElementTemplates (aAfDatabasePath: "\\\\myPisystemName\\afmodel1", nameFilter: ["Generator"]){
    afElements (attributeValueFilter: ["manufacturer=Caterpillar"]){
      name
      afAttributes (nameFilter: ["runtime", "fuelLevel"]){
        name
        value
        timeStamp
        uof
      }
    }
  }
}
~~~

## Usage: Retrieving Time Series Data

### tsPlotValues

This works like one would expect: we use the plot() functionality of AF and retrieve time series values between a given start and end datetime string. The AF SDK does a solid good job parsing lots of string formats to convert to a DateTime value. Also, "-1w" and "*", for "one week ago" and "right now", works. Keep in mind that anything DateTime in this project is refering to UTC. The plotDensity parameter has been the stuff of controversy and a bit of dark magic for decades:

- larger than 0: returns a pretty plot. Values around 100-200 work pretty well.
- equal to 0: returns all values in the archive. Use with care for large timespans.
- smaller than 0: returns interpolated values for exactly that many equal spaced time spans.

For instance:

~~~GraphQL
{
  ...
  ...
  afAttributes (nameFilter: ["kW", "kWh"]){
    name, value, timeStamp
    tsPlotValues (startDateTime: "-1w", endDateTime: "*", plotDensity: 100){
      timeStamp
      value
    }
  }
}
~~~

### tsAggregateValues

This should be next on the list of functionality to be added. It's slicing a given timeSpan into a number of pieces, for instance hourly for the last week, and returning min, max, avg, sum (event based), sum (time based). Straight forward...

## GraphQL Schema

It is not the goal of this project to replicate the extensive scopes and awesome functionality of the PI WebAPI or the AF .NET SDK. The schema includes the following simplified types and entrance queries:

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
  afElements(nameFilter: [String], attributeValueFilter: [String]): [GraphQlAfElement]
}
~~~

### afElement

~~~GraphQL
type GraphQlAfElement {
  name: String,
  path: String,
  template: String,
  afElements(nameFilter: [String], attributeValueFilter: [String]): [GraphQlAfElement],
  afAttributes(nameFilter: [String]): [GraphQlAfAttribute]
}
~~~

### afElementTemplate

~~~GraphQL
type GraphQlAfElementTemplate {
  name: String,
  afElements(nameFilter: [String], attributeValueFilter: [String]): [GraphQlAfElement],
}
~~~

### afAttribute

~~~GraphQL
type GraphQlAfAttribute {
  name: String,
  value: String,
  timeStamp: String,
  uom: String,
  afAttributes(nameFilter: [String]): [GraphQlAfAttribute],
  tsPlotValues(startDateTime: String, endDateTime: String, plotDensity: Int): [GraphQlTsValue]
}
~~~

### afTsValue

~~~GraphQL
type GraphQlTsValue {
  timeStamp: String,
  value: String
}
~~~

### Queryies (yes, there's a Hello World in there...)

~~~GraphQL
type Query {
  piSystem(name: String): GraphQlPiSystem
  afDatabase(aAfDatabasePath: String!): GraphQlAfDatabase
  afElement(aAfElementPath: String!): GraphQlAfElement
  afElementTemplates(aAfDatabasePath: String!, nameFilter: [String]): [GraphQlAfElementTemplate]
  hello: String
}
~~~
