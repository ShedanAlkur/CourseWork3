using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace CourseWork3
{
    /// Оригинальный код из проекта Ensalada
    public static class ActionBuilder
    {
        private static Dictionary<string, Expression> _exprs = new Dictionary<string, Expression>();

        private static int _paramCount = -1;
        private static int _exprCount = -1;

        private static List<ParameterExpression> _parametersUsed = new List<ParameterExpression>();
        private static List<string> _parametersName = new List<string>();

        private static string functionStr = "";

        private static string GetTopPriorityActionString(string functionBody)
        {
            var subs = new string(functionBody);
            while (subs.LastIndexOf('(') != -1 && subs.IndexOf(')') != -1)
            {
                var indexOfLastOpeningParanthesis = subs.LastIndexOf('(');
                var indexOfFirstRelativeClosingParanthesis =
                    new string(subs.Skip(indexOfLastOpeningParanthesis).ToArray()).IndexOf(')') +
                    indexOfLastOpeningParanthesis - 1;
                var expressionLength = indexOfFirstRelativeClosingParanthesis - indexOfLastOpeningParanthesis;
                subs = subs.Substring(subs.LastIndexOf('(') + 1, expressionLength);
            }

            var operatorIndex = 0;

            if (subs.IndexOf("^") != -1) operatorIndex = subs.IndexOf("^");
            else if (subs.IndexOf("*") != -1) operatorIndex = subs.IndexOf("*");
            else if (subs.IndexOf("/") != -1) operatorIndex = subs.IndexOf("/");
            else if (subs.IndexOf("+") != -1) operatorIndex = subs.IndexOf("+");
            else if (subs.IndexOf("-") != -1) operatorIndex = subs.IndexOf("-");

            var op1 = "";
            var index = -1;
            while (++index != operatorIndex)
                if (subs[index].Equals('+') || subs[index].Equals('-') || subs[index].Equals('*') ||
                    subs[index].Equals('^') || subs[index].Equals('/'))
                    op1 = "";
                else op1 += subs[index];
            ++index; // Пропускаем оператор
            var op2 = "";
            op2 = new string(subs.Skip(index).TakeWhile(ch =>
                !(ch.Equals('+') || ch.Equals('-') || ch.Equals('*') || ch.Equals('^') || ch.Equals('/'))).ToArray());
            return op1 + subs[operatorIndex] + op2;
        }

        private static string ReplaceParameterNames(string action, string renameFrom, string renameTo)
        {
            var rebuiltAction = "";
            for (var i = 0; i < action.Length; ++i)
                if (renameFrom.Length <= action.Length - i)
                {
                    var compare = action.Substring(i, renameFrom.Length);
                    if (compare.Equals(renameFrom))
                    {
                        var replacementConditionOne = false;
                        if (i - 1 >= 0)
                        {
                            if (action[i - 1].Equals('+') || action[i - 1].Equals('-') || action[i - 1].Equals('*') ||
                                action[i - 1].Equals('/') || action[i - 1].Equals('^') || action[i - 1].Equals('(') ||
                                action[i - 1].Equals(')'))
                                replacementConditionOne = true;
                        }
                        else
                        {
                            replacementConditionOne = true;
                        }

                        var replacemenetConditionTwo = false;
                        if (renameFrom.Length + i >= action.Length)
                        {
                            replacemenetConditionTwo = true;
                        }
                        else
                        {
                            if (action[i + renameFrom.Length].Equals('+') ||
                                action[i + renameFrom.Length].Equals('-') ||
                                action[i + renameFrom.Length].Equals('*') ||
                                action[i + renameFrom.Length].Equals('/') ||
                                action[i + renameFrom.Length].Equals('(') ||
                                action[i + renameFrom.Length].Equals(')') ||
                                action[i + renameFrom.Length].Equals('^'))
                                replacemenetConditionTwo = true;
                        }


                        if (replacemenetConditionTwo && replacementConditionOne)
                        {
                            rebuiltAction += renameTo;
                            i += renameFrom.Length - 1;
                        }
                        else
                        {
                            rebuiltAction += action[i];
                        }
                    }
                    else
                    {
                        rebuiltAction += action[i];
                    }
                }
                else
                {
                    rebuiltAction += action[i];
                }

            return rebuiltAction;
        }

        private static Expression GetExpressionFromString(string expressionString, ref string action)
        {
            var indexOfAny = expressionString.IndexOfAny(new[] { '/', '+', '^', '*', '-' });
            if (indexOfAny == -1)
                return null;

            var sign = expressionString[indexOfAny];
            var operands = expressionString.Split(sign);
            Expression operand1, operand2;

            var searchOperand1 = _parametersUsed.Where(parameter => parameter.Name.Equals(operands[0]));
            var searchOperand2 = _parametersUsed.Where(parameter => parameter.Name.Equals(operands[1]));

            if (_exprs.ContainsKey(operands[0]))
            {
                operand1 = _exprs[operands[0]];
            }
            else if (double.TryParse(operands[0].Replace(".", ","), out var res))
            {
                operand1 = Expression.Constant(res, typeof(double));
            }
            else if (searchOperand1.Count() != 0)
            {
                operand1 = searchOperand1.First();
            }
            else
            {
                _parametersName.Add(operands[0]);
                operand1 = Expression.Parameter(typeof(double), "p" + ++_paramCount);
                _parametersUsed.Add((ParameterExpression)operand1);
                functionStr = ReplaceParameterNames(functionStr, operands[0], "p" + _paramCount);
                action = ReplaceParameterNames(action, operands[0], "p" + _paramCount);
            }

            if (_exprs.ContainsKey(operands[1]))
            {
                operand2 = _exprs[operands[1]];
            }
            else if (double.TryParse(operands[1].Replace(".", ","), out var res))
            {
                operand2 = Expression.Constant(res, typeof(double));
            }
            else if (searchOperand2.Count() != 0)
            {
                operand2 = searchOperand2.First();
            }
            else
            {
                _parametersName.Add(operands[1]);
                operand2 = Expression.Parameter(typeof(double), "p" + ++_paramCount);
                _parametersUsed.Add((ParameterExpression)operand2);
                functionStr = ReplaceParameterNames(functionStr, operands[1], "p" + _paramCount);
                action = ReplaceParameterNames(action, operands[1], "p" + _paramCount);
            }

            switch (sign)
            {
                case '/':
                    return Expression.Divide(operand1, operand2);
                case '+':
                    return Expression.Add(operand1, operand2);
                case '-':
                    return Expression.Subtract(operand1, operand2);
                case '*':
                    return Expression.Multiply(operand1, operand2);
                case '^':
                    return Expression.Power(operand1, operand2);
                default:
                    throw new Exception("Неизвестный знак действия!");
            }
        }

        private static string RemovePointlessParanthesses(string function)
        {
            var copy = new string(function);
            for (var i = 0; i < copy.Length; ++i)
                if (copy[i].Equals('('))
                {
                DEEP_SEARCH:
                    var startIndex = i;
                    ++i;
                    var operatorMet = false;
                    while (!copy[i].Equals(')'))
                    {
                        if (copy[i].Equals('('))
                            goto DEEP_SEARCH;

                        if (copy[i].Equals('+') || copy[i].Equals('-') || copy[i].Equals('*') || copy[i].Equals('^') ||
                            copy[i].Equals('/'))
                        {
                            operatorMet = true;
                            break;
                        }

                        ++i;
                        if (i == copy.Length) return copy;
                    }

                    if (!operatorMet)
                    {
                        copy = copy.Remove(startIndex, 1).Remove(i - 1, 1);
                        i = -1;
                    }
                }

            return copy;
        }

        private static Expression GetResultingExpression()
        {
            return _exprs["a" + _exprCount];
        }

        private static void BuildExpressionTree()
        {
            functionStr = functionStr.Replace(" ", "");

            while (functionStr.IndexOfAny(new[] { '/', '+', '^', '*', '-' }) != -1)
            {
                var action = GetTopPriorityActionString(functionStr);
                var expression = GetExpressionFromString(action, ref action);
                if (expression == null)
                {
                    functionStr = RemovePointlessParanthesses(functionStr);
                }
                else
                {
                    var actionPseudoname = "a" + ++_exprCount;
                    _exprs.Add(actionPseudoname, expression);
                    functionStr = functionStr.Replace(action, actionPseudoname);
                    functionStr = RemovePointlessParanthesses(functionStr);
                }
            }
        }

        private static void CleanUp()
        {
            functionStr = "";
            _exprCount = -1;
            _paramCount = -1;
            _parametersUsed = new List<ParameterExpression>();
            _exprs = new Dictionary<string, Expression>();
        }

        private static void CheckExpressionValidity()
        {
            var openingParanthesisCount = (from symbol in functionStr
                                           where symbol.Equals('(')
                                           select symbol).Count();
            var closingParanthesisCount = (from symbol in functionStr
                                           where symbol.Equals(')')
                                           select symbol).Count();
            if (openingParanthesisCount != closingParanthesisCount)
                throw new Exception("Количество открывающих и закрывающих скобок не совпадает!");
        }


        public static Delegate CompileString(string functionBody, out List<string> parametersName)
        {
            functionStr = functionBody;
            CheckExpressionValidity();
            BuildExpressionTree();
            var resultingExpression = GetResultingExpression();
            var returningDelegate = Expression.Lambda(resultingExpression, _parametersUsed);
            parametersName = _parametersName;
            CleanUp();
            return returningDelegate.Compile();
        }


        public static void Test()
        {
            Delegate sampleFunction = CompileString("(((2400))) * (((((((((L+5)/4)))))))) - (200/(L+148*15))", out _);
            var alpha = sampleFunction.DynamicInvoke(2);
        }
    }
}
