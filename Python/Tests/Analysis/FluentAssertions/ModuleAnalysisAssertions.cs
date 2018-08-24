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
using FluentAssertions.Primitives;
using Microsoft.PythonTools.Analysis.Analyzer;
using Microsoft.PythonTools.Interpreter;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal sealed class ModuleAnalysisAssertions : ReferenceTypeAssertions<ModuleAnalysis, ModuleAnalysisAssertions> {
        private readonly InterpreterScopeAssertions _interpreterScopeAssertions;

        public ModuleAnalysisAssertions(ModuleAnalysis moduleAnalysis) {
            Subject = moduleAnalysis;
            _interpreterScopeAssertions = new InterpreterScopeAssertions(Subject.Scope);
        }

        protected override string Identifier => nameof(ModuleAnalysis);
        
        public AndWhichConstraint<ModuleAnalysisAssertions, IPythonModule> HavePythonModuleVariable(string name, string because = "", params object[] reasonArgs) {
            NotBeNull(because, reasonArgs);
            var constraint = _interpreterScopeAssertions.HavePythonModuleVariable(name, because, reasonArgs);
            return new AndWhichConstraint<ModuleAnalysisAssertions, IPythonModule>(this, constraint.Which);
        }

        public AndWhichConstraint<ModuleAnalysisAssertions, ClassScope> HaveClassInfoVariable(string name, string because = "", params object[] reasonArgs) {
            NotBeNull(because, reasonArgs);
            var constraint = _interpreterScopeAssertions.HaveClassInfoVariable(name, because, reasonArgs);
            return new AndWhichConstraint<ModuleAnalysisAssertions, ClassScope>(this, constraint.Which);
        }

        public AndWhichConstraint<ModuleAnalysisAssertions, FunctionScope> HaveFunctionInfoVariable(string name, string because = "", params object[] reasonArgs) {
            NotBeNull(because, reasonArgs);
            var constraint = _interpreterScopeAssertions.HaveFunctionInfoVariable(name, because, reasonArgs);
            return new AndWhichConstraint<ModuleAnalysisAssertions, FunctionScope>(this, constraint.Which);
        }

        public AndWhichConstraint<ModuleAnalysisAssertions, VariableDefTestInfo> HaveVariable(string name, string because = "", params object[] reasonArgs) {
            NotBeNull(because, reasonArgs);
            var constraint = _interpreterScopeAssertions.HaveVariable(name, because, reasonArgs);
            return new AndWhichConstraint<ModuleAnalysisAssertions, VariableDefTestInfo>(this, constraint.Which);
        }

        public AndConstraint<ModuleAnalysisAssertions> HaveClasses(params string[] classNames)
            => HaveClasses(classNames, string.Empty);

        public AndConstraint<ModuleAnalysisAssertions> HaveClasses(IEnumerable<string> classNames, string because = "", params object[] reasonArgs) {
            NotBeNull(because, reasonArgs);
            _interpreterScopeAssertions.HaveClasses(classNames, because, reasonArgs);
            return new AndConstraint<ModuleAnalysisAssertions>(this);
        }

        public AndConstraint<ModuleAnalysisAssertions> HaveFunctions(params string[] functionNames) 
            => HaveFunctions(functionNames, string.Empty);

        public AndConstraint<ModuleAnalysisAssertions> HaveFunctions(IEnumerable<string> functionNames, string because = "", params object[] reasonArgs) {
            NotBeNull(because, reasonArgs);
            _interpreterScopeAssertions.HaveFunctions(functionNames, because, reasonArgs);
            return new AndConstraint<ModuleAnalysisAssertions>(this);
        }

        public AndConstraint<ModuleAnalysisAssertions> NotHaveVariable(string name, string because = "", params object[] reasonArgs) {
            NotBeNull(because, reasonArgs);
            _interpreterScopeAssertions.NotHaveVariable(name, because, reasonArgs);
            return new AndConstraint<ModuleAnalysisAssertions>(this);
        }
    }
}