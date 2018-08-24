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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PythonTools.Analysis.Analyzer;
using Microsoft.PythonTools.Analysis.Values;
using Microsoft.PythonTools.Interpreter;
using Microsoft.PythonTools.Interpreter.Ast;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal static class AssertionsFactory {
        public static BoundBuiltinMethodInfoAssertions Should(this AnalysisValueTestInfo<BoundBuiltinMethodInfo> testInfo)
            => new BoundBuiltinMethodInfoAssertions(testInfo);

        public static BuiltinClassInfoAssertions Should(this AnalysisValueTestInfo<BuiltinClassInfo> testInfo)
            => new BuiltinClassInfoAssertions(testInfo);

        public static BuiltinInstanceInfoAssertions Should(this AnalysisValueTestInfo<BuiltinInstanceInfo> testInfo)
            => new BuiltinInstanceInfoAssertions(testInfo);

        public static BuiltinFunctionInfoAssertions Should(this AnalysisValueTestInfo<BuiltinFunctionInfo> testInfo)
            => new BuiltinFunctionInfoAssertions(testInfo);

        public static BuiltinModuleAssertions Should(this AnalysisValueTestInfo<BuiltinModule> testInfo)
            => new BuiltinModuleAssertions(testInfo);

        public static ClassInfoAssertions Should(this AnalysisValueTestInfo<ClassInfo> testInfo)
            => new ClassInfoAssertions(testInfo);

        public static DictionaryInfoAssertions Should(this AnalysisValueTestInfo<DictionaryInfo> testInfo)
            => new DictionaryInfoAssertions(testInfo);

        public static FunctionInfoAssertions Should(this AnalysisValueTestInfo<FunctionInfo> testInfo)
            => new FunctionInfoAssertions(testInfo);

        public static InstanceInfoAssertions Should(this AnalysisValueTestInfo<InstanceInfo> testInfo)
            => new InstanceInfoAssertions(testInfo);

        public static ParameterInfoAssertions Should(this AnalysisValueTestInfo<ParameterInfo> testInfo)
            => new ParameterInfoAssertions(testInfo);

        public static ProtocolInfoAssertions Should(this AnalysisValueTestInfo<ProtocolInfo> testInfo)
            => new ProtocolInfoAssertions(testInfo);

        public static SequenceInfoAssertions Should(this AnalysisValueTestInfo<SequenceInfo> testInfo)
            => new SequenceInfoAssertions(testInfo);

        public static AnalysisVariableCollectionAssertions Should(this IEnumerable<IAnalysisVariable> analysisVariables)
            => new AnalysisVariableCollectionAssertions(analysisVariables);

        public static AstPythonFunctionAssertions Should(this AstPythonFunction pythonFunction)
            => new AstPythonFunctionAssertions(pythonFunction);

        public static FunctionScopeAssertions Should(this FunctionScope functionScope)
            => new FunctionScopeAssertions(functionScope);

        public static InterpreterScopeAssertions Should(this InterpreterScope interpreterScope)
            => new InterpreterScopeAssertions(interpreterScope);

        public static MemberContainerAssertions<IMemberContainer> Should(this IMemberContainer memberContainer)
            => new MemberContainerAssertions<IMemberContainer>(memberContainer);

        public static ParameterResultAssertions Should(this ParameterResult overloadResult)
            => new ParameterResultAssertions(overloadResult);
    }
}