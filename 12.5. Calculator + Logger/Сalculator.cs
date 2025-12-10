using MyNewLogger;

ConsoleLogger consoleLogger = new ConsoleLogger();
FileLogger fileLogger = new FileLogger();
CompositeLogger compositeLogger = new CompositeLogger(consoleLogger, fileLogger);
Calculator calculator = new Calculator(compositeLogger);

calculator.GetHelpInfo();
bool isOpen = true;

while (isOpen)
{
    try 
    {
        string userInput = Console.ReadLine().ToLower().Trim();
        calculator.CommandChecker(userInput, ref isOpen);

        string[] userSplittedInput = calculator.SplitUserInput(userInput);
        if (calculator.OperationValidator(userSplittedInput) == true)
        {
            var operand1 = calculator.GetOperand1(userSplittedInput);
            var op = calculator.GetOperator(userSplittedInput);
            var operand2 = calculator.GetOperand2(userSplittedInput);
            Dictionary<string, Func<double, double, double>> operation = new Dictionary<string, Func<double, double, double>>
        {
            {"+", calculator.Add},
            {"-", calculator.Subtract},
            {"*", calculator.Multiply },
            {"/", calculator.Divide },
            {"%", calculator.DivideModulo },
            {"^", calculator.RaiseToPower }
        };
            Console.WriteLine(operation[op](operand1, operand2));
        }
        else if (userInput != "help" && userInput != "exit") Console.WriteLine("Invalid input.");
    }
    catch
    {

    }
}

class Calculator
{
    ILogger _logger;
    public Calculator(ILogger logger)
    {
        _logger = logger;
    }
    public double Add(double addend1, double addend2)
    {
        double result = addend1 + addend2;
        _logger.LogInformation($"{addend1} + {addend2} = {result}");
        return result;
    }
    public double Subtract(double minuend, double subtrahend)
    {
        double result = minuend - subtrahend;
        _logger.LogInformation($"{minuend} - {subtrahend} = {result}");
        return result;
    }
    public double Divide(double dividend, double divider)
    {
        try
        {
            if (divider == 0)
                throw new DivideByZeroException();

            double result = dividend / divider;
            _logger.LogInformation($"{dividend} / {divider} = {result}");
            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{dividend} / {divider} = Error");
            throw;
        }

    }
    public double Multiply(double multiplier, double multiplier2)
    {
        double result = multiplier * multiplier2;
        _logger.LogInformation($"{multiplier} * {multiplier2} = {result}");
        return result;
    }
    public double DivideModulo(double dividend, double divider)
    {
        try
        {
            if (divider == 0)
            throw new DivideByZeroException();

            double result = dividend % divider;
            _logger.LogInformation($"{dividend} % {divider} = {result}");
            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{dividend} % {divider} = Error");
            throw;
        }
    }
    public double RaiseToPower(double baseOfThePower, double power)
    {
        double result = Math.Pow(baseOfThePower, power);
        _logger.LogInformation($"{baseOfThePower} ^ {power} = {result}");
        return result;
    }
    public void GetHelpInfo() => Console.WriteLine("Enter the operation in the following format: \"operator1 operand operator2\". Available operators: + - * / % ^" +
        "\nType \"help\" to show info and \"exit\" to terminate the programm.");
    public string[] SplitUserInput(string input) => input.Split();
    public bool OperationValidator(string[] userSplittedInput)
    {
        if (userSplittedInput.Length == 3) return true;
        else return false;
    }
    public double GetOperand1(string[] userSplittedInput) => double.Parse(userSplittedInput[0]);
    public string GetOperator(string[] userSplittedInput) => userSplittedInput[1];
    public double GetOperand2(string[] userSplittedInput) => double.Parse(userSplittedInput[2]);
    public void CommandChecker(string userInput, ref bool isOpen)
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
}