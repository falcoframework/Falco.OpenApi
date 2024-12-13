namespace HelloWorld

open Falco
open Falco.OpenApi
open Falco.Routing
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Program =
    let endpoints =
        [
            get "/" (Response.ofPlainText "Hello World!")
                |> OpenApi.name "HelloWorld"
                |> OpenApi.summary "This is a summary"
                |> OpenApi.description "This is a test description, which is the long form of the summary"
                |> OpenApi.returnType typeof<string>
        ]

    [<EntryPoint>]
    let main args =
        let bldr = WebApplication.CreateBuilder(args)

        bldr.Services
            .AddFalcoOpenApi() // <-- activate Falco metadata services
            .AddSwaggerGen()
            |> ignore

        let wapp = bldr.Build()

        wapp.UseHttpsRedirection()
            .UseSwagger()
            .UseSwaggerUI()
        |> ignore

        wapp.UseRouting()
            .UseFalco(endpoints)
            .Run()

        0
