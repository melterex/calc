
using System.Globalization;
public class Calculator : ICalculator
{
    public double ParseAndCompute(string expression)
    {
        var parts = ToParts(expression);
        var rpn = ToRpn(parts);
        return CalcRpn(rpn);
    }

    private int Priority(string op)
    {
        return op switch { "+" or "-" => 1, "*" or "/" => 2, _ => 0 };
    }
    
    private List<string> ToParts(string expr)
    {
        var parts = new List<string>();
        for (int i = 0; i < expr.Length; i++)
        {
            if (char.IsWhiteSpace(expr[i]))
            {
                continue;
            }
            if (char.IsDigit(expr[i]) || expr[i] == '.')
            {
                string number = "";
                for (;i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.');)
                {
                    number += expr[i++];
                }
                i--;
                parts.Add(number);
            }
            else if ("+-*/()".Contains(expr[i]))
            {
                parts.Add(expr[i].ToString());
            }
            else throw new FormatException($"недопустимый символ {expr[i]}");
        }
        return parts;
    }

    private List<string> ToRpn(List<string> parts)
    {
        var result = new List<string>();
        var operators = new Stack<string>();
        
        foreach (var part in parts)
        {
            if (double.TryParse(part, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                result.Add(part);
            else if (part == "(")
                operators.Push(part);
            else if (part == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    result.Add(operators.Pop());
                }
                if (operators.Count == 0)
                {
                    throw new FormatException("неправильная сп");
                }
                operators.Pop();
            }
            else
            {
                while (operators.Count > 0 && Priority(operators.Peek()) >= Priority(part))
                {
                    result.Add(operators.Pop());
                }
                operators.Push(part);
            }
        }

        while (operators.Count > 0)
        {
            var top = operators.Pop();
            if (top == "(" || top == ")")
            {
                throw new FormatException("неправильная сп");
            }
            result.Add(top);
        }

        return result;
    }

    private double CalcRpn(List<string> rpn)
    {
        var stack = new Stack<double>();

        foreach (var part in rpn)
        {
            if (double.TryParse(part, NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
            {
                stack.Push(num);
            }
            else
            {
                if (stack.Count < 2)
                {
                    throw new FormatException("неправильный ввод");
                }
                double b = stack.Pop();
                double a = stack.Pop();

                double res = part switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => b != 0 ? a / b : throw new DivideByZeroException("на ноль делить нельзя"),
                    _ => throw new InvalidOperationException()
                };
                stack.Push(res);
            }
        }

        if (stack.Count != 1)
        {
            throw new FormatException("неправильный ввод");
        }
        return stack.Pop();
    }
    
}

