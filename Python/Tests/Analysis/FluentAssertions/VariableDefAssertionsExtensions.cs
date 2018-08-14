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
using Microsoft.PythonTools.Interpreter;
using AndWhichConstraint = FluentAssertions.AndWhichConstraint<Microsoft.PythonTools.Analysis.FluentAssertions.ModuleAnalysisAssertions, Microsoft.PythonTools.Analysis.FluentAssertions.VariableDefTestInfo>;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal static class VariableDefAssertionsExtensions {
        public static AndWhichConstraint OfTypes(this AndWhichConstraint andWhichConstraint, params BuiltinTypeId[] typeIds)
            => andWhichConstraint.OfTypes(typeIds, string.Empty);

        public static AndWhichConstraint OfTypes(this AndWhichConstraint andWhichConstraint, IEnumerable<BuiltinTypeId> typeIds, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveTypes(typeIds, because, reasonArgs);
            return andWhichConstraint;
        }

        public static AndWhichConstraint OfTypes(this AndWhichConstraint andWhichConstraint, params string[] classNames)
            => andWhichConstraint.OfTypes(classNames, string.Empty);

        public static AndWhichConstraint OfTypes(this AndWhichConstraint andWhichConstraint, IEnumerable<string> classNames, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveClassNames(classNames, because, reasonArgs);
            return andWhichConstraint;
        }

        public static AndWhichConstraint WithDescription(this AndWhichConstraint andWhichConstraint, string description, string because = "", params object[] reasonArgs) {
            andWhichConstraint.Which.Should().HaveDescription(description, because, reasonArgs);
            return andWhichConstraint;
        }

        public static AndWhichConstraint WithValueOfType<TValue>(this AndWhichConstraint andWhichConstraint, BuiltinTypeId typeId, string because = "", params object[] reasonArgs) where TValue : AnalysisValue {
            andWhichConstraint.Which.Should().HaveType(typeId).And.HaveValue<TValue>(because, reasonArgs);
            return andWhichConstraint;
        }
    }
}