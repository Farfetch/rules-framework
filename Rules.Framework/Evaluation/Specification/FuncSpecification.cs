using System;
using System.Collections.Generic;

namespace Rules.Framework.Evaluation.Specification
{
    internal class FuncSpecification<T> : SpecificationBase<T>
    {
        private readonly Func<T, bool> evalFunc;

        public FuncSpecification(Func<T, bool> evalFunc)
        {
            this.evalFunc = evalFunc;
        }

        public override bool IsSatisfiedBy(T input) => this.evalFunc.Invoke(input);
    }
}