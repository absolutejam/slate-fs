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
open Slate.FSharpExtended.OperationsEx

open SlateExample.Icons
open SlateExample.Styles
open SlateExample.Elements

let isActive f editor =
    let pred = NodeEx.(|Element|_|) >> Option.bind f >> Option.isSome
    Editor.nodes (editor, match' = pred) |> (not << Seq.isEmpty)

module Helpers =
    let withTitleElementFirstChild node f =
        node |> NodeEx.ifElement (Elements.ifTitleElement (NodeEx.withElementFirstChild (NodeEx.ifText f)))

    /// Run f if...
    ///   * operation is a `SetSelection`
    ///   * has `newProperties`
    ///   * has `anchor`
    let withSelectionCursorPath op f =
        OperationsEx.ifSetSelection op <| fun selectionOp ->
            selectionOp.newProperties |> Option.iter (fun newProperties ->
                newProperties.anchor |> Option.iter (fun anchor ->
                    f anchor.path
                )
            )

let jsonify x = Fable.Core.JS.JSON.stringify (x, space=4)


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


let withLayout (editor: IEditor) =
    let normalizeNode = editor.normalizeNode
    editor.normalizeNode <- fun (node, path) ->
        // Force the first node to be a title element
        if path.[0] = 0 then
            Transforms.setNodes (editor, {| elementType = "title" |}, at = Location.Path [|0|])

        normalizeNode (node, path)

    let apply = editor.apply
    editor.apply <- fun op ->
        apply op

//        Browser.Dom.console.log ("Got operation: ", op.``type``)

        OperationsEx.ifSplitNode op <| fun splitNodeOp ->
            Browser.Dom.console.log ("Split node:", jsonify splitNodeOp)
//            let beforePath = (Array.copy splitNodeOp.path)
//            beforePath.[0] <- beforePath.[0] - 1
//            let nodes = Array.ofSeq <| Editor.nodes (editor, at=Location.Path beforePath)
//            Browser.Dom.console.log ("Got nodes before:", jsonify nodes)

        Helpers.withSelectionCursorPath op <| fun cursorPath ->
            // First element selected
            if cursorPath.[0] = 0 then
                let nodes = Array.ofSeq <| Editor.nodes (editor, at=Location.Path [||])
                let firstNode, _ = Array.get nodes 1
                Helpers.withTitleElementFirstChild firstNode <| fun text ->
                    // If the text is empty, reset it to the placeholder
                    if text.text = "" || isNull text.text then
                        let firstNodePath = Location.Path [|0|]
                        Transforms.delete (editor, at=firstNodePath)
                        Transforms.insertNodes (editor, [| Elements.titlePlaceholder "Title..." |], at=firstNodePath)

            else  // Not first element
                let nodes = Array.ofSeq <| Editor.nodes (editor, at=Location.Path [||])
                let firstNode, _ = Array.get nodes 1
                firstNode |> NodeEx.ifElement (Elements.ifTitleElement (fun title ->
                    if title.placeholder then
                        let firstNodePath = Location.Path [|0|]
                        Transforms.setNodes (editor, {| placeholder = false |}, at=firstNodePath)
                ))

    editor

let renderElement (props: RenderElementProps) =
    match props.element with
    | Elements.TitleElement _   -> ElementComponents.TitleElement props
    | Elements.SectionElement _ -> ElementComponents.SectionElement props
    | _                         -> ElementComponents.ParagraphElement props

[<ReactComponent>]
let Example (props: {| nodeState: INode[] * (INode[] -> unit) |}) =
    let nodes, setNodes = props.nodeState

    let editor = React.useMemo ((fun () -> createEditor () |> withReact |> withHistory |> withLayout), [||])
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

