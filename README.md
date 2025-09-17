# Falco.OpenAPI

[![NuGet Version](https://img.shields.io/nuget/v/Falco.OpenApi.svg)](https://www.nuget.org/packages/Falco.OpenApi)
[![build](https://github.com/FalcoFramework/Falco.OpenApi/actions/workflows/build.yml/badge.svg)](https://github.com/FalcoFramework/Falco.OpenApi/actions/workflows/build.yml)

```fsharp
open Falco
open Falco.OpenApi
open Falco.Routing

let endpoints =
    [
        get "/" (Response.ofPlainText "Hello World!")
            |> OpenApi.name "HelloWorld"
            |> OpenApi.summary "This is a summary"
            |> OpenApi.description "This is a test description, which is the long form of the summary."
            |> OpenApi.returnType typeof<string>
    ]
```

[Falco.OpenAPI](https://github.com/FalcoFramework/Falco.OpenAPI) is a library for generating OpenAPI documentation for [Falco](https://github.com/FalcoFramework/Falco) applications. It provides a set of combinators for annotating Falco routes with OpenAPI metadata, which can be used to generate OpenAPI documentation.

## Key Features

- Generates OpenAPI 3.0 documentation.
- Provides a set of combinators for annotating Falco routes with OpenAPI metadata.
- Generates OpenAPI documentation from metadata associated to Falco routes.
- Allows for customization of the generated OpenAPI documentation.

## Design Goals

- Minimalistic and easy to use.
- Explicit, allowing for fine-grained control over the generated OpenAPI documentation.
- Type-safe and to leverage the type system to prevent common mistakes.
- Composable, allowing for the generation of complex OpenAPI documentation.

## Getting Started

This guide assumes you have a [Falco](https://github.com/FalcoFramework/Falco) project setup. If you don't, you can create a new Falco project using the following command:

```shell
> dotnet new web -lang F# -o HelloWorld
> cd HelloWorldApp
```

Install the nuget package:

```shell
> dotnet add package Falco
> dotnet add package Falco.OpenApi
```

Remove any `*.fs` files created automatically, create a new file named `Program.fs` and set the contents to the following:

```fsharp
open Falco
open Falco.Routing
open Microsoft.AspNetCore.Builder

let bldr = WebApplication.CreateBuilder()
let wapp = bldr.Build()

let endpoints =
    [
        get "/" (Response.ofPlainText "Hello World!")
    ]

wapp.UseRouting()
    .UseFalco(endpoints)
    .Run()
```

Now, let's incorporate OpenAPI documentation into our Falco application. Update the `Program.fs` file to the following:

```fsharp
// ..

let endpoints =
    [
        get "/" (Response.ofPlainText "Hello World!")
            |> OpenApi.name "HelloWorld"
            |> OpenApi.summary "This is a summary"
            |> OpenApi.description "This is a test description, which is the long form of the summary."
            |> OpenApi.returnType typeof<string>
    ]

// ..
```

Which produces the following OpenAPI documentation:

```json
{
  "openapi": "x.x.x",
  "info": {
    "title": "HelloWorld",
    "version": "1.0"
  },
  "paths": {
    "/": {
      "get": {
        "tags": [
          "HelloWorld"
        ],
        "summary": "This is a summary",
        "description": "This is a test description, which is the long form of the summary",
        "operationId": "HelloWorld",
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "string"
                }
              }
            }
          }
        }
      }
    }
  },
  "components": { }
}
```

## Specs

### Name (Operation ID)

```fsharp
open Falco
open Falco.OpenApi
open Falco.Routing

let endpoints =
    [
        get "/" (Response.ofPlainText "Hello World!")
            |> OpenApi.name "HelloWorld"
    ]
```

### Summary

```fsharp
open Falco
open Falco.OpenApi
open Falco.Routing

let endpoints =
    [
        get "/" (Response.ofPlainText "Hello World!")
            |> OpenApi.summary "This is a summary"
    ]
```

### Description

```fsharp
open Falco
open Falco.OpenApi
open Falco.Routing

let endpoints =
    [
        get "/" (Response.ofPlainText "Hello World!")
            |> OpenApi.description "This is a description"
    ]
```

### Return Type

```fsharp
open Falco
open Falco.OpenApi
open Falco.Routing

let endpoints =
    [
        get "/" (Response.ofPlainText "Hello World!")
            |> OpenApi.returns typeof<string>
            // or,
            // |> OpenApi.returns { Return = typeof<string>; ContentType = ["text/plain"]; Status = 200 }
    ]
```

### Route Parameters

```fsharp
open Falco
open Falco.OpenApi
open Falco.Routing

let endpoints =
    [
        mapGet "/hello/{name?}"
            (fun route -> route?name.AsString("world"))
            Response.ofPlainText
            |> OpenApi.route [
                { Type = typeof<string>; Name = "name"; Required = false } ]
    ]
```

### Query Parameters

```fsharp
open Falco
open Falco.OpenApi
open Falco.Routing

let endpoints =
    [
        mapGet "/hello"
            (fun route ->
                let age = route?age.AsInt(0)
                $"Hello there, your are {age} years old.")
            Response.ofPlainText
            |> OpenApi.route [
                { Type = typeof<int>; Name = "age"; Required = false } ]
    ]
```

### Header Parameters

```fsharp
open Falco
open Falco.OpenApi
open Falco.Routing

let endpoints =
    [
        get "/"
            (fun ctx ->
                let headers = Request.getHeaders ctx
                let versionId = headers.GetString "X-Version-ID"
                $"Hello, you are using version {versionId}")
            |> OpenApi.header [
                { Type = typeof<string>; Name = "X-Version-ID"; Required = true } ]
    ]
```

### Tags

```fsharp
open Falco
open Falco.OpenApi
open Falco.Routing

let endpoints =
    [
        get "/" (Response.ofPlainText "Hello World!")
            |> OpenApi.tags [
                "tag1"
                "tag2" ]
    ]
```

## Find a bug?

There's an [issue](https://github.com/FalcoFramework/Falco.OpenApi/issues) for that.

## License

Licensed under [Apache License 2.0](https://github.com/FalcoFramework/Falco.OpenApi/blob/master/LICENSE).
