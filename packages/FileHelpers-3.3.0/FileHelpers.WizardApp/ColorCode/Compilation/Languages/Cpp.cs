﻿// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;
using ExamplesFx.ColorCode.Common;

namespace ExamplesFx.ColorCode.Compilation.Languages
{
    internal class Cpp : ILanguage
    {
        public string Id
        {
            get { return LanguageId.Cpp; }
        }

        public string Name
        {
            get { return "C++"; }
        }

        public string FirstLinePattern
        {
            get { return null; }
        }

        public IList<LanguageRule> Rules
        {
            get
            {
                return new List<LanguageRule> {
                    new LanguageRule(
                        @"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/",
                        new Dictionary<int, string> {
                            {0, ScopeName.Comment},
                        }),
                    new LanguageRule(
                        @"(//.*?)\r?$",
                        new Dictionary<int, string> {
                            {1, ScopeName.Comment}
                        }),
                    new LanguageRule(
                        @"(?s)(""[^\n]*?(?<!\\)"")",
                        new Dictionary<int, string> {
                            {0, ScopeName.String}
                        }),
                    new LanguageRule(
                        @"\b(abstract|array|auto|bool|break|case|catch|char|ref class|class|const|const_cast|continue|default|delegate|delete|deprecated|dllexport|dllimport|do|double|dynamic_cast|each|else|enum|event|explicit|export|extern|false|float|for|friend|friend_as|gcnew|generic|goto|if|in|initonly|inline|int|interface|literal|long|mutable|naked|namespace|new|noinline|noreturn|nothrow|novtable|nullptr|operator|private|property|protected|public|register|reinterpret_cast|return|safecast|sealed|selectany|short|signed|sizeof|static|static_cast|ref struct|struct|switch|template|this|thread|throw|true|try|typedef|typeid|typename|union|unsigned|using|uuid|value|virtual|void|volatile|wchar_t|while)\b",
                        new Dictionary<int, string> {
                            {0, ScopeName.Keyword},
                        }),
                };
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}