# Slate

Fable bindings for [Slatejs](https://www.slatejs.org/)

## Dev Setup

```bash
dotnet tool restore
dotnet restore
dotnet paket install
```

Restore NPM packages (Client):

```bash
cd src/Slate
yarn
cd ../..
```

To serve the example project:

```bash
./fake serve.frontend
```

## Documentation

### Element

You should define some types that implement `IElement` so you can deal with strongly-typed
elements in your codebase. This does involve a little type-juggling, but it's worth it in the
long run.

For example,

```f#
type CodeElement =
    {
        language: string
        children: INode[]
    }
    interface IElement with
        member this.children = this.children
        member this.elementType = "code"
```

And it is also advised to define a couple of helpers.

### Elements

The `IElement` interface provides the required `children` property but also an `elementType`
property. This was implemented to ensure there was a more reliable, standardised way to
test element types instead of testing for the presence of fields.

The easiest way to test for a specific element type is to test the `elementType` property
and if it matches, cast the `IElement` to your element type

```f#
let (|MyElement|_|) (element: IElement) =
    if element.elementType = "default"
    then Some (element :?> MyElement)
    else None
```

**NOTE:** You can still optionally test the structure of the object (eg. existence of fields) but
this requires dropping down to using js interop, such as `Emit` or using tools provided by
the `Fable.Extras` package