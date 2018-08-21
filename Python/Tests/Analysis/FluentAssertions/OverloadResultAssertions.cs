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

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using static Microsoft.PythonTools.Analysis.FluentAssertions.AssertionsUtilities;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    internal class OverloadResultAssertions : ReferenceTypeAssertions<IOverloadResult, OverloadResultAssertions> {
        public OverloadResultAssertions(IOverloadResult overloadResult) {
            Subject = overloadResult;
        }

        protected override string Identifier => nameof(IOverloadResult);

        public AndConstraint<OverloadResultAssertions> HaveName(string name, string because = "", params object[] reasonArgs) {
            Execute.Assertion.ForCondition(string.Equals(Subject.Name, name, StringComparison.Ordinal))
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected '{Subject.Name}' to have name {name}{{reason}}.");

            return new AndConstraint<OverloadResultAssertions>(this);
        }

        public AndWhichConstraint<OverloadResultAssertions, ParameterResult> HaveParameterAt(int index, string because = "", params object[] reasonArgs) {
            var parameters = Subject.Parameters;
            Execute.Assertion.ForCondition(parameters.Length > index)
                .BecauseOf(because, reasonArgs)
                .FailWith(parameters.Length > 0
                    ? $"Expected '{Subject.Name}' to have parameter at index {index}{{reason}}, but it has only {parameters.Length} parameters."
                    : $"Expected '{Subject.Name}' to have parameter at index {index}{{reason}}, but it has none.");

            return new AndWhichConstraint<OverloadResultAssertions, ParameterResult>(this, Subject.Parameters[index]);
        }

        public AndWhichConstraint<OverloadResultAssertions, ParameterResult> HaveSingleParameter(string because = "", params object[] reasonArgs) {
            var parameters = Subject.Parameters;
            Execute.Assertion.ForCondition(parameters.Length == 1)
                .BecauseOf(because, reasonArgs)
                .FailWith(parameters.Length > 0
                    ? $"Expected '{Subject.Name}' overload to have only one parameter{{reason}}, but it has {parameters.Length} parameters."
                    : $"Expected '{Subject.Name}' overload to have one parameter{{reason}}, but it has none.");

            return new AndWhichConstraint<OverloadResultAssertions, ParameterResult>(this, parameters[0]);
        }

        public AndConstraint<OverloadResultAssertions> HaveParameters(params string[] parameters) => HaveParameters(parameters, string.Empty);

        public AndConstraint<OverloadResultAssertions> HaveParameters(IEnumerable<string> parameters, string because = "", params object[] reasonArgs) {
            var current = Subject.Parameters.Select(pr => pr.Name).ToArray();
            var expected = parameters.ToArray();
            var currentString = string.Join(",", current);
            var expectedString = string.Join(",", expected);

            Execute.Assertion.ForCondition(current.Length == expected.Length)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected '{Subject.Name}' overload to have {expected.Length} parameters ({expectedString}){{reason}}, but it has {current.Length} ({string.Join(",", current)}).");

            for (var i = 0; i < current.Length; i++) {
                Execute.Assertion.ForCondition(string.Equals(current[i], expected[i], StringComparison.Ordinal))
                    .BecauseOf(because, reasonArgs)
                    .FailWith($"Expected '{Subject.Name}' overload to have parameters ({expectedString}){{reason}}, but it has ({currentString}), which is different from expected at {i}.");
            }

            return new AndConstraint<OverloadResultAssertions>(this);
        }

        public AndConstraint<OverloadResultAssertions> HaveSingleReturnType(string type, string because = "", params object[] reasonArgs) {
            var returnTypes = ((IOverloadResult2)Subject).ReturnType;
            Execute.Assertion.ForCondition(returnTypes.Count == 1)
                .BecauseOf(because, reasonArgs)
                .FailWith(returnTypes.Count > 0
                    ? $"Expected '{Subject.Name}' overload to have only one return type{{reason}}, but it has {returnTypes.Count} return types."
                    : $"Expected '{Subject.Name}' overload to have a return type{{reason}}, but it has none.");

            if (returnTypes.Count == 1) {
                Execute.Assertion.ForCondition(string.Equals(returnTypes[0], type, StringComparison.Ordinal))
                    .BecauseOf(because, reasonArgs)
                    .FailWith($"Expected '{Subject.Name}' overload to have return type [{type}]{{reason}}, but it has [{returnTypes[0]}].");
            }

            return new AndConstraint<OverloadResultAssertions>(this);
        }
    }
}