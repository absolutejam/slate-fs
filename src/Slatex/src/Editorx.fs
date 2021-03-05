/// FSharp-specific utilities that provide a more idiomatic API
namespace Slate.FSharpExtended

open Slate.Node
open Slate.Types
open Slate.Editor

/// Methods for transforming an `IEditor` in a functional way
type Editorx =
    static member updateChildren (updateFun: INode -> INode) (editor: IEditor) =
        editor.children <- editor.children |> Array.map updateFun

    static member updateSelected (updateFun: IElement -> INode) (editor: IEditor) =
        ()
//        match editor.selection with
//        | None -> ()
//        | Some selection ->
//            Editor.withoutNormalizing (editor, fun () ->
//                let rangeRef = Editor.rangeRef (editor, editor.selection, RangeRef.Inward)
//                let start, end' = Range.edges at
//
//            )


    static member updateElements (updateFun: IElement -> INode) (editor: IEditor)  =
        Editor.withoutNormalizing (editor, fun () ->
            editor.children <- editor.children |> Array.map (Nodex.mapElement updateFun)
        )

    static member updateEditors (updateFun: IEditor -> INode) (editor: IEditor)  =
        editor.children <- editor.children |> Array.map (Nodex.mapEditor updateFun)

    static member updateTexts (updateFun: IText -> INode) (editor: IEditor)  =
        editor.children <- editor.children |> Array.map (Nodex.mapText updateFun)

