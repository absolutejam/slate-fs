namespace Build

#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"
#load "./build/utils.fsx"

#if !FAKE
#r "netstandard"
#r "Facades/netstandard" // https://github.com/ionide/ionide-vscode-fsharp/issues/839#issuecomment-396296095
#endif

module Targets =
    open Fake.IO
    open Fake.Core
    open Build.Utils
    open Fake.Core.TargetOperators

    Target.initEnvironment ()

    let clientPath = Path.getFullName "./src/Slate"

    module Launch =
        let client = async {
            let cmd = "fable watch . --sourceMaps --typedArrays false --run webpack-dev-server --outDir ./out"
            Tool.dotNet cmd clientPath
        }

        let tailwind = async {
            let cmd = "tailwind build css/tailwind-source.css  -o css/tailwind-generated.css"
            Tool.run "npx" cmd clientPath
        }

        let browser = async {
            do! Async.Sleep 5000
            Tool.openBrowser "http://localhost:9090"
        }


    //============================================================================//
    // Targets
    //============================================================================//

    Target.create "clean" <| fun _ ->
        Paths.makeCleanablePaths clientPath
        |> Shell.cleanDirs

    Target.create "installClient" <| fun _ ->
        printfn "Node version:"
        Tool.run "node" "--version" __SOURCE_DIRECTORY__

        printfn "Yarn version:"
        Tool.run "yarn" "--version" __SOURCE_DIRECTORY__
        Tool.run "yarn" "install --frozen-lockfile" __SOURCE_DIRECTORY__

    /// Generate CSS artifacts
    Target.create "util.tailwind" <| fun _ ->
        Run.now Launch.tailwind

    /// Run frontend
    Target.create "serve.frontend" <| fun _ ->
        Run.parallelised [ Launch.tailwind; Launch.client ]


    //============================================================================//

    "serve.frontend" ==> "util.tailwind"

    Target.runOrDefaultWithArguments "serve.frontend"
