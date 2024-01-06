namespace FSharpApi_8

open Microsoft.AspNetCore.Http

#nowarn "20"
open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

module Program =
    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)
        let app = builder.Build()

        app.MapGet("/",  Func<_, _>(fun (ctx: HttpContext) ->
            task {        
                return Results.Ok
                    {| hello = "world" |}
            })) |> ignore

        app.Run()
        0
