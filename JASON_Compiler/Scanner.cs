using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    // Reserved Keywords
    Int, Float, String, Read, Write, Repeat, Until, If, ElseIf, Else, Then, Return, Endl,

    // Operators
    PlusOp, MinusOp, MultiplyOp, DivideOp, LessThanOp, GreaterThanOp, EqualOp, NotEqualOp, AndOp, OrOp,
    AssignmentOp,

    // Punctuation
    LParanthesis, RParanthesis, LCurly, RCurly, Semicolon, Comma,

    // Other Tokens
    Identifier, Number, StringLiteral
}

namespace TINY_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            // Fill Reserved Words Dictionary
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);

            // Fill Operators Dictionary
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add(":=", Token_Class.AssignmentOp);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LCurly);
            Operators.Add("}", Token_Class.RCurly);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
        }

        public void StartScanning(string SourceCode)
        {
            int i = 0;
            while (i < SourceCode.Length)
            {
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = "";

                if (char.IsWhiteSpace(CurrentChar))
                {
                    i++;
                    continue; 
                }

                // Handle Identifiers and Reserved Words
                if (char.IsLetter(CurrentChar)) 
                {
                    while (i < SourceCode.Length && char.IsLetterOrDigit(SourceCode[i]))
                    {
                        CurrentLexeme += SourceCode[i];
                        i++;
                    }
                    FindTokenClass(CurrentLexeme);
                    continue; 
                }
                // Handle Numbers (Integers and Floats)
                else if (char.IsDigit(CurrentChar))
                {
                    while (i < SourceCode.Length && char.IsDigit(SourceCode[i]))
                    {
                        CurrentLexeme += SourceCode[i];
                        i++;
                    }
                    // Handle floats
                    if (i < SourceCode.Length && SourceCode[i] == '.')
                    {
                        CurrentLexeme += SourceCode[i];
                        i++;
                        while (i < SourceCode.Length && char.IsDigit(SourceCode[i]))
                        {
                            CurrentLexeme += SourceCode[i];
                            i++;
                        }
                    }
                    Tokens.Add(new Token { lex = CurrentLexeme, token_type = Token_Class.Number });
                    continue; 
                }
                // Handle String Literals
                else if (CurrentChar == '"')
                {
                    CurrentLexeme += CurrentChar; 
                    i++;
                    while (i < SourceCode.Length && SourceCode[i] != '"')
                    {
                        CurrentLexeme += SourceCode[i];
                        i++;
                    }
                    if (i < SourceCode.Length) 
                    {
                        CurrentLexeme += SourceCode[i]; // Add the closing quote
                        i++;
                    }
                    Tokens.Add(new Token { lex = CurrentLexeme, token_type = Token_Class.StringLiteral });
                    continue; 
                }
                // Handle Comments 
                else if (CurrentChar == '/' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '*')
                {
                    i += 2;
                    while (i + 1 < SourceCode.Length && !(SourceCode[i] == '*' && SourceCode[i + 1] == '/'))
                    {
                        i++;
                    }
                    if (i + 1 < SourceCode.Length)
                    {
                        i += 2; 
                    }
                    continue; 
                }
                // Handle Operators and Punctuation
                else
                {
                    // Check for 2-character operators first
                    if (i + 1 < SourceCode.Length)
                    {
                        string twoCharOp = SourceCode.Substring(i, 2);
                        if (Operators.ContainsKey(twoCharOp))
                        {
                            FindTokenClass(twoCharOp);
                            i += 2;
                            continue;
                        }
                    }

                    // Check for 1-character operators
                    string oneCharOp = CurrentChar.ToString();
                    if (Operators.ContainsKey(oneCharOp))
                    {
                        FindTokenClass(oneCharOp);
                        i++;
                        continue;
                    }

                    // Handle unrecognized characters if necessary
                    i++;
                }
            }
            TINY_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token Tok = new Token();
            Tok.lex = Lex;

            // Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
            }
            // Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
            }
            // Is it an identifier?
            else
            {
                Tok.token_type = Token_Class.Identifier;
            }
            Tokens.Add(Tok);
        }
    }
}