module Slate.Operations

open Fable.Core
open Slate.Types
open Fable.Core.JsInterop

module OperationTypes =
    type InsertNodeOperation =
         inherit IOperation
         abstract member path: IPath
         abstract member node: INode

    type InsertTextOperation =
        inherit IOperation
        abstract member path: IPath
        abstract member offset: Number
        abstract member text: string

    type MergeNodeOperation =
        inherit IOperation
        abstract member path: IPath
        abstract member position: Number
        abstract member properties: Map<string, obj>

    type MoveNodeOperation =
        inherit IOperation
        abstract member path: IPath
        abstract member newPath: IPath

    type RemoveNodeOperation =
        inherit IOperation
        abstract member path: IPath
        abstract member node: INode

    type RemoveTextOperation =
        inherit IOperation
        abstract member path: IPath
        abstract member offset: Number
        abstract member text: string

    type SetNodeOperation =
        inherit IOperation
        abstract member path: IPath
        abstract member properties: Map<string, obj>
        abstract member newProperties: Map<string, obj>

    type SetSelectionOperation =
        inherit IOperation
        abstract member properties: IPartialRange option
        abstract member newProperties: IPartialRange option

    type SplitNodeOperation =
        inherit IOperation
        abstract member path: IPath
        abstract member position: Number
        abstract member properties: (string * obj) seq

module Internal =
    type Operations =
        abstract member isNodeOperation:      value: obj -> bool
        abstract member isOperation:          value: obj -> bool
        abstract member isOperationList:      value: obj -> bool
        abstract member isSelectionOperation: value: obj -> bool
        abstract member isTextOperation:      value: obj -> bool
        abstract member inverse:              op: IOperation -> IOperation

    let operationInterface : Operations = import "Operation" "slate"

type Operations =
    static member isOperation (value: obj) = Internal.operationInterface.isOperation value
    static member isOperationList (value: obj) = Internal.operationInterface.isOperationList value
    static member inverse (op: IOperation) = Internal.operationInterface.inverse op

    // This works because the SelectionOperation union has been narrowed to a single type for F#
    static member isSelectionOperation (value: obj) = Internal.operationInterface.isSelectionOperation value
    // TODO: This doesn't really work as there's no TextOperation union
    static member isTextOperation (value: obj) = Internal.operationInterface.isTextOperation value
    // TODO: This doesn't really work as there's no NodeOperation union
    static member isNodeOperation (value: obj) = Internal.operationInterface.isNodeOperation value
