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
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.PythonTools.Interpreter;

namespace Microsoft.PythonTools.Analysis.FluentAssertions
{
    public sealed class PythonProjectEntryAssertions : ReferenceTypeAssertions<IPythonProjectEntry, PythonProjectEntryAssertions> {
        public PythonProjectEntryAssertions(IPythonProjectEntry projectEntry) {
            Subject = projectEntry;
        }

        protected override string Identifier => nameof(IPythonProjectEntry);

        public AndConstraint<PythonProjectEntryAssertions> HaveVariable(string name, params BuiltinTypeId[] typeIds)
            => HaveVariable(name, typeIds, string.Empty);

        public AndConstraint<PythonProjectEntryAssertions> HaveVariable(string name, IEnumerable<BuiltinTypeId> typeIds, string because = "", params object[] reasonArgs) {
            Subject.Should().NotBeNull();

            Execute.Assertion.ForCondition(Subject.Analysis.Scope.TryGetVariable(name, out var variableDef))
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected module {Subject.ModuleName} to have variable {name} defined{{reason}}.");

            var expectedTypeIds = typeIds.ToArray();
            var missingTypeIds = expectedTypeIds.Except(variableDef.Types.Select(t => t.TypeId));

            if (missingTypeIds.Any()) {
                var message = expectedTypeIds.Length > 1
                    ? $"Expected variable {name} to have types {expectedTypeIds}{{reason}}, but couldn't find {missingTypeIds}"
                    : $"Expected variable {name} to have type {expectedTypeIds[0]}{{reason}}";

                Execute.Assertion
                    .BecauseOf(because, reasonArgs)
                    .FailWith(message);
            }

            return new AndConstraint<PythonProjectEntryAssertions>(this);
        }
    }
}