module Slate.Text

open Slate.Types
open Fable.Core.JsInterop

module Internal =
    module TextTypes =
        type EqualsOptions = abstract member loose: bool option

    open TextTypes

    type Text =
        abstract member equals:      text: IText * another: IText * ?options: EqualsOptions -> bool
        abstract member isText:      value: obj -> bool
        abstract member isTextList:  value: obj -> bool
        abstract member isTextProps: props: obj -> bool
        abstract member matches:     text: IText * decorations: IRange[] -> IText[]

    let textInterface : Text = import "Text" "slate"

type Text =
    static member equals (text: IText, another: IText) = Internal.textInterface.equals (text, another)
    static member isText (value: obj) = Internal.textInterface.isText value
    static member isTextList (value: obj) = Internal.textInterface.isTextList value
    static member isTextProps (props: obj) = Internal.textInterface.isTextProps props
    static member matches (text: IText, decorations: IRange[]) = Internal.textInterface.matches (text, decorations)

