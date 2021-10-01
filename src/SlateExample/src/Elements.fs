module SlateExample.Elements

open Feliz
open Slatex
open Slate.Types
open Fable.Core.JsInterop

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
        children:        INode[]
        isPlaceholder:   bool
        placeholderText: string option
    }
    static member elementType = "title"
    interface IElement with
        member this.elementType = TitleElement.elementType
        member this.children = this.children

module Elements =
    let (|SectionElement|_|) (node: INode) =
        Nodex.(|Element|_|) node |> Option.bind (fun element ->
            if (!!element.elementType) = SectionElement.elementType
            then Some (element :?> SectionElement)
            else None
        )

    let (|ParagraphElement|_|) (node: INode) =
        Nodex.(|Element|_|) node |> Option.bind (fun element ->
            if (!!element.elementType) = ParagraphElement.elementType
            then Some (element :?> ParagraphElement)
            else None
        )

    let (|TitleElement|_|) (node: INode) =
        Nodex.(|Element|_|) node |> Option.bind (fun element ->
            if (!!element.elementType) = TitleElement.elementType
            then Some (element :?> TitleElement)
            else None
        )

    let isSectionElement (node: INode)   = (|SectionElement|_|) node |> Option.isSome
    let isParagraphElement (node: INode) = (|ParagraphElement|_|) node |> Option.isSome
    let isTitleElement (node: INode)     = (|TitleElement|_|) node |> Option.isSome

    let mapSectionElement (mapper: _ -> IElement) (node: IElement) =
        match node with
        | SectionElement sectionElement -> mapper sectionElement
        | _ -> node

    let ifSectionElement (mapper: _ -> unit) (element: INode) =
        match element with
        | SectionElement sectionElement -> mapper sectionElement
        | _ -> ()

    let mapParagraphElement (mapper: _ -> IElement) (element: IElement) =
        match element with
        | ParagraphElement paragraphElement -> mapper paragraphElement
        | _ -> element

    let ifParagraphElement (mapper: _ -> unit) (node: INode) =
        match node with
        | ParagraphElement paragraphElement -> mapper paragraphElement
        | _ -> ()

    let mapTitleElement (mapper: _ -> IElement) (node: IElement) =
        match node with
        | TitleElement titleElement -> mapper titleElement
        | _ -> node

    let ifTitleElement (mapper: _ -> unit) (element: INode) =
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
                elementType     = TitleElement.elementType
                children        = [| Nodex.textNode title |]
                isPlaceholder   = false
                placeholderText = None
            |}

    let titleWithPlaceholder (title: string) (placeholder: string) =
        unbox<IElement>
            {|
                elementType     = TitleElement.elementType
                children        = [| Nodex.textNode title |]
                isPlaceholder   = false
                placeholderText = Some placeholder
            |}

    let paragraph (paragraph: string) =
        unbox<IElement>
            {|
                elementType = ParagraphElement.elementType
                children    = [| Nodex.textNode paragraph |]
            |}


module ElementComponents =
    [<ReactComponent>]
    let SectionElement (props: RenderElementProps) =
        Html.div [
            yield! splatElementAttributes props.attributes
            prop.classes [ "p-3"; "border"; "border-gray-1" ]
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
                "text-xl"; "font-medium"; "pt-2"; "pb-4"
                if el.isPlaceholder then yield! [ "italic"; "text-gray-2" ]
            ]
            prop.children [ props.children ]
        ]

    [<ReactComponent>]
    let ParagraphElement (props: RenderElementProps) =
        Html.p [
            yield! splatElementAttributes props.attributes
            prop.children [ props.children ]
        ]
