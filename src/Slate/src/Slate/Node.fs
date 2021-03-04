module Slate.Node

open Slate.Types
open Fable.Core.JsInterop

module Internal =
    module NodeTypes =
        type ChildrenOptions = abstract member reverse: bool option

    open NodeTypes

    type Node =
        abstract member children: root: INode * path: IPath * ?options: ChildrenOptions -> NodeEntry<Descendant> seq
        abstract member first: root: INode * path: IPath -> NodeEntry<INode>

    let nodeInterface : Node = import "Node" "slate"

type Node =
    static member children (root: INode, path: IPath, ?reverse: bool) =
        let opts = reverse |> Option.map (fun r -> {| reverse = Some r |})
        Internal.nodeInterface.children (root, path, unbox<Internal.NodeTypes.ChildrenOptions> opts)

    static member first (root: INode, path: IPath) =
        Internal.nodeInterface.first (root, path)
