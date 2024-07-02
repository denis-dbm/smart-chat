using LLama;
using LLama.Common;

const string modelPath = "/Users/dbm/Downloads/Phi-3-mini-4k-instruct-fp16.gguf";

HashSet<string> nonPrintableWords = ["<|assistant|>", "<|end|>"];

var parameters = new ModelParams(modelPath)
{
    ContextSize = 1024
};

using var model = await LLamaWeights.LoadFromFileAsync(parameters);
using var context = model.CreateContext(parameters);
var executor = new InteractiveExecutor(context);

ChatSession session = new(executor);

InferenceParams inferenceParams = new()
{
    AntiPrompts = ["<|end|>"]
};

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("The chat session has started. Ask me something:");
Console.ForegroundColor = ConsoleColor.Green;
string userInput = Console.ReadLine() ?? string.Empty;

while (userInput != "exit")
{
    Console.ForegroundColor = ConsoleColor.White;

    var answer = session.ChatAsync(new ChatHistory.Message(AuthorRole.User, userInput), inferenceParams);

    await foreach (var token in answer)
        if (!nonPrintableWords.Contains(token))
            Console.Write(token);

    Console.WriteLine();
    Console.WriteLine("---");

    Console.ForegroundColor = ConsoleColor.Green;
    userInput = Console.ReadLine() ?? string.Empty;
}