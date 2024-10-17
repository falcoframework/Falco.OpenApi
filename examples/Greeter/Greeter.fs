namespace OpenApi

open Falco
open Falco.OpenApi
open Falco.Routing
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

type Greeting =
    { Message : string }

module Program =
    let endpoints =
        [
            mapGet "/hello/{name?}"
                (fun route ->
                    { Message = route?name.AsString("world") })
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

    [<EntryPoint>]
    let main args =
        let bldr = WebApplication.CreateBuilder(args)

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
        0
