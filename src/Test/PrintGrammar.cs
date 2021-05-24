using System;
using Kusto.Language;
using Kusto.Language.Parsing;

namespace Test
{
    class PrintGrammar
    {
        static Kusto.Language.Symbols.TableSymbol table =  new Kusto.Language.Symbols.TableSymbol("T", 
            new Kusto.Language.Symbols.ColumnSymbol("a",Kusto.Language.Symbols.ScalarTypes.Real),
            new Kusto.Language.Symbols.ColumnSymbol("b",Kusto.Language.Symbols.ScalarTypes.Real));
        static Kusto.Language.Symbols.DatabaseSymbol database = new Kusto.Language.Symbols.DatabaseSymbol("db", table);
        static GlobalState globals = Kusto.Language.GlobalState.Default.WithDatabase(database);
    
        public static void Query()
        {
            Console.WriteLine(GrammarBuilder.BuildGrammar(QueryGrammar.From(globals).QueryBlock));
        }

        public static void Command()
        {
            Console.WriteLine(GrammarBuilder.BuildGrammar(CommandGrammar.From(globals).CommandBlock));
        }

        public static void Directive()
        {
            Console.WriteLine(GrammarBuilder.BuildGrammar(DirectiveGrammar.DirectiveBlock));
        }
    }
}