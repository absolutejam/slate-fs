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

    let ifElement (mapper: IElement -> unit) (node: obj) =
        match node with
        | Element element -> mapper element
        | _ -> ()

    let mapElement (mapper: IElement -> INode) (node: INode) =
        match node with
        | Element element -> mapper element
        | _ -> node

    let mapEditor (mapper: IEditor -> INode) (node: INode) =
        match node with
        | Editor editor -> mapper editor
        | _ -> node

    let mapText (mapper: IText -> INode) (node: INode) =
        match node with
        | Text text -> mapper text
        | _ -> node

    let textNode (text: string) = unbox<IText> {| text = text |}
    let elementNode (children: INode[]) = unbox<IElement> {| children = children |}

