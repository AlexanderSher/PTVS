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
using Microsoft.PythonTools.Analysis.Analyzer;
using Microsoft.PythonTools.Analysis.Values;
using Microsoft.PythonTools.Interpreter;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    internal static class AssertionsUtilities {
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
                default:
                    return string.Empty;
            }
        }
    }
}