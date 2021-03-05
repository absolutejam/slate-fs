# RealmWeaver.Client

## Plugin WIP

```
Root
\_ Core
    \_ ...
\_ Plugin
    \_ Plugin1
    \_ Plugin2
```

### Model

The main app model will be something like...

```f#
    type AppModel =
         { CurrentPage: PageModel }

    type PageModel =
    | PageModel of App.Types.PageModel
    | PluginPageModel of Plugin.PageModel
```

Where Plugin.PageModel is generated...

```f#
    type PageModel =
    | DowntimeActivitiesPageModel of DowntimeActivitiesPlugin.PluginPageModels
    | FooPageModel of FooPlugin.PluginPageModels
    | ...
```

### View

Wrap the `Root.renderPage` function...

```f#
    let renderPage (currentPageModel: PageModel) (dispatch: Msg -> unit) =
        match currentPageModel with
        | PageModel pageModel   -> App.View.render pageModel (CoreMsg >> dispatch)
        | PluginPageModel pageModel -> Plugin.render pageModel (PluginMsg >> dispatch)
```

And `Plugin.renderPage` is auto-generated...

```f#
    // module Plugin
    let render (model: Model) (dispatch: Msg -> unit) =
        match model with
        | DowntimeActivitiesPageModel model -> DowntimeActivitiesPlugin.render model (DowntimeActivitiesMsg >> dispatch)
        | FooPageModel model -> FooPlugin.render model (FooMsg >> dispatch)
```

and finally...

```f#
    // module DowntimeActivitiesPlugin
    let render (model: Model) (dispatch: Msg -> unit) =
        match model with
        | TaxesManagement m -> Pages.Taxes.View.render m (TaxesMsg >> dispatch)
        | BanditAttacksSummary m -> Pages.BanditAttacks.View.Render m (BanditAttacks >> dispatch)
        | ...
```

### Msg

The main app Msg will be...

```f#
    type AppMsg =
    | CoreMsg of App.Types.Msg
    | PluginMsg of Plugin.Msg
```

where Plugin.Msg is generated from all plugins...

```f#
    type Msg =
    | DowntimeActivitiesMsg of DowntimeActivitiesPlugin.Msg
    | FooMsg of FooPlugin.Msg
    | ...
```

### Update

Wrap the app update...

```f#
    match appMsg with
    | CoreMsg msg -> App.update msg
    | PluginMsg msg -> Plugin.update
```

where Plugin.update is generated...

```f#
    match pluginMsg with
    | DowtimeActivitiesMsg msg -> DowntimeActivitiesPlugin.spec.Update
    | FooMsg msg -> FooPlugin.spec.Update
    | ...
```