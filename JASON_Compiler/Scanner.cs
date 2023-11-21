using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public enum Token_Class
{
    //Begin, Call, Declare,End,Do, EndIf,EndUntil, EndWhile,Integer,Parameters, Procedure, Program,Real, Set,While,Dot,
    ELSE,  If, INT,
     Read,  Then, Until,  Write,
     Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant, Comment, String
}
namespace JASON_Compiler
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
            ReservedWords.Add("IF", Token_Class.If);
            //ReservedWords.Add("BEGIN", Token_Class.Begin);
            //ReservedWords.Add("CALL", Token_Class.Call);
            //ReservedWords.Add("DECLARE", Token_Class.Declare);
            //ReservedWords.Add("END", Token_Class.End);
            //ReservedWords.Add("DO", Token_Class.Do);
            //ReservedWords.Add("ELSE", Token_Class.Else);
            //ReservedWords.Add("ENDIF", Token_Class.EndIf);
            //ReservedWords.Add("ENDUNTIL", Token_Class.EndUntil);
            //ReservedWords.Add("ENDWHILE", Token_Class.EndWhile);
            //ReservedWords.Add("INTEGER", Token_Class.Integer);
            //ReservedWords.Add("PARAMETERS", Token_Class.Parameters);
            //ReservedWords.Add("PROCEDURE", Token_Class.Procedure);
            //ReservedWords.Add("PROGRAM", Token_Class.Program);
            //ReservedWords.Add("READ", Token_Class.Read);
            //ReservedWords.Add("REAL", Token_Class.Real);
            //ReservedWords.Add("SET", Token_Class.Set);
            ReservedWords.Add("THEN", Token_Class.Then);
            ReservedWords.Add("UNTIL", Token_Class.Until);
            //ReservedWords.Add("WHILE", Token_Class.While);
            ReservedWords.Add("WRITE", Token_Class.Write);

            //Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("!", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);



        }

    public void StartScanning(string SourceCode)
        {
            for(int i=0; i<SourceCode.Length;i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') //if you read a character
                {
                    j++;
                    while(j < SourceCode.Length && ((SourceCode[j] >= 'A' && SourceCode[j] <= 'z') || (SourceCode[j] >= '0' && SourceCode[j] <= '9')))
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }

                else if(CurrentChar >= '0' && CurrentChar <= '9')
                {
                    j++;
                    while (j < SourceCode.Length && ( SourceCode[j] == '.' || (SourceCode[j] >= '0' && SourceCode[j] <= '9')))
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                    }
                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }

                if (CurrentChar == '/')
                {
                    j++;

                    if (j < SourceCode.Length && SourceCode[j] == '*')
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                        while (j < SourceCode.Length && !(SourceCode[j] == '*' && SourceCode[j + 1] == '/'))
                        {
                            CurrentLexeme += SourceCode[j].ToString();
                            j++;
                        }
                        CurrentLexeme += SourceCode[j].ToString() + SourceCode[j + 1].ToString();
                        // Process the comment token here using CurrentLexeme
                        // FindTokenClass(CurrentLexeme);
                        j += 2;
                        i = j - 1;
                    }
                    FindTokenClass(CurrentLexeme);
                }
                //else if(CurrentChar == '/')
                //{
                //    j++;

                //    while ((j-1) < SourceCode.Length && ( SourceCode[j] == '*' && SourceCode[j+1] != '/' ) )
                //    {
                //        CurrentLexeme += SourceCode[j].ToString();
                //        j++;
                //    }
                //    CurrentLexeme += SourceCode[j].ToString();
                //    //CurrentLexeme += SourceCode[j+1].ToString();
                //    FindTokenClass(CurrentLexeme);
                //    j++;
                //    i = j - 1;
                //}
                else if (CurrentChar == '"')
                {
                    j++;
                    while (j < SourceCode.Length && SourceCode[j] != '"')
                    {
                        CurrentLexeme += SourceCode[j].ToString();
                        j++;
                    }
                    CurrentLexeme += SourceCode[j].ToString();
                    FindTokenClass(CurrentLexeme);
                    j++;
                    i = j - 1;
                }

                else
                {
                    FindTokenClass(CurrentLexeme);
                }
            }
            
            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
           // Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?

            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }

            //Is it a Constant?

            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Constant;
                Tokens.Add(Tok);
            }

            //Is it a Constant?

            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }

            //Is it an operator?

            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }

            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }

            //Is it an undefined?
            else
                Errors.Error_List.Add(Lex);

        }

        bool isIdentifier(string lex)
        {
            // Check if the lex is an identifier or not.
            Regex regex = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*");  
            
            return regex.IsMatch(lex);
        }
        bool isConstant(string lex)
        {
            // Check if the lex is a constant (Number) or not.
            Regex regex = new Regex(@"^\d+(\.\d+)?$");

            return regex.IsMatch(lex);
        }

        bool isComment(string lex)
        {
            // Check if the lex is a constant (Number) or not.
            //  Regex regex = new Regex(@"^/\*[^/]*\*/$");
            // Regex regex = new Regex(@"/\*(?:[^*]|\*(?!/))*\*/");
            Regex regex = new Regex(@"^/\*([\s\S]*?)\*/\s*$");

            return regex.IsMatch(lex);
        }

        bool isString(string lex)
        {
            // Check if the lex is a constant (Number) or not.
            Regex regex = new Regex(@"^\"".*\""$");
            return regex.IsMatch(lex);
        }

    }
}
