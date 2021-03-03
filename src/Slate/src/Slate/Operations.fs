module Slate.Operations

open Fable.Core
open Slate.Types
open Fable.Core.JsInterop


type Operations =
    static member insertNode (type': string, path: IPath, node: INode) =
        unbox<InsertNodeOperation> {| ``type`` = type'; path = path; node = node |}

    static member insertText (type': string, path: IPath, offset: int, text: string) =
        unbox<InsertTextOperation> {| ``type`` = type'; path = path; offset = offset; text = text |}

    static member insertText (type': string, path: IPath, position: int, properties: (string * obj) seq) =
        unbox<MergeNodeOperation> {| ``type`` = type'; path = path; position = position; properties = properties |}

    static member insertText (type': string, path: IPath, position: int, properties: obj) =
        unbox<MergeNodeOperation> {| ``type`` = type'; path = path; position = position; properties = !!properties |}

    static member setNode (type': string, path: IPath, properties: (string * obj) seq, newProperties: (string * obj) seq) =
        unbox<SetNodeOperation> {| ``type`` = type'; path = path; properties = properties; newProperties = newProperties |}

    static member setNode (type': string, path: IPath, properties: obj, newProperties: obj) =
        unbox<SetNodeOperation> {| ``type`` = type'; path = path; properties = !!properties; newProperties = !!newProperties |}
