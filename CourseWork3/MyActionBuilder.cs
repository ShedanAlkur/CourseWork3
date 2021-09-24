using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CourseWork3
{
    class MyActionBuilder
    {
        #region Статические атрибуты

        /// <summary>
        /// Словарь поддерживаемых бинарных операторов.
        /// </summary>
        static private readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> operatorsDic
            = new Dictionary<string, Func<Expression, Expression, BinaryExpression>>()
            { 
                ["+"] = Expression.Add,
                ["-"] = Expression.Subtract,
                ["*"] = Expression.Multiply,
                ["/"] = Expression.Divide,
                ["^"] = Expression.Power,
            };
        static private readonly string[] operators = operatorsDic.Keys.ToArray();
        static private bool IsOperator(string value) => operatorsDic.ContainsKey(value);
        static private Dictionary<string, byte> PriorityOfOperators
            = new Dictionary<string, byte>()
            {
                ["("] = 0,
                [")"] = 0,
                ["+"] = 1,
                ["-"] = 1,
                ["*"] = 2,
                ["/"] = 2,
                ["^"] = 3
            };
        static private bool IsLowerPriprity(string op1, string op2)
            => PriorityOfOperators[op1] < PriorityOfOperators[op2];

        /// <summary>
        /// Словарь поддерживаемых функций одной переменной.
        /// </summary>
        static private readonly Dictionary<string, Func<Expression, Expression>> functionsDic
            = new Dictionary<string, Func<Expression, Expression>>()
            {
                ["sqrt"] = GetExpressionFromMethod("Sqrt", typeof(Math)),
                ["sqr"] = GetExpressionFromMethod("Sqr", typeof(MyActionBuilder)),
                ["sin"] = GetExpressionFromMethod("Sin", typeof(Math)),
                ["cos"] = GetExpressionFromMethod("Cos", typeof(Math)),
                ["tg"] = GetExpressionFromMethod("Tan", typeof(Math)),
                ["abs"] = GetExpressionFromMethod("Abs", typeof(MyActionBuilder)),
                ["minus"] = GetExpressionFromMethod("Minus", typeof(MyActionBuilder)),
            };
        static private readonly string[] functions = functionsDic.Keys.ToArray();
        static private bool IsFunction(string value) => functionsDic.ContainsKey(value);

        /// <summary>
        /// Словарь поддерживаемых математический констант.
        /// </summary>
        static private  readonly Dictionary<string, Expression> constantsDic
            = new Dictionary<string, Expression>()
            {
                ["pi"] = Expression.Constant(Math.PI, typeof(double)),
                ["e"] = Expression.Constant(Math.E, typeof(double)),
            };
        static private readonly string[] constants = constantsDic.Keys.ToArray();
        static private bool IsConst(string value) => constantsDic.ContainsKey(value);


        static private ParameterExpression CreateParameter(string name) =>
            Expression.Parameter(typeof(double), name);
        static private Expression CreateConstant(double value) =>
            Expression.Constant(value, typeof(double));

        /// <summary>
        /// Метод создает из метода одной переменной заданного класса делегат, который принимает и возвращает Expression.
        /// </summary>
        /// <param name="method">Имя метода.</param>
        /// <param name="containingClass">Класс, содержащий метод. Вариант использования: typeof(Math) </param>
        /// <returns>Полученный из метода делегат.</returns>
        static private Func<Expression, Expression> GetExpressionFromMethod(string method, Type containingClass)
        {
            return (Expression value) =>  Expression.Call(
                containingClass.GetMethod(method,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                , value);
        }

        static private double Abs(double value) => Math.Abs(value);
        static private double Sqr(double value) => value * value;
        static private double Minus(double value) => -value;

        static private string splitToTokensPattern = @"";

        static MyActionBuilder()
        {
            // Создание шаблона регулярного выражения для разбиения входного арифметического выражения на токены.
            splitToTokensPattern += @"\d+(?:[\.]\d+)?";
            splitToTokensPattern += @"|,";
            foreach (var op in operators) splitToTokensPattern += @"|\" + op;
            splitToTokensPattern += @"|\(";
            splitToTokensPattern += @"|\)";
            splitToTokensPattern += @"|[A-z1-9_]+";
        }

        #endregion

        private Dictionary<string, ParameterExpression> parameters =
            new Dictionary<string, ParameterExpression>();

        private List<string> output = new List<string>();

        static private bool ConditionForMinus(string value)
            => !(IsOperator(value) || IsFunction(value) || value.Equals(",") || value.Equals("("));

        static private bool IsNumberOrParam(string value)
            => !(IsOperator(value) || IsFunction(value) || value.Equals(",") || value.Equals("(") || value.Equals(")"));

        static public string[] SplitToTokens(string input) 
        {
            string[] tokens = Regex.Matches(input.ToLower(), splitToTokensPattern)
                .Cast<Match>()
                .Select(match => match.Value)
                .ToArray();

            if (tokens.Select(x => x.Length).Sum() != input.Replace(" ", "").Length)
                throw new ArgumentException("Входная строка имела неверный формат");

            return tokens;
        }

        public string[] ConvertToRPN(string[] tokens)
        {
            // https://ru.wikipedia.org/wiki/алгоритм_сортировочной_станции

            Stack<string> stack = new Stack<string>();

            for (int i = 0; i < tokens.Length; i++) // Пока не все токены обработаны
            {
                
                if (IsFunction(tokens[i])) // Если токен - функция, то поместить его в стек.
                { stack.Push(tokens[i]); }
                else if (tokens[i].Equals(",")) // Если токен - разделитель аргументов функции, то
                {
                    while (!stack.Peek().Equals("(")) // Пока токен на вершине стека не открывающая скобка
                    {
                        output.Add(stack.Pop()); // Переложить оператор из стека в выходную очередь.
                        if (stack.Count == 0) // Если стек закончился до того, как был встречен токен открывающей скобки, то в выражении ошибка.
                            throw new ArgumentException("В выражении пропущен разделитель аргументов функции (запятая), либо пропущена открывающая скобка.");
                    }
                }
                else if (IsOperator(tokens[i])) // Если токен - оператор op1, то
                {
                    if (tokens[i].Equals("-") && (i == 0 || !ConditionForMinus(tokens[i - 1]))) // Если токен - оператор вычитания, и токен является первым входным токеном или предыдущий токен не был числом или переменной, то
                    { stack.Push("minus"); continue; } // Положить в стек функцию унарного минуса minus, установить флаг унарного минуса в True.
                    while (stack.Count != 0 && !IsFunction(stack.Peek()) && !IsLowerPriprity(stack.Peek(), tokens[i])) // Пока присутствует на вершине стека токер оператора op2, чей приоритет выше или равен приоритету op1
                    {
                        output.Add(stack.Pop()); // Переложить оператор op2 из стека в выходную очередь.
                    }
                    stack.Push(tokens[i]); // Положить op1 в стек.
                }
                else if (tokens[i].Equals("(")) // Если токен - открывающая скобка, то положить его в стек.
                { stack.Push(tokens[i]); }
                else if (tokens[i].Equals(")")) // Если токен - закрывающая скобка, то
                {
                    while (!stack.Peek().Equals("(")) // Пока токен на вершине стека не открывающая скобка
                    {
                        output.Add(stack.Pop()); // Переложить оператор из стека в выходную очередь.
                        if (stack.Count == 0) // Если стек закончился до того, как был встречен токен открывающей скобки, то в выражении пропущена скобка.
                            throw new ArgumentException("В выражении пропущена скобка.");
                    }
                    stack.Pop(); // Выкинуть открывающую скобку из стека, но не добравлять в очередь вывода.
                    while (stack.Count > 0 && IsFunction(stack.Peek())) // Пока токен на вершине стека - функция, переложить её в выходную очередь.
                    { output.Add(stack.Pop()); }
                }
                else // Если токен - число или переменная, то добавить его в очередь вывода .
                { 
                    output.Add(tokens[i]);
                    while (stack.Count > 0 && IsFunction(stack.Peek())) // Пока токен на вершине стека - функция, переложить её в выходную очередь.
                    { output.Add(stack.Pop()); }
                }
            }
            // Если больше не осталось токено на входе
            while (stack.Count > 0) // Пока есть токены операторы в стеке
            {
                if (stack.Peek().Equals("(")) // Если токе оператор на вершине стека - открывающая скобка, то в выражении пропущена скобка.
                    throw new ArgumentException("В выражении пропущена скобка.");
                // Переложить оператор из стека в выходную очередь.
                output.Add(stack.Pop());
            }

            return output.ToArray();
        }

        public Expression BuildExpression(string[] RPN)
        {
            throw new NotImplementedException();
        }



        public Delegate CompileString(string expression, string[] paramsName = null)
        {
            this.Clear();

            if (paramsName != null) foreach (string param in paramsName)
                    parameters.Add(param, CreateParameter(param));

            string[] tokens = SplitToTokens(expression);
            string[] RPN = ConvertToRPN(tokens);
            Expression resExpression = BuildExpression(tokens);
            return Expression.Lambda(resExpression, parameters.Values).Compile();
        }

        public void Clear()
        {
            parameters.Clear();
            //stack.Clear();
            output.Clear();
        }
    }
}
