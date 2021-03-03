module SlateExample.Layout

open Feliz
open SlateExample.Examples
open SlateExample.ErrorBoundary

let header =
    Html.header [
        prop.classes [
            tw.``flex``; tw.``w-full``; tw.``justify-center``; tw.``items-center``; tw.``px-4``
        ]
        prop.children [
            Html.div [
                prop.classes [
                    tw.``flex``; tw.``m-auto``; tw.``p-4``; tw.``border-b``; tw.``border-rose-800``; tw.``font-header``; tw.``text-2xl``; tw.``text-rose-800``
                ]
                prop.children [
                    Html.h1 [ Html.text "Slate.fs" ]
                ]
            ]
        ]
    ]


[<ReactComponent>]
let ErrorComponent () =
    Html.div [
        prop.children [ Html.text "It's gone bad" ]
    ]

let body =
    Html.div [
        prop.classes [
            tw.``flex``; tw.``flex-col``; tw.``container``; tw.``m-auto``; tw.``p-10``
        ]
        prop.children [
            BasicExample.Example ()
//            (ErrorBoundary {
//                Inner = BasicExample.Example ()
//                ErrorComponent = ErrorComponent ()
//                OnError = fun (exn, info) ->
//                    Browser.Dom.console.log ("Exception", exn)
//                    Browser.Dom.console.log ("Info", info)
//            } :> ReactElement)
        ]
    ]
