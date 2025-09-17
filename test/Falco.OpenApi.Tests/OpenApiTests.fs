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
    let json = JsonDocument.Parse(content)
    Assert.False(isNull json)

    let paths = json.RootElement.GetProperty("paths")
    Assert.Equal(2, paths.GetPropertyCount())

    let helloPath = paths.GetProperty("/hello/{name}")
    Assert.Equal(1, helloPath.GetPropertyCount())
    let helloGet = helloPath.GetProperty("get")
    let helloGetTags = helloGet.GetProperty("tags")
    Assert.Equal(1, helloGetTags.GetArrayLength())
    Assert.Equal("Greeting", helloGetTags.[0].GetString())
    Assert.Equal("A friendly greeter", helloGet.GetProperty("summary").GetString())
    Assert.Equal("This endpoint will provide a customized greeting based on the name and age (optional) provided.", helloGet.GetProperty("description").GetString())
    Assert.Equal("Greeting", helloGet.GetProperty("operationId").GetString())
    let helloGetParameters = helloGet.GetProperty("parameters")
    Assert.Equal(3, helloGetParameters.GetArrayLength())
    let nameParam = helloGetParameters.[0]
    Assert.Equal("Name", nameParam.GetProperty("name").GetString())
    Assert.Equal("path", nameParam.GetProperty("in").GetString())
    Assert.True(nameParam.GetProperty("required").GetBoolean())
    Assert.Equal("string", nameParam.GetProperty("schema").GetProperty("type").GetString())
    let ageParam = helloGetParameters.[1]
    Assert.Equal("Age", ageParam.GetProperty("name").GetString())
    Assert.Equal("query", ageParam.GetProperty("in").GetString())
    let ageRequiredExists, _ = ageParam.TryGetProperty("required")
    Assert.False(ageRequiredExists)
    Assert.Equal("string", ageParam.GetProperty("schema").GetProperty("type").GetString())
    let headerParam = helloGetParameters.[2]
    Assert.Equal("X-Request-ID", headerParam.GetProperty("name").GetString())
    Assert.Equal("header", headerParam.GetProperty("in").GetString())
    let headerRequiredExists, _ = headerParam.TryGetProperty("required")
    Assert.False(headerRequiredExists)
    Assert.Equal("string", headerParam.GetProperty("schema").GetProperty("type").GetString())
    let helloGetRequestBody = helloGet.GetProperty("requestBody")
    let helloGetRequestBodyContent = helloGetRequestBody.GetProperty("content")
    Assert.Equal(1, helloGetRequestBodyContent.GetPropertyCount())
    let textPlain = helloGetRequestBodyContent.GetProperty("text/plain")
    let textPlainSchema = textPlain.GetProperty("schema")
    Assert.Equal("string", textPlainSchema.GetProperty("type").GetString())
    Assert.True(helloGetRequestBody.GetProperty("required").GetBoolean())
    let helloGetResponses = helloGet.GetProperty("responses")
    Assert.Equal(1, helloGetResponses.GetPropertyCount())
    let helloGet200 = helloGetResponses.GetProperty("200")
    Assert.Equal("OK", helloGet200.GetProperty("description").GetString())
    let helloGet200Content = helloGet200.GetProperty("content")
    Assert.Equal(1, helloGet200Content.GetPropertyCount())


(*
{
  "openapi": "3.0.1",
  "info": {
    "title": "Falco.OpenApi.Tests.App",
    "version": "1.0"
  },
  "paths": {
    "/hello/{name}": {
      "get": {
        "tags": [
          "Greeting"
        ],
        "summary": "A friendly greeter",
        "description": "This endpoint will provide a customized greeting based on the name and age (optional) provided.",
        "operationId": "Greeting",
        "parameters": [
          {
            "name": "Name",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "Age",
            "in": "query",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "X-Request-ID",
            "in": "header",
            "schema": {
              "type": "string"
            }
          }
        ],
        "requestBody": {
          "content": {
            "text/plain": {
              "schema": {
                "type": "string"
              }
            }
          },
          "required": true
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/Greeting"
                }
              }
            }
          }
        }
      }
    },
    "/": {
      "get": {
        "tags": [
          "HelloWorld"
        ],
        "summary": "A hello world greeter",
        "description": "This is a simple endpoint that will return a greeting message.",
        "operationId": "HelloWorld",
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "Greeting": {
        "type": "object",
        "properties": {
          "message": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      }
    }
  }
}
*)
