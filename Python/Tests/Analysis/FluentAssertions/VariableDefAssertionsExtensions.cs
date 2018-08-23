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
namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal static class VariableDefAssertionsExtensions {
        public static AndWhichConstraint<TAssertion, VariableDefTestInfo> OfType<TAssertion>(this AndWhichConstraint<TAssertion, VariableDefTestInfo> andWhichConstraint, BuiltinTypeId typeId, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveType(typeId, because, reasonArgs);
            return andWhichConstraint;
        }

        public static AndWhichConstraint<TAssertion, VariableDefTestInfo> OfTypes<TAssertion>(this AndWhichConstraint<TAssertion, VariableDefTestInfo> andWhichConstraint, params BuiltinTypeId[] typeIds)
            => andWhichConstraint.OfTypes(typeIds, string.Empty);

        public static AndWhichConstraint<TAssertion, VariableDefTestInfo> OfTypes<TAssertion>(this AndWhichConstraint<TAssertion, VariableDefTestInfo> andWhichConstraint, IEnumerable<BuiltinTypeId> typeIds, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveTypes(typeIds, because, reasonArgs);
            return andWhichConstraint;
        }

        public static AndWhichConstraint<TAssertion, VariableDefTestInfo> OfType<TAssertion>(this AndWhichConstraint<TAssertion, VariableDefTestInfo> andWhichConstraint, string className, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveClassName(className, because, reasonArgs);
            return andWhichConstraint;
        }

        public static AndWhichConstraint<TAssertion, VariableDefTestInfo> OfTypes<TAssertion>(this AndWhichConstraint<TAssertion, VariableDefTestInfo> andWhichConstraint, params string[] classNames)
            => andWhichConstraint.OfTypes(classNames, string.Empty);

        public static AndWhichConstraint<TAssertion, VariableDefTestInfo> OfTypes<TAssertion>(this AndWhichConstraint<TAssertion, VariableDefTestInfo> andWhichConstraint, IEnumerable<string> classNames, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveClassNames(classNames, because, reasonArgs);
            return andWhichConstraint;
        }

        public static AndWhichConstraint<TAssertion, VariableDefTestInfo> WithDescription<TAssertion>(this AndWhichConstraint<TAssertion, VariableDefTestInfo> andWhichConstraint, string description, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveDescription(description, because, reasonArgs);
            return andWhichConstraint;
        }

        public static AndWhichConstraint<TAssertion, AnalysisValueTestInfo<TValue>> WithValue<TAssertion, TValue>(this AndWhichConstraint<TAssertion, VariableDefTestInfo> andWhichConstraint, string because = "", params object[] reasonArgs) where TValue : AnalysisValue {
            var constraint = andWhichConstraint.Which.Should().HaveValue<TValue>(because, reasonArgs);
            return new AndWhichConstraint<TAssertion, AnalysisValueTestInfo<TValue>>(andWhichConstraint.And, constraint.Which);
        }

        public static AndWhichConstraint<ModuleAnalysisAssertions, AnalysisValueTestInfo<TValue>> WithValue<TValue>(this AndWhichConstraint<ModuleAnalysisAssertions, VariableDefTestInfo> andWhichConstraint, string because = "", params object[] reasonArgs) where TValue : AnalysisValue 
            => andWhichConstraint.WithValue<ModuleAnalysisAssertions, TValue>(because, reasonArgs);

        public static AndWhichConstraint<InterpreterScopeAssertions, AnalysisValueTestInfo<TValue>> WithValue<TValue>(this AndWhichConstraint<InterpreterScopeAssertions, VariableDefTestInfo> andWhichConstraint, string because = "", params object[] reasonArgs) where TValue : AnalysisValue 
            => andWhichConstraint.WithValue<InterpreterScopeAssertions, TValue>(because, reasonArgs);

        public static AndWhichConstraint<FunctionScopeAssertions, AnalysisValueTestInfo<TValue>> WithValue<TValue>(this AndWhichConstraint<FunctionScopeAssertions, VariableDefTestInfo> andWhichConstraint, string because = "", params object[] reasonArgs) where TValue : AnalysisValue 
            => andWhichConstraint.WithValue<FunctionScopeAssertions, TValue>(because, reasonArgs);
    }
}