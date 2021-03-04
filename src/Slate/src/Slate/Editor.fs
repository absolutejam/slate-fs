module Slate.Editor

open Fable.Core
open Slate.Types
open Fable.Core.JsInterop

/// Wraps the js implementation
module Internal  =
    module EditorTypes =
        type AboveOptions =
            abstract member at: Location option
            abstract member match': NodeMatch option
            abstract member mode: Mode option
            abstract member voids: bool option

        type BeforeAfterOptions =
            abstract member distance: int option
            abstract member unit: Unit option
            abstract member voids: bool option

        type DeleteOptions = abstract member unit: Unit option

        type LeafOptions =
            abstract member depth: int option
            abstract member edge: Edge option

        type LevelsOptions =
            abstract member at: Location option
            abstract member match': NodeMatch option
            abstract member reverse: bool option
            abstract member voids: bool option

        type NextOptions =
            abstract member at: Location option
            abstract member match': NodeMatch option
            abstract member mode: Mode option
            abstract member voids: bool option

        type NodeOptions =
            abstract member depth: int option
            abstract member edge: Edge option

        type NodesOptions =
            abstract member at: Location option
            abstract member match': (INode -> bool) option
            abstract member mode: Mode option
            abstract member universal: bool option
            abstract member reverse: bool option
            abstract member voids: bool option

        type NormalizeOptions = abstract member force: bool option

        type ParentOptions =
            abstract member depth: int option
            abstract member edge: Edge option

        type PathOptions =
            abstract member depth: int option
            abstract member edge: Edge option

        type PathRefOptions = abstract member affinity: Affinity option
        type PointOptions = abstract member edge: Edge option
        type PointRefOptions = abstract member edge: Edge option

        type PositionsOptions =
            abstract member at: Location option
            abstract member unit: Unit option
            abstract member reverse: bool option

        type PreviousOptions =
            abstract member at: Location option
            abstract member match': NodeMatch
            abstract member mode: Mode option
            abstract member voids: bool option

        type RangeRefOptions = abstract member affinity: RangeAffinity option
        type StringOptions = abstract member voids: bool option
        type UnhangRangeOptions = abstract member voids: bool option

        type VoidOptions =
            abstract member at: Location option
            abstract member mode: Mode option
            abstract member voids: bool option

    open EditorTypes

    type Editor =
        abstract member above:              editor: IEditor * ?options: AboveOptions -> NodeEntry<'t> option
        abstract member addMark:            editor: IEditor * key: string * value: obj -> unit
        abstract member after:              editor: IEditor * at: Location * ?options: BeforeAfterOptions -> IPoint option
        abstract member before:             editor: IEditor * at: Location * ?options: BeforeAfterOptions -> IPoint option
        abstract member deleteBackward:     editor: IEditor * ?options: DeleteOptions -> unit
        abstract member deleteFragment:     editor: IEditor -> unit
        abstract member edges:              editor: IEditor * at: Location -> IPoint * IPoint
        abstract member end':               editor: IEditor * at: Location -> IPoint
        abstract member first:              editor: IEditor * at: Location -> NodeEntry<INode>
        abstract member fragment:           editor: IEditor * at: Location -> Descendant[]
        abstract member hasBlocks:          editor: IEditor * element: IElement -> bool
        abstract member hasInlines:         editor: IEditor * element: IElement -> bool
        abstract member hasTexts:           editor: IEditor * element: IElement -> bool
        abstract member insertBreak:        editor: IEditor -> unit
        abstract member insertFragment:     editor: IEditor * fragment: INode[] -> unit
        abstract member insertNode:         editor: IEditor * node: INode -> unit
        abstract member insertText:         editor: IEditor * text: string -> unit
        abstract member isBlock:            editor: IEditor * value: obj -> bool
        abstract member isEditor:           value: obj -> bool
        abstract member isEnd:              editor: IEditor * point: IPoint * at: Location -> bool
        abstract member isEdge:             editor: IEditor * point: IPoint * at: Location -> bool
        abstract member isEmpty:            editor: IEditor * element: IElement -> bool
        abstract member isInline:           editor: IEditor * value: obj -> bool
        abstract member isNormalizing:      editor: IEditor -> bool
        abstract member isStart:            editor: IEditor * point: IPoint * at: Location -> bool
        abstract member isVoid:             editor: IEditor * value: obj -> bool
        abstract member last:               editor: IEditor * at: Location -> NodeEntry<INode>
        abstract member leaf:               editor: IEditor * at: Location * ?options: LeafOptions -> NodeEntry<IText>
        abstract member levels:             editor: IEditor *  ?options: LevelsOptions -> NodeEntry<INode> seq // TODO: Generator<...>
        abstract member marks:              editor: IEditor -> IText // TODO: Omit<...>
        abstract member next:               editor: IEditor * ?options: NextOptions -> NodeEntry<INode> option
        abstract member node:               editor: IEditor * at: Location * ?options: NodeOptions -> NodeEntry<INode>
        abstract member nodes:              editor: IEditor * ?options: NodesOptions -> NodeEntry<INode> seq // TODO: Generator<...>
        abstract member normalize:          editor: IEditor * ?options: NormalizeOptions -> unit
        abstract member parent:             editor: IEditor * at: Location * ?options: ParentOptions -> NodeEntry<Ancestor>
        abstract member path:               editor: IEditor * at: Location * ?options: PathOptions -> IPath
        abstract member pathRef:            editor: IEditor * path: IPath * ?options: PathRefOptions -> IPathRef
        abstract member pathRefs:           editor: IEditor -> IPathRef[]
        abstract member point:              editor: IEditor * at: Location * ?options: PointOptions -> IPoint
        abstract member pointRef:           editor: IEditor * point: IPoint * ?options: PointRefOptions -> IPointRef
        abstract member pointRefs:          editor: IEditor -> IPointRef[]
        abstract member positions:          editor: IEditor * ?options: PositionsOptions -> IPoint seq // TODO: Generator<...>
        abstract member previous:           editor: IEditor * ?options: PreviousOptions -> NodeEntry<INode> option
        abstract member range:              editor: IEditor * at: Location * ?to': Location -> IRange
        abstract member rangeRef:           editor: IEditor * range: IRange * ?options: RangeRefOptions -> IRangeRef
        abstract member rangeRefs:          editor: IEditor -> IRangeRef[]
        abstract member removeMark:         editor: IEditor * key: string -> unit
        abstract member start:              editor: IEditor * at: Location -> IPoint
        abstract member string:             editor: IEditor * at: Location * ?options: StringOptions -> string
        abstract member unhangRange:        editor: IEditor * range: IRange * ?options: UnhangRangeOptions -> IRange
        abstract member void':              editor: IEditor * ?options: VoidOptions -> NodeEntry<IElement> option
        abstract member withoutNormalizing: editor: IEditor * editorFun: (unit -> unit) -> unit

    let editorInterface : Editor = import "Editor" "slate"

