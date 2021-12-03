using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionBuilder
{
    public struct NestedExpressionParameter
    {
        public ParameterExpression externalParam;
        public Expression internalParam;
        public string internalParamName;

        public NestedExpressionParameter(ParameterExpression externalParam, Expression internalParam, string internalParamName)
        {
            this.externalParam = externalParam;
            this.internalParam = internalParam;
            this.internalParamName = internalParamName;
        }
    }
}
