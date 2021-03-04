module SlateExample.Elements

open Feliz
open Fable.Core.JsInterop

open Slate.Types
open Slate.FSharpExtended

type SectionElement =
    {
        children: INode[]
    }
    static member elementType = "section"
    interface IElement with
        member this.elementType = SectionElement.elementType
        member this.children = this.children

type ParagraphElement =
    {
        children:  INode[]
    }
    static member elementType = "paragraph"
    interface IElement with
        member this.elementType = ParagraphElement.elementType
        member this.children = this.children

type TitleElement =
    {
        children:    INode[]
        placeholder: bool
    }
    static member elementType = "title"
    interface IElement with
        member this.elementType = TitleElement.elementType
        member this.children = this.children

module Elements =
    let (|SectionElement|_|) (element: IElement) =
        if (!!element.elementType) = SectionElement.elementType
        then Some (element :?> SectionElement)
        else None

    let (|ParagraphElement|_|) (element: IElement) =
        if (!!element.elementType) = ParagraphElement.elementType
        then Some (element :?> ParagraphElement)
        else None

    let (|TitleElement|_|) (element: IElement) =
        if (!!element.elementType) = TitleElement.elementType
        then Some (element :?> TitleElement)
        else None

    let isSectionElement (element: IElement)   = (|SectionElement|_|) element |> Option.isSome
    let isParagraphElement (element: IElement) = (|ParagraphElement|_|) element |> Option.isSome
    let isTitleElement (element: IElement)     = (|TitleElement|_|) element |> Option.isSome

    let mapSectionElement (mapper: _ -> IElement) (element: IElement) =
        match element with
        | SectionElement sectionElement -> mapper sectionElement :> INode
        | _ -> element :> INode

    let ifSectionElement (mapper: _ -> unit) (element: IElement) =
        match element with
        | SectionElement sectionElement -> mapper sectionElement
        | _ -> ()

    let mapParagraphElement (mapper: _ -> IElement) (element: IElement) =
        match element with
        | ParagraphElement paragraphElement -> mapper paragraphElement :> INode
        | _ -> element :> INode

    let ifParagraphElement (mapper: _ -> unit) (element: IElement) =
        match element with
        | ParagraphElement paragraphElement -> mapper paragraphElement
        | _ -> ()

    let mapTitleElement (mapper: _ -> IElement) (element: IElement) =
        match element with
        | TitleElement titleElement -> mapper titleElement :> INode
        | _ -> element :> INode

    let ifTitleElement (mapper: _ -> unit) (element: IElement) =
        match element with
        | TitleElement titleElement -> mapper titleElement
        | _ -> ()

    let section (children: IElement[]) =
        unbox<IElement>
            {|
                elementType = SectionElement.elementType
                children    = children
            |}

    let title (title: string) =
        unbox<IElement>
            {|
                elementType = TitleElement.elementType
                children    = [| NodeEx.textNode title |]
                placeholder = false
            |}

    let titlePlaceholder (title: string) =
        unbox<IElement>
            {|
                elementType = TitleElement.elementType
                children    = [| NodeEx.textNode title |]
                placeholder = true
            |}

    let paragraph (paragraph: string) =
        unbox<IElement>
            {|
                elementType = ParagraphElement.elementType
                children    = [| NodeEx.textNode paragraph |]
            |}


module ElementComponents =
    [<ReactComponent>]
    let SectionElement (props: RenderElementProps) =
        Html.div [
            yield! splatElementAttributes props.attributes
            prop.classes [ tw.``p-3``; tw.``border``; tw.``border-gray-1`` ]
            prop.children [
                props.children
            ]
        ]

    [<ReactComponent>]
    let TitleElement (props: RenderElementProps) =
        let el = props.element :?> TitleElement
        Html.h2 [
            yield! splatElementAttributes props.attributes
            prop.classes [
                tw.``text-xl``; tw.``font-medium``; tw.``pt-2``; tw.``pb-4``
                if el.placeholder then yield! [ tw.``italic``; tw.``text-gray-2`` ]
            ]
            prop.children [
                props.children
            ]
        ]

    [<ReactComponent>]
    let ParagraphElement (props: RenderElementProps) =
        Html.p [
            yield! splatElementAttributes props.attributes
            prop.children [ props.children ]
        ]
