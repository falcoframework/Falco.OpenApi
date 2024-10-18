module Falco.OpenApi.Tests.App

open Falco
open Falco.OpenApi
open Falco.Routing
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

type Greeting =
    { Message : string }

let endpoints =
    [
        get "/" (Response.ofPlainText "Hello World!")
            |> OpenApi.name "HelloWorld"
            |> OpenApi.summary "A hello world greeter"
            |> OpenApi.description "This is a simple endpoint that will return a greeting message."

        mapGet "/hello/{name?}"
            (fun route ->
                let name = route?name.AsStringNonEmpty("world")
                let age = route?age.AsIntOption()

                let message =
                    match age with
                    | Some a -> $"Hello {name}, you are {a} years old!"
                    | _ -> $"Hello {name}!"

                { Message = message })
            Response.ofJson
                |> OpenApi.name "Greeting"
                |> OpenApi.summary "A friendly greeter"
                |> OpenApi.description "This endpoint will provide a customized greeting based on the name and age (optional) provided."
                |> OpenApi.route [
                    { Type = typeof<string>; Name = "Name"; Required = false } ]
                |> OpenApi.query [
                    { Type = typeof<int>; Name = "Age"; Required = false } ]
                |> OpenApi.acceptsType typeof<string>
                |> OpenApi.returnType typeof<Greeting>
    ]

let bldr = WebApplication.CreateBuilder()

bldr.Services
    .AddFalcoOpenApi()
    .AddSwaggerGen()
    |> ignore

let wapp = bldr.Build()

wapp.UseHttpsRedirection()
    .UseSwagger()
    .UseSwaggerUI()
|> ignore

wapp.UseFalco(endpoints)
|> ignore

wapp.Run()

type Program() = class end
