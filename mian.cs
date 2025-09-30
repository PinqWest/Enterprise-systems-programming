using System;

class Program
{
    static void Main()
    {
        double memory = 0; // Для операций с памятью

        while (true)
        {
            Console.WriteLine("Калькулятор");
            Console.WriteLine("Выберите операцию: + - * / % 1/x x^2 √ M+ M- MR Q(выход)");
            Console.Write("Операция: ");
            string op = Console.ReadLine();

            if (op == "Q" || op == "q") break; // Выход из программы

            if (op == "MR")
            {
                Console.WriteLine("Память: " + memory);
                continue;
            }

            Console.Write("Введите число: ");
            double x;
            if (!double.TryParse(Console.ReadLine(), out x))
            {
                Console.WriteLine("Ошибка! Введите число.");
                continue;
            }

            double y = 0;
            if (op == "+" || op == "-" || op == "*" || op == "/" || op == "%")
            {
                Console.Write("Введите второе число: ");
                if (!double.TryParse(Console.ReadLine(), out y))
                {
                    Console.WriteLine("Ошибка! Введите число.");
                    continue;
                }
            }

            switch (op)
            {
                case "+": Console.WriteLine("Результат: " + (x + y)); break;
                case "-": Console.WriteLine("Результат: " + (x - y)); break;
                case "*": Console.WriteLine("Результат: " + (x * y)); break;
                case "/":
                    if (y != 0)
                        Console.WriteLine("Результат: " + (x / y));
                    else
                        Console.WriteLine("Ошибка: Деление на ноль!");
                    break;
                case "%":
                    if (y != 0)
                        Console.WriteLine("Результат: " + (x % y));
                    else
                        Console.WriteLine("Ошибка: Деление на ноль!");
                    break;
                case "1/x":
                    if (x != 0)
                        Console.WriteLine("Результат: " + (1 / x));
                    else
                        Console.WriteLine("Ошибка: Деление на ноль!");
                    break;
                case "x^2": Console.WriteLine("Результат: " + (x * x)); break;
                case "√":
                    if (x >= 0)
                        Console.WriteLine("Результат: " + Math.Sqrt(x));
                    else
                        Console.WriteLine("Ошибка: Невозможно извлечь корень из отрицательного числа!");
                    break;
                case "M+": memory += x; Console.WriteLine("Память: " + memory); break;
                case "M-": memory -= x; Console.WriteLine("Память: " + memory); break;
                default: Console.WriteLine("Неизвестная операция!"); break;
            }

            Console.WriteLine();
        }
    }
}