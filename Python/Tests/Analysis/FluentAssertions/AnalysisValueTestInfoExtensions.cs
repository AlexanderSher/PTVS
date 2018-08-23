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

using System.Diagnostics.CodeAnalysis;
using Microsoft.PythonTools.Analysis.Values;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal static class AnalysisValueTestInfoExtensions {
        public static BoundBuiltinMethodInfoAssertions Should(this AnalysisValueTestInfo<BoundBuiltinMethodInfo> testInfo)
            => new BoundBuiltinMethodInfoAssertions(testInfo.Value, testInfo.OwnerScope);

        public static BuiltinClassInfoAssertions Should(this AnalysisValueTestInfo<BuiltinClassInfo> testInfo)
            => new BuiltinClassInfoAssertions(testInfo.Value, testInfo.OwnerScope);

        public static BuiltinInstanceInfoAssertions Should(this AnalysisValueTestInfo<BuiltinInstanceInfo> testInfo)
            => new BuiltinInstanceInfoAssertions(testInfo.Value, testInfo.OwnerScope);

        public static BuiltinFunctionInfoAssertions Should(this AnalysisValueTestInfo<BuiltinFunctionInfo> testInfo)
            => new BuiltinFunctionInfoAssertions(testInfo.Value, testInfo.OwnerScope);

        public static BuiltinModuleAssertions Should(this AnalysisValueTestInfo<BuiltinModule> testInfo)
            => new BuiltinModuleAssertions(testInfo.Value, testInfo.OwnerScope);

        public static ClassInfoAssertions Should(this AnalysisValueTestInfo<ClassInfo> testInfo)
            => new ClassInfoAssertions(testInfo.Value, testInfo.OwnerScope);

        public static FunctionInfoAssertions Should(this AnalysisValueTestInfo<FunctionInfo> testInfo)
            => new FunctionInfoAssertions(testInfo.Value, testInfo.OwnerScope);

        public static ParameterInfoAssertions Should(this AnalysisValueTestInfo<ParameterInfo> testInfo)
            => new ParameterInfoAssertions(testInfo.Value, testInfo.OwnerScope);

        public static ProtocolInfoAssertions Should(this AnalysisValueTestInfo<ProtocolInfo> testInfo)
            => new ProtocolInfoAssertions(testInfo.Value, testInfo.OwnerScope);
    }
}