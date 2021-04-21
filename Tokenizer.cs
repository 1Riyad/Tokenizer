using System;
using System.Collections.Generic;

namespace Tokenizer
{
    class Tokenization
    {
        public class Token
        {
            public int Position { set; get; }
            public int LineNumber { set; get; }
            public string Type { set; get; }
            public string Value { set; get; }

            // constructor 
            public Token(int position, int lineNumber, string type, string value) { }
        }


        public abstract class Tokenizable
        {
            public abstract bool tokenizable(Tokenizer tokenizer);
            public abstract Token tokenize(Tokenizer tokenizer);            
        }

        public class Tokenizer
        {
            private List<Token> tokens;
            public bool enableHistory;
            public Input input;


            public Tokenizer(string source, Tokenizable[] handlers) {
                this.input = new Input(source);
            }
            public Tokenizer(Input source, Tokenizable[] handlers) {
                this.input = source;
                // ToDO: make sure it is not null.
            }

            public Token tokenize() { return null; }
            public List<Token> all() { return null; }
        }


        public class IdTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                // _ | letter + [letter | digit | _ ]*
                char currentChar = t.input.peek();
                return Char.IsLetter(currentChar) || currentChar == '_';
            }

            public override Token tokenize(Tokenizer t)
            {
                // 1. Initialize token
                    //Token token = new Token();
                    //token.Position = t.input.Position;
                    //token.LineNumber = t.input.LineNumber;
                    //token.Type = "identifier";
                    //token.Value = "";
                Token token = new Token(t.input.Position, t.input.LineNumber, "identifier", "");


                // 2. do action
                char currentChar = t.input.peek();

                while (t.input.hasMore() && (Char.IsLetterOrDigit(currentChar) || currentChar == '_'))
                {
                    token.Value += t.input.step().Character;
                    currentChar = t.input.peek();
                }

                // 3. return token
                return token;
            }
        }


        static void Main(string[] args)
        {

            Tokenizer t = new Tokenizer(new Input("Rio Dan"), new Tokenizable[] {  });
            //List<Token> tokens = t.all();
            IdTokenizer idt = new IdTokenizer();
            if (idt.tokenizable(t))
            {
                Token token = idt.tokenize(t);
                Console.WriteLine(token.Value);
            }
        }
        
    }
}

