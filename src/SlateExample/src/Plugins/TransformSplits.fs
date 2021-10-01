module SlateExample.Plugins.TransformSplits

open Slate.Types
open Slate.Editor
open Slate.Transforms

open Slatex
open SlateExample
open SlateExample.Elements
open FsToolkit.ErrorHandling

// TODO: Not tested with nested elements
/// When an element is split (via. the return key), if `predicate` is true,
/// replace the newly created element with a `TextElement` containing the same text
let withTransformSplits (predicate: IElement -> bool) (editor: IEditor) =
    let insertBreak = editor.insertBreak

    let mutable breakInserted = false
    let insertBreakOnce = fun () ->
        insertBreak ()
        breakInserted <- true

    editor.insertBreak <- fun _ ->
        option {
            let! currentSelection = editor.selection
            let parent, _ = Editor.parent (editor, at = Location.Path currentSelection.anchor.path)

            let! element = Nodex.(|Element|_|) parent

            // Bail out if the predicate does not match
            if not (predicate element) then return ()

            let! textEl = element.children |> Array.tryHead |> Option.map unbox<IText>

            insertBreakOnce ()

            if currentSelection.anchor.offset <= textEl.text.Length then
                let! newSelection = editor.selection
                let newNode, newNodePath = Editor.parent (editor, at=Location.Path newSelection.anchor.path)
                let newTitleEl = newNode |> unbox<TitleElement>
                let! newTextEl = newTitleEl.children |> Array.tryHead |> Option.map unbox<IText>

                Transforms.delete (editor, at = Location.Path newNodePath)
                Transforms.insertNodes (editor, [| Elements.paragraph newTextEl.text |], at = Location.Path newNodePath)
                Helpers.requestAnimationFrame (fun _ -> Transforms.move (editor, distance = 1, unit = PointerUnit.Line))
        }
        |> Option.defaultWith insertBreakOnce

    editor

