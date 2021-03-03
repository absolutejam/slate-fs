module SlateExample.App

open Feliz
open SlateExample

[<ReactComponent>]
let App () =
    Html.div [
        prop.classes [ tw.``flex`` ]
        prop.children [
            Layout.header
            Layout.body
        ]
    ]

open Browser.Dom

ReactDOM.render (
    App, document.getElementById "feliz-app"
)