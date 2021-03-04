module SlateExample.App

open Feliz
open SlateExample

[<ReactComponent>]
let App () =
    Html.div [
        prop.classes [ tw.``flex``; tw.``flex-col``; tw.``w-full``; tw.``justify-center`` ]
        prop.children [
            Layout.header
            Layout.Body ()
        ]
    ]

open Browser.Dom

ReactDOM.render (
    App, document.getElementById "feliz-app"
)