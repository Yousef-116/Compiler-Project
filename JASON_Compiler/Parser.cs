using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Runtime.Hosting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace JASON_Compiler
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
        List<Token> TokenStream = JASON_Compiler.Jason_Scanner.Tokens;
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
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("program");

                node.Children.Add(function_statement());
                node.Children.Add(main_function());

                return node;
            }
            return null;
        }
        private Node function_statement()
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
                }
                return node;
            }
            return null;
        }
        private Node function_declaration()
        {
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
                }
                return node;
            }
            return null;
        }

        private Node parameter_list_dash()
        {

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
                return node;
            }
            return null;
        }
        private Node function_body()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("function_body");

                node.Children.Add(match(Token_Class.LCurlybracket));
                node.Children.Add(Statements());
                node.Children.Add(return_statement());
                node.Children.Add(match(Token_Class.RCurlybracket));
                return node;
            }
            return null;
        }
        private Node main_function()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("main_function");

                node.Children.Add(DataType());
                node.Children.Add(match(Token_Class.Main));
                node.Children.Add(match(Token_Class.LParanthesis));
                node.Children.Add(match(Token_Class.RParanthesis));
                node.Children.Add(function_body());
                
                
                return node;
            }
            return null;
        }

        private Node DataType()
        {

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
                return node;
            }
            return null;
        }

        private Node Statements()
        {

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
                }

                return node;
            }
            return null;
        }

        private Node Statement()
        {

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("Statement");
                if (TokenStream[InputPointer].token_type == Token_Class.ReservedWordFLOAT ||
                TokenStream[InputPointer].token_type == Token_Class.ReservedWordINT ||
                TokenStream[InputPointer].token_type == Token_Class.ReservedWordSTRING)
                {
                    node.Children.Add(declaration_statement());

                }
                // function_call or assignment_statement
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    int tempInputPointer = InputPointer + 1;

                    if (TokenStream[tempInputPointer].token_type == Token_Class.assignmentOP) // assignment_statement
                    {
                        node.Children.Add(assignment_statement());
                    }
                    else if (TokenStream[tempInputPointer].token_type == Token_Class.LParanthesis) // function_call
                    {
                        node.Children.Add(function_call());
                    }

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
                else if (TokenStream[InputPointer].token_type == Token_Class.Comment)
                {
                    node.Children.Add(match(Token_Class.Comment));
                }

                return node;
            }
            return null;
        }
        private Node declaration_statement()
        {
            //Declaration_Statement->Datatype identifier Decls;
            //Decls-> , identifer Decls | Decl_Assignment Decls | ε
            //Decl_Assignment-> := Expression 
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
            //Declaration_Statement->Datatype identifier Decls;
            //Decls-> , identifer Decls | Decl_Assignment Decls | ε
            //Decl_Assignment-> := Expression 
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("declaration_statement_dash");
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    node.Children.Add(match(Token_Class.Comma));
                    node.Children.Add(match(Token_Class.Idenifier));
                    node.Children.Add(declaration_statement_dash());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.assignmentOP)
                {
                    node.Children.Add(match(Token_Class.assignmentOP));
                    node.Children.Add(expression_statement());
                    node.Children.Add(declaration_statement_dash());
                }
                return node;
            }
            return null;
        }
        private Node assignment_statement()
        {
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

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("write_statement");
                node.Children.Add(match(Token_Class.Write));

                if (TokenStream[InputPointer].token_type == Token_Class.Endl)
                {
                    node.Children.Add(match(Token_Class.Endl));
                }
                else //expr
                {
                    node.Children.Add(expression_statement());
                }

                node.Children.Add(match(Token_Class.Semicolon));

                return node;
            }
            return null;
        }
        private Node read_statement()
        {
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
                }
                return node;
            }
            return null;
        }

        private Node else_statement()
        {

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("else_statement");
                if (TokenStream[InputPointer].token_type == Token_Class.ELSE)
                {
                    node.Children.Add(match(Token_Class.ELSE));
                    node.Children.Add(Statements());
                    node.Children.Add(match(Token_Class.End));
                }

                return node;
            }
            return null;
        }
        private Node return_statement()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("return_statement");

                node.Children.Add(match(Token_Class.Return));
                node.Children.Add(expression_statement());
                node.Children.Add(match(Token_Class.Semicolon));

                return node;
            }
            return null;
        }


        private Node condition_statement()
        {
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

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("condition");
                node.Children.Add(match(Token_Class.Idenifier));

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

                node.Children.Add(term());

                return node;
            }
            return null;
        }
        private Node boolean_expression()
        {

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("boolean_expression");
                if (TokenStream[InputPointer].token_type == Token_Class.ANDOp)
                {
                    node.Children.Add(match(Token_Class.ANDOp));
                    node.Children.Add(condition_statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.OROp)
                {
                    node.Children.Add(match(Token_Class.OROp));
                    node.Children.Add(condition_statement());
                }

                return node;
            }
            return null;
        }
        private Node expression_statement()
        {

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("expression_statement");
                if (TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    node.Children.Add(match(Token_Class.String));
                }
                // equation
                else if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis ||
                    (TokenStream[InputPointer + 1].token_type == Token_Class.PlusOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.MinusOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.MultiplyOp ||
                     TokenStream[InputPointer + 1].token_type == Token_Class.DivideOp))
                {
                    node.Children.Add(equation());
                }
                // term
                else
                {
                    node.Children.Add(term());
                }

                return node;
            }
            return null;
            
        }

        private Node equation()
        {
            //Equation->bracket | nobracket
            //nobracket->term OPpart
            //bracket-> (term OPpart)
            //OPpart->Arithemtic_Operator Equation | ε 

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("equation");
                if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    node.Children.Add(bracket_equation());
                }
                else
                {
                    node.Children.Add(non_bracket_equation());
                }
                return node;
            }
            return null;
        }
        private Node bracket_equation()
        {

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("bracket_equation");
                if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    node.Children.Add(match(Token_Class.LParanthesis));
                    node.Children.Add(term());
                    node.Children.Add(equation_arithmetic_part());
                    node.Children.Add(match(Token_Class.RParanthesis));
                }
                return node;
            }
            return null;
        }


        private Node non_bracket_equation()
        {
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
            //OPpart->Arithemtic_Operator Equation | ε 

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("equation_arithmetic_part");
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                {
                    node.Children.Add(match(Token_Class.PlusOp));
                    node.Children.Add(equation());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                {
                    node.Children.Add(match(Token_Class.MinusOp));
                    node.Children.Add(equation());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    node.Children.Add(match(Token_Class.MultiplyOp));
                    node.Children.Add(equation());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                    node.Children.Add(match(Token_Class.DivideOp));
                    node.Children.Add(equation());
                }


                return node;
            }
            return null;
        }


        private Node term()
        {

            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("term");
                if (TokenStream[InputPointer].token_type == Token_Class.Int)
                {
                    node.Children.Add(match(Token_Class.Int));
                    return node;
                }
                else if (TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                {
                    node.Children.Add(function_call());
                    return node;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    node.Children.Add(match(Token_Class.Idenifier));
                    return node;
                }

                MessageBox.Show("Error in term - token not number or identifier or func_call");
                return null;
            }
            return null;
        }




        private Node function_call()
        {
            if (InputPointer < TokenStream.Count)
            {
                Node node = new Node("function_call");

                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(match(Token_Class.LParanthesis));
                node.Children.Add(parameter_list());
                node.Children.Add(match(Token_Class.RParanthesis));

                return node;
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
                    InputPointer++;
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
