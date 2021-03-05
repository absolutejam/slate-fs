module SlateExample.Plugins.Layout

open Slate.Node
open Slate.Core
open Slate.Types
open Slate.Editor
open Slate.Element
open Slate.Operations
open Slate.Transforms
open Slate.Operations.OperationTypes

open Slate.FSharpExtended

open SlateExample.Icons
open SlateExample.Styles
open SlateExample.Elements

/// This forces the first element in the editor to be a `title` element, as well as
/// triggering placeholder if the title is empty and not selected
let withLayout (editor: IEditor) =
    let normalizeNode = editor.normalizeNode
    editor.normalizeNode <- fun (node, path) ->
        // Force the first node to be a title element
        if path.[0] = 0 then
            Transforms.setNodes (editor, {| elementType = "title" |}, at=Location.Path [|0|])
        // TODO: Delete & replace to remove extra properties?
        // TODO: Handle non-text elements

        normalizeNode (node, path)

    let apply = editor.apply
    editor.apply <- fun op ->
        apply op

        let activeOpts      = {| isPlaceholder = false |}
        let placeholderOpts = {| isPlaceholder = true  |}

        ignore <| option {
            let! selectionOp = Operationsx.(|SetSelection|_|) op

            // Current cursor path
            let! newProperties = selectionOp.newProperties
            let! anchor = newProperties.anchor
            let cursorPath = anchor.path

            let firstNode, firstNodePath = Editor.node (editor, at=Location.Path [|0|])
            let! firstNodeTitle = Elements.(|TitleElement|_|) firstNode
            let! placeholderText = firstNodeTitle.placeholderText

            // The contained text element
            let firstNodeChild, firstNodeChildPath = Node.children (firstNode, path=[||]) |> Array.ofSeq |> Array.head
            let firstNodeText = firstNodeChild |> unbox<IText>

            let firstNodeChildAbsolutePath = [| yield! firstNodePath; yield! firstNodeChildPath |]

            // If the first element is selected and it's a placeholder, revert it to an empty active title
            if cursorPath.[0] = firstNodePath.[0] && firstNodeTitle.isPlaceholder then
                Transforms.setNodes (editor, activeOpts, at=Location.Path firstNodePath)
                Transforms.insertText (editor, "", at=Location.Path firstNodeChildAbsolutePath)

            // If any other element is active and the first tile is empty, make it a placeholder
            elif cursorPath.[0] <> firstNodePath.[0] && (firstNodeText.text = "" || isNull firstNodeText.text) then
                Transforms.setNodes (editor, placeholderOpts, at=Location.Path firstNodePath)
                Transforms.insertText (editor, placeholderText, at=Location.Path firstNodeChildAbsolutePath)
        }

    editor
