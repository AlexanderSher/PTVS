// Python Tools for Visual Studio
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions.Execution;
using Microsoft.PythonTools.Analysis.Analyzer;
using Microsoft.PythonTools.Analysis.Values;
using Microsoft.PythonTools.Interpreter;
using Microsoft.PythonTools.Parsing;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    internal static class AssertionsUtilities {
        public static bool Is3X(InterpreterScope scope) 
            => ((ModuleScope)scope.GlobalScope).Module.ProjectEntry.ProjectState.LanguageVersion.Is3x();

        public static void AssertTypeIds(IEnumerable<AnalysisValue> actualTypeIds, IEnumerable<BuiltinTypeId> typeIds, string name, bool languageVersionIs3X, string because, object[] reasonArgs)
            => AssertTypeIds(FlattenAnalysisValues(actualTypeIds).Select(av => av.PythonType?.TypeId ?? av.TypeId), typeIds, name, languageVersionIs3X, because, reasonArgs);

        public static void AssertTypeIds(IEnumerable<BuiltinTypeId> actualTypeIds, IEnumerable<BuiltinTypeId> typeIds, string name, bool languageVersionIs3X, string because, object[] reasonArgs) {
            var expectedTypeIds = typeIds.Select(t => {
                switch (t) {
                    case BuiltinTypeId.Str:
                        return languageVersionIs3X ? BuiltinTypeId.Unicode : BuiltinTypeId.Bytes;
                    case BuiltinTypeId.StrIterator:
                        return languageVersionIs3X ? BuiltinTypeId.UnicodeIterator : BuiltinTypeId.BytesIterator;
                    default:
                        return t;
                }
            }).ToArray();

            var actual = actualTypeIds.ToArray();
            var excess = actual.Except(expectedTypeIds);
            var missing = expectedTypeIds.Except(actual)
                .ToArray();

            if (missing.Any()) {
                var actualString = actual.Length > 0 ? string.Join(", ", actual) : "none specified";
                var message = expectedTypeIds.Length > 1
                    ? $"Expected {name} to have types {string.Join(", ", expectedTypeIds)}{{reason}}, but couldn't find {string.Join(", ", missing)}"
                    : $"Expected {name} to have type '{expectedTypeIds[0]}'{{reason}}, but it has {actualString}";

                Execute.Assertion
                    .BecauseOf(because, reasonArgs)
                    .FailWith(message);
            }
        }

        public static string GetQuotedNames(IEnumerable<object> values) {
            return GetQuotedNames(values.Select(GetName));
        }

        public static string GetQuotedNames(IEnumerable<string> names) {
            var sb = new StringBuilder();
            string previousName = null;
            foreach (var name in names) {
                sb.AppendQuotedName(previousName, ", ");
                previousName = name;
            }

            sb.AppendQuotedName(previousName, " and ");
            return sb.ToString();
        }

        private static StringBuilder AppendQuotedName(this StringBuilder stringBuilder, string name, string prefix) {
            if (!string.IsNullOrEmpty(name)) {
                if (stringBuilder.Length > 0) {
                    stringBuilder.Append(prefix);
                }

                stringBuilder
                    .Append("'")
                    .Append(name)
                    .Append("'");
            }

            return stringBuilder;
        }

        public static string GetQuotedName(object value) {
            var name = GetName(value);
            return string.IsNullOrEmpty(name) ? string.Empty : $"'{name}'";
        }

        public static string GetName(object value) {
            switch (value) {
                case IHasQualifiedName qualifiedName:
                    return qualifiedName.FullyQualifiedName;
                case IPythonModule pythonModule:
                    return pythonModule.Name;
                case BuiltinInstanceInfo builtinInstanceInfo:
                    return builtinInstanceInfo.Name ?? $"instance of {builtinInstanceInfo.ClassInfo.FullyQualifiedName}";
                case InterpreterScope interpreterScope:
                    return interpreterScope.Name;
                case AnalysisValue analysisValue:
                    return analysisValue.Name;
                case string str:
                    return str;
                default:
                    return string.Empty;
            }
        }
       
        public static IEnumerable<AnalysisValue> FlattenAnalysisValues(IEnumerable<AnalysisValue> analysisValues) {
            foreach (var analysisValue in analysisValues) {
                if (analysisValue is MultipleMemberInfo mmi) {
                    foreach (var value in FlattenAnalysisValues(mmi.Members)) {
                        yield return value;
                    }
                } else {
                    yield return analysisValue;
                }
            }
        }
    }
}