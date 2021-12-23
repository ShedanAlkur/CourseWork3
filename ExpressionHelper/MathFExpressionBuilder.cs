using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ExpressionBuilder
{
    public class MathFExpressionBuilder
    {
        #region Статические атрибуты

        #region Бинарные операторы
        /// <summary>
        /// Словарь поддерживаемых бинарных операторов.
        /// </summary>
        static private readonly Dictionary<string, Func<Expression, Expression, Expression>> operators
            = new Dictionary<string, Func<Expression, Expression, Expression>>()
            { 
                // разбирать как сейчас: @"\" + op
                ["+"] = Expression.Add,
                ["-"] = Expression.Subtract,
                ["*"] = Expression.Multiply,
                ["/"] = Expression.Divide,
                ["^"] = ExpressionHelper.CreateExpressionFromBinaryFunc(typeof(MathF), "Pow"),
                // разюирать просто op
                [">"] = Expression.GreaterThan,
                [">="] = Expression.GreaterThanOrEqual,
                ["<"] = Expression.LessThan,
                ["<="] = Expression.LessThanOrEqual,
                ["=="] = Expression.Equal,
                ["!="] = Expression.NotEqual,
                ["||"] = Expression.Or,
                ["&&"] = Expression.And,
            };

        /// <summary>
        /// 
        /// </summary>
        static private readonly string[] operatorsNameForRegexPattern = { @"\+", "-", @"\*", @"\/", @"\^", 
            ">=", ">", "<=", "<", "==", "!=",
            @"&&", @"\|\|",
        };

        /// <summary>
        /// Является ли токен бинарным оператором.
        /// </summary>
        static private bool IsOperator(string token) => operators.ContainsKey(token);

        /// <summary>
        /// Словарь приоритетов бинарных операторов.
        /// </summary>
        static private Dictionary<string, byte> PriorityOfOperators
            = new Dictionary<string, byte>()
            {
                {"(", 0},
                {")", 0},
                {"||", 1},
                {"&&", 2},
                {"==", 3},
                {"!=", 3},
                {">",  4},
                {">=", 4},
                {"<",  4},
                {"<=", 4},
                {"+", 5},
                {"-", 5},
                {"*", 6},
                {"/", 6},
                {"^", 7},
            };

        /// <summary>
        /// Является ли приоритет первого бинарного оператора ниже, чем приоритет второго.
        /// </summary>
        /// <param name="op1">Первый бинарный оператор.</param>
        /// <param name="op2">Второй бинарный оператор.</param>
        /// <returns></returns>
        static private bool IsLowerPriority(string op1, string op2)
            => PriorityOfOperators[op1] < PriorityOfOperators[op2];
        #endregion

        /// <summary>
        /// Словарь поддерживаемых унарных функций.
        /// </summary>
        static private readonly Dictionary<string, Func<Expression, Expression>> unaryFunctions
            = new Dictionary<string, Func<Expression, Expression>>()
            {
                ["sqrt"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathF), nameof(MathF.Sqrt)),
                ["sqr"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathFExpressionBuilder), nameof(MathFExpressionBuilder.Sqr)),
                ["sin"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathF), nameof(MathF.Sin)),
                ["cos"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathF), nameof(MathF.Cos)),
                ["tg"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathF), nameof(MathF.Tan)),
                ["abs"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathF), nameof(MathF.Abs)),
                ["minus"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathFExpressionBuilder), nameof(MathFExpressionBuilder.Minus)),
                ["round"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathFExpressionBuilder), nameof(MathFExpressionBuilder.Round)),
                ["ceil"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathF), nameof(MathF.Ceiling)),
                ["floor"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathF), nameof(MathF.Floor)),
                ["!"] = Expression.Not,
            };

        /// <summary>
        /// Является ли токен унарной функцией.
        /// </summary>
        static private bool IsUnaryFunction(string token) => unaryFunctions.ContainsKey(token);

        /// <summary>
        /// Словарь поддерживаемых бинарных функций.
        /// </summary>
        static private readonly Dictionary<string, Func<Expression, Expression, Expression>> binaryFunctions
            = new Dictionary<string, Func<Expression, Expression, Expression>>()
            {
                ["min"] = ExpressionHelper.CreateExpressionFromBinaryFunc(typeof(MathF), nameof(MathF.Min)),
                ["max"] = ExpressionHelper.CreateExpressionFromBinaryFunc(typeof(MathF), nameof(MathF.Max)),
                ["log"] = ExpressionHelper.CreateExpressionFromBinaryFunc(typeof(MathFExpressionBuilder), nameof(MathFExpressionBuilder.Log)),
            };

        /// <summary>
        /// Является ли токен бинарной функцией.
        /// </summary>
        static private bool IsBinaryFunction(string token) => binaryFunctions.ContainsKey(token);

        /// <summary>
        /// Словарь поддерживаемых тернарных функций.
        /// </summary>
        static private readonly Dictionary<string, Func<Expression, Expression, Expression, Expression>> ternaryFunctions
            = new Dictionary<string, Func<Expression, Expression, Expression, Expression>>()
            {
                ["clamp"] = ExpressionHelper.CreateExpressionFromTernaryFunc(typeof(MathFExpressionBuilder), nameof(Clamp)),
                ["if"] = Expression.Condition,
            };

        /// <summary>
        /// Является ли токен тернарной функцией.
        /// </summary>
        static private bool IsTernaryFunction(string token) => ternaryFunctions.ContainsKey(token);


        /// <summary>
        /// Является ли токен функцией.
        /// </summary>
        static private bool IsFunction(string token) => IsUnaryFunction(token) || IsBinaryFunction(token) || IsTernaryFunction(token);

        /// <summary>
        /// Словарь используемых ключевых слов, возвращающих числовое значение.
        /// </summary>
        static private readonly Dictionary<string, Expression> constants
            = new Dictionary<string, Expression>()
            {
                ["pi"] = ExpressionHelper.CreateConstant(MathF.PI),
                ["e"] = ExpressionHelper.CreateConstant(MathF.E),
                ["random"] = ExpressionHelper.CreateMethodCallExpression(typeof(MathFExpressionBuilder), nameof(MathFExpressionBuilder.RandomaMethod)),
                ["true"] = ExpressionHelper.CreateConstant(true),
                ["false"] = ExpressionHelper.CreateConstant(false),
            };

        /// <summary>
        /// Является ли токен ключевым слоо, возвращающим числовое значение.
        /// </summary>
        static private bool IsConst(string token) => constants.ContainsKey(token);

        static private Random random;
        static public Random Random
        {
            get
            {
                if (random == null) random = new Random();
                return random;
            }
            set
            {
                random = Random;
            }
        }

        #region Функции для извлечения MethodCallExpression
        static private float Abs(float value) => MathF.Abs(value);
        static private float Sqr(float value) => value * value;
        static private float Minus(float value) => -value;
        static private float Round(float value) => MathF.Round(value);
        static private float Clamp(float value, float minValue, float maxValue)// => MathF.Max(minValue, MathF.Min(value, maxValue));
        {
            if (value < minValue) return minValue;
            else if (value > maxValue) return maxValue;
            return value;
        }
        static private float Log(float x, float y) => MathF.Log(x, y);
        static private float RandomaMethod() => (float)Random.NextDouble();
        #endregion

        static private string splitToTokensPattern = @"";

        static private readonly System.Globalization.CultureInfo ci;

        static private readonly string separator = ";";

        static MathFExpressionBuilder()
        {
            if (operatorsNameForRegexPattern.Length != operators.Count) 
                throw new SystemException($"Количество бинарных операторов в {nameof(operators)} и операторов для Regex шаблона в {nameof(operatorsNameForRegexPattern)} не совпадает");

            // Создание шаблона регулярного выражения для разбиения входного арифметического выражения на токены.
            splitToTokensPattern += @"\d+(?:[\.]\d+)?";
            splitToTokensPattern += @"|" + separator;
            foreach (var op in operatorsNameForRegexPattern) splitToTokensPattern += @"|" + op;
            splitToTokensPattern += "|!";
            splitToTokensPattern += @"|\(";
            splitToTokensPattern += @"|\)";
            splitToTokensPattern += @"|[A-z1-9_]+";

            ci = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
        }

        /// <summary>
        /// Проверяет условие того, что последующий токен вычитания - унарный минус.
        /// </summary>
        /// <param name="value">Проверяемый токен.</param>
        /// <returns>Последующий токен вычитания должен быть унарным минусом.</returns>
        static private bool ConditionForMinus(string value)
            => !(IsOperator(value) || IsFunction(value) || value.Equals(separator) || value.Equals("("));
        #endregion

        /// <summary>
        /// Словарь используемых в арифметическом выражении параметров.
        /// </summary>
        private Dictionary<string, ParameterExpression> parameters =
            new Dictionary<string, ParameterExpression>();

        /// <summary>
        /// Словарь используемых в арифметическом выражении параметров, которые особым образом извлекаются из входных параметров итогового делегата.
        /// </summary>
        private Dictionary<string, Expression> nestedParameters =
            new Dictionary<string, Expression>();

        public Delegate ResultDelegate { private set; get; }

        /// <summary>
        /// Массив имен используемых параметров в арифметическом выражении.
        /// </summary>
        public string[] Parameters { get => nestedParameters.Keys.Concat(parameters.Keys).ToArray(); }

        /// <summary>
        /// Является ли токен числом, константой или параметром.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static private bool IsNumberOrParam(string value)
            => !(IsOperator(value) || IsFunction(value) || value.Equals(";") || value.Equals("(") || value.Equals(")"));

        /// <summary>
        /// Разделяет входное арифметическое выражение на токены.
        /// </summary>
        /// <param name="input">Входная строка арифметического выражения.</param>
        /// <returns>Токены исходного арифметического выражения.</returns>
        public string[] SplitToTokens(string input) 
        {
            string[] tokens = Regex.Matches(input.ToLower(), splitToTokensPattern)
                .Cast<Match>()
                .Select(match => match.Value)
                .ToArray();

            if (tokens.Select(x => x.Length).Sum() != input.Replace(" ", "").Length)
                throw new ArgumentException("Входная строка имела неверный формат");

            return tokens;
        }

        /// <summary>
        /// Преобразует последовательность токенов инфиксного арифметического выражения в последовательность постфиксного (обратная польская нотация).
        /// </summary>
        /// <param name="tokens">Последовательность токенов инфиксного арифметического выражения.</param>
        /// <returns>Последовательность токенов постфиксного арифметического выражения.</returns>
        public string[] ConvertToRPN(string[] tokens)
        {
            // https://ru.wikipedia.org/wiki/алгоритм_сортировочной_станции

            Stack<string> stack = new Stack<string>();
            List<string> output = new List<string>();

            for (int i = 0; i < tokens.Length; i++) // Пока не все токены обработаны
            {
                if (IsFunction(tokens[i])) // Если токен - функция, то поместить его в стек.
                { stack.Push(tokens[i]); }
                else if (tokens[i].Equals(separator)) // Если токен - разделитель аргументов функции, то
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
                    while (stack.Count != 0 && !IsFunction(stack.Peek()) && !IsLowerPriority(stack.Peek(), tokens[i])) // Пока присутствует на вершине стека токен оператора op2, чей приоритет выше или равен приоритету op1
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

        /// <summary>
        /// Строит операционное дерево Expression на основе постфиксного арифметического выражения.
        /// </summary>
        /// <param name="tokens">Последовательность токенов постфиксного арифметического выражения.</param>
        /// <returns>Операционное дерево Expression</returns>
        public Expression BuildExpression(string[] tokens)
        {
            Stack<Expression> stack = new Stack<Expression>();

            for (int i = 0; i < tokens.Length; i++) // Пока не все токены обработаны
            {
                if (IsOperator(tokens[i])) // Если токен - бинарный оператор, то соответствующая операция выполняется над двумя значениями, взятыми из вершины стека. Результат операции помещается в стек.
                {
                    var exp2 = stack.Pop();
                    var exp1 = stack.Pop();
                    stack.Push(operators[tokens[i]](exp1, exp2));
                }
                else if (IsUnaryFunction(tokens[i])) // Если токен - унарная функция, то соответствующая операция выполняется над одним значением, взятым из вершины стека. Результат операции помещается в стек. 
                  stack.Push(unaryFunctions[tokens[i]](stack.Pop()));
                else if (IsBinaryFunction(tokens[i])) // Если токен - бинарная функция, то соответствующая операция выполняется над двумя значениями, взятыми из вершины стека. Результат операции помещается в стек. 
                {
                    var exp2 = stack.Pop();
                    var exp1 = stack.Pop();
                    stack.Push(binaryFunctions[tokens[i]](exp1, exp2));
                }
                else if (IsTernaryFunction(tokens[i])) // Если токен - тернарная функция, то соответствующая операция выполняется над тремя значениями, взятыми из вершины стека. Результат операции помещается в стек. 
                {
                    var exp3 = stack.Pop();
                    var exp2 = stack.Pop();
                    var exp1 = stack.Pop();
                    stack.Push(ternaryFunctions[tokens[i]](exp1, exp2, exp3));
                }
                else // Если токен - операнд, то он помещается на вершину стека.
                {
                    if (float.TryParse(tokens[i], System.Globalization.NumberStyles.Any, ci, out float value)) // Если операнд - число
                        stack.Push(ExpressionHelper.CreateConstant<float>(value));
                    else if (IsConst(tokens[i])) // Если операнд - математическая константа
                        stack.Push(constants[tokens[i]]);
                    else if (nestedParameters.ContainsKey(tokens[i])) // если операнд - вложенный параметр
                        stack.Push(nestedParameters[tokens[i]]);
                    else if (parameters.ContainsKey(tokens[i])) // Если операнд - сохраненный параметр (нормальный)
                        stack.Push(parameters[tokens[i]]);
                    else // Если операнд - несохраненный параметр (нормальный)
                    {
                        parameters.Add(tokens[i], ExpressionHelper.CreateParameter<float>(tokens[i]));
                        stack.Push(parameters[tokens[i]]);
                    }
                }
            }
            if (stack.Count > 1) // На всякий случай
                throw new NotImplementedException();

            return stack.Pop();
        }

        /// <summary>
        /// Преобразует инфиксное арифметическое выражение в делегат.
        /// </summary>
        /// <param name="infixExpression">Арифметическое выражение в инфиксной форме. Пример: (A + B).</param>
        /// <param name="paramsName">Имена параметров типа float, которые в заданной последовательности должен принимать делегат.</param>
        /// <returns>Делегат, соответствующий арифемтическому выражению.</returns>
        public Delegate CompileString(string infixExpression, params string[] paramsName)
        {
            this.Clear();

            if (paramsName != null) foreach (string param in paramsName)
                    parameters.Add(param.ToLower(), ExpressionHelper.CreateParameter<float>(param.ToLower()));

            string[] infixTokens = SplitToTokens(infixExpression);
            string[] postfixTokens = ConvertToRPN(infixTokens);
            Expression resExpression = BuildExpression(postfixTokens);
            ResultDelegate = Expression.Lambda(resExpression, parameters.Values).Compile();
            return ResultDelegate;
        }

        /// <summary>
        /// Преобразует набор токенов инфиксного арифметическое выражение в делегат.
        /// </summary>
        /// <param name="infixTokens">Набор токенов арифметического выражения в инфиксной форме.</param>
        /// <param name="paramsName">Имена параметров, которые в заданной последовательности должен принимать делегат.</param>
        /// <returns>Делегат, соответствующий арифемтическому выражению.</returns>
        public Delegate CompileTokens(string[] infixTokens, params string[] paramsName)
        {
            this.Clear();

            if (paramsName != null) foreach (string param in paramsName)
                    parameters.Add(param.ToLower(), ExpressionHelper.CreateParameter<float>(param.ToLower()));

            string[] postfixTokens = ConvertToRPN(infixTokens);
            Expression resExpression = BuildExpression(postfixTokens);
            ResultDelegate = Expression.Lambda(resExpression, parameters.Values).Compile();
            return ResultDelegate;
        }


        public Delegate CompileTokens(string[] infixTokens, NestedExpressionParameter[] nestedExpressionParameter, params string[] paramsName)
        {
            this.Clear();

            if (paramsName != null) foreach (string param in paramsName)
                    parameters.Add(param.ToLower(), ExpressionHelper.CreateParameter<float>(param.ToLower()));

            var externalNestedParams = new List<ParameterExpression>();
            foreach (var param in nestedExpressionParameter)
            {
                nestedParameters.Add(param.internalParamName.ToLower(), param.internalParam);
                if (param.externalParam != null) externalNestedParams.Add(param.externalParam);
            }

            string[] postfixTokens = ConvertToRPN(infixTokens);
            Expression resExpression = BuildExpression(postfixTokens);
            List<ParameterExpression> generalParams = new List<ParameterExpression>(externalNestedParams);
            generalParams.AddRange(parameters.Values);
            ResultDelegate = Expression.Lambda(resExpression, generalParams).Compile();
            return ResultDelegate;
        }

        /// <summary>
        /// Удаляет все результаты расчетов из экземпляра класса.
        /// </summary>
        public void Clear()
        {
            parameters.Clear();
            nestedParameters.Clear();
            ResultDelegate = null;
        }
    }
}
