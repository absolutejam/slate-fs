/// FSharp-specific utilities that provide a more idiomatic API
namespace Slatex

open Slate.Node
open Slate.Types
open Slate.Editor

/// Methods for transforming an `IEditor` in a functional way
type Editorx =
    static member selection (editor: IEditor) = editor.selection

    static member children (editor: IEditor) = editor.children
