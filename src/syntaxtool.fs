namespace Ionide
open Fable.Import
open Fable.Import.vscode
open Fable.Import.textmate
open System
open Fable.Core
open Fable.Core.JsInterop
open Ionide.VSCode.Helpers


module SyntaxTools =
        
    let activate (context:vscode.ExtensionContext) =   
        let ochan = window.createOutputChannel "Textmate Tokens"
        ochan.show()
        
        //let syntaxbin = VSCode.getPluginPath "Ionide.FSharp.SyntaxTool"
        let jsongrammar = context.extensionPath + @"\syntaxes\fsharp.json"
        // ochan.appendLine syntaxbin
        ochan.appendLine context.extensionPath
        ochan.appendLine jsongrammar
        
        let reg = Registry()
        
        let gram = reg.loadGrammarFromPathSync jsongrammar
     
        ochan.appendLine <| gram.ToString()
        
        let tokenizeFile () =
            vscode.window.showInformationMessage "inside tokenize file" |> ignore

            let folder (stk:StackElement,acc:ResizeArray<IToken>) (line:string) =
                let res = gram.tokenizeLine(line,stk)
                res.tokens|>Seq.iter(fun t -> ochan.appendLine(String.Concat(t.scopes) ))
                acc.AddRange res.tokens
                (res.ruleStack, acc)
                
            let editor = vscode.window.activeTextEditor
            let text = (editor.document.getText()).Split[|'\n';|]
        
            let _,tokens = Array.fold folder (null,ResizeArray()) text
            tokens |> Seq.iter(fun t -> ochan.appendLine (t.ToString()))     
            ()
     
        vscode.window.setStatusBarMessage "this is a test" |> ignore
        
        let disposable = 
            commands.registerCommand ("syntaxTools.TokenizeFile", 
                tokenizeFile |> unbox) |> ignore
        
        context.subscriptions.Add disposable
        // Promise.create (fun resolve reject ->
            
        // |> Promise.onSuccess (fun () -> 
        //     ()
        // )
        // |> Promise.onFail (fun error ->
        //     vscode.window.showErrorMessage(error.ToString()) |> ignore
        //     ()
        // ) // prevent unhandled rejected promises
        // |> ignore
        ()
        
    let deactivate (disposables: Disposable[]) = ()