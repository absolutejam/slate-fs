module SlateExample.ErrorBoundary

open Fable

type [<AllowNullLiteral>] InfoComponentObject =
    abstract componentStack: string with get

type ErrorBoundaryProps =
    {
        Inner : React.ReactElement
        ErrorComponent : React.ReactElement
        OnError : exn * InfoComponentObject -> unit
    }

type ErrorBoundaryState =
    {
        HasErrors : bool
    }

type ErrorBoundary (props) =
    inherit React.Component<ErrorBoundaryProps, ErrorBoundaryState>(props)
    do base.setInitState { HasErrors = false }

    override x.componentDidCatch (error, info) =
        let info = info :?> InfoComponentObject
        x.props.OnError (error, info)
        x.setState (fun state props -> { HasErrors = true })

    override x.render () =
        if (x.state.HasErrors) then
            x.props.ErrorComponent
        else
            x.props.Inner
