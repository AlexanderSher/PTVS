using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.PythonTools.Analysis.Analyzer;
using Microsoft.PythonTools.Analysis.Values;
using Microsoft.PythonTools.Interpreter;
using Microsoft.PythonTools.Interpreter.Ast;
using static Microsoft.PythonTools.Analysis.FluentAssertions.AssertionsUtilities;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    internal class AnalysisValueAssertions<TAnalysisValue> : AnalysisValueAssertions<TAnalysisValue, AnalysisValueAssertions<TAnalysisValue>>
        where TAnalysisValue : AnalysisValue {

        public AnalysisValueAssertions(TAnalysisValue subject, InterpreterScope ownerScope) : base(subject, ownerScope) { }
    }

    internal class AnalysisValueAssertions<TAnalysisValue, TAssertions> : ReferenceTypeAssertions<TAnalysisValue, TAssertions>
        where TAnalysisValue : AnalysisValue
        where TAssertions : AnalysisValueAssertions<TAnalysisValue, TAssertions> {

        protected InterpreterScope OwnerScope { get; }

        public AnalysisValueAssertions(TAnalysisValue subject, InterpreterScope ownerScope) {
            OwnerScope = ownerScope;
            Subject = subject;
        }

        protected override string Identifier => nameof(AnalysisValue);

        public AndConstraint<TAssertions> BeOfMemberType(PythonMemberType memberType, string because = "", params object[] reasonArgs) {
            Execute.Assertion.ForCondition(Subject.MemberType == memberType)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected {Subject.Name} in {OwnerScope.Name} to be {memberType}{{reason}}, but it is {Subject.MemberType}.");

            return new AndConstraint<TAssertions>((TAssertions)this);
        }

        public AndWhichConstraint<TAssertions, IPythonType> BeOfPythonType(IPythonType pythonType, string because = "", params object[] reasonArgs) {
            Execute.Assertion.ForCondition(Subject.PythonType == pythonType)
                .BecauseOf(because, reasonArgs)
                .FailWith(Subject.PythonType != null
                    ? $"Expected {Subject.Name} in {OwnerScope.Name} to be {GetQuotedName(pythonType)}{{reason}}, but it is {GetQuotedName(Subject.PythonType)}."
                    : $"Expected {Subject.Name} in {OwnerScope.Name} to be {GetQuotedName(pythonType)}{{reason}}, but it is null.");

            return new AndWhichConstraint<TAssertions, IPythonType>((TAssertions)this, pythonType);
        }
        
        public AndConstraint<TAssertions> HaveOverloads(string because = "", params object[] reasonArgs) {
            Execute.Assertion.ForCondition(Subject.Overloads.Any())
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected {Subject.DeclaringModule.ModuleName}.{Subject.Name} to have overloads{{reason}}.");

            return new AndConstraint<TAssertions>((TAssertions)this);
        }

        public AndWhichConstraint<TAssertions, OverloadResult> HaveOverloadCount(int count, string because = "", params object[] reasonArgs) {
            var overloads = Subject.Overloads.ToArray();
            Execute.Assertion.ForCondition(overloads.Length == count)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected '{OwnerScope.Name}.{Subject.Name}' to have single overload{{reason}}, but it {GetOverloadsString(overloads.Length)}.");

            return new AndWhichConstraint<TAssertions, OverloadResult>((TAssertions)this, overloads[0]);
        }

        public AndWhichConstraint<TAssertions, OverloadResult> HaveSingleOverload(string because = "", params object[] reasonArgs) {
            var overloads = Subject.Overloads.ToArray();
            Execute.Assertion.ForCondition(overloads.Length == 1)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected '{OwnerScope.Name}.{Subject.Name}' to have single overload{{reason}}, but it {GetOverloadsString(overloads.Length)}.");

            return new AndWhichConstraint<TAssertions, OverloadResult>((TAssertions)this, overloads[0]);
        }

        public AndWhichConstraint<TAssertions, OverloadResult> HaveOverloadAt(int index, string because = "", params object[] reasonArgs) {
            var overloads = Subject.Overloads.ToArray();
            Execute.Assertion.ForCondition(overloads.Length > index)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected '{OwnerScope.Name}.{Subject.Name}' to have overload at index {index}{{reason}}, but it {GetOverloadsString(overloads.Length)}.");

            return new AndWhichConstraint<TAssertions, OverloadResult>((TAssertions)this, overloads[index]);
        }

        private static string GetOverloadsString(int overloadsCount)
            => overloadsCount > 1 
                ? $"has {overloadsCount} overloads" 
                : overloadsCount > 0 
                    ? "has only one overload" 
                    : "has no overloads";

        public AndWhichConstraint<TAssertions, AnalysisValueTestInfo<TMember>> HaveMember<TMember>(string name, string because = "", params object[] reasonArgs)
            where TMember : AnalysisValue
        {
            NotBeNull(because, reasonArgs);

            Execute.Assertion.ForCondition(GetMember(name, out TMember typedMember, out var errorMessage))
                .BecauseOf(because, reasonArgs)
                .FailWith(errorMessage);

            return new AndWhichConstraint<TAssertions, AnalysisValueTestInfo<TMember>>((TAssertions)this, new AnalysisValueTestInfo<TMember>(typedMember, OwnerScope));
        }

        private bool GetMember<TMember>(string name, out TMember typedMember, out string errorMessage) where TMember : AnalysisValue {
            try { 
                var member = Subject.GetMember(null, new AnalysisUnit(null, null, OwnerScope, true), name);
                typedMember = member as TMember;

                errorMessage = member != null 
                    ? typedMember != null 
                        ? null
                        : $"Expected {GetName(Subject)} to have a member {name} of type {typeof(TMember)}{{reason}}, but its type is {member.GetType()}."
                    : $"Expected {GetName(Subject)} to have a member {name} of type {typeof(TMember)}{{reason}}.";
                return typedMember != null;
            } catch (Exception e) {
                errorMessage = $"Expected {GetName(Subject)} to have a member {name} of type {typeof(TMember)}{{reason}}, but {nameof(GetMember)} has failed with exception: {e}.";
                typedMember = null;
                return false;
            }
        }
    }
}