module Slate.FSharpExtended.OperationsEx

open Slate.Types
open Fable.Core.JsInterop
open Slate.Operations.OperationTypes

module OperationsEx =
    let (|InsertNode|_|) (op: IOperation) =
        if (!!op.``type``) = "insert_node"
        then Some (op :?> InsertNodeOperation)
        else None

    let (|InsertText|_|) (op: IOperation) =
        if (!!op.``type``) = "insert_text"
        then Some (op :?> InsertTextOperation)
        else None

    let (|MergeNode|_|) (op: IOperation) =
        if (!!op.``type``) = "merge_node"
        then Some (op :?> MergeNodeOperation)
        else None

    let (|MoveNode|_|) (op: IOperation) =
        if (!!op.``type``) = "move_node"
        then Some (op :?> MoveNodeOperation)
        else None

    let (|RemoveNode|_|) (op: IOperation) =
        if (!!op.``type``) = "remove_node"
        then Some (op :?> RemoveNodeOperation)
        else None

    let (|RemoveText|_|) (op: IOperation) =
        if (!!op.``type``) = "remove_text"
        then Some (op :?> RemoveTextOperation)
        else None

    let (|SetNode|_|) (op: IOperation) =
        if (!!op.``type``) = "set_node"
        then Some (op :?> SetNodeOperation)
        else None

    let (|SetSelection|_|) (op: IOperation) =
        if (!!op.``type``) = "set_selection"
        then Some (op :?> SetSelectionOperation)
        else None

    let (|SplitNode|_|) (op: IOperation) =
        if (!!op.``type``) = "split_node"
        then Some (op :?> SplitNodeOperation)
        else None

    let ifInsertNode op (mapper: _ -> unit) =
        match op with
        | InsertNode n -> mapper n
        | _ -> ()

    let ifInsertText op (mapper: _ -> unit) =
        match op with
        | InsertText n -> mapper n
        | _ -> ()

    let ifMergeNode op (mapper: _ -> unit) =
        match op with
        | MergeNode n -> mapper n
        | _ -> ()

    let ifMoveNode (mapper: _ -> unit) = function
        | MoveNode n -> mapper n
        | _ -> ()

    let ifRemoveNode op (mapper: _ -> unit) =
        match op with
        | RemoveNode n -> mapper n
        | _ -> ()

    let ifRemoveText op (mapper: _ -> unit) =
        match op with
        | RemoveNode n -> mapper n
        | _ -> ()

    let ifSetNode op (mapper: _ -> unit) =
        match op with
        | SetNode n -> mapper n
        | _ -> ()

    let ifSetSelection op (mapper: _ -> unit) =
        match op with
        | SetSelection n -> mapper n
        | _ -> ()

    let ifSplitNode op (mapper: _ -> unit) =
        match op with
        | SplitNode n -> mapper n
        | _ -> ()

type OperationsEx =
    static member insertNode (path: IPath, node: INode) =
        unbox<InsertNodeOperation> {| ``type`` = "insert_node"; path = path; node = node |}

    static member insertText (path: IPath, offset: int, text: string) =
        unbox<InsertTextOperation> {| ``type`` = "insert_text"; path = path; offset = offset; text = text |}

    static member mergeNode (path: IPath, position: int, properties: (string * obj) seq) =
        unbox<MergeNodeOperation> {| ``type`` = "merge_node"; path = path; position = position; properties = properties |}

    static member moveNode (path: IPath, newPath: IPath) =
        unbox<MoveNodeOperation> {| ``type`` = "move_node"; path = path; newPath = newPath |}

    static member removeNode (path: IPath, node: INode) =
        unbox<RemoveNodeOperation> {| ``type`` = "remove_node"; path = path; node = node |}

    static member removeText (path: IPath, offset: Number, text: string) =
        unbox<RemoveTextOperation> {| ``type`` = "remove_text"; path = path; offset = offset; text = text |}

    static member setNode (properties: obj, newProperties: obj) =
        unbox<SetNodeOperation> {| ``type`` = "set_node"; properties = !!properties; newProperties = !!newProperties |}

    static member setNode (properties: (string * obj) seq, newProperties: (string * obj) seq) =
        unbox<SetNodeOperation> {| ``type`` = "set_node"; properties = createObj properties; newProperties = createObj newProperties |}

    static member setSelection (?properties: IPartialRange, ?newProperties: IPartialRange) =
        unbox<SetSelectionOperation> {| ``type`` = "set_selection"; properties = properties; newProperties = newProperties |}

    static member splitNode (path: IPath, position: Number, properties: obj) =
        unbox<SetNodeOperation> {| ``type`` = "split_node"; path = path; position = position; properties = !!properties |}

    static member splitNode (path: IPath, position: Number, properties: (string * obj) seq) =
        unbox<SetNodeOperation> {| ``type`` = "split_node"; path = path; position = position; properties = createObj properties |}

