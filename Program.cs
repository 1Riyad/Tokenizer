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

            public char peek(int index = 1)
            {
                if (this.hasMore(index))
                {
                    return this.input[this.currentPostion + index];
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
            public bool hasMore(int index= 1) { return (this.currentPostion + index) < this.input.Length; }

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


        //public class NumberTokenizer : Tokenizable
        //{
        //    public override bool tokenizable(Tokenizer t)
        //    {
        //        return t.hasMore() && Char.IsDigit(t.peek());
        //    }

        //    public override Token tokenize(Tokenizer t)
        //    {
        //        Token token = new Token();
        //        token.value = "";
        //        token.type = "number";
        //        token.position = t.currentPostion;
        //        token.lineNumber = t.lineNumber;

        //        while (t.hasMore() && Char.IsDigit(t.peek()))
        //        {
        //            token.value += t.next();
        //        }
        //        return token;
        //    }
        //}

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
                //return null;
            }
        }

        public class ColorHashTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && t.peek() == '#';
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.value += t.next();
                token.type = "color-hash";
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;

                while (t.hasMore()
                    && !char.IsWhiteSpace(t.peek())
                    && "0123456789abcdefABCDEF".Contains(t.peek()))
                {
                    token.value += t.next();
                }
                return token;
            }
        }

        public class NumberTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && char.IsDigit(t.peek());
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.value = "";
                token.type = "int";
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;

                while (t.hasMore()
                    && !char.IsWhiteSpace(t.peek()))
                {
                    if(char.IsDigit(t.peek())) // if token.type == "decimal" then throw error
                    {
                        token.value += t.next();
                    }
                    else if(t.peek() == '.')
                    {
                        token.type = "decimal";
                        token.value += t.next();
                    }
                }
                return token;
            }
        }


        static void Main(string[] args)
        {
            //string testCase = "123 3456   Tuwaiq_BootCamp3 #abc123";
            string testCase = "#123abc 1.1 22 55.6";
            Tokenizer t = new Tokenizer(testCase);
            Tokenizable[] handlers = new Tokenizable[] { /*new NumberTokenizer(),*/
                                                        new NumberTokenizer(),
                                                         new WhiteSpaceTokenizer(),
                                                         new IdTokenizer(),
                                                         new ColorHashTokenizer()};
            Token token = t.tokenizer(handlers);
            while (token != null)
            {
                Console.WriteLine(token.value);
                token = t.tokenizer(handlers);
            }
        }
    }
}

