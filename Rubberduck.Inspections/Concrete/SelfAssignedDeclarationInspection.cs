﻿using System.Collections.Generic;
using System.Linq;
using Rubberduck.Inspections.Abstract;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Parsing.Inspections.Resources;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;

namespace Rubberduck.Inspections.Concrete
{
    public sealed class SelfAssignedDeclarationInspection : InspectionBase
    {
        public SelfAssignedDeclarationInspection(RubberduckParserState state)
            : base(state, CodeInspectionSeverity.Hint) { }

        public override CodeInspectionType InspectionType => CodeInspectionType.CodeQualityIssues;

        public override IEnumerable<IInspectionResult> GetInspectionResults()
        {
            return UserDeclarations
                .Where(declaration => declaration.IsSelfAssigned 
                    && declaration.IsTypeSpecified
                    && !SymbolList.ValueTypes.Contains(declaration.AsTypeName)
                    && declaration.DeclarationType == DeclarationType.Variable
                    && (declaration.AsTypeDeclaration == null
                        || declaration.AsTypeDeclaration.DeclarationType != DeclarationType.UserDefinedType)
                    && declaration.ParentScopeDeclaration != null
                    && declaration.ParentScopeDeclaration.DeclarationType.HasFlag(DeclarationType.Member))
                .Select(issue => new InspectionResult(this,
                                                      string.Format(InspectionsUI.SelfAssignedDeclarationInspectionResultFormat, issue.IdentifierName),
                                                      issue));
        }
    }
}
