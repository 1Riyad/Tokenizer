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
            public string input;
            public int currentPostion;
            public int lineNumber;
            //public Tokenizable handlers;

            public Tokenizer(string input)
            {
                this.input = input;
                this.currentPostion = -1;
                this.lineNumber = 1;
            }


            public char peek(int index =1)
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

            public bool hasMore(int index = 1) { return (this.currentPostion + index) < this.input.Length; }

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

        public class HexColorTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && t.peek() == '#'
                    && (t.currentPostion == -1 || char.IsWhiteSpace(t.peek(0)));// is # in the begging OR there is a space before it
            }

            public override Token tokenize(Tokenizer t)
            {
                int counter = 0;

                Token token = new Token();
                token.value += t.next(); //  add the #
                token.type = "hex-color";
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;


                while (t.hasMore()
                    && !char.IsWhiteSpace(t.peek())
                    && counter < 6)
                {
                    if(!"0123456789abcdefABCDEF".Contains(t.peek()))
                    {
                        // has at least one hex value after #
                        if (counter > 0)
                            return token;
                        else
                        {
                            t.currentPostion = token.position;
                            return null;
                        }
                    }
                    counter++;
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
                    if(char.IsDigit(t.peek()))
                    {
                        token.value += t.next();
                    }
                    else if(t.peek() == '.')
                    {
                        if(token.type == "decimal")
                        {
                            return token;
                        }
                        token.type = "decimal";
                        token.value += t.next();
                    }
                }
                // add 0 to the end if there is not zero !
                token.value = token.value[token.value.Length - 1] == '.'
                    ? (token.value += '0')
                    : token.value;

                return token;
            }
        }
        public class PunctuationTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return (t.hasMore() && Char.IsPunctuation(t.peek()));
             
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.value = "";
                token.type = "Punctuation";
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;

                while (t.hasMore() && Char.IsPunctuation(t.peek()))
                {
                    token.value += t.next();
                }
                return token;
            }
        }

        public class ILCommentTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && (t.peek() == '/') && (t.peek(2) == '/');
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.value = "";
                token.type = "Comment";
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;

                while (t.hasMore() && (t.peek() != '\n'))
                {

                    token.value += t.next();
                }
                
                return token;
                //return null;
            }

        }

        public class MuitLinesCommentTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && ((t.peek() == '/') && (t.peek(2) == '*'));
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.value = "";
                token.type = "CommentTokenizerMuitLines";
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;
               token.value += t.next();
                token.value += t.next();
                    while (t.hasMore()  )
                    {

                    if((t.peek() + "" + t.peek(2) == "*/"))
                    {
                        token.value += t.next();
                        token.value += t.next();
                        break;
                    }

                    token.value += t.next();
                }
                
                return token;
            }

        }
        public class SingleQuotTokenizer : Tokenizable
        {
            bool isClosed = false;
            int pos;
            public override bool tokenizable(Tokenizer t)
            {
                return t.hasMore() && t.peek() == char.Parse("'");
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token();
                token.value = "";
                token.type = "Single Quot";
                pos =t.currentPostion;
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;
                isClosed = false;
                while (t.hasMore())
                {
                    token.value += t.next();
                    if (t.peek() == char.Parse("'"))
                    {
                        isClosed = true;
                        token.value += t.next();
                        break;
                    }

                }
                if(isClosed){
                return token;
                }
                else{
                    t.currentPostion = pos;
                    token.value = "";
                    if(t.hasMore()){
                    t.next();
                    return token;
                    }
                    
                    while (t.hasMore())
                {
                    token.value += t.next();
                    if (t.peek() == char.Parse("'"))
                    {
                        isClosed = true;
                        token.value += t.next();
                        break;
                    }

                }
                return token;
                }
            }
        }
        public class StringTokenizer : Tokenizable
        {
            bool isClosed1 = false;
            int pos1;
            public override bool tokenizable(Tokenizer t)
            {

                return t.hasMore() && t.peek() == '\"';
            }

            public override Token tokenize(Tokenizer t)
            {
                
                Token token = new Token();
                token.value = "";
                token.type = "Double Quot";
                pos1 =t.currentPostion;
                token.position = t.currentPostion;
                token.lineNumber = t.lineNumber;
                isClosed1 = false;
                while (t.hasMore())
                {
                    token.value += t.next();
                    if (t.peek() == ('\"'))
                    {
                        isClosed1 = true;
                        token.value += t.next();
                        break;
                    }

                }
                if(isClosed1){
                return token;
                }else{
                    t.currentPostion = pos1;
                    token.value = "";
                    if(t.hasMore()){
                    t.next();
                    return token;
                    }
                    
                    while (t.hasMore())
                {
                    token.value += t.next();
                    if (t.peek() == '\"')
                    {
                        isClosed1 = true;
                        token.value += t.next();
                        break;
                    }

                }
                return token;
                }
            }
        }

        //public class NumberTokenizer : Tokenizable
        //{
        //    public override bool tokenizable(Tokenizer t)
        //    {
        //        return t.hasMore() && char.IsDigit(t.peek());
        //    }

        //    public override Token tokenize(Tokenizer t)
        //    {
        //        Token token = new Token();
        //        token.value = "";
        //        token.type = "int";
        //        token.position = t.currentPostion;
        //        token.lineNumber = t.lineNumber;

        //        while (t.hasMore()
        //            && !char.IsWhiteSpace(t.peek()))
        //        {
        //            if(char.IsDigit(t.peek())) // if token.type == "decimal" then throw error
        //            {
        //                token.value += t.next();
        //            }
        //            else if(t.peek() == '.')
        //            {
        //                token.type = "decimal";
        //                token.value += t.next();
        //            }
        //        }
        //        return token;
        //    }
        //}

        static void Main(string[] args)
        {
            string testCase = "@ #ab 1.2 2. 51555.6 2.5548 1555.5848 .336 f#f'' '999' \" 999 #123abc 3456   Tuwaiq_BootCamp3 #abc123 123 1.1 22 . 55.6 Hi_hdfj; /*  1.1 22 */ //Tuwaiq_BootCamp3 ";
            Tokenizer t = new Tokenizer(testCase);
            Tokenizable[] handlers = new Tokenizable[] { /*new NumberTokenizer(),*/
                                                        new NumberTokenizer(),
                                                        new SingleQuotTokenizer(),
                                                        new StringTokenizer(),
                                                         new ILCommentTokenizer(),
                                                         new MuitLinesCommentTokenizer(),
                                                         new WhiteSpaceTokenizer(),
                                                         new IdTokenizer(),
                                                         new HexColorTokenizer(),
                                                         new PunctuationTokenizer()};
            //Token token = t.tokenizer(handlers);
            //Console.WriteLine("----------------------");
            //while (token != null)
            //{
            //    Console.WriteLine($"{token.value} [{token.type}]");
            //    Console.WriteLine("----------------------");
            //    token = t.tokenizer(handlers);
            //}
            Token token = null;
            while(t.currentPostion + 1 < t.input.Length)
            {
                token = t.tokenizer(handlers);
                if (token != null)
                    Console.WriteLine($"{token.value} [{token.type}]");

                else t.next();
                //else if (token == null && t.hasMore()) t.next();

                //else break;
            }
        }
    }
}

