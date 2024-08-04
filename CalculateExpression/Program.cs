using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RegexpTwoOperand
{
	internal class Program
	{
		static bool running = true;
		static void Main(string[] args)
		{

			//string input = "3+4*25/(5*40)-36";

			Console.WriteLine("Введите выражение типа: 3+(6-4)^2*458-6%2 Для выхода нажмите введите \"esc\" ");
			while (running)
			{
				Console.Write("Введите выражение: ");
				string input = Console.ReadLine();
				if (input == "esc") break;
				if (input == null) continue; // если строка пуста, запрашиваем ввод снова

				while (true)
				{
					// поиск выражения в скобках
					Match match = Regex.Match(input, @"\(([^()]+)\)");
					if (!match.Success)
					{
						break;
					}

					// обработка найденного подвыражения
					string innerExpression = match.Groups[1].Value;
					string result = ExecuteOperation(innerExpression);
					input = input.Replace(match.Value, result);
				}

				// обработка выражения без скобок
				string finalResult = ExecuteOperation(input);
				Console.WriteLine($"Результат: {finalResult}");
			}
			Console.WriteLine("Конец программы");
		}
		static string ExecuteOperation(string expression)
		{
			// oбработка приоритетных операций с учетом отрицательных чисел
			string[] patterns =
				{
					@"(-?\d+(\.\d+)?)([\^])(-?\d+(\.\d+)?)",    // group[0]- всё выражение целиком, group[1]- (-?\d+(\.\d+)?) - целое и десятичное число
																// group[2] - (\.\d+)? - десятичное число, group[3] - ([\^]) - оператор
																// group[4] - (-?\d+(\.\d+)?) - правое число целиком, group[5] - (\.\d+)? - десятичное число правого числа
					@"(-?\d+(\.\d+)?)([\*/%])(-?\d+(\.\d+)?)",
					@"((?<=^|[\^*/%+])\s*-?\d+(\.\d+)?)([\+-])(-?\d+(\.\d+)?)"
				};
			//	(-?\d + (\.\d +)?) -Этот фрагмент предназначен для поиска чисел, которые могут быть целыми или с плавающей точкой,
			//						и могут иметь необязательный знак минус.
			//	-?                 -Необязательный знак минус.Знак? означает, что предыдущий символ(-) может присутствовать или отсутствовать.
			//	\d +			   -Одна или более цифр. \d соответствует любой цифре, а + означает "один или более".
			//  (\.\d +) ?         -Необязательная дробная часть числа.
			//  \.                 -Экранированный символ точки, который соответствует самой точке.
			//	\d +			   -Одна или более цифр после точки.
			//	?				   -Знак вопроса означает, что вся группа(\.\d +) может присутствовать или отсутствовать.

			foreach (var item in patterns)
			{
				Match match = Regex.Match(expression, item);

				while (match.Success)
				{
					string leftOperand = match.Groups[1].Value;
					string operatorSymbol = match.Groups[3].Value;
					string rightOperand = match.Groups[4].Value;

					// Выполнение операции
					double left = Convert.ToDouble(leftOperand);
					double right = Convert.ToDouble(rightOperand);
					double result = 0;

					switch (operatorSymbol)
					{
						case "^":
							result = Math.Pow(left, right);
							break;
						case "*":
							result = left * right;
							break;
						case "/":
							result = left / right;
							break;
						case "%":
							result = left % right;
							break;
						case "+":
							result = left + right;
							break;
						case "-":
							result = left - right;
							break;
					}
					// замена подвыражения на результат
					expression = expression.Replace(match.Value, result.ToString());
					match = Regex.Match(expression, item);
				}
			}
			return expression;
		}
	}
}
