namespace Slate.FSharpExtended

open Slate.Types
open Fable.Core.JsInterop

type private Text'    = Slate.Text.Text
type private Editor'  = Slate.Editor.Editor
type private Element' = Slate.Element.Element

module NodeEx =
    let (|Text|_|) (node: obj) =
        if Text'.isText node
        then Some (node :?> IText)
        else None

    let (|Element|_|) (node: obj) =
        if Element'.isElement node
        then Some (node :?> IElement)
        else None

    let (|Editor|_|) (node: obj) =
        if Editor'.isEditor node
        then Some (node :?> IEditor)
        else None

    let mapElement (mapper: IElement -> INode) (node: INode) =
        match node with
        | Element element -> mapper element
        | _ -> node

    let ifElement (mapper: IElement -> unit) (node: obj) =
        match node with
        | Element element -> mapper element
        | _ -> ()

    let mapEditor (mapper: IEditor -> INode) (node: INode) =
        match node with
        | Editor editor -> mapper editor
        | _ -> node

    let ifEditor (mapper: IEditor -> unit) (node: INode) =
        match node with
        | Editor editor -> mapper editor
        | _ -> ()

    let mapText (mapper: IText -> INode) (node: INode) =
        match node with
        | Text text -> mapper text
        | _ -> node

    let ifText (mapper: IText -> unit) (node: INode) =
        match node with
        | Text text -> mapper text
        | _ -> ()

    let textNode (text: string) = unbox<IText> {| text = text |}
    let elementNode (children: INode[]) = unbox<IElement> {| children = children |}

    let withElementFirstChild (firstChildFun: INode -> unit) (node: IElement) =
        node |> ifElement (fun el -> firstChildFun el.children.[0])