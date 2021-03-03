module Slate.Element

open Slate.Types
open Fable.Core.JsInterop

module Internal =
    type Element =
        abstract member isAncestor:     value: obj -> bool
        abstract member isElement:      value: obj -> bool
        abstract member isElementList:  value: obj -> bool
        abstract member isElementProps: value: obj -> bool
        abstract member matches:        element: IElement * props: obj -> bool

    let elementInterface : Element = import "Element" "slate"

type Element =
    static member isAncestor (value: obj) = Internal.elementInterface.isAncestor value
    static member isElement (value: obj) = Internal.elementInterface.isElement value
    static member isElementList (value: obj) = Internal.elementInterface.isElementList value
    static member isElementProps (value: obj) = Internal.elementInterface.isElementProps value
    static member matches (element: IElement, props: obj) = Internal.elementInterface.matches (element, props)
