namespace Fable.Import
open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS

module textmate =

    /// A registry helper that can locate grammar file paths given scope names.
    type [<AllowNullLiteral>] IGrammarLocator =
        abstract getFilePath: scopeName: string -> string
        abstract getInjections: scopeName: string -> ResizeArray<string>


    /// The registry that will hold all grammars.
    and [<AllowNullLiteral>] [<Import("Registry","vscode-textmate")>] Registry(?locator: IGrammarLocator) =
        
        /// Load the grammar for `scopeName` and all referenced included grammars asynchronously.
        member __.loadGrammar(scopeName: string, callback: Func<obj, IGrammar, unit>): unit = failwith "JS only"
        
        /// Load the grammar at `path` synchronously.
        member __.loadGrammarFromPathSync(path: string): IGrammar = failwith "JS only"
        
        /// Get the grammar for `scopeName`. The grammar must first be created via `loadGrammar` or `loadGrammarFromPathSync`.
        member __.grammarForScopeName(scopeName: string): IGrammar = failwith "JS only"

    /// A grammar
    and [<AllowNullLiteral>] IGrammarInfo =
        abstract fileTypes: ResizeArray<string> with get, set
        abstract name: string with get, set
        abstract scopeName: string with get, set
        abstract firstLineMatch: string with get, set

    and [<AllowNullLiteral>] IGrammar =
        /// Tokenize `lineText` using previous line state `prevState`.
        abstract tokenizeLine: lineText: string * prevState: StackElement -> ITokenizeLineResult

    and [<AllowNullLiteral>] ITokenizeLineResult =
        abstract tokens: ResizeArray<IToken> with get, set
        /// The `prevState` to be passed on to the next line tokenization.
        abstract ruleStack: StackElement with get, set

    and [<AllowNullLiteral>] IToken =
        abstract startIndex: float with get, set
        abstract endIndex: float with get, set
        abstract scopes: ResizeArray<string> with get, set

    /// Should not be used by consumers, as its shape might change at any time.
    /// **IMPORTANT** - Immutable!
    and [<AllowNullLiteral>] StackElement =
        abstract _stackElementBrand: unit with get
        abstract equals: other: StackElement -> bool


