module SlateExample.ErrorBoundary

open Fable
open Fable.React
open SlateExample

[<AllowNullLiteral>]
type InfoComponentObject =
    abstract componentStack: string with get


type ErrorBoundaryProps =
    {
        Inner : React.ReactElement
        ErrorComponent : React.ReactElement
        OnError : exn * InfoComponentObject -> unit
    }

type ErrorBoundaryState =
    { HasErrors : bool }

type ErrorBoundary private (props: ErrorBoundaryProps) =
    inherit React.Component<ErrorBoundaryProps, ErrorBoundaryState>(props)
    do base.setInitState { HasErrors = false }

    override this.componentDidCatch (error, info) =
        let info = info |> unbox<InfoComponentObject>
        this.props.OnError (error, info)
        this.setState (fun state props -> { state with HasErrors = true })

    override this.render () =
        if this.state.HasErrors then
            this.props.ErrorComponent
        else
            this.props.Inner

    static member RenderCatchFn (element, errorHandler, errorComponent) =
        React.Helpers.ofType<ErrorBoundary,_,_>
            {
                Inner = element
                ErrorComponent = errorComponent
                OnError = errorHandler
            } [||]
