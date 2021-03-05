module SlateExample.Helpers

open Fable.Core
open Fable.Core.JsInterop

let jsonify x = JS.JSON.stringify (x, space=4)

[<Emit("Object.entries($0 || {})")>]
let objectProperties (o: 't) : (string * obj)[] = jsNative

let mapOfObject (o: 't) : Map<string, obj> =
    objectProperties o |> Map.ofArray

[<Emit("setTimeout($0, $1)")>]
let setTimeout (fn: unit -> unit) timeout : unit = jsNative

[<Emit("requestAnimationFrame($0)")>]
let requestAnimationFrame (fn: unit -> unit) : unit = jsNative

