﻿// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;
using ExamplesFx.ColorCode.Common;

namespace ExamplesFx.ColorCode.Compilation.Languages
{
    internal class Aspx : ILanguage
    {
        public string Id
        {
            get { return LanguageId.Aspx; }
        }

        public string Name
        {
            get { return "ASPX"; }
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
                        @"(?s)(<%)(--.*?--)(%>)",
                        new Dictionary<int, string> {
                            {1, ScopeName.HtmlServerSideScript},
                            {2, ScopeName.HtmlComment},
                            {3, ScopeName.HtmlServerSideScript}
                        }),
                    new LanguageRule(
                        @"(?s)<!--.*?-->",
                        new Dictionary<int, string> {
                            {0, ScopeName.HtmlComment}
                        }),
                    new LanguageRule(
                        @"(?i)(<%)(@)(?:\s+([a-z0-9]+))*(?:\s+([a-z0-9]+)(=)(""[^\n]*?""))*\s*?(%>)",
                        new Dictionary<int, string> {
                            {1, ScopeName.HtmlServerSideScript},
                            {2, ScopeName.HtmlTagDelimiter},
                            {3, ScopeName.HtmlElementName},
                            {4, ScopeName.HtmlAttributeName},
                            {5, ScopeName.HtmlOperator},
                            {6, ScopeName.HtmlAttributeValue},
                            {7, ScopeName.HtmlServerSideScript}
                        }),
                    new LanguageRule(
                        @"(?s)(?:(<%=|<%)(?!=|@|--))(?:.*?)(%>)",
                        new Dictionary<int, string> {
                            {1, ScopeName.HtmlServerSideScript},
                            {2, ScopeName.HtmlServerSideScript}
                        }),
                    new LanguageRule(
                        @"(?is)(<!)(DOCTYPE)(?:\s+([a-z0-9]+))*(?:\s+(""[^""]*?""))*(>)",
                        new Dictionary<int, string> {
                            {1, ScopeName.HtmlTagDelimiter},
                            {2, ScopeName.HtmlElementName},
                            {3, ScopeName.HtmlAttributeName},
                            {4, ScopeName.HtmlAttributeValue},
                            {5, ScopeName.HtmlTagDelimiter}
                        }),
                    new LanguageRule(RuleFormats.JavaScript, RuleCaptures.JavaScript),
                    new LanguageRule(
                        @"(?xi)(</?)
                                         (?: ([a-z][a-z0-9-]*)(:) )*
                                         ([a-z][a-z0-9-_]*)
                                         (?:
                                            [\s\n]+([a-z0-9-_]+)[\s\n]*(=)[\s\n]*([^\s\n""']+?)
                                           |[\s\n]+([a-z0-9-_]+)[\s\n]*(=)[\s\n]*(""[^\n]+?"")
                                           |[\s\n]+([a-z0-9-_]+)[\s\n]*(=)[\s\n]*('[^\n]+?')
                                           |[\s\n]+([a-z0-9-_]+) )*
                                         [\s\n]*
                                         (/?>)",
                        new Dictionary<int, string> {
                            {1, ScopeName.HtmlTagDelimiter},
                            {2, ScopeName.HtmlElementName},
                            {3, ScopeName.HtmlTagDelimiter},
                            {4, ScopeName.HtmlElementName},
                            {5, ScopeName.HtmlAttributeName},
                            {6, ScopeName.HtmlOperator},
                            {7, ScopeName.HtmlAttributeValue},
                            {8, ScopeName.HtmlAttributeName},
                            {9, ScopeName.HtmlOperator},
                            {10, ScopeName.HtmlAttributeValue},
                            {11, ScopeName.HtmlAttributeName},
                            {12, ScopeName.HtmlOperator},
                            {13, ScopeName.HtmlAttributeValue},
                            {14, ScopeName.HtmlAttributeName},
                            {15, ScopeName.HtmlTagDelimiter}
                        }),
                    new LanguageRule(
                        @"(?i)&[a-z0-9]+?;",
                        new Dictionary<int, string> {
                            {0, ScopeName.HtmlEntity}
                        })
                };
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}