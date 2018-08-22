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
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.PythonTools.Analysis.Analyzer;
using Microsoft.PythonTools.Analysis.Values;
using Microsoft.PythonTools.Interpreter;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal sealed class InterpreterScopeAssertions : ReferenceTypeAssertions<InterpreterScope, InterpreterScopeAssertions> {
        public InterpreterScopeAssertions(InterpreterScope interpreterScope) {
            Subject = interpreterScope;
        }

        protected override string Identifier => nameof(InterpreterScope);

        public AndWhichConstraint<InterpreterScopeAssertions, VariableDefTestInfo> HaveVariable(string name, string because = "", params object[] reasonArgs) {
            NotBeNull();

            Execute.Assertion.ForCondition(Subject.TryGetVariable(name, out var variableDef))
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected module '{Subject.Name}' to have variable {name}{{reason}}.");

            return new AndWhichConstraint<InterpreterScopeAssertions, VariableDefTestInfo>(this, new VariableDefTestInfo(variableDef, name, Subject));
        }
        
        public AndConstraint<InterpreterScopeAssertions> HaveClasses(params string[] classNames)
            => HaveClasses(classNames, string.Empty);

        public AndConstraint<InterpreterScopeAssertions> HaveClasses(IEnumerable<string> classNames, string because = "", params object[] reasonArgs) {
            NotBeNull();

            foreach (var className in classNames) {
                HaveVariable(className, because, reasonArgs).Which.Should().HaveMemberType(PythonMemberType.Class, because, reasonArgs);
            }

            return new AndConstraint<InterpreterScopeAssertions>(this);
        }

        public AndConstraint<InterpreterScopeAssertions> HaveFunctions(params string[] functionNames) 
            => HaveFunctions(functionNames, string.Empty);

        public AndConstraint<InterpreterScopeAssertions> HaveFunctions(IEnumerable<string> functionNames, string because = "", params object[] reasonArgs) {
            Subject.Should().NotBeNull();

            foreach (var functionName in functionNames) {
                HaveVariable(functionName, because, reasonArgs).Which.Should().HaveMemberType(PythonMemberType.Function, because, reasonArgs);
            }

            return new AndConstraint<InterpreterScopeAssertions>(this);
        }
    }
}