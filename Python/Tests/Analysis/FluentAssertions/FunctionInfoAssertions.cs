﻿// Python Tools for Visual Studio
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

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.PythonTools.Analysis.Analyzer;
using Microsoft.PythonTools.Analysis.Values;
using static Microsoft.PythonTools.Analysis.FluentAssertions.AssertionsUtilities;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal sealed class FunctionInfoAssertions : AnalysisValueAssertions<FunctionInfo, FunctionInfoAssertions> {
        public FunctionInfoAssertions(FunctionInfo subject, InterpreterScope ownerScope) : base(subject, ownerScope) { }

        protected override string Identifier => nameof(FunctionInfo);

        public AndWhichConstraint<FunctionInfoAssertions, FunctionScope> HaveFunctionScope(string because = "", params object[] reasonArgs) {
            var unit = Subject.AnalysisUnit;
            Execute.Assertion.ForCondition(unit != null)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected {GetName()} to have analysis unit specified{{reason}}.");

            var scope = unit.Scope;
            Execute.Assertion.ForCondition(scope != null)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected {GetName()} analysis unit to have scope specified{{reason}}.");

            var typedScope = scope as FunctionScope;
            Execute.Assertion.ForCondition(typedScope != null)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected {GetName()} analysis unit scope to be of type {nameof(FunctionScope)}{{reason}}, but it has type {scope.GetType()}.");

            return new AndWhichConstraint<FunctionInfoAssertions, FunctionScope>(this, typedScope);
        }

        protected override string GetName()
            => $"function {GetQuotedName(Subject)} in a scope {GetQuotedName(OwnerScope)}";
    }
}