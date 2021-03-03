namespace Build

#r "paket: groupref build //"
#load "../.fake/build.fsx/intellisense.fsx"

open Fake.IO
open Fake.Core
open Fake.DotNet

module Utils =

    module Paths =
        let makeProjectPath path = $"./src/RealmWeaver.{path}" |> Path.getFullName
        let makeCleanablePaths path =
            [ for leaf in [| "obj"; "bin"; "deploy" |] do Path.combine path leaf ]
        let makeDeployPath path = Path.combine path "deploy"

    module Tool =
        /// Helper to run binary
        let findTool tool =
            ProcessUtils.tryFindFileOnPath tool
            |> Option.defaultWith (fun _ ->
                failwith $"{tool} was not found in path. Please install it and make sure it's available from your path.")

        let run cmd args workingDir =
            let cmd' = findTool cmd
            let arguments = args |> String.split ' ' |> Arguments.OfArgs

            Command.RawCommand (cmd', arguments)
            |> CreateProcess.fromCommand
            |> CreateProcess.withWorkingDirectory workingDir
            |> CreateProcess.ensureExitCode
            |> Proc.run
            |> ignore

        let dotNet cmd workingDir =
            let result = DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
            if result.ExitCode <> 0 then
                failwith $"'dotnet {cmd}' failed in {workingDir}"

        let openBrowser url =
            //https://github.com/dotnet/corefx/issues/10361
            Command.ShellCommand url
            |> CreateProcess.fromCommand
            |> CreateProcess.ensureExitCodeWithMessage "opening browser failed"
            |> Proc.run
            |> ignore

        let dockerComposeDown (configFiles: string list) =
            let configFileArgs = List.concat [ for file in configFiles -> ["-f"; file ]]
            let downArgs = (configFileArgs @ [ "down" ]) |> Arguments.OfArgs

            let command =  Command.RawCommand ("docker-compose", downArgs)

            printfn $"Running: ${command}"
            command
            |> CreateProcess.fromCommand
            |> CreateProcess.ensureExitCode
            |> Proc.run
            |> ignore

        let dockerComposeUp (configFiles: string list) =
            let configFileArgs = List.concat [ for file in configFiles -> ["-f"; file ]]
            let upArgs =
                (configFileArgs @ [ "up"; "-d" ])
                |> Arguments.OfArgs

            let followArgs =
                (configFileArgs @ [ "logs"; "-f" ])
                |> Arguments.OfArgs

            let commands = [
                Command.RawCommand ("docker-compose", upArgs)
                Command.RawCommand ("docker-compose", followArgs)
            ]

            for command in commands do
                printfn $"Running: ${command}"
                command
                |> CreateProcess.fromCommand
                |> CreateProcess.ensureExitCode
                |> Proc.run
                |> ignore


        let buildDocker tag =
            let args = $"build -t %s{tag} ."
            run "docker" args __SOURCE_DIRECTORY__


    module Run =
        let now = Async.RunSynchronously

        let parallelised (asyncs: Async<unit> list) =
            asyncs
            |> Async.Parallel
            |> Async.RunSynchronously
            |> ignore

        let sequentially (asyncs: Async<unit> list) =
            for a in asyncs do now a


