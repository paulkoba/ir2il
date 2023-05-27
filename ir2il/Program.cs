using Cecilifier.Runtime;
using ir2cil.Lexer;
using ir2cil.Parser;
using ir2cil.Parser.NodeTypes;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    public static void Main(string[] args)
    {
        string filename = "E:\\Coursework\\ir2il\\examples\\parser_test.ll";
        if (args.Length == 0)
        {
            Console.WriteLine("No valid file provided for compilation. Using " + filename + " as fallback.");
        }
        else
        {
            filename = args[0];
        }


        Lexer lexer = new Lexer();

        List<Token> tokens = lexer.LexFile(filename);

        Console.WriteLine("Finished tokenization. Resulting tokens: ");

        foreach(Token token in tokens) 
        {
            Console.WriteLine(token.ToString());
        }

        Parser parser = new Parser(tokens);
        Module module = parser.Parse();
        module.PopulateParents();

        Console.WriteLine("AST:\n" + module.ToString());

        module.SetBuildParameters("hello", "1.0.0", "example.exe");
        module.Codegen();
    }
}