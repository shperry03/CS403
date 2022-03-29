using System;
using System.IO;
using System.Collections.Generic;



namespace project2_tool
{
    public class GenerateAst
    {
        public static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(64);
            }

            string outputDir = args[0];

            var exprList = new List<string>();

            exprList.Add("Assign   : Token name, Expr value");
            exprList.Add("Binary   : Expr left, Token operator, Expr right");
            exprList.Add("Grouping : Expr expression");
            exprList.Add("Literal  : Object value");
            exprList.Add("Unary    : Token operator, Expr right");
            exprList.Add("Variable : Token name");

            var newStmtList = new List<string>();

            newStmtList.Add("Block      : List<Stmt> statements");
            newStmtList.Add("Expression : Expr expression");
            newStmtList.Add("Print      : Expr expression");
            newStmtList.Add("Var        : Token name, Expr initializer");

            defineAst(outputDir, "Stmt", newStmtList);
        }

        private static void defineAst(string OutputDir, string BaseName, List<string> types)
        {
            string Path = OutputDir + "/" + BaseName +".cs";
            
            var Code = new List<string>();

            Code.Add("namespace project2");
            Code.Add("{");
            Code.Add("    public abstract class" + BaseName);
            Code.Add("{");
            
            defineVisitor(Code, BaseName, types);

            foreach (var type in types){
                string[] typeParts = type.Split(':');
                string className = typeParts[0].Trim();
                string fields = typeParts[1].Trim();

                defineType(Code, BaseName, className, fields);
            }

            Code.Add("");
            Code.Add("    abstract <T> T accept(Visitor<T> visitor);");

            Code.Add("}");

            File.WriteAllLines(Path,Code);
        }

        private static void defineType(List<string> code, string BaseName, string className, string fields)
        {
            code.Add("");
            code.Add("       static class " + className + ": " + BaseName);
            code.Add("        {");
            code.Add("            " + className + "(" + fields + ") {");
            
            string[] fieldsList = fields.Split(',');
            foreach(string field in fieldsList)
            {
                string name = field.Split(' ')[1];
                code.Add("                this." + name + " = " + name + ";");
            }
            code.Add("            }");

            code.Add("");
            foreach(string field in fieldsList)
            {
                code.Add("          final "+ field +";");
            }

            code.Add("            public override T Accept<T>(Visitor<T> visitor)");
            code.Add("            {");
            code.Add("                return visitor.visit" + className + BaseName + "(this);");
            code.Add("            }");
            code.Add("        }");
        }

        private static void defineVisitor(List<string> code, string BaseName, List<string> types)
        {
            code.Add("        public interface Visitor<T>");
            code.Add("        {");

            foreach(string type in types)
            {
                string typeName = type.Split(':')[0].Trim();
                code.Add("            T visit" + typeName + BaseName + "(" + typeName + " " + BaseName.ToLower() + ");");
            }

            code.Add("        }");
        }
    }
}