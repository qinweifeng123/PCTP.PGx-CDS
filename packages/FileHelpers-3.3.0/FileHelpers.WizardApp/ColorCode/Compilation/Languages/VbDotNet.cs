﻿// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;
using ExamplesFx.ColorCode.Common;

namespace ExamplesFx.ColorCode.Compilation.Languages
{
    internal class VbDotNet : ILanguage
    {
        public string Id
        {
            get { return LanguageId.VbDotNet; }
        }

        public string Name
        {
            get { return "VB.NET"; }
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
                        @"('''[^\n]*?)\r?$",
                        new Dictionary<int, string> {
                            {1, ScopeName.Comment},
                        }),
                    new LanguageRule(
                        @"((?:'|REM\s+).*?)\r?$",
                        new Dictionary<int, string> {
                            {1, ScopeName.Comment},
                        }),
                    new LanguageRule(
                        @"""(?:""""|[^""\n])*""",
                        new Dictionary<int, string> {
                            {0, ScopeName.String},
                        }),
                    new LanguageRule(
                        @"(?:\s|^)(\#End\sRegion|\#Region|\#Const|\#End\sExternalSource|\#ExternalSource|\#If|\#Else|\#End\sIf)(?:\s|\(|\r?$)",
                        new Dictionary<int, string> {
                            {1, ScopeName.PreprocessorKeyword},
                        }),
                    new LanguageRule(
                        @"(?i)\b(AddHandler|AddressOf|Aggregate|Alias|All|And|AndAlso|Ansi|Any|As|Ascending|(?<!<)Assembly|Auto|Average|Boolean|By|ByRef|Byte|ByVal|Call|Case|Catch|CBool|CByte|CChar|CDate|CDec|CDbl|Char|CInt|Class|CLng|CObj|Const|Continue|Count|CShort|CSng|CStr|CType|Date|Decimal|Declare|Default|DefaultStyleSheet|Delegate|Descending|Dim|DirectCast|Distinct|Do|Double|Each|Else|ElseIf|End|Enum|Equals|Erase|Error|Event|Exit|Explicit|False|Finally|For|Friend|From|Function|Get|GetType|GoSub|GoTo|Group|Group|Handles|If|Implements|Imports|In|Inherits|Integer|Interface|Into|Is|IsNot|Join|Let|Lib|Like|Long|LongCount|Loop|Max|Me|Min|Mod|Module|MustInherit|MustOverride|My|MyBase|MyClass|Namespace|New|Next|Not|Nothing|NotInheritable|NotOverridable|(?<!\.)Object|Off|On|Option|Optional|Or|Order|OrElse|Overloads|Overridable|Overrides|ParamArray|Partial|Preserve|Private|Property|Protected|Public|RaiseEvent|ReadOnly|ReDim|RemoveHandler|Resume|Return|Select|Set|Shadows|Shared|Short|Single|Skip|Static|Step|Stop|String|Structure|Sub|Sum|SyncLock|Take|Then|Throw|To|True|Try|TypeOf|Unicode|Until|Variant|When|Where|While|With|WithEvents|WriteOnly|Xor|SByte|UInteger|ULong|UShort|Using|CSByte|CUInt|CULng|CUShort)\b",
                        new Dictionary<int, string> {
                            {1, ScopeName.Keyword},
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