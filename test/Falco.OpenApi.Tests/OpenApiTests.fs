module Falco.OpenApi.Tests.OpenApi

open System.Text.Json
open Xunit

[<Fact>]
let ``Can GET /hello``() =
    let factory = FalcoOpenApiTestServer.createFactory ()
    let client = factory.CreateClient()
    let response = client.GetAsync("/").Result
    let content = response.Content.ReadAsStringAsync().Result
    Assert.Equal("Hello World!", content)

[<Fact>]
let ``Can GET /hello/{name?}`` () =
    let factory = FalcoOpenApiTestServer.createFactory ()
    let client = factory.CreateClient()
    let response = client.GetAsync("/hello").Result
    let content = response.Content.ReadAsStringAsync().Result
    Assert.Equal("""{"Message":"Hello world!"}""", content)

    let response = client.GetAsync("/hello/John").Result
    let content = response.Content.ReadAsStringAsync().Result
    Assert.Equal("""{"Message":"Hello John!"}""", content)

    let response = client.GetAsync("/hello/John?age=42").Result
    let content = response.Content.ReadAsStringAsync().Result
    Assert.Equal("""{"Message":"Hello John, you are 42 years old!"}""", content)

[<Fact>]
let ``Can GET /swagger/v1/swagger.json`` () =
    let factory = FalcoOpenApiTestServer.createFactory ()
    let client = factory.CreateClient()
    let response = client.GetAsync("/swagger/v1/swagger.json").Result
    let content = response.Content.ReadAsStringAsync().Result
    let json = JsonSerializer.Deserialize<obj>(content)
    Assert.False(isNull json)
