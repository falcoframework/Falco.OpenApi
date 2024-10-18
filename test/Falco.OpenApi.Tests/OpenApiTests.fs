module Falco.OpenApi.Tests.OpenApi

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

let private expectedOpenApiDoc = """{
  "openapi": "3.0.1",
  "info": {
    "title": "testhost",
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
}"""

[<Fact>]
let ``Can GET /swagger/v1/swagger.json`` () =
    let factory = FalcoOpenApiTestServer.createFactory ()
    let client = factory.CreateClient()
    let response = client.GetAsync("/swagger/v1/swagger.json").Result
    let content = response.Content.ReadAsStringAsync().Result
    Assert.Equal(expectedOpenApiDoc, content)
