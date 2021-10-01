module SlateExample.Layout

open Feliz
open Slate.Types

open SlateExample.Elements
open SlateExample.ErrorBoundary
open SlateExample.Examples.BasicExample

/// Header segment
let header =
    Html.header [
        prop.classes [ "flex"; "w-full"; "justify-center"; "items-center"; "px-4" ]
        prop.children [
            Html.div [
                prop.classes [ "flex"; "m-auto"; "p-4"; "border-b-2"; "border-rose-600"; "font-header"; "text-2xl"; "text-rose-600" ]
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

type PageComponents =
    [<ReactComponent>]
    static member Output (nodes: INode[]) =
        Html.div [
            prop.classes [ "flex"; "text-xs" ]
            prop.children [
                Html.pre [
                    prop.classes [ "flex"; "p-4" ]
                    prop.children [
                        Html.text (Fable.Core.JS.JSON.stringify (nodes, space=4))
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
        prop.classes [ "flex"; "flex-col"; "space-y-4"; "container"; "m-auto"; "items-center"; "p-10" ]
        prop.children [
//            Example.Render (nodes = nodes, setNodes = setNodes)
//            PageComponents.Output (nodes = nodes)

            ErrorBoundary.RenderCatchFn (
                element = Example.Render (nodes = nodes, setNodes = setNodes),
                errorHandler = (fun (exn, info) ->
                    Browser.Dom.console.log ("Exception", exn)
                    Browser.Dom.console.log ("Info", info)),
                errorComponent = ErrorComponent ()
            )
        ]
    ]
