module SlateExample.Layout

open Feliz
open Slate.Types

open SlateExample.Examples
open SlateExample.Elements
open SlateExample.ErrorBoundary

let header =
    Html.header [
        prop.classes [
            tw.``flex``; tw.``w-full``; tw.``justify-center``; tw.``items-center``; tw.``px-4``
        ]
        prop.children [
            Html.div [
                prop.classes [
                    tw.``flex``; tw.``m-auto``; tw.``p-4``; tw.``border-b-2``; tw.``border-rose-600``; tw.``font-header``; tw.``text-2xl``; tw.``text-rose-600``
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

[<ReactComponent>]
let Output (props: {| nodes: INode[] |}) =
    Html.div [
        prop.classes [ tw.``flex``; tw.``text-xs`` ]
        prop.children [
            Html.pre [
                prop.classes [ tw.``flex``; tw.``p-4`` ]
                prop.children [
                    Html.text (Fable.Core.JS.JSON.stringify (props.nodes, space=4))
                ]
            ]
        ]
    ]


[<ReactComponent>]
let Body () =
    let initialState : INode[] =
        [|
            Elements.titleWithPlaceholder "Welcome!" "Title..."
            Elements.paragraph "Here is some starting text..."
        |]

    let nodes, setNodes = React.useState initialState

    Html.div [
        prop.classes [
            tw.``flex``; tw.``flex-col``; tw.``space-y-4``; tw.``container``; tw.``m-auto``; tw.``items-center``; tw.``p-10``
        ]
        prop.children [
            BasicExample.Example {| nodeState = (nodes, setNodes) |}
            Output {| nodes = nodes |}

//            (ErrorBoundary {
//                Inner = BasicExample.Example ()
//                ErrorComponent = ErrorComponent ()
//                OnError = fun (exn, info) ->
//                    Browser.Dom.console.log ("Exception", exn)
//                    Browser.Dom.console.log ("Info", info)
//            } :> ReactElement)
        ]
    ]
