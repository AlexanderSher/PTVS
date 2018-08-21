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
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.PythonTools.Interpreter;
using ModuleAnalysisAndWhichConstraint = FluentAssertions.AndWhichConstraint<Microsoft.PythonTools.Analysis.FluentAssertions.ModuleAnalysisAssertions, Microsoft.PythonTools.Analysis.FluentAssertions.VariableDefTestInfo>;
using InterpreterScopeAndWhichConstraint = FluentAssertions.AndWhichConstraint<Microsoft.PythonTools.Analysis.FluentAssertions.InterpreterScopeAssertions, Microsoft.PythonTools.Analysis.FluentAssertions.VariableDefTestInfo>;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal static class VariableDefAssertionsExtensions {
        public static ModuleAnalysisAndWhichConstraint OfTypes(this ModuleAnalysisAndWhichConstraint andWhichConstraint, params BuiltinTypeId[] typeIds)
            => andWhichConstraint.OfTypes(typeIds, string.Empty);

        public static ModuleAnalysisAndWhichConstraint OfTypes(this ModuleAnalysisAndWhichConstraint andWhichConstraint, IEnumerable<BuiltinTypeId> typeIds, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveTypes(typeIds, because, reasonArgs);
            return andWhichConstraint;
        }

        public static ModuleAnalysisAndWhichConstraint OfTypes(this ModuleAnalysisAndWhichConstraint andWhichConstraint, params string[] classNames)
            => andWhichConstraint.OfTypes(classNames, string.Empty);

        public static ModuleAnalysisAndWhichConstraint OfTypes(this ModuleAnalysisAndWhichConstraint andWhichConstraint, IEnumerable<string> classNames, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveClassNames(classNames, because, reasonArgs);
            return andWhichConstraint;
        }

        public static ModuleAnalysisAndWhichConstraint WithDescription(this ModuleAnalysisAndWhichConstraint andWhichConstraint, string description, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveDescription(description, because, reasonArgs);
            return andWhichConstraint;
        }

        public static AndWhichConstraint<ModuleAnalysisAssertions, AnalysisValueTestInfo<TValue>> WithValue<TValue>(this ModuleAnalysisAndWhichConstraint andWhichConstraint, string because = "", params object[] reasonArgs) where TValue : AnalysisValue {
            var constraint = andWhichConstraint.Which.Should().HaveValue<TValue>(because, reasonArgs);
            return new AndWhichConstraint<ModuleAnalysisAssertions, AnalysisValueTestInfo<TValue>>(andWhichConstraint.And, constraint.Which);
        }

        public static AndWhichConstraint<ModuleAnalysisAssertions, AnalysisValueTestInfo<TValue>> WithValueOfType<TValue>(this ModuleAnalysisAndWhichConstraint andWhichConstraint, BuiltinTypeId typeId, string because = "", params object[] reasonArgs) where TValue : AnalysisValue {
            var constraint = andWhichConstraint.Which.Should().HaveType(typeId, because, reasonArgs).And.HaveValue<TValue>(because, reasonArgs);
            return new AndWhichConstraint<ModuleAnalysisAssertions, AnalysisValueTestInfo<TValue>>(andWhichConstraint.And, constraint.Which);
        }

        public static AndWhichConstraint<InterpreterScopeAssertions, AnalysisValueTestInfo<TValue>> WithValue<TValue>(this InterpreterScopeAndWhichConstraint andWhichConstraint, string because = "", params object[] reasonArgs) where TValue : AnalysisValue {
            var constraint = andWhichConstraint.Which.Should().HaveValue<TValue>(because, reasonArgs);
            return new AndWhichConstraint<InterpreterScopeAssertions, AnalysisValueTestInfo<TValue>>(andWhichConstraint.And, constraint.Which);
        }

        public static AndWhichConstraint<InterpreterScopeAssertions, AnalysisValueTestInfo<TValue>> WithValueOfType<TValue>(this InterpreterScopeAndWhichConstraint andWhichConstraint, BuiltinTypeId typeId, string because = "", params object[] reasonArgs) where TValue : AnalysisValue {
            var constraint = andWhichConstraint.Which.Should().HaveType(typeId, because, reasonArgs).And.HaveValue<TValue>(because, reasonArgs);
            return new AndWhichConstraint<InterpreterScopeAssertions, AnalysisValueTestInfo<TValue>>(andWhichConstraint.And, constraint.Which);
        }
    }
}