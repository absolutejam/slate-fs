module SlateExample.Examples.BasicExample

open Feliz
open Browser.Types
open FsToolkit.ErrorHandling

open Slate.Node
open Slate.Core
open Slate.Types
open Slate.Editor
open Slate.Element
open Slate.Operations
open Slate.Transforms
open Slate.Operations.OperationTypes

open Slate.FSharpExtended

open SlateExample.Icons
open SlateExample.Styles
open SlateExample.Elements

let isActive f editor =
    let pred = Nodex.(|Element|_|) >> Option.bind f >> Option.isSome
    Editor.nodes (editor, match'=pred) |> (not << Seq.isEmpty)


[<ReactComponent>]
let Button (props: {| style: string; activePredicate: IEditor -> bool; icon: string list -> ReactElement |}) =
    let editor = useSlate ()
    let isActive = props.activePredicate editor
    Html.button [
        prop.classes [ tw.``flex``; tw.``items-center``; tw.``p-2``; tw.``border``; tw.``focus:outline-none``; tw.``border-gray-2`` ]
        prop.children [
            props.icon [ if isActive then tw.``text-gray-3`` else tw.``text-gray-2`` ]
            |> Icons.iconWrapper 16 16
        ]
        prop.onClick <| fun ev ->
            ev.preventDefault ()
            Transforms.setNodes (
                editor,
                {| elementType = if isActive then ParagraphElement.elementType else TitleElement.elementType |}
            )
    ]

[<ReactComponent>]
let Toolbar () =
     Html.div [
         prop.classes [ tw.``flex``; tw.``w-full``; tw.``space-x-2`` ]
         prop.children [
             Button {| style = "title"; icon = Icons.header; activePredicate = isActive Elements.(|TitleElement|_|) |}
         ]
     ]


let renderElement (props: RenderElementProps) =
    match props.element with
    | Elements.TitleElement _   -> ElementComponents.TitleElement props
    | Elements.SectionElement _ -> ElementComponents.SectionElement props
    | _                         -> ElementComponents.ParagraphElement props

[<ReactComponent>]
let Example (props: {| nodeState: INode[] * (INode[] -> unit) |}) =
    let nodes, setNodes = props.nodeState

    let editor = React.useMemo ((fun () ->
        createEditor ()
        |> withReact
        |> withHistory
        |> withLayout
        |> withTransformSplits (fun el -> el.elementType = "title")), [||])
    let renderElement = React.useCallback (renderElement, [||])

    let onKeyDown (ev: KeyboardEvent) =
        match ev.key, ev.metaKey with
        | "k", true ->
            ev.preventDefault ()
            let isTitle = isActive Elements.(|TitleElement|_|) editor
            Transforms.setNodes (
                editor,
                {| elementType = if isTitle then ParagraphElement.elementType else TitleElement.elementType |}
            )
        | _ -> ()

    Html.div [
        prop.classes [ tw.``flex``; tw.``flex-col``; tw.``space-y-2`` ]
        prop.style [ style.width 500 ]
        prop.children [
            Slate.init [
                 Slate.editor editor
                 Slate.value nodes
                 Slate.onChange setNodes
                 Slate.children [
                     Toolbar ()
                     Slate.editable [
                         Editable.renderElement renderElement
                         prop.autoFocus true
                         prop.classes [ tw.``border``; tw.``border-gray-2``; tw.``p-6`` ]
                         prop.onKeyDown onKeyDown
                     ]
                 ]
            ]
        ]
    ]

