using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tiny_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream = Tiny_Compiler.Tiny_Scanner.Tokens;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {

            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = Program();
            return root;
        }

        public Node Program()
        {
            // program -> function_statement main_function
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("program");

                node.Children.Add(function_statement());
                node.Children.Add(main_function());

                while (InputPointer < TokenStream.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected Nothing and " + TokenStream[InputPointer].token_type + " found\r\n");
                    InputPointer++;
                }

                return node;
            }
            return null;
        }
        private Node function_statement()
        {
            // function_statement -> function_declaration function_body function_statement | e
            if (InputPointer + 1 < TokenStream.Count)
            {
                if ((InputPointer < TokenStream.Count) && TokenStream[InputPointer + 1].token_type != Token_Class.Main)
                {
                    Node node = new Node("function_statement");

                    if (TokenStream[InputPointer].token_type == Token_Class.ReservedWordFLOAT ||
                        TokenStream[InputPointer].token_type == Token_Class.ReservedWordINT ||
                        TokenStream[InputPointer].token_type == Token_Class.ReservedWordSTRING)
                    {
                        node.Children.Add(function_declaration());
                        node.Children.Add(function_body());
                        node.Children.Add(function_statement());
                        return node;
                    }

                    // e
                    return null;
                }
                return null;
            }
            return null;
        }
        private Node function_declaration()
        {
            // function_declaration -> DataType Identifier "(" parameter_list ")"
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("function_declaration");

                node.Children.Add(DataType());
                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(match(Token_Class.LParanthesis));
                node.Children.Add(parameter_list());
                node.Children.Add(match(Token_Class.RParanthesis));
                return node;
            }
            return null;
        }

        private Node parameter_list()
        {
            // parameter_list -> DataType identifier parameter_list_dash | e
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("parameter_list");
                if (TokenStream[InputPointer].token_type == Token_Class.ReservedWordFLOAT ||
                    TokenStream[InputPointer].token_type == Token_Class.ReservedWordINT ||
                    TokenStream[InputPointer].token_type == Token_Class.ReservedWordSTRING)
                {
                    node.Children.Add(DataType());
                    node.Children.Add(match(Token_Class.Idenifier));
                    node.Children.Add(parameter_list_dash());
                    return node;
                }

                // e
                return null;
            }
            return null;
        }

        private Node parameter_list_dash()
        {
            // parameter_list_dash -> "," DataType identifier parameter_list_dash | e
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("parameter_list_dash");
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    node.Children.Add(match(Token_Class.Comma));
                    node.Children.Add(DataType());
                    node.Children.Add(match(Token_Class.Idenifier));
                    node.Children.Add(parameter_list_dash());
                    return node;
                }

                // e
                return null;
            }
            return null;
        }
        private Node function_body()
        {
            // function_body -> "{" statements return_statement "}"
            
            Node node = new Node("function_body");

            Node tempLCurlyNode = match(Token_Class.LCurlybracket);
            //if (tempLCurlyNode == null)
            //    return null;

            node.Children.Add(tempLCurlyNode);

            node.Children.Add(Statements());

            Node tempReturnNode = return_statement();
            //if (tempReturnNode == null)
            //{
            //    InputPointer--;
            //    //return node;
            //}
            node.Children.Add(tempReturnNode);

            Node tempRCurlyNode = match(Token_Class.RCurlybracket);
            //if (tempRCurlyNode == null)
            //    return node;

            node.Children.Add(tempRCurlyNode);

            return node;
            
        }
        private Node main_function()
        {
            // main_function -> DataType "main" "(" ")" function_body
           
            Node node = new Node("main_function");

            node.Children.Add(DataType());

            Node tempMainNode = match(Token_Class.Main);
            //if (tempMainNode == null)
            //    return null;
            node.Children.Add(tempMainNode);

            Node tempLParanthesisNode = match(Token_Class.LParanthesis);
            //if (tempLParanthesisNode == null)
            //    return node;
            node.Children.Add(tempLParanthesisNode);

            Node tempRParanthesisNode = match(Token_Class.RParanthesis);
            //if (tempRParanthesisNode == null)
            //    return node;
            node.Children.Add(tempRParanthesisNode);

            Node tempFunctionBodyNode = function_body();
            //if (tempFunctionBodyNode == null) 
            //    return node;
            node.Children.Add(tempFunctionBodyNode);


            return node;
            
        }

        private Node DataType()
        {
            // DataType -> int | float | string
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("DataType");
                if (TokenStream[InputPointer].token_type == Token_Class.ReservedWordFLOAT)
                {
                    node.Children.Add(match(Token_Class.ReservedWordFLOAT));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.ReservedWordINT)
                {
                    node.Children.Add(match(Token_Class.ReservedWordINT));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.ReservedWordSTRING)
                {
                    node.Children.Add(match(Token_Class.ReservedWordSTRING));
                }
                else // empty
                {
                    Errors.Error_List.Add("Parsing Error: Expected DataType and " + TokenStream[InputPointer].token_type + " found");
                    //InputPointer++;
                    return null;
                }
                return node;
            }
            return null;
        }

        private Node Statements()
        {
            // Statements -> statement statements | e
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("Statements");
                if (TokenStream[InputPointer].token_type == Token_Class.ReservedWordFLOAT ||
                TokenStream[InputPointer].token_type == Token_Class.ReservedWordINT ||
                TokenStream[InputPointer].token_type == Token_Class.ReservedWordSTRING ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                TokenStream[InputPointer].token_type == Token_Class.Write ||
                TokenStream[InputPointer].token_type == Token_Class.Read ||
                TokenStream[InputPointer].token_type == Token_Class.Repeat ||
                TokenStream[InputPointer].token_type == Token_Class.If ||
                TokenStream[InputPointer].token_type == Token_Class.Comment)
                {
                    node.Children.Add(Statement());
                    node.Children.Add(Statements());
                    return node;
                }

                // e
                return null;
            }
            return null;
        }

        private Node Statement()
        {
            // Statement -> declaration_stmt | assignment_stmt | write_stmt | read_stmt | repeat_stmt | if_stmt | functioncall_stmt
            if (InputPointer + 1 < TokenStream.Count)
            {
                Node node = new Node("Statement");
                if (TokenStream[InputPointer].token_type == Token_Class.ReservedWordFLOAT ||
                TokenStream[InputPointer].token_type == Token_Class.ReservedWordINT ||
                TokenStream[InputPointer].token_type == Token_Class.ReservedWordSTRING)
                {
                    node.Children.Add(declaration_statement());

                }
                else if (TokenStream[InputPointer + 1].token_type ==  Token_Class.LParanthesis)
                {
                    node.Children.Add(function_call_statement());
                }
                // assignment_statement
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    node.Children.Add(assignment_statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Write)
                {
                    node.Children.Add(write_statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Read)
                {
                    node.Children.Add(read_statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
                {
                    node.Children.Add(repeat_statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.If)
                {
                    node.Children.Add(if_statement());
                }

               

                return node;
            }
            return null;
        }

        private Node function_call_statement()
        {
            if(InputPointer >= TokenStream.Count) 
            {
                return null;
            }
            Node node = new Node("function_call_statement");
            node.Children.Add(function_call());
            node.Children.Add(match(Token_Class.Semicolon));
            return node;
        }

        private Node declaration_statement()
        {
            // Declaration_Statement->Datatype identifier declaration_statement_dash;
            // declaration_statement_dash -> "," identifer declaration_statement_dash | Decl_Assignment declaration_statement_dash | ε

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("declaration_statement");

                node.Children.Add(DataType());
                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(declaration_statement_dash());
                node.Children.Add(match(Token_Class.Semicolon));
                return node;
            }
            return null;
        }

        private Node declaration_statement_dash()
        {
            // declaration_statement_dash -> "," identifer declaration_statement_dash | ":=" expression_statement  declaration_statement_dash | ε
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("declaration_statement_dash");
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    node.Children.Add(match(Token_Class.Comma));
                    node.Children.Add(match(Token_Class.Idenifier));
                    node.Children.Add(declaration_statement_dash());
                    return node;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.assignmentOP)
                {
                    node.Children.Add(match(Token_Class.assignmentOP));
                    node.Children.Add(expression_statement());
                    node.Children.Add(declaration_statement_dash());
                    return node;
                }

                // e
                return null;
            }
            return null;
        }
        private Node assignment_statement()
        {
            // assignment_statement -> identifier ":=" expression_statement ";"
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("assignment_statement");

                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(match(Token_Class.assignmentOP));
                node.Children.Add(expression_statement());
                node.Children.Add(match(Token_Class.Semicolon));

                return node;
            }
            return null;
        }
        private Node write_statement()
        {
            // write_statement -> "write" output ";"
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("write_statement");
                
                node.Children.Add(match(Token_Class.Write));
                node.Children.Add(output());
                node.Children.Add(match(Token_Class.Semicolon));

                return node;
            }
            return null;
        }

        private Node output()
        {
            // output -> expression_statement | "endl"
            Node node = new Node("output");

            if (TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                node.Children.Add(match(Token_Class.Endl));
            }
            else //expr
            {
                Node tempExpressionNode = expression_statement();
                if (tempExpressionNode == null)
                    return null;

                node.Children.Add(tempExpressionNode);
            }

            return node;
        }

        private Node read_statement()
        {
            // read_statement -> "read" identifier ";"
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("read_statement");

                node.Children.Add(match(Token_Class.Read));
                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(match(Token_Class.Semicolon));

                return node;
            }
            return null;
        }
        private Node repeat_statement()
        {
            // repeat_statement -> "repeat" statements "until" condition_statement
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("repeat_statement");

                node.Children.Add(match(Token_Class.Repeat));
                node.Children.Add(Statements());
                node.Children.Add(match(Token_Class.Until));
                node.Children.Add(condition_statement());

                return node;
            }
            return null;
        }
        private Node if_statement()
        {
            // if_statement -> "if" condition_statement "then" statements else_if_statement else_statement "end"
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("if_statement");

                node.Children.Add(match(Token_Class.If));
                node.Children.Add(condition_statement());
                node.Children.Add(match(Token_Class.Then));
                node.Children.Add(Statements());
                node.Children.Add(else_if_statement());
                node.Children.Add(else_statement());
                node.Children.Add(match(Token_Class.End));

                return node;
            }
            return null;
        }
        private Node else_if_statement()
        {
            // else_if_statement -> "elseif" condition_statement "then" statements else_if_statement | e
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("else_if_statement");
                if (TokenStream[InputPointer].token_type == Token_Class.Elseif)
                {
                    node.Children.Add(match(Token_Class.Elseif));
                    node.Children.Add(condition_statement());
                    node.Children.Add(match(Token_Class.Then));
                    node.Children.Add(Statements());
                    node.Children.Add(else_if_statement());
                    return node;
                }

                // e
                return null;
            }
            return null;
        }

        private Node else_statement()
        {
            // else_statement -> "else" statements | e
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("else_statement");
                if (TokenStream[InputPointer].token_type == Token_Class.ELSE)
                {
                    node.Children.Add(match(Token_Class.ELSE));
                    node.Children.Add(Statements());
                    return node;
                }

                // e
                return null;
            }
            return null;
        }
        private Node return_statement()
        {
            // return_statement -> "return" expression_statement ";"
            
            Node node = new Node("return_statement");

            Node tempReturnNode = match(Token_Class.Return);
            //if (tempReturnNode == null)
            //    return null;

            node.Children.Add(tempReturnNode);

            node.Children.Add(expression_statement());

            Node tempSemiColonNode = match(Token_Class.Semicolon);
            //if (tempSemiColonNode == null)
            //    return node;
            node.Children.Add(tempSemiColonNode);

            return node;
            
        }


        private Node condition_statement()
        {
            // condition_statement -> condition boolean_expression
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("condition_statement");

                node.Children.Add(condition());
                node.Children.Add(boolean_expression());

                return node;
            }
            return null;
        }
        private Node condition()
        {
            // condition -> identifier condition_operator term
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("condition");
                
                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(condition_operator());
                node.Children.Add(term());

                return node;
            }
            return null;
        }

        private Node condition_operator()
        {
            // condition_pperator -> < | > | = | <>
            Node node = new Node("condition_operator");

            if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                node.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                node.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                node.Children.Add(match(Token_Class.EqualOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                node.Children.Add(match(Token_Class.NotEqualOp));
            }
            else // empty
            {
                Errors.Error_List.Add("Parsing Error: Expected ConditionOperator and "+ TokenStream[InputPointer].token_type + " found\r\n");
                //InputPointer++;
                return null;
            }

            return node;
        }

        private Node boolean_expression()
        {
            // boolean_expression -> "&&" condition_statement | "||" condition_statement | e
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("boolean_expression");
                if (TokenStream[InputPointer].token_type == Token_Class.ANDOp)
                {
                    node.Children.Add(match(Token_Class.ANDOp));
                    node.Children.Add(condition_statement());
                    return node;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.OROp)
                {
                    node.Children.Add(match(Token_Class.OROp));
                    node.Children.Add(condition_statement());
                    return node;
                }

                // e
                return null;
            }
            return null;
        }
        private Node expression_statement()
        {
            // expression_statement -> string | term | equation
            if (InputPointer + 1 < TokenStream.Count)
            {
                Node node = new Node("expression_statement");
                if (TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    node.Children.Add(match(Token_Class.String));
                }
                // equation -> bracket | non bracket | func_call
                else if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis ||
                    (TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis))
                {
                    node.Children.Add(equation());
                }
                // term -> number | identifier 
                else if (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float ||
                         TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    node.Children.Add(term());
                }
                else // empty
                {
                    Errors.Error_List.Add("Parsing Error: Expected ExpressionStatement and " + TokenStream[InputPointer].token_type + " found\r\n");
                    //InputPointer++;
                    return null;
                }

                return node;
            }
            return null;

        }

        private Node equation()
        {
            // Equation->bracket | non-bracket
            // non-bracket -> term equation_arithmetic_part
            // bracket -> "(" term equation_arithmetic_part ")" equation_arithmetic_part
            // equation_arithmetic_part -> Arithemtic_Operator Equation | ε 
            if (InputPointer + 1 < TokenStream.Count)
            {
                Node node = new Node("equation");
                if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    node.Children.Add(bracket_equation());
                }
                // term
                else if (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float ||
                         TokenStream[InputPointer].token_type == Token_Class.Idenifier ||
                         TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                {
                    node.Children.Add(non_bracket_equation());
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Equation and " + TokenStream[InputPointer].token_type + " found\r\n");
                    //InputPointer++;
                    return null;
                }
            
                return node;
            }
            return null;
        }
        private Node bracket_equation()
        {
            // bracket -> "(" term equation_arithmetic_part ")"
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("bracket_equation");
                if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    node.Children.Add(match(Token_Class.LParanthesis));
                    node.Children.Add(term());
                    node.Children.Add(equation_arithmetic_part());
                    node.Children.Add(match(Token_Class.RParanthesis));
                    node.Children.Add(equation_arithmetic_part());
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected " +  Token_Class.LParanthesis + " and " + TokenStream[InputPointer].token_type + " found\r\n");
                    //InputPointer++;
                    return null;
                }
                return node;
            }
            return null;
        }


        private Node non_bracket_equation()
        {
            // non-bracket -> term equation_arithmetic_part
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("non_bracket_equation");

                node.Children.Add(term());
                node.Children.Add(equation_arithmetic_part());

                return node;
            }
            return null;
        }
        private Node equation_arithmetic_part()
        {
            // equation_arithmetic_part -> Arithemtic_Operator Equation | ε 
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("equation_arithmetic_part");
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp ||
                   TokenStream[InputPointer].token_type == Token_Class.MinusOp ||
                   TokenStream[InputPointer].token_type == Token_Class.MultiplyOp ||
                   TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                    node.Children.Add(Arithmetic_Operator());
                    node.Children.Add(equation());
                    return node;
                }
                // e
                return null;
            }
            return null;
        }

        private Node Arithmetic_Operator()
        {
            if (InputPointer >= TokenStream.Count)
            {
                return null;
            }
            Node node = new Node("Arithemtic_Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                node.Children.Add(match(Token_Class.PlusOp));
                //return node;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                node.Children.Add(match(Token_Class.MinusOp));
                //return node;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                node.Children.Add(match(Token_Class.MultiplyOp));
                //return node;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                node.Children.Add(match(Token_Class.DivideOp));
               // return node;
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected  Arithemitc_Operator  and " + TokenStream[InputPointer].token_type + " found\r\n");
                //InputPointer++;
                return null;
            }
            return node;
        }

        private Node term()
        {
            // term -> number | identifier | function_call
            if (InputPointer + 1 < TokenStream.Count)
            {
                Node node = new Node("term");

                // number
                if (TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float)
                {
                    node.Children.Add(number());
                    return node;
                }

                // func_call
                else if (TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                {
                    node.Children.Add(function_call());
                    return node;
                }
                // identifier
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    node.Children.Add(match(Token_Class.Idenifier));
                    return node;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Term and " + TokenStream[InputPointer].token_type + " found\r\n");
                    //InputPointer++;
                    return null;
                }

            }
            return null;
        }

        private Node number()
        {
            // number -> int | float
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("number");

                if (TokenStream[InputPointer].token_type == Token_Class.Int)
                {
                    node.Children.Add(match(Token_Class.Int));
                    return node;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Float)
                {
                    node.Children.Add(match(Token_Class.Float));
                    return node;
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected Number and " + TokenStream[InputPointer].token_type + " found\r\n");
                    //InputPointer++;
                    return null;
                }

            }
            return null;
        }

        private Node function_call()
        {
            // function_call -> identifier "(" function_call_arguments ")"
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("function_call");

                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(match(Token_Class.LParanthesis));
                node.Children.Add(function_call_arguments());
                node.Children.Add(match(Token_Class.RParanthesis));

                return node;
            }
            return null;
        }

        private Node function_call_arguments()
        {
            // function_call_arguments -> identifier function_call_arguments_dash | e
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("function_call_arguments");


                if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    node.Children.Add(match(Token_Class.Idenifier));
                    node.Children.Add(function_call_arguments_dash());
                    return node;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Int)
                {
                    node.Children.Add(match(Token_Class.Int));
                    node.Children.Add(function_call_arguments_dash());
                    return node;
                }

                // e
                return null;
            }
            return null;
        }

        private Node function_call_arguments_dash()
        {
            // function_call_arguments_dash -> "," identifier function_call_arguments_dash | e
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("function_call_arguments_dash");

                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    node.Children.Add(match(Token_Class.Comma));

                    if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                        node.Children.Add(match(Token_Class.Idenifier));
                    else if (TokenStream[InputPointer].token_type == Token_Class.Int)
                        node.Children.Add(match(Token_Class.Int));
                    else
                    {
                        Errors.Error_List.Add("Parsing Error: Expected " + Token_Class.Idenifier + " and " + TokenStream[InputPointer].token_type + " found\r\n");
                        //InputPointer++;
                        return null;
                    }

                    node.Children.Add(function_call_arguments_dash());

                    return node;
                }

                // e
                return null;
            }
            return null;
        }







        // Implement your logic here

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    //InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
