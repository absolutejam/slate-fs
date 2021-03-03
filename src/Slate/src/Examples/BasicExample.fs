module SlateExample.Examples.BasicExample

open Feliz
open Browser.Types
open Fable.Core.JsInterop

open Slate.Node
open Slate.Core
open Slate.Types
open Slate.Editor
open Slate.Element
open Slate.Transforms

open Slate.FSharpExtended

open SlateExample.Icons
open SlateExample.Styles

module ElementComponents =
    [<ReactComponent>]
    let SectionElement (props: RenderElementProps) =
        Html.div [
            yield! splatElementAttributes props.attributes
            prop.classes [ tw.``p-3``; tw.``border``; tw.``border-gray-1`` ]
            prop.children [
                props.children
            ]
        ]

    [<ReactComponent>]
    let TitleElement (props: RenderElementProps) =
        Html.h2 [
            yield! splatElementAttributes props.attributes
            prop.classes [ tw.``text-xl``; tw.``font-medium`` ]
            prop.children [
                props.children
            ]
        ]

    [<ReactComponent>]
    let ParagraphElement (props: RenderElementProps) =
        Html.p [
            yield! splatElementAttributes props.attributes
            prop.children [ props.children ]
        ]

type SectionElement =
    {
        children: INode[]
    }
    static member elementType = "section"
    interface IElement with
        member this.elementType = SectionElement.elementType
        member this.children = this.children

type ParagraphElement =
    {
        paragraph: string
        children:  INode[]
    }
    static member elementType = "paragraph"
    interface IElement with
        member this.elementType = ParagraphElement.elementType
        member this.children = this.children

type TitleElement =
    {
        title:    string
        children: INode[]
    }
    static member elementType = "title"
    interface IElement with
        member this.elementType = TitleElement.elementType
        member this.children = this.children

module Elements =
    let (|SectionElement|_|) (element: IElement) =
        if (!!element.elementType) = SectionElement.elementType
        then Some (element :?> SectionElement)
        else None

    let (|ParagraphElement|_|) (element: IElement) =
        if (!!element.elementType) = ParagraphElement.elementType
        then Some (element :?> ParagraphElement)
        else None

    let (|TitleElement|_|) (element: IElement) =
        if (!!element.elementType) = TitleElement.elementType
        then Some (element :?> TitleElement)
        else None

    let isSectionElement (element: IElement)   = (|SectionElement|_|) element |> Option.isSome
    let isParagraphElement (element: IElement) = (|ParagraphElement|_|) element |> Option.isSome
    let isTitleElement (element: IElement)     = (|TitleElement|_|) element |> Option.isSome

    let mapSectionElement (mapper: _ -> IElement) (element: IElement) =
        match element with
        | SectionElement sectionElement -> mapper sectionElement :> INode
        | _ -> element :> INode

    let mapParagraphElement (mapper: _ -> IElement) (element: IElement) =
        match element with
        | ParagraphElement paragraphElement -> mapper paragraphElement :> INode
        | _ -> element :> INode

    let mapTitleElement (mapper: _ -> IElement) (element: IElement) =
        match element with
        | TitleElement titleElement -> mapper titleElement :> INode
        | _ -> element :> INode

    let section (children: IElement[]) =
        unbox<IElement>
            {|
                elementType = SectionElement.elementType
                children    = children
            |}

    let title (title: string) =
        unbox<IElement>
            {|
                elementType = TitleElement.elementType
                title       = title
                children    = [| NodeEx.textNode title |]
            |}

    let paragraph (paragraph: string) =
        unbox<IElement>
            {|
                elementType = ParagraphElement.elementType
                paragraph   = paragraph
                children    = [| NodeEx.textNode paragraph |]
            |}


let isActive f editor =
    let pred node _path = NodeEx.(|Element|_|) node |> Option.bind f |> Option.isSome
    Editor.nodes (editor, match' = pred) |> (not << Seq.isEmpty)

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

[<ReactComponent>]
let AdminToolbar (props: {| output: string |}) =
    let editor = useSlate ()

    Html.div [
        prop.classes [ tw.``flex``; tw.``flex-col``; tw.``w-full``; tw.``space-y-4`` ]
        prop.children [
            Html.div [
                prop.classes [ tw.``flex``; tw.``w-full``; tw.``space-x-2`` ]
                prop.children [
                   Html.button [
                       prop.classes [ tw.``flex``; tw.``items-center``; tw.``px-3``; tw.``py-1``; tw.``border``; tw.``focus:outline-none`` ]
                       prop.children [ Html.text "Show nodes" ]
                   ]
                ]
            ]
            Html.div [
                prop.classes [ tw.``flex``; tw.``w-full``; tw.``text-xs`` ]
                prop.children [
                    Html.pre [
                        prop.classes [ tw.``flex``; tw.``p-4`` ]
                        prop.children [ Html.text props.output ]
                    ]
                ]
            ]
        ]
    ]

let withLayout (editor: IEditor) =
    let normalizeNode = editor.normalizeNode
    editor.normalizeNode <- fun (node, path) ->
        if path.Length = 0 then
            /// First item
            if editor.children.Length < 1 then
                let title = Elements.paragraph "Untitled"
                let newPath = Location.Path (Array.append path [| 0 |])
                Transforms.insertNodes (editor, [| title |], at=newPath)

            /// Second item
            if editor.children.Length < 2 then
                let paragraph = Elements.paragraph "Some text"
                let newPath = Location.Path (Array.append path [| 1 |])
                Transforms.insertNodes (editor, [| paragraph |], at=newPath)

            /// Force the first element to be a title
            for (child, childPath) in Node.children (editor, path) do
                let elType = if childPath.[0] = 0 then "title" else "paragraph"

                child |> NodeEx.ifElement (fun el ->
                    if el.elementType <> elType then
                        let newProperties = {| elementType = elType |}
                        Transforms.setNodes (editor, newProperties, at = Location.Path childPath)
                )

                child |> NodeEx.ifElement (fun el ->
                    Browser.Dom.console.log ("Waffle")
                    if childPath.[0] = 0 then
                        let title = el :?> TitleElement
                        match Array.tryHead title.children with
                        | Some (NodeEx.Text t) when t.text = "" || isNull t.text ->
                            Browser.Dom.console.log ("Empty text - Will recreate!")
                        | None ->
                            Browser.Dom.console.log ("No text - Will recreate!")
                        | _ -> ()
                )

        normalizeNode (node, path)

    editor

[<ReactComponent>]
let Example () =
    let editor = React.useMemo ((fun () -> createEditor () |> withReact |> withLayout), [||])

    let initialState : INode[] =
        [|
            Elements.title "Welcome!"
            Elements.paragraph "Here is some starting text..."
        |]

    let (nodes, setNodes) = React.useState (initialState)

    // Define a rendering function based on the element passed to `props`. We use
    // `useCallback` here to memoize the function for subsequent renders.
    let renderElement =
        React.useCallback ((fun (props: RenderElementProps) ->
            match props.element with
            | Elements.TitleElement _   -> ElementComponents.TitleElement props
            | Elements.SectionElement _ -> ElementComponents.SectionElement props
            | _                         -> ElementComponents.ParagraphElement props
        ), [||])

    let (output, setOutput) = React.useState ("")

    let onKeyDown (ev: KeyboardEvent) =
        match ev.key, ev.metaKey with
        | "k", true ->
            ev.preventDefault ()
            let isTitle = isActive Elements.(|TitleElement|_|) editor
            Transforms.setNodes (
                editor,
                {| elementType = if isTitle then ParagraphElement.elementType else TitleElement.elementType |}
            )

//            nodes |> Array.map (Node.mapElement (unbox << function
//               | CodeElement.CodeElement e ->
//                   Nodes.Default (children = e.children)
//               | _ as e ->
//                   Nodes.Code (language = "F#", children = e.children)
//            ))
//            |> setNodes
        | _ -> ()

    Html.div [
        prop.classes [ tw.``space-y-2`` ]
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
                         prop.onKeyDown (fun ev ->
                            setOutput (Fable.Core.JS.JSON.stringify (editor.children, space=4))
                            onKeyDown ev
                         )
                     ]
                     AdminToolbar {| output = output |}
                 ]
            ]
        ]
    ]

