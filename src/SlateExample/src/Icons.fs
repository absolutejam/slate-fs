module SlateExample.Icons

open Feliz

module Icons =
    let baseIcon (classes: string seq) (viewBox: int * int * int * int) (paths: string seq) =
        Svg.svg [
            svg.classes ([ "fill-current" ] @ List.ofSeq classes)
            svg.viewBox viewBox
            svg.children [ for path in paths -> Svg.path [ svg.d path ]]
        ]

    let iconWrapper (width: int) (height: int) icon =
        Html.div [
            prop.classes [ "flex"; "items-center" ]
            prop.children [
                Html.div [
                    prop.classes [ "flex"; "align-baseline"; "justify-center" ]
                    prop.style [ style.width width; style.height height ]
                    prop.children [ icon ]
                ]
            ]
        ]

    let header (classes: string seq) =
        baseIcon classes (0, 0, 24, 24) [
            "M18 20L18 4 15 4 15 10 9 10 9 4 6 4 6 20 9 20 9 13 15 13 15 20z"
        ]

