using MyNewLogger;

List<ILogger> loggers = new List<ILogger>() { new ConsoleLogger(), new FileLogger("log.txt") };
CompositeLogger compositeLogger = new CompositeLogger(loggers);
Calculator calculator = new Calculator(compositeLogger);
var operations = new Dictionary<string, Func<double, double, double>>
        {
            {"+", calculator.Add},
            {"-", calculator.Subtract},
            {"*", calculator.Multiply },
            {"/", calculator.Divide },
            {"%", calculator.DivideModulo },
            {"^", calculator.RaiseToPower }
        };
UI.GetHelpInfo();
bool isOpen = true;

while (isOpen)
{
    try
    {
        string userInput = Console.ReadLine().ToLower().Trim();
        calculator.SelectOperation(operations, userInput, ref isOpen);
    }
    catch
    {

    }
}

class Calculator
{
    public ILogger Logger { get; private set; }
    public Calculator(ILogger logger)
    {
        Logger = logger;
    }
    public double Add(double addend1, double addend2)
    {
        double result = addend1 + addend2;
        //Logger.LogInformation($"{addend1} + {addend2} = {result}"); //оставил на всякий случай, если вариант с логированием в одном месте окажется хуйней
        return result;
    }
    public double Subtract(double minuend, double subtrahend)
    {
        double result = minuend - subtrahend;
        //Logger.LogInformation($"{minuend} - {subtrahend} = {result}");
        return result;
    }
    public double Divide(double dividend, double divider)
    {
        if (divider == 0)
            throw new DivideByZeroException();

        double result = dividend / divider;
        //Logger.LogInformation($"{dividend} / {divider} = {result}");
        return result;

    }
    public double Multiply(double multiplier, double multiplier2)
    {
        double result = multiplier * multiplier2;
        //Logger.LogInformation($"{multiplier} * {multiplier2} = {result}"); 
        return result;
    }
    public double DivideModulo(double dividend, double divider)
    {
        if (divider == 0)
            throw new DivideByZeroException();

        double result = dividend % divider;
        //Logger.LogInformation($"{dividend} % {divider} = {result}");
        return result;
    }
    public double RaiseToPower(double baseOfThePower, double power)
    {
        double result = Math.Pow(baseOfThePower, power);
        //Logger.LogInformation($"{baseOfThePower} ^ {power} = {result}");
        return result;
    }
}

static class UI
{
    public static void SelectOperation(this Calculator calculator, Dictionary<string, Func<double, double, double>> operations, string userInput, ref bool isOpen)
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
        }
        else if (IsInputValid(splittedInput))
        {
            var operand1 = double.Parse(splittedInput[0]);
            var op = splittedInput[1];
            var operand2 = double.Parse(splittedInput[2]);
            try
            {
                var result = operations[op](operand1, operand2);
                calculator.Logger.LogInformation($"{operand1} {op} {operand2} = {result}");
            }
            catch (Exception exception)
            {
                calculator.Logger.LogError(exception, $"{operand1} {op} {operand2} = Error");
                throw;
            }
        }
        else Console.WriteLine("Invalid input.");
    }
    public static bool IsInputValid(string[] splittedInput)
    {
        return splittedInput.Length == 3;
    }
    public static void GetHelpInfo() => Console.WriteLine("Enter the operation in the following format: \"operator1 operand operator2\". Available operators: + - * / % ^" +
    "\nType \"help\" to show info and \"exit\" to terminate the programm.");
}