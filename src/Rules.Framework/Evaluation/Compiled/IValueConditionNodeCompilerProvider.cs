namespace Rules.Framework.Evaluation.Compiled
{
    using Rules.Framework.Core;
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal interface IValueConditionNodeCompilerProvider
    {
        IValueConditionNodeCompiler GetValueConditionNodeCompiler(string multiplicity);
    }
}
