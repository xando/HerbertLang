
using HerbertLang;

public class Herbert {
    static int Main(string[] args) {
        if (args.Length == 0) {
            Console.Error.WriteLine("Please provide a file path as an argument.");
            return -1;
        }

        string filePath = args[0];

        if (!File.Exists(filePath)) {
            Console.Error.WriteLine($"The file '{filePath}' does not exist.");
            return -1;
        }

        var inputCode = File.ReadAllText(filePath);
        var outputCode = Interpreter.eval(inputCode);

        Console.WriteLine(outputCode.code);
        Console.Error.WriteLine(outputCode.error);

        if (outputCode.error != "") {
            return -1;
        }
        return 0;
    }
}