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
