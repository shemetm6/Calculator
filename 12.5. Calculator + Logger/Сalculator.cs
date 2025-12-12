using MyNewLogger;
using System;

List<ILogger> loggers = new List<ILogger>() { new ConsoleLogger(), new FileLogger("log.txt") };
CompositeLogger compositeLogger = new CompositeLogger(loggers);
var operations = new Dictionary<string, Func<double, double, double>>
        {
            {"+", Calculator.Add},
            {"-", Calculator.Subtract},
            {"*", Calculator.Multiply },
            {"/", Calculator.Divide },
            {"%", Calculator.DivideModulo },
            {"^", Calculator.RaiseToPower }
        };
UI.GetHelpInfo();
bool isOpen = true;

while (isOpen)
{
    try
    {
        string userInput = Console.ReadLine().ToLower().Trim();
        UI.HandleInput(compositeLogger, operations, userInput, ref isOpen);
    }
    catch
    {
        //Если здесь ловить исключение и выводить+записывать через композитный логгер, то сообщение исключения продублируется.
        //Получается надо убирать try catch из HandleInput.
        //Но HandleInput создает переменные оператора и операндов, к которым я хочу иметь доступ, чтобы в лог записалось выражение: {operand1} {op} {operand2} = Error
        //В противном случае в лог записывается только сам факт ошибки, а выражение нет
        //Вообщем хотелось бы перенести сюда всё, что в HandleInput в блоке catch, но как это сделать я хуй знает
    }
}

static class Calculator
{
    public static double Add(double addend1, double addend2)
    {
        return addend1 + addend2;
    }

    public static double Subtract(double minuend, double subtrahend)
    {
        return minuend - subtrahend;
    }

    public static double Divide(double dividend, double divider)
    {
        if (divider == 0)
            throw new DivideByZeroException();

        return dividend / divider;
    }
    public static double Multiply(double multiplier, double multiplier2)
    {
        return multiplier * multiplier2;
    }

    public static double DivideModulo(double dividend, double divider)
    {
        if (divider == 0)
            throw new DivideByZeroException();

        return dividend % divider;
    }
    public static double RaiseToPower(double baseOfThePower, double power)
    {
        return Math.Pow(baseOfThePower, power);
    }
}

static class UI
{
    public static void HandleInput(CompositeLogger compositeLogger, Dictionary<string, Func<double, double, double>> operations, string userInput, ref bool isOpen)
    {
        string[] splittedInput = userInput.Split();

        if (userInput == "help" || userInput == "exit")
        {
            switch (userInput)
            {
                case "exit":
                    isOpen = false;
                    break;
                case "help":
                    GetHelpInfo();
                    break;
            }
            return;
        }
        if (IsInputValid(splittedInput))
        {
            var operand1 = double.Parse(splittedInput[0]);
            var op = splittedInput[1];
            var operand2 = double.Parse(splittedInput[2]);
            try
            {
                var result = operations[op](operand1, operand2);
                compositeLogger.LogInformation($"{operand1} {op} {operand2} = {result}");
            }
            catch (Exception exception)
            {
                compositeLogger.LogError(exception, $"{operand1} {op} {operand2} = Error");
                throw;
            }
            return;
        }
        Console.WriteLine("Invalid input.");
    }
    public static bool IsInputValid(string[] splittedInput)
    {
        return splittedInput.Length == 3;
    }
    public static void GetHelpInfo()
    {
        Console.WriteLine("Enter the operation in the following format: \"operator1 operand operator2\". Available operators: + - * / % ^" +
    "\nType \"help\" to show info and \"exit\" to terminate the programm.");
    }
}