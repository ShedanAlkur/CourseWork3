using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ExpressionBuilder
{
    public class MathExpressionBuilder
    {
        #region Статические атрибуты

        /// <summary>
        /// Словарь поддерживаемых бинарных операторов.
        /// </summary>
        static private readonly Dictionary<string, Func<Expression, Expression, BinaryExpression>> operators
            = new Dictionary<string, Func<Expression, Expression, BinaryExpression>>()
            { 
                ["+"] = Expression.Add,
                ["-"] = Expression.Subtract,
                ["*"] = Expression.Multiply,
                ["/"] = Expression.Divide,
                ["^"] = Expression.Power,
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
                ["("] = 0,
                [")"] = 0,
                ["+"] = 1,
                ["-"] = 1,
                ["*"] = 2,
                ["/"] = 2,
                ["^"] = 3
            };

        /// <summary>
        /// Является ли приоритет первого бинарного оператора ниже, чем приоритет второго.
        /// </summary>
        /// <param name="op1">Первый бинарный оператор.</param>
        /// <param name="op2">Второй бинарный оператор.</param>
        /// <returns></returns>
        static private bool IsLowerPriority(string op1, string op2)
            => PriorityOfOperators[op1] < PriorityOfOperators[op2];

        /// <summary>
        /// Словарь поддерживаемых функций одной переменной.
        /// </summary>
        static private readonly Dictionary<string, Func<Expression, Expression>> functions
            = new Dictionary<string, Func<Expression, Expression>>()
            {
                ["sqrt"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(Math), nameof(Math.Sqrt)),
                ["sqr"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathExpressionBuilder), nameof(MathExpressionBuilder.Sqr)),
                ["sin"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(Math), nameof(Math.Sin)),
                ["cos"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(Math), nameof(Math.Cos)),
                ["tg"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(Math), nameof(Math.Tan)),
                ["abs"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathExpressionBuilder), nameof(MathExpressionBuilder.Abs)),
                ["minus"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathExpressionBuilder), nameof(MathExpressionBuilder.Minus)),
                ["round"] = ExpressionHelper.CreateExpressionFromUnaryFunc(typeof(MathExpressionBuilder), nameof(MathExpressionBuilder.Round))
            };

        /// <summary>
        /// Является ли токен бинарным оператором.
        /// </summary>
        static private bool IsFunction(string token) => functions.ContainsKey(token);

        /// <summary>
        /// Словарь используемых ключевых слов, возвращающих числовое значение.
        /// </summary>
        static private  readonly Dictionary<string, Expression> constants
            = new Dictionary<string, Expression>()
            {
                ["pi"] = ExpressionHelper.CreateConstant<double>(Math.PI),
                ["e"] = ExpressionHelper.CreateConstant<double>(Math.E),
                ["random"] = Expression.Call(
                typeof(MathExpressionBuilder).GetMethod(nameof(MathExpressionBuilder.Random),
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)),
    };
        /// <summary>
        /// Является ли токен ключевым слоо, возвращающим числовое значение.
        /// </summary>
        static private bool IsConst(string token) => constants.ContainsKey(token);

        /// <summary>
        /// Метод создает из функции одной переменной func<double, double> заданного класса делегат, который принимает и возвращает Expression.
        /// </summary>
        /// <param name="method">Имя метода.</param>
        /// <param name="containingClass">Класс, содержащий метод. Вариант использования: typeof(Math) </param>
        /// <returns>Полученный из метода делегат.</returns>
        static private Func<Expression, Expression> GetExpressionFromFunc(string method, Type containingClass)
        {
            return (Expression value) =>  Expression.Call(
                containingClass.GetMethod(method,
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                , value);
        }



        static private Random rnd = new Random();

        static private double Abs(double value) => Math.Abs(value);
        static private double Sqr(double value) => value * value;
        static private double Minus(double value) => -value;
        static private double Round(double value) => Math.Round(value);
        static private double Random() => rnd.NextDouble();

        static private string splitToTokensPattern = @"";

        static MathExpressionBuilder()
        {
            // Создание шаблона регулярного выражения для разбиения входного арифметического выражения на токены.
            splitToTokensPattern += @"\d+(?:[\.]\d+)?";
            splitToTokensPattern += @"|;";
            foreach (var op in operators.Keys) splitToTokensPattern += @"|\" + op;
            splitToTokensPattern += @"|\(";
            splitToTokensPattern += @"|\)";
            splitToTokensPattern += @"|[A-z1-9_]+";
        }

        /// <summary>
        /// Проверяет условие того, что последующий токен вычитания - унарный минус.
        /// </summary>
        /// <param name="value">Проверяемый токен.</param>
        /// <returns>Последующий токен вычитания должен быть унарным минусом.</returns>
        static private bool ConditionForMinus(string value)
            => !(IsOperator(value) || IsFunction(value) || value.Equals(",") || value.Equals("("));
        #endregion

        /// <summary>
        /// Словарь используемых параметров в арифметическом выражении.
        /// </summary>
        private Dictionary<string, ParameterExpression> parameters =
            new Dictionary<string, ParameterExpression>();

        public Delegate ResultDelegate { private set; get; }

        /// <summary>
        /// Массив имен используемых параметров в арифметическом выражении.
        /// </summary>
        public string[] Parameters { get => parameters.Keys.ToArray(); }

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
                else if (tokens[i].Equals(";")) // Если токен - разделитель аргументов функции, то
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
                    while (stack.Count != 0 && !IsFunction(stack.Peek()) && !IsLowerPriority(stack.Peek(), tokens[i])) // Пока присутствует на вершине стека токер оператора op2, чей приоритет выше или равен приоритету op1
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
            System.Globalization.CultureInfo ci = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            Stack<Expression> stack = new Stack<Expression>();

            for (int i = 0; i < tokens.Length; i++) // Пока не все токены обработаны
            {
                if (IsOperator(tokens[i])) // Если токен - бинарный оператор, то соответствующая операция выполняется над двумя значениями, взятыми из вершины стека. Результат операции помещается в стек.
                {
                    var exp2 = stack.Pop();
                    var exp1 = stack.Pop();
                    stack.Push(operators[tokens[i]](exp1, exp2));
                }
                else if (IsFunction(tokens[i])) // Если токен - унарная функция, то соответствующая операция выполняется над одним значением, взятым из вершины стека. Результат операции помещается в стек. 
                  stack.Push(functions[tokens[i]](stack.Pop()));
                else // Если токен - операнд, то он помещается на вершину стека.
                {
                    if (double.TryParse(tokens[i], System.Globalization.NumberStyles.Any, ci, out double value)) // Если операнд - число
                        stack.Push(ExpressionHelper.CreateConstant<double>(value));
                    else if (IsConst(tokens[i])) // Если операнд - математическая константа
                        stack.Push(constants[tokens[i]]);
                    else if (parameters.ContainsKey(tokens[i])) // Если операнд - сохраненный параметр
                        stack.Push(parameters[tokens[i]]);
                    else // Если операнд - несохраненный параметр
                    {
                        parameters.Add(tokens[i], ExpressionHelper.CreateParameter<double>(tokens[i]));
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
        /// <param name="paramsName">Имена параметров, которые в заданной последовательности должен принимать делегат.</param>
        /// <returns>Делегат, соответствующий арифемтическому выражению.</returns>
        public Delegate CompileString(string infixExpression, params string[] paramsName)
        {
            this.Clear();

            if (paramsName != null) foreach (string param in paramsName)
                    parameters.Add(param.ToLower(), ExpressionHelper.CreateParameter<double>(param.ToLower()));

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
        public Delegate CompileString(string[] infixTokens, params string[] paramsName)
        {
            this.Clear();

            if (paramsName != null) foreach (string param in paramsName)
                    parameters.Add(param.ToLower(), ExpressionHelper.CreateParameter<double>(param.ToLower()));

            string[] postfixTokens = ConvertToRPN(infixTokens);
            Expression resExpression = BuildExpression(postfixTokens);
            ResultDelegate = Expression.Lambda(resExpression, parameters.Values).Compile();
            return ResultDelegate;
        }

        /// <summary>
        /// Удаляет все результаты расчетов из экземпляра класса.
        /// </summary>
        public void Clear()
        {
            parameters.Clear();
            ResultDelegate = null;
        }
    }
}
