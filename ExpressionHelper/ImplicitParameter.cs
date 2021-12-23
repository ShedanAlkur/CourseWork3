using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionBuilder
{
    public class ImplicitParameter
    {
        public ParameterExpression externalParameter;

        public InternalImplicitParameter[] internalParameters;

        public ImplicitParameter(ParameterExpression externalParameter, params InternalImplicitParameter[] internalParameters)
        {
            this.externalParameter = externalParameter;
            this.internalParameters = internalParameters;
        }
    }
}
