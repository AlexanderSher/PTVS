using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;

namespace Microsoft.PythonTools.Analysis.FluentAssertions {
    [ExcludeFromCodeCoverage]
    internal sealed class AnalysisVariableCollectionAssertions : SelfReferencingCollectionAssertions<IAnalysisVariable, AnalysisVariableCollectionAssertions> {
        public AnalysisVariableCollectionAssertions(IEnumerable<IAnalysisVariable> actualValue) : base(actualValue) { }

        protected override string Identifier => nameof(IAnalysisVariable);

        public AndConstraint<AnalysisVariableCollectionAssertions> HaveReferenceAt(IPythonProjectEntry projectEntry, SourceSpan sourceSpan, VariableType variableType, string because = "", params object[] reasonArgs) {
            var error = FindVariable(projectEntry.DocumentUri, projectEntry.ModuleName, sourceSpan, variableType);
            Execute.Assertion.ForCondition(error == string.Empty)
                .BecauseOf(because, reasonArgs)
                .FailWith(error);

            return new AndConstraint<AnalysisVariableCollectionAssertions>(this);
        }

        public AndConstraint<AnalysisVariableCollectionAssertions> HaveReferenceAt(Uri documentUri, SourceSpan sourceSpan, VariableType variableType, string because = "", params object[] reasonArgs) {
            var error = FindVariable(documentUri, documentUri.AbsolutePath, sourceSpan, variableType);
            Execute.Assertion.ForCondition(error == string.Empty)
                .BecauseOf(because, reasonArgs)
                .FailWith(error);

            return new AndConstraint<AnalysisVariableCollectionAssertions>(this);
        }

        private string FindVariable(Uri documentUri, string moduleName, SourceSpan sourceSpan, VariableType variableType) {
            var candidates = Subject.Where(av => Equals(av.Location.DocumentUri, documentUri)).ToArray();
            if (candidates.Length == 0) {
                return $"Expected to have reference in the module '{moduleName}'{{reason}}, but no references has been found";
            }

            var candidate = candidates.FirstOrDefault(av => av.Location.Span == sourceSpan);
            if (candidate == null) {
                var matchingTypes = candidates.Where(av => av.Type == variableType).ToArray();
                var matchingTypesString = matchingTypes.Length > 0
                    ? $"References that match type '{variableType}' have spans {string.Join(" ,", matchingTypes.Select(av => av.Location.Span.ToString()))}"
                    : $"There are no references with type '{variableType}' either";

                return $"Expected to have reference at {sourceSpan}{{reason}}, but module '{moduleName}' has no references at that source span. {matchingTypesString}";
            }

            return candidate.Type == variableType 
                ? string.Empty 
                : $"Expected to have reference of type '{variableType}'{{reason}}, but reference in module '{moduleName}' at {sourceSpan} has type '{candidate.Type}'";
        }

        private enum FindVariableResult {
            DocumentNotFound,
            LocationDoesNotMatch,
            VariableTypeIsWrong,
            VariableIsFounc
        }
    }
}