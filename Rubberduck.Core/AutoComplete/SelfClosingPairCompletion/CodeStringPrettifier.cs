﻿using System;
using System.Linq;
using Rubberduck.Common;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using Rubberduck.VBEditor;

namespace Rubberduck.AutoComplete.SelfClosingPairCompletion
{
    public class CodeStringPrettifier : ICodeStringPrettifier
    {
        public CodeString Prettify(ICodeModule module, CodeString original)
        {
            var originalCode = original.Code;
            var originalPosition = original.CaretPosition.StartColumn;
            var originalNonWhitespaceCharacters = 0;
            for (var i = 0; i < originalPosition; i++)
            {
                if (originalCode[i] != ' ')
                {
                    originalNonWhitespaceCharacters++;
                }
            }

            var indent = originalCode.TakeWhile(c => c == ' ').Count();

            module.DeleteLines(original.SnippetPosition.StartLine, original.SnippetPosition.LineCount);
            module.InsertLines(original.SnippetPosition.StartLine, originalCode);
            var prettifiedCode = module.GetLines(original.SnippetPosition);

            var prettifiedNonWhitespaceCharacters = 0;
            var prettifiedCaretCharIndex = 0;
            for (var i = 0; i < prettifiedCode.Length; i++)
            {
                if (prettifiedCode[i] != ' ')
                {
                    prettifiedNonWhitespaceCharacters++;
                    if (prettifiedNonWhitespaceCharacters == originalNonWhitespaceCharacters)
                    {
                        prettifiedCaretCharIndex = i;
                        break;
                    }
                }
            }

            var prettifiedPosition = new Selection(original.SnippetPosition.StartLine - 1, originalCode.Trim().Length == 0 ? indent : prettifiedCaretCharIndex + 1).ToOneBased();
            using (var pane = module.CodePane)
            {
                pane.Selection = prettifiedPosition;
            }

            var result = new CodeString(prettifiedCode, new Selection(0, prettifiedPosition.StartColumn - 1), prettifiedPosition);
            return result;
        }
    }
}
