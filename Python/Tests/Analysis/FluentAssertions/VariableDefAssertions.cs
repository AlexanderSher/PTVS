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
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.PythonTools.Analysis.Values;
using Microsoft.PythonTools.Interpreter;
using Microsoft.PythonTools.Parsing;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    internal sealed class VariableDefTestInfo {
        private readonly VariableDef _variableDef;
        private readonly string _moduleName;
        private readonly string _name;
        private readonly PythonLanguageVersion _languageVersion;

        public VariableDefTestInfo(VariableDef variableDef, string moduleName, string name, PythonLanguageVersion languageVersion) {
            _variableDef = variableDef;
            _moduleName = moduleName;
            _name = name;
            _languageVersion = languageVersion;
        }

        public VariableDefAssertions Should() => new VariableDefAssertions(_variableDef, _moduleName, _name, _languageVersion);

        public static implicit operator VariableDef(VariableDefTestInfo ti) => ti._variableDef;
    }

    internal sealed class VariableDefAssertions : ReferenceTypeAssertions<VariableDef, VariableDefAssertions> {
        private readonly string _moduleName;
        private readonly string _name;
        private readonly PythonLanguageVersion _languageVersion;

        public VariableDefAssertions(VariableDef variableDef, string moduleName, string name, PythonLanguageVersion languageVersion) {
            Subject = variableDef;
            _moduleName = moduleName;
            _name = name;
            _languageVersion = languageVersion;
        }

        protected override string Identifier => nameof(VariableDef);

        public AndConstraint<VariableDefAssertions> HaveType(BuiltinTypeId typeId, string because = "", params object[] reasonArgs)
            => HaveTypes(new[]{ typeId }, because, reasonArgs);

        public AndConstraint<VariableDefAssertions> HaveTypes(params BuiltinTypeId[] typeIds)
            => HaveTypes(typeIds, string.Empty);

        public AndConstraint<VariableDefAssertions> HaveTypes(IEnumerable<BuiltinTypeId> typeIds, string because = "", params object[] reasonArgs) {
            var languageVersionIs3X = _languageVersion.Is3x();
            var expectedTypeIds = typeIds.Select(t => {
                switch (t) {
                    case BuiltinTypeId.Str:
                        return languageVersionIs3X ? BuiltinTypeId.Unicode : BuiltinTypeId.Bytes;
                    case BuiltinTypeId.StrIterator:
                        return languageVersionIs3X ? BuiltinTypeId.UnicodeIterator : BuiltinTypeId.BytesIterator;
                    default:
                        return t;
                }
            }).ToArray();

            var missingTypeIds = expectedTypeIds.Except(Flatten(Subject.Types).Select(av => av.PythonType?.TypeId ?? av.TypeId)).ToArray();

            if (missingTypeIds.Any()) {
                var message = expectedTypeIds.Length > 1
                    ? $"Expected {_moduleName}.{_name} to have types {string.Join(", ", expectedTypeIds)}{{reason}}, but couldn't find {string.Join(", ", missingTypeIds)}"
                    : $"Expected {_moduleName}.{_name} to have type {expectedTypeIds[0]}{{reason}}";

                Execute.Assertion
                    .BecauseOf(because, reasonArgs)
                    .FailWith(message);
            }

            return new AndConstraint<VariableDefAssertions>(this);
        }

        public AndConstraint<VariableDefAssertions> HaveClassNames(params string[] classNames)
            => HaveClassNames(classNames, string.Empty);

        public AndConstraint<VariableDefAssertions> HaveClassNames(IEnumerable<string> classNames, string because = "", params object[] reasonArgs) {
            var expectedClassNames = classNames.ToArray();
            var missingClassNames = expectedClassNames.Except(Flatten(Subject.Types).Select(av => av.ShortDescription)).ToArray();

            if (missingClassNames.Any()) {
                var message = expectedClassNames.Length > 1
                    ? $"Expected variable {_name} to have types {string.Join(", ", expectedClassNames)}{{reason}}, but couldn't find {string.Join(", ", missingClassNames)}"
                    : $"Expected variable {_name} to have type {expectedClassNames[0]}{{reason}}";

                Execute.Assertion
                    .BecauseOf(because, reasonArgs)
                    .FailWith(message);
            }

            return new AndConstraint<VariableDefAssertions>(this);
        }

        public AndConstraint<VariableDefAssertions> HaveMemberType(PythonMemberType memberType, string because = "", params object[] reasonArgs) {
            Execute.Assertion.ForCondition(Subject.Types is AnalysisValue av && av.MemberType == memberType)
                .BecauseOf(because, reasonArgs)
                .FailWith(Subject.Types != null
                    ? $"Expected {_moduleName}.{_name} to be {memberType}, but it is {((AnalysisValue) Subject.Types).MemberType} {{reason}}."
                    : $"Expected {_moduleName}.{_name} to be {memberType} {{reason}}.");

            return new AndConstraint<VariableDefAssertions>(this);
        }

        public AndConstraint<VariableDefAssertions> HaveDescription(string description, string because = "", params object[] reasonArgs) {
            var values = Flatten(Subject.Types).ToArray();
            var value = AssertSingle(because, reasonArgs, values);

            var actualDescription = values[0].Description;
            var actualShortDescription = values[0].ShortDescription;
            Execute.Assertion.ForCondition(description == actualDescription || description != actualShortDescription)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected description of {_moduleName}.{_name} to have description {description}{{reason}}, but found {actualDescription} or {actualShortDescription}.");

            return new AndConstraint<VariableDefAssertions>(this);
        }

        public AndWhichConstraint<VariableDefAssertions, TValue> HaveValue<TValue>(string because = "", params object[] reasonArgs) where TValue : AnalysisValue {
            var values = Flatten(Subject.Types).ToArray();
            var value = AssertSingle(because, reasonArgs, values);

            var typedValue = value as TValue;
            Execute.Assertion.ForCondition(typedValue != null)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected {_moduleName}.{_name} to have value of type {typeof(TValue)}{{reason}}, but its value has type {value.GetType()}.");

            return new AndWhichConstraint<VariableDefAssertions, TValue>(this, typedValue);
        }
        
        private AnalysisValue AssertSingle(string because, object[] reasonArgs, AnalysisValue[] values) {
            Execute.Assertion.ForCondition(values.Length == 1)
                .BecauseOf(because, reasonArgs)
                .FailWith(values.Length == 0 
                    ? $"Expected module {_moduleName} to have single value{{reason}}, but found none."
                    : $"Expected module {_moduleName} to have single value{{reason}}, but found multiple: {string.Join(", ", values.Select(v => v.Name))}");

            return values[0];
        }

        private static IEnumerable<AnalysisValue> Flatten(IEnumerable<AnalysisValue> analysisValues) {
            foreach (var analysisValue in analysisValues) {
                if (analysisValue is MultipleMemberInfo mmi) {
                    foreach (var value in Flatten(mmi.Members)) {
                        yield return value;
                    }
                } else {
                    yield return analysisValue;
                }
            }
        }
    }
}