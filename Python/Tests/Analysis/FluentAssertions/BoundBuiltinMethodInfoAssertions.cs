using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.PythonTools.Analysis.Values;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal sealed class BoundBuiltinMethodInfoAssertions : ReferenceTypeAssertions<BoundBuiltinMethodInfo, BoundBuiltinMethodInfoAssertions> {
        public BoundBuiltinMethodInfoAssertions(BoundBuiltinMethodInfo subject) {
            Subject = subject;
        }

        protected override string Identifier => nameof(BoundBuiltinMethodInfo);

        public AndConstraint<BoundBuiltinMethodInfoAssertions> HaveOverloads(string because = "", params object[] reasonArgs) {
            Execute.Assertion.ForCondition(Subject.Overloads.Any())
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected {Subject.DeclaringModule.ModuleName}.{Subject.Name} to have overloads{{reason}}.");

            return new AndConstraint<BoundBuiltinMethodInfoAssertions>(this);
        }
        
        public AndWhichConstraint<BoundBuiltinMethodInfoAssertions, OverloadResult> HaveOverloadAt(int index, string because = "", params object[] reasonArgs) {
            var overloads = Subject.Overloads.ToArray();
            var function = Subject.Method.Function;
            Execute.Assertion.ForCondition(overloads.Length > index)
                .BecauseOf(because, reasonArgs)
                .FailWith(overloads.Length > 0 
                    ? $"Expected {function.DeclaringModule.Name}.{function.Name} to have overload at index {index}{{reason}}, but it has only {overloads.Length} overloads."
                    : $"Expected {function.DeclaringModule.Name}.{function.Name} to have overload at index {index}{{reason}}, but it no overloads.");

            return new AndWhichConstraint<BoundBuiltinMethodInfoAssertions, OverloadResult>(this, overloads[index]);
        }
        
        public AndWhichConstraint<BoundBuiltinMethodInfoAssertions, OverloadResult> HaveOverloadWithParametersAt(int index, string because = "", params object[] reasonArgs) {
            var constraint = HaveOverloadAt(index);
            var overload = constraint.Which;
            var function = Subject.Method.Function;

            Execute.Assertion.ForCondition(overload.Parameters.Length > 0)
                .BecauseOf(because, reasonArgs)
                .FailWith($"Expected overload {overload.Name} at index {index} of {function.DeclaringModule.Name}.{function.Name} to have parameters{{reason}}, but it has none.");

            return constraint;
        }
    }
}