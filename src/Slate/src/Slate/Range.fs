module Slate.Range

open Slate.Types
open Slate.Operations
open Fable.Core.JsInterop

module Internal =
    module RangeTypes =
        type EdgesOptions = abstract member reverse: bool option
        type TransformOptions = abstract member affinity: RangeAffinity option

    open RangeTypes

    type Range =
        abstract member edges:        range: IRange * ?options: EdgesOptions -> IPoint[]
        abstract member end':         range: IRange -> IPoint
        abstract member equals:       range: IRange * another: IRange -> bool
        abstract member includes:     range: IRange * target: Location -> bool
        abstract member intersection: range: IRange * another: IRange -> IRange option
        abstract member isBackward:   range: IRange -> bool
        abstract member isCollapsed:  range: IRange -> bool
        abstract member isExpanded:   range: IRange -> bool
        abstract member isForward:    range: IRange -> bool
        abstract member isRange:      value: obj -> bool
        abstract member points:       range: IRange -> PointEntry seq // TODO: Generator<...>
        abstract member start:        range: IRange -> IPoint
        abstract member transform:    range: IRange * op: Operation * ?options: TransformOptions -> IRange option

    let rangeInterface: Range = import "Range" "slate"

type Range =
    static member edges (range: IRange, ?reverse: bool) =
        let options = unbox<Internal.RangeTypes.EdgesOptions> {| reverse = reverse |}
        Internal.rangeInterface.edges (range, options)


