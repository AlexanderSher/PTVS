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
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.PythonTools.Analysis.Analyzer;
using Microsoft.PythonTools.Analysis.Values;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal sealed class ClassInfoAssertions : AnalysisValueAssertions<ClassInfo, ClassInfoAssertions> {
        public ClassInfoAssertions(ClassInfo subject, InterpreterScope ownerScope) : base(subject, ownerScope) {}

        protected override string Identifier => nameof(ClassInfo);
        
        public AndWhichConstraint<ClassInfoAssertions, ClassScope> HaveScope(string because = "", params object[] reasonArgs) {
            Execute.Assertion.ForCondition(Subject.Scope != null)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected {Subject.DeclaringModule.ModuleName}.{Subject.Name} to have scope specified{{reason}}.");

            return new AndWhichConstraint<ClassInfoAssertions, ClassScope>(this, Subject.Scope);
        }
    }
}