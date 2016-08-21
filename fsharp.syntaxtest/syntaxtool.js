"use strict";

Object.defineProperty(exports, "__esModule", {
    value: true
});
exports.activate = activate;
exports.deactivate = deactivate;

var _vscode = require("vscode");

var _vscodeTextmate = require("vscode-textmate");

var _fableCore = require("fable-core");

function activate(context) {
    var ochan = _vscode.window.createOutputChannel("Textmate Tokens");

    ochan.show();
    var jsongrammar = context.extensionPath + "\\syntaxes\\fsharp.json";
    ochan.appendLine(context.extensionPath);
    ochan.appendLine(jsongrammar);
    var reg = new _vscodeTextmate.Registry();
    var gram = reg.loadGrammarFromPathSync(jsongrammar);

    (function (arg00) {
        ochan.appendLine(arg00);
    })(_fableCore.Util.toString(gram));

    var tokenizeFile = function tokenizeFile(unitVar0) {
        _vscode.window.showInformationMessage("inside tokenize file");

        var folder = function folder(tupledArg) {
            var stk = tupledArg[0];
            var acc = tupledArg[1];
            return function (line) {
                var res = gram.tokenizeLine(line, stk);

                _fableCore.Seq.iterate(function (t) {
                    ochan.appendLine(_fableCore.String.join("", t.scopes));
                }, res.tokens);

                _fableCore.Array.addRangeInPlace(res.tokens, acc);

                return [res.ruleStack, acc];
            };
        };

        var editor = _vscode.window.activeTextEditor;
        var text = editor.document.getText().split("\n");

        var patternInput = _fableCore.Seq.fold(function ($var1, $var2) {
            return folder($var1)($var2);
        }, [null, []], text);

        var tokens = patternInput[1];

        _fableCore.Seq.iterate(function (t) {
            ochan.appendLine(_fableCore.Util.toString(t));
        }, tokens);
    };

    _vscode.window.setStatusBarMessage("this is a test");

    var disposable = _vscode.commands.registerCommand("syntaxTools.TokenizeFile", tokenizeFile);

    context.subscriptions.push(disposable);
}

function deactivate(disposables) {}
//# sourceMappingURL=syntaxtool.js.map