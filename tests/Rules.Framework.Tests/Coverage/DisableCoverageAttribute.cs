using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace Rules.Framework.Tests.Coverage
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Assembly)]
    [MulticastAttributeUsage(MulticastTargets.Class | MulticastTargets.Struct)]
    [ProvideAspectRole(StandardRoles.PerformanceInstrumentation)]
    [ExcludeFromCodeCoverage]
    public sealed class DisableCoverageAttribute : TypeLevelAspect, IAspectProvider
    {
        public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
        {
            Type disabledType = (Type)targetElement;

            var introducedExclusion = new CustomAttributeIntroductionAspect(
                  new ObjectConstruction(typeof(ExcludeFromCodeCoverageAttribute)));

            return new[] { new AspectInstance(disabledType, introducedExclusion) };
        }
    }
}