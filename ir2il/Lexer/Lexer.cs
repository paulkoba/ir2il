using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir2cil.Lexer
{
    enum Keyword
    {
        Define,
        Noundef,
        Dso_local,
        Declare,
        Attributes,

        None,
    }

    enum TokenType
    {
        AttributeGroup,
        LeftParenthese,
        RightParenthese,
        LeftBracket,
        RightBracket,
        LeftBrace,
        RightBrace,
        Keyword,
        LocalIdentifier,
        GlobalIdentifier,
        FunctionName,
        Attribute,
        Newline,
        None,
        EOF,
        Metadata,
        String,
        Equals,
        Comma
    }

    class LexerUtils
    {
        public static Keyword StringToKeyword(string s)
        {
            switch(s)
            {
                case "define":
                    return Keyword.Define;
                case "noundef":
                    return Keyword.Noundef;
                case "dso_local":
                    return Keyword.Dso_local;
                case "declare":
                    return Keyword.Declare;
                case "attributes":
                    return Keyword.Attributes;
                default:
                    return Keyword.None;
            }
        }
    }

    class Token
    {
        public Token(TokenType type, string value)
        {
            this.type = type;
            this.value = value;
        }

        public TokenType type = TokenType.None;
        public string value = "";

        public override string ToString()
        {
            return "{ " + type.ToString() + ", \"" + value.ToString() + "\" }";
        }
    }

    internal class Lexer
    {
        List<Token> tokens = new List<Token>();
        Token currentToken = new Token(TokenType.None, "");

        public Lexer() { }

        public List<Token> LexFile(string filename)
        {
            ClearState();

            if (filename == null)
            {
                throw new ArgumentNullException(filename);
            }

            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            string[] text = File.ReadAllLines(filename);

            foreach (string line in text)
            {
                Console.WriteLine(line);
            }

            List<Token> tokens = DoLex(text);

            return tokens;
        }

        private List<Token> DoLex(string[] lines)
        {
            foreach (string str in lines)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    // Login related to parsing string literals should always be at the beginning of the lexer loop
                    if (str[i] == '\"')
                    {
                        if (currentToken.type == TokenType.None)
                        {
                            currentToken.type = TokenType.String;
                            continue;
                        }

                        if (currentToken.type == TokenType.String)
                        {
                            FlushToken();
                            continue;
                        }

                        FlushToken();
                        currentToken.type = TokenType.String;
                        continue;
                    }

                    if (currentToken.type == TokenType.String)
                    {
                        currentToken.value += str[i];
                        continue;
                    }

                    if (str[i] == '@' || str[i] == '%' || str[i] == '#' || str[i] == '!')
                    {
                        FlushToken();
                        if (str[i] == '@')
                        {
                            currentToken.type = TokenType.GlobalIdentifier;
                        }

                        if (str[i] == '%')
                        {
                            currentToken.type = TokenType.LocalIdentifier;
                        }

                        if (str[i] == '#')
                        {
                            currentToken.type = TokenType.AttributeGroup;
                        }

                        if (str[i] == '!')
                        {
                            currentToken.type = TokenType.Metadata;
                        }
                    }

                    if (Char.IsLetterOrDigit(str[i]) || str[i] == '.' || str[i] == '_')
                    {
                        currentToken.value += str[i];

                        if (currentToken.type == TokenType.None)
                        {
                            currentToken.type = TokenType.Keyword;
                        }
                    }

                    // Handle comments
                    if (str[i] == ';')
                    {
                        break;
                    }

                    switch (str[i])
                    {
                        case ' ':
                            FlushToken();
                            break;
                        case '(':
                            FlushToken();
                            tokens.Add(new Token(TokenType.LeftParenthese, "("));
                            break;
                        case ')':
                            FlushToken();
                            tokens.Add(new Token(TokenType.RightParenthese, ")"));
                            break;
                        case '[':
                            FlushToken();
                            tokens.Add(new Token(TokenType.LeftBracket, "["));
                            break;
                        case ']':
                            FlushToken();
                            tokens.Add(new Token(TokenType.RightBracket, "]"));
                            break;
                        case '{':
                            tokens.Add(new Token(TokenType.LeftBrace, "{"));
                            break;
                        case '}':
                            FlushToken();
                            tokens.Add(new Token(TokenType.RightBrace, "}"));
                            break;
                        case ',':
                            FlushToken();
                            tokens.Add(new Token(TokenType.Comma, ","));
                            break;
                        case '=':
                            FlushToken();
                            tokens.Add(new Token(TokenType.Equals, "="));
                            break;
                    }
                }
                FlushToken();

                tokens.Add(new Token(TokenType.Newline, ""));
            }

            tokens.Add(new Token(TokenType.EOF, ""));

            return tokens;
        }

        private void ClearState()
        {
            tokens.Clear();
            currentToken = new Token(TokenType.None, "");
        }

        private void FlushToken()
        {
            if (currentToken.type == TokenType.None)
            {
                return;
            }

            tokens.Add(currentToken);
            currentToken = new Token(TokenType.None, "");
        }
    }
}