/// The default `Editor` static methods, tweaked to make more F#-friendly
type Editor =
    static member rangeRef (editor: IEditor, range: IRange, ?affinity: RangeAffinity) =
        let opts = affinity |> Option.map (fun r -> {| affinity = Some r |})
        Internal.editorInterface.rangeRef (editor, range, unbox<Internal.EditorTypes.RangeRefOptions> opts)

    // above

    static member addMark (editor: IEditor, key: string, value: obj) =
        editor.addMark (key, value) // TODO: obj??

    // after

    // before

    // deleteBackward

    // deleteForward

    static member deleteFragment (editor: IEditor) =
        Internal.editorInterface.deleteFragment (editor: IEditor)

    static member edges (editor: IEditor, at: Location): IPoint * IPoint =
        Internal.editorInterface.edges (editor, at)

    static member end' (editor: IEditor, at: Location): IPoint =
        Internal.editorInterface.end' (editor, at)

    static member isEditor (value: obj): bool =
        Internal.editorInterface.isEditor (value)

    static member nodes
        (
            editor: IEditor,
            ?at: Location,
            ?match': INode -> bool,
            ?mode: NodesMode,
            ?universal: bool,
            ?reverse: bool,
            ?voids: bool
        )
        =
        let options = {| at = at; ``match`` = match'; mode = mode; universal = universal; reverse = reverse; voids = voids |}
        Internal.editorInterface.nodes (editor, unbox<Internal.EditorTypes.NodesOptions> options)

    static member point (editor: IEditor, at: Location, ?edge: Edge) =
        let options = unbox<Internal.EditorTypes.PointOptions> {| edge = edge |}
        Internal.editorInterface.point (editor, at, options)

    static member start (editor: IEditor, at: Location): IPoint =
        Internal.editorInterface.start (editor, at)

    static member withoutNormalizing (editor: IEditor, editorFun: unit -> unit) =
        Internal.editorInterface.withoutNormalizing (editor, editorFun)