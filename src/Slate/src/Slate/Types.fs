module Slate.Types

open Feliz
open Fable.Core
open Fable.React
open Browser.Types
type Event = Browser.Types.Event

type Number = int

type IPath = Number[]

type IPoint =
    abstract member point: IPath
    abstract member offset: Number

module Point =
    let Of (point: IPath) (offset: Number) =
        unbox<IPoint> {| point = point; offset = offset |}

type [<StringEnum; RequireQualifiedAccess>] PointAnchor = Anchor | Focus
type [<StringEnum; RequireQualifiedAccess>] Affinity = Forward | Backward
type [<StringEnum; RequireQualifiedAccess>] RangeAffinity = Forward | Backward | Outward | Inward
type [<StringEnum; RequireQualifiedAccess>] Edge = Start | End
type [<StringEnum; RequireQualifiedAccess>] Mode = Highest | Lowest
type [<StringEnum; RequireQualifiedAccess>] NodesMode = Highest | Lowest | All

type PointEntry = IPoint * PointAnchor

type IRange =
    abstract member anchor: IPoint
    abstract member focus: IPoint

module Range =
    let Of anchor focus =
        unbox<IRange> {| anchor = anchor; focus = focus |}

type IPathRef =
    abstract member current: IPath option
    abstract member affinity: Affinity option
    abstract member unref: unit -> IPath option

type IPointRef =
    abstract member current: IPoint option
    abstract member affinity: Affinity option
    abstract member unref: unit -> IPoint option

type IRangeRef =
    abstract member current: IRange option
    abstract member affinity: Affinity option
    abstract member unref: unit -> Range option

/// Used by `Transforms`
type [<RequireQualifiedAccess; Erase>]
    Location =
    | Path  of IPath
    | Point of IPoint
    | Range of IRange

/// Marker interface
and INode = interface end

type NodeMatch = INode -> IPath -> bool

and IEditor =
    inherit INode
    abstract member children: INode[] with get, set
    abstract member selection: IRange option
    abstract member operations: Operation[]
    abstract member marks: Map<string, obj> // TODO: ??
    abstract member isInline: IElement -> bool
    abstract member isVoid: IElement -> bool
    abstract member normalizeNode: (NodeEntry<INode> -> unit) with get, set
    abstract member onChange: unit -> unit
    abstract member addMark: (string * obj) -> unit
    abstract member apply: Operation -> unit
    abstract member deleteBackward: Unit -> unit
    abstract member deleteForward: Unit -> unit
    abstract member deleteFragment: unit -> unit
    abstract member getFragment: unit -> Descendant []
    abstract member insertBreak: unit -> unit
    abstract member insertFragment: Node [] -> unit
    abstract member insertNode: Node -> unit
    abstract member insertText: string -> unit
    abstract member removeMark: string -> unit

and IElement =
    inherit INode
    abstract member children: INode[]
    abstract member elementType: string

and IText =
    inherit INode
    abstract member text: string

and [<RequireQualifiedAccess; Erase>]
    Ancestor =
    | Editor of IEditor
    | Element of IElement

and [<RequireQualifiedAccess; Erase>]
    Descendant =
    | Element of IElement
    | Text of Text

and NodeEntry<'t> = 't * IPath

and [<StringEnum>] Unit = Character | Word | Line | Block

and InsertNodeOperation =
     abstract member ``type``: string
     abstract member path: IPath
     abstract member node: INode

and InsertTextOperation =
    abstract member ``type``: string // insert_text
    abstract member path: IPath
    abstract member offset: Number
    abstract member text: string

and MergeNodeOperation =
    abstract member ``type``: string // merge_node
    abstract member path: IPath
    abstract member position: Number
    abstract member properties: Map<string, obj>

and MoveNodeOperation =
    abstract member ``type``: string // move_node
    abstract member path: IPath
    abstract member newPath: IPath

and RemoveNodeOperation =
    abstract member ``type``: string // remove_node
    abstract member path: IPath
    abstract member node: INode

and RemoveTextOperation =
    abstract member ``type``: string // remove_text
    abstract member path: IPath
    abstract member offset: Number
    abstract member text: string

and SetNodeOperation =
    abstract member ``type``: string // set_node
    abstract member path: IPath
    abstract member properties: Map<string, obj>
    abstract member newProperties: Map<string, obj>

and SetSelectionOperationNewPropertiesOptions =
    abstract member ``type``: string
    abstract member newProperties: IRange

and SetSelectionOperationPartialOptions =
    abstract member ``type``: string
    abstract member properties: Map<string, obj>
    abstract member newProperties: Map<string, obj>

and SetSelectionOperationPropertiesOptions =
    abstract member ``type``: string
    abstract member properties: Map<string, obj>

and [<RequireQualifiedAccess; Erase>]
    SetSelectionOperation =
    | NewProperties of SetSelectionOperationNewPropertiesOptions
    | Partial       of SetSelectionOperationPartialOptions
    | Properties    of SetSelectionOperationPropertiesOptions

and SplitNodeOperation =
    abstract member ``type``: string // split_node
    abstract member path: IPath
    abstract member position: Number
    abstract member properties: Map<string, obj>

and [<RequireQualifiedAccess; Erase>]
    NodeOperation =
    | InsertNodeOperation of InsertNodeOperation
    | MergeNodeOperation  of MergeNodeOperation
    | MoveNodeOperation   of MoveNodeOperation
    | RemoveNodeOperation of RemoveNodeOperation
    | SetNodeOperation    of SetNodeOperation
    | SplitNodeOperation  of SplitNodeOperation

and [<RequireQualifiedAccess; Erase>]
    SelectionOperation =
    | SelectionOperation of SetSelectionOperation

and [<RequireQualifiedAccess; Erase>]
    TextOperation =
    | InsertTextOperation of InsertTextOperation
    | RemoveTextOperation of RemoveTextOperation

and [<RequireQualifiedAccess; Erase>]
    Operation =
    | NodeOperation      of NodeOperation
    | SelectionOperation of SelectionOperation
    | TextOperation      of TextOperation


/// Attributes required by `Element`s for rendering
type RenderElementAttributes =
    {
        ``data-slate-node``: string // 'element'
        ``data-slate-inline``: bool // true
        ``data-slate-void``: bool   // true
        dir: string                 // 'rtl'
        ref: IRefValue<HTMLElement option>
    }

/// Parses a `RenderElementAttributes` into a list of properties
///
/// This must be used when defining a custom element, to ensure it has all required data attributes
/// used by Slate.
let splatElementAttributes (attrs: RenderElementAttributes) =
    [
        prop.ref attrs.ref
        Interop.mkAttr "dir"               attrs.dir
        Interop.mkAttr "data-slate-node"   attrs.``data-slate-node``
        Interop.mkAttr "data-slate-inline" attrs.``data-slate-inline``
        Interop.mkAttr "data-slate-void"   attrs.``data-slate-void``
    ]

(*
 *  `RenderElementProps` are passed to the `renderElement` handler.
 *)
type RenderElementProps =
    {
        children: ReactElement
        element: IElement
        attributes: RenderElementAttributes
    }

type RenderLeafAttributes = { ``data-slate-leaf``: bool }

(*
 *  `RenderLeafProps` are passed to the `renderLeaf` handler.
 *)
type RenderLeafProps =
    {
        children: obj
        leaf: IText
        text: IText
        attributes: RenderLeafAttributes
    }

type IEditable =
    abstract member decorate : (NodeEntry<INode> -> Range[]) option
    abstract member onDOMBeforeInput : (Event -> unit) option
    abstract member placeholder: string option
    abstract member readOnly: bool option
    abstract member role: string option
    abstract member style: IReactProperty list
    abstract member renderElement: (RenderElementProps -> ReactElement) option
    abstract member renderLeaf: (RenderLeafProps -> ReactElement) option

