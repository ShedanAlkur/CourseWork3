using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExpressionBuilder
{
    public class InternalImplicitParameter
    {
        public Expression Value;
        public string Name;

        public InternalImplicitParameter(Expression value, string name)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
