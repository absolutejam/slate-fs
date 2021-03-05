module RealmWeaver.Client.Routing

open Browser.Dom
open Feliz.Router

/// Identifiers for creating an instance of a page based on provided values
[<RequireQualifiedAccess>]
type Page =
    | RealmsList
    | LocationsList
    | NotFound

module Paths =
    let [<Literal>] Realms = "realms"
    let [<Literal>] Locations = "locations"
    let [<Literal>] Factions = "factions"

module Page =
    let NotFound = RealmWeaver.Client.Pages.NotFound.View.NotFound
    let RealmsList = RealmWeaver.Client.Pages.RealmList.View.RealmsList
    let LocationsList = RealmWeaver.Client.Pages.LocationList.View.LocationsList

    /// The `Page` for a non-matching URL
    let fallback = Page.NotFound

    /// Parse URL segments into a `Page`, which can be used to initialise the page's model
    let parseFromUrlSegments = function
        | [ Paths.Realms  ]   -> Page.RealmsList
        | [ Paths.Locations ] -> Page.LocationsList
        | other ->
            console.log $"Fallback: {other}"
            fallback

    /// Create an instance of the page's component from a `Page`
    let initialisePage = function
        | Page.RealmsList    -> RealmsList ()
        | Page.LocationsList -> LocationsList ()
        | Page.NotFound      -> NotFound ()

let routeTo (path: string array) (e: Browser.Types.MouseEvent) =
    e.preventDefault ()
    Router.navigate path