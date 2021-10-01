module SlateExample.App

open Feliz
open Browser.Dom
open SlateExample

[<ReactComponent>]
let App () =
    Html.div [
        prop.classes [ "flex"; "flex-col"; "w-full"; "justify-center" ]
        prop.children [
            Layout.header
            Layout.Body ()
        ]
    ]


ReactDOM.render (
    App, document.getElementById "feliz-app"
)