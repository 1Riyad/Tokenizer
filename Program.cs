using System;

namespace Tokenizer
{
    class Program
    {
        public class Token
        {
            public string type;
            public string value;
            public int position;
            public int lineNumber;
            //public Tokenizable[] handlers;
        }

        public abstract class Tokenizable
        {
            public abstract bool tokenizable(Tokenizer tokenizer);
            public abstract Token tokenize(Tokenizer tokenizer);
        }

        public class Tokenizer
        {
            private string input;
            public int currentPostion;
            public int lineNumber;
            //public Tokenizable handlers;

            public Tokenizer(string input)
            {
                this.input = input;
                this.currentPostion = -1;
                this.lineNumber = 1;
            }

            public char peek()
            {
                if (this.hasMore())
                {
                    return this.input[currentPostion + 1];
                }
                else
                    return '\0';
            }

            public char next()
            {
                char currentChar = this.input[++this.currentPostion];
                if (currentChar == '\n')
                    this.lineNumber++;
                return currentChar;
            }
            public bool hasMore() { return (this.currentPostion + 1) < this.input.Length; }

            public Token tokenizer(Tokenizable[] handlers)
            {
                foreach (var t in handlers)
                {
                    if (t.tokenizable(this))
                    {
                        return t.tokenize(this);
                    }
                }
                //throw new Exception("Unexpected token");
                return null;
            }
        }


        public class NumberTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && Char.IsDigit(t.peek());
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.value = "";
                token.type = "number";
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;

                while (t.hasMore() && Char.IsDigit(t.peek()))
                {
                    token.value += t.next();
                }
                return token;
            }
        }

        public class WhiteSpaceTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && Char.IsWhiteSpace(t.peek());
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.value = "";
                token.type = "White space";
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;

                while (t.hasMore() && Char.IsWhiteSpace(t.peek()))
                {
                    token.value += t.next();
                }
                return token;
            }
        }

        public class IdTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && (Char.IsLetter(t.peek()) || t.peek() == '_');
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.value = "";
                token.type = "id";
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;

                while (t.hasMore() && (Char.IsLetterOrDigit(t.peek()) || t.peek() == '_'))
                {
                    token.value += t.next();
                }
                return token;
            }
        }


        static void Main(string[] args)
        {
            Tokenizer t = new Tokenizer("123 3456   Tuwaiq_BootCamp3");
            Tokenizable[] handlers = new Tokenizable[] { new NumberTokenizer(),
                                                         new WhiteSpaceTokenizer(),
                                                         new IdTokenizer() };
            Token token = t.tokenizer(handlers);
            while (token != null)
            {
                Console.WriteLine(token.value);
                token = t.tokenizer(handlers);
            }

        }
    }
}

