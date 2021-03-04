module Slate.Transforms

open Feliz
open Fable.Core
open Slate.Types
open Slate.Operations
open Fable.Core.JsInterop

type NodeProps =
    static member custom name (custom: 't) = name, box custom

    (* Element / Editor *)
    static member children (children: INode[]) = nameof children, box children

    (* Text *)
    static member text (text: string) = nameof text, box text

    (* Editor *)
    static member selection (selection: IRange option) = nameof selection, box selection
    static member operations (operation: IOperation[]) = nameof operation, box operation
    static member marks (marks: Map<string, obj>) = nameof marks, box marks
    static member isInline (isInline: IElement -> bool) = nameof isInline, box isInline
    static member isVoid (isVoid: IElement -> bool) = nameof isVoid, box isVoid
    static member normalizeNode (normalizeNode: NodeEntry<INode> -> unit) = nameof normalizeNode, box normalizeNode
    static member onChange (onChange: unit -> unit) = nameof onChange, box onChange
    static member addMark (addMark: (string * obj) -> unit) = nameof addMark, box addMark
    static member apply (apply: IOperation -> unit) = nameof apply, box apply
    static member deleteBackward (deleteBackward: Unit -> unit) = nameof deleteBackward, box deleteBackward
    static member deleteForward (deleteForward: Unit -> unit) = nameof deleteForward, box deleteForward
    static member deleteFragment (deleteFragment: unit -> unit) = nameof deleteFragment, box deleteFragment
    static member getFragment (getFragment: unit -> Descendant[]) = nameof getFragment, box getFragment
    static member insertBreak (insertBreak: unit -> unit) = nameof insertBreak, box insertBreak
    static member insertFragment (insertFragment: INode[] -> unit) = nameof insertFragment, box insertFragment
    static member insertNode (insertNode: INode -> unit) = nameof insertNode, box insertNode
    static member insertText (insertText: string -> unit) = nameof insertText, box insertText
    static member removeMark (removeMark: string -> unit) = nameof removeMark, box removeMark

module Internal =
    module TransformsTypes =
        type DeleteParams =
            abstract member at: Location option
            abstract member distance: int option
            abstract member unit: Unit option
            abstract member reverse: bool option
            abstract member hanging: bool option
            abstract member voids: bool option

        type SetNodesParams =
            abstract member at: Location option
            abstract member match': NodeMatch
            abstract member mode: NodesMode option
            abstract member hanging: bool option
            abstract member split: bool option
            abstract member voids: bool option

        type InsertNodesParams =
            abstract member at: Location option
            abstract member match': NodeMatch
            abstract member mode: NodesMode option
            abstract member hanging: bool option
            abstract member split: bool option
            abstract member voids: bool option

        type NodeProps =
            abstract member children: INode[]
            abstract member text: string
            abstract member selection: IRange
            abstract member operations: IOperation[]
            abstract member marks: Map<string, obj>
            abstract member isInline: IElement -> bool
            abstract member isVoid: IElement -> bool
            abstract member normalizeNode: NodeEntry<INode> -> unit
            abstract member onChange: unit -> unit
            abstract member addMark: (string * obj) -> unit
            abstract member apply: IOperation -> unit
            abstract member deleteBackward: Unit -> unit
            abstract member deleteForward: Unit -> unit
            abstract member deleteFragment: unit -> unit
            abstract member getFragment: unit -> Descendant[]
            abstract member insertBreak: unit -> unit
            abstract member insertFragment: INode[] -> unit
            abstract member insertNode: INode -> unit
            abstract member insertText: string -> unit
            abstract member removeMark: string -> unit

    open TransformsTypes

    type Transforms =
        abstract member delete:      editor: IEditor * ?options: DeleteParams -> unit
        abstract member setNodes:    editor: IEditor * props: obj * ?options: SetNodesParams -> unit
        abstract member insertNodes: editor: IEditor * nodes: INode[] * ?options: InsertNodesParams -> unit

    let transformsInterface : Transforms = import "Transforms" "slate"

type Transforms =
    static member delete
        (
            editor: IEditor,
            ?at: Location,
            ?distance: int,
            ?unit: Unit,
            ?reverse: bool,
            ?hanging: bool,
            ?voids: bool
        )
        =
        let options =
            {|
              at = at
              distance = distance
              ``unit`` = unit
              reverse = reverse
              hanging = hanging
              voids = voids
            |}
        Internal.transformsInterface.delete (editor, unbox<Internal.TransformsTypes.DeleteParams> options)

    static member setNodes
        (
            editor: IEditor,
            props: (string * obj) list,
            ?at: Location,
            ?match': NodeMatch,
            ?mode: NodesMode,
            ?hanging: bool,
            ?split: bool,
            ?voids: bool
        )
        =
        let nodeProps = createObj props
        let options = {| at = at; ``match`` = match'; mode = mode; hanging = hanging; split = split; voids = voids |}
        Internal.transformsInterface.setNodes (editor, nodeProps, unbox<Internal.TransformsTypes.SetNodesParams> options)

    static member setNodes
        (
            editor: IEditor,
            props: obj,
            ?at: Location,
            ?match': NodeMatch,
            ?mode: NodesMode,
            ?hanging: bool,
            ?split: bool,
            ?voids: bool
        )
        =
        let nodeProps = !!props
        let options = {| at = at; ``match`` = match'; mode = mode; hanging = hanging; split = split; voids = voids |}
        Internal.transformsInterface.setNodes (editor, nodeProps, unbox<Internal.TransformsTypes.SetNodesParams> options)

    static member insertNodes
        (
            editor: IEditor,
            nodes: INode[],
            ?at: Location,
            ?match': NodeMatch,
            ?mode: NodesMode,
            ?hanging: bool,
            ?split: bool,
            ?voids: bool
        )
        =
        let options = {| at = at; ``match`` = match'; mode = mode; hanging = hanging; split = split; voids = voids |}
        Internal.transformsInterface.insertNodes (editor, nodes, unbox<Internal.TransformsTypes.InsertNodesParams> options)
