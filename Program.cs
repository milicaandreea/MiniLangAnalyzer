using System;
using System.IO;
using System.Collections.Generic;
using Antlr4.Runtime;

namespace project1
{
    internal class Program
    {
        static void Main()
        {
            string filePath = @"C:\Users\DELL\OneDrive\Desktop\tema2\project1\input.txt";


            try
            {
                // Citeste continutul fisierului
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Fisierul '{filePath}' nu a fost gasit.");
                    return;
                }

                string code = File.ReadAllText(filePath);
                Console.WriteLine("Continutul fisierului:");
                Console.WriteLine(code);
                Console.WriteLine($"Lungimea codului citit: {code.Length}");

                // Creeaza lexer si parser
                var parser = CreateParser(code);

                var programTree = parser.program();

                Console.WriteLine("\nArborele generat:");
                Console.WriteLine(programTree.ToStringTree(parser));

                if (programTree == null || programTree.ChildCount == 0)
                {
                    Console.WriteLine("\nArborele generat este gol.");
                    return;
                }

                //Console.WriteLine("\nArborele generat:");
                //Console.WriteLine(programTree.ToStringTree(parser));

                // Proceseaza funcțiile
                var functions = ExtractFunctions(programTree);
                SaveToFile("functions.txt", functions, "Functiile");

                // Proceseaza variabilele globale
                var globalVariables = ExtractGlobalVariables(programTree);
                SaveToFile("global_variables.txt", globalVariables, "Variabilele globale");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A apărut o eroare: {ex.Message}");
            }
        }

        static MiniLangParser CreateParser(string code)
        {
            AntlrInputStream inputStream = new AntlrInputStream(code);
            MiniLangLexer lexer = new MiniLangLexer(inputStream);

            // Debugging: afișează tokenii generați
            //Console.WriteLine("\nTokeni generați de lexer:");
            //foreach (var token in lexer.GetAllTokens())
            //{
            //    Console.WriteLine($"<Token: {token.Type}, Text: '{token.Text}', Linie: {token.Line}, Coloană: {token.Column}>");
            //}

            lexer.Reset(); // Resetează lexer-ul după ce am consumat tokenii pentru debugging
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);

            MiniLangParser parser = new MiniLangParser(tokenStream);
            parser.RemoveErrorListeners(); // Înlătură ascultătorii standard de erori
            //parser.AddErrorListener(new DiagnosticErrorListener()); // Adaugă debugging pentru parser
            //parser.AddErrorListener(new ConsoleErrorListener<IToken>()); // Afișează erorile în consolă

            parser.Trace = false; // Activează trasarea regulilor

            return parser;
        }


        static List<string> ExtractFunctions(MiniLangParser.ProgramContext programTree)
        {
            var functions = new List<string>();

            // Debugging: Afișează informații despre fiecare copil al arborelui
            //foreach (var child in programTree.children)
            //{
            //    Console.WriteLine($"Tip nod: {child.GetType()}, Text nod: {child.GetText()}");
            //}



            foreach (var child in programTree.children)
            {
                if (child is MiniLangParser.FunctionDeclContext functionDecl)
                {
                    string functionName = functionDecl.GetChild(1).GetText(); // Numele funcției
                    string returnType = functionDecl.GetChild(0).GetText(); // Tipul returnat
                    bool isRecursive = functionDecl.block()?.GetText().Contains(functionName) ?? false;
                    string functionType = isRecursive ? "recursivă" : "iterativă";

                    var controlStructures = new List<string>();
                    if (functionDecl.block() != null)
                    {
                        foreach (var statement in functionDecl.block().statement())
                        {
                            for (int i = 0; i < statement.ChildCount; i++)
                            {
                                var statementChild = statement.GetChild(i);
                                if (statementChild is MiniLangParser.IfStatementContext ifStatement)
                                {
                                    string structureInfo = ifStatement.ELSE() != null
                                        ? $"if...else, linia {ifStatement.Start.Line}"
                                        : $"if, linia {ifStatement.Start.Line}";
                                    controlStructures.Add(structureInfo);
                                }
                                else if (statementChild.GetText().StartsWith("for"))
                                {
                                    controlStructures.Add($"for, linia {statement.Start.Line}");
                                }
                                else if (statementChild.GetText().StartsWith("while"))
                                {
                                    controlStructures.Add($"while, linia {statement.Start.Line}");
                                }
                            }
                        }
                    }

                    string parameters = "Fără parametri";
                    if (functionDecl.paramList() != null)
                    {
                        parameters = string.Join(", ", functionDecl.paramList()
                            .param()
                            .Select(param => $"{param.GetChild(0).GetText()} {param.GetChild(1).GetText()}"));
                    }

                    var localVariables = new List<string>();
                    if (functionDecl.block() != null)
                    {
                        foreach (var statement in functionDecl.block().statement())
                        {
                            if (statement is MiniLangParser.StatementContext statementContext
                                && statementContext.children[0] is MiniLangParser.VarDeclContext varDecl)
                            {
                                string localType = varDecl.GetChild(0).GetText(); // Tipul variabilei
                                string localName = varDecl.GetChild(1).GetText(); // Numele variabilei
                                string localValue = varDecl.expression()?.GetText() ?? "null"; // Valoarea sau null
                                localVariables.Add($"{localType} {localName} = {localValue}");
                            }
                        }
                    }

                    // Las functionInfo exact așa cum era scris de tine
                    var functionInfo = $"Funcție: {functionName}\n" +
                                       $"Tip: {functionType}\n" +
                                       $"Main: {(functionName == "main" ? "da" : "nu")}\n" +
                                       $"Tip returnare: {returnType}\n" +
                                       $"Parametri: {parameters}\n" +
                                       $"Variabile locale: {string.Join(", ", localVariables)}\n" +
                                       $"Structuri de control: {string.Join("; ", controlStructures)}\n";

                    functions.Add(functionInfo);
                }
            }
            return functions;
        }

        static List<string> ExtractGlobalVariables(MiniLangParser.ProgramContext programTree)
        {
            var globalVariables = new List<string>();
            foreach (var node in programTree.children)
            {
                if (node is MiniLangParser.VarDeclContext varDecl)
                {
                    string type = varDecl.GetChild(0).GetText();
                    string name = varDecl.ID().GetText();
                    string value = varDecl.expression()?.GetText() ?? "null";
                    globalVariables.Add($"<Tip: {type}, Nume: {name}, Valoare: {value}>");
                }
            }
            return globalVariables;
        }

        static void SaveToFile(string filePath, List<string> content, string itemType)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.NewLine = "\r\n";
                foreach (var item in content)
                {
                    writer.WriteLine(item.TrimEnd());
                    writer.WriteLine("----------------------");
                }
            }
            Console.WriteLine($"\n{itemType} au fost salvate in fisierul '{filePath}'.");
        }
    }
}
