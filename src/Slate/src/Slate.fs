module Slate.Core

open Feliz
open Fable.Core
open Slate.Types
open Fable.Core.JsInterop

[<Import("createEditor", from="slate")>]
let createEditor (): IEditor = jsNative

[<Import("withReact", from="slate-react")>]
let withReact (editor: IEditor) : IEditor = jsNative

[<Import("withHistory", from="slate-history")>]
let withHistory (editor: IEditor) : IEditor = jsNative

[<Import("useSlate", from="slate-react")>]
let useSlate () : IEditor = jsNative

type Editable =
    static member inline placeholder value = prop.placeholder value
    static member inline readOnly value = prop.readOnly value

    static member inline decorate (decorate: NodeEntry<INode> -> IRange[]) =
        Interop.mkAttr "decorate" decorate

    static member inline onDOMBeforeInput (onDOMBeforeInput: Event -> unit) =
        Interop.mkAttr "onDOMBeforeInput" onDOMBeforeInput

    static member inline role (role: string) =
        Interop.mkAttr "role" role

    static member inline renderElement (renderElement: RenderElementProps -> ReactElement) =
        Interop.mkAttr "renderElement" renderElement

    static member inline renderLeaf (renderLeaf: RenderLeafProps -> ReactElement) =
        Interop.mkAttr "renderLeaf" renderLeaf

    static member inline init (props: IReactProperty seq) =
        Interop.reactApi.createElement (
            import "Editable" "slate-react",
            createObj !!props
        )

[<RequireQualifiedAccess>]
type SlateArg = Prop of IReactProperty | Children of ReactElement list

type Slate =
    static member inline editor (editor: IEditor) =
        SlateArg.Prop <| Interop.mkAttr "editor" editor

    static member inline value (value: INode[]) =
        SlateArg.Prop <| Interop.mkAttr "value" value

    static member inline onChange (onChange: INode[] -> unit) =
        SlateArg.Prop <| Interop.mkAttr "onChange" onChange

    static member inline children = SlateArg.Children

    static member inline editable (props: IReactProperty list) =
        Interop.reactApi.createElement (
            import "Editable" "slate-react",
            createObj !!props
        )

    static member inline init (props: SlateArg seq) =
        let props, children =
            (([], []), props)
            ||> Seq.fold (fun (props, children) slateArg ->
                match slateArg with
                | SlateArg.Prop prop -> props @ [prop], children
                | SlateArg.Children newChildren -> props, newChildren
            )

        Interop.reactApi.createElement
            (
                import "Slate" "slate-react",
                createObj !!props,
                children
            )

