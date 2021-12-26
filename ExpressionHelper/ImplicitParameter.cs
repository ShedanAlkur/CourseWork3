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

        static public ImplicitParameter Create(Dictionary<string, float> constants)
        {
            InternalImplicitParameter[] internalParameters = new InternalImplicitParameter[constants.Count];
            int counter = 0;
            foreach(var s in constants)
                internalParameters[counter++] = new InternalImplicitParameter(ExpressionHelper.CreateConstant(s.Value), s.Key);
            return new ImplicitParameter(null, internalParameters);
        }
    }
}
