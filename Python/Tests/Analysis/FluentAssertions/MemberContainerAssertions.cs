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
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.PythonTools.Analysis.Values;
using Microsoft.PythonTools.Interpreter;
using Microsoft.PythonTools.Interpreter.Ast;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal class MemberContainerAssertions : ReferenceTypeAssertions<IMemberContainer, MemberContainerAssertions> {
        public MemberContainerAssertions(IMemberContainer memberContainer) {
            Subject = memberContainer;
        }

        protected override string Identifier => nameof(IMemberContainer);

        public AndWhichConstraint<MemberContainerAssertions, TMember> HaveMember<TMember>(string name, string because = "", params object[] reasonArgs)
            where TMember : class, IMember  {
            NotBeNull();

            var member = Subject.GetMember(null, name);
            var typedMember = member as TMember;
            Execute.Assertion.ForCondition(member != null)
                .BecauseOf(because, reasonArgs)
                .FailWith($"{{reason}}.")
                .Then
                .ForCondition(typedMember != null)
                .BecauseOf(because, reasonArgs)
                .FailWith($"{{reason}}.");

            return new AndWhichConstraint<MemberContainerAssertions, TMember>(this, typedMember);
        }
    }
}