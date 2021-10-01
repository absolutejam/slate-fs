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

open Slatex

open SlateExample.Icons
open SlateExample.Elements
open SlateExample.Plugins.Layout
open SlateExample.Plugins.TransformSplits

let activeElementIs f editor =
    let predicate = Nodex.(|Element|_|) >> Option.bind f >> Option.isSome
    Editor.nodes (editor, match' = predicate) |> Seq.isEmpty |> not

type Components =
    [<ReactComponent>]
    static member Button
        (
            style: string,
            activePredicate: IEditor -> bool,
            icon: string list -> ReactElement
        ) =
        let editor = useSlate ()
        let isActive = activePredicate editor

        Html.button [
            prop.classes [ "flex"; "items-center"; "p-2"; "border"; "focus:outline-none"; "border-gray-2" ]
            prop.children [
                Icons.iconWrapper 16 16 (
                    icon [ if isActive then "text-gray-3" else "text-gray-2" ]
                )
            ]
            prop.onClick <| fun ev ->
                ev.preventDefault ()
                Transforms.setNodes (
                    editor,
                    {| elementType = if isActive then ParagraphElement.elementType else TitleElement.elementType |}
                )
        ]

    [<ReactComponent>]
    static member Toolbar () =
         Html.div [
             prop.classes [ "flex"; "w-full"; "space-x-2" ]
             prop.children [
                Components.Button (
                    style = "title",
                    icon = Icons.header,
                    activePredicate = activeElementIs Elements.(|TitleElement|_|)
                )
             ]
         ]


let renderElement (props: RenderElementProps) =
    match props.element with
    | Elements.TitleElement _   -> ElementComponents.TitleElement props
    | Elements.SectionElement _ -> ElementComponents.SectionElement props
    | _                         -> ElementComponents.ParagraphElement props

type Example =
    [<ReactComponent>]
    static member Render (nodes: INode[], setNodes: INode[] -> unit) =
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
                let isTitle = activeElementIs Elements.(|TitleElement|_|) editor
                Transforms.setNodes (
                    editor,
                    {| elementType = if isTitle then ParagraphElement.elementType else TitleElement.elementType |}
                )
            | _ -> ()

        Html.div [
            prop.classes [ "flex"; "flex-col"; "space-y-2" ]
            prop.style [ style.width 500 ]
            prop.children [
                Slate.init [
                     Slate.editor editor
                     Slate.value nodes
                     Slate.onChange setNodes
                     Slate.children [
                         Components.Toolbar ()
                         Slate.editable [
                             Editable.renderElement renderElement
                             prop.autoFocus true
                             prop.classes [ "border"; "border-gray-2"; "p-6" ]
                             prop.onKeyDown onKeyDown
                         ]
                     ]
                ]
            ]
        ]

