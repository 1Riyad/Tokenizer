using System;

namespace Tokenizer
{
    class Input
    {
        private readonly string input;
        private readonly int length;
        private int position;
        private int lineNumber;



        public int Position
        {
            get
            {
                return this.position;
            }
        }
        public int Length { get { return this.length; } }
        public int NextPosition { get { return this.position + 1; } }
        public int LineNumber { get { return this.lineNumber; } }
        public char Character
        {
            get
            {
                if (this.position > -1) return this.input[this.position];
                else return '\0';
            }
        }
        public Input(string input)
        {
            this.input = input;
            this.length = input.Length;
            this.position = -1;
            this.lineNumber = 1;
        }


        public bool hasMore(int numOfSteps = 1)
        {
            if (numOfSteps <= 0) throw new Exception("Invalid number steps");
            return (this.position + numOfSteps) < this.length;
        }

        public bool hasLess(int numOfSteps = 1)
        {
            if (numOfSteps <= 0) throw new Exception("Invalid number of steps");
            return (this.position - numOfSteps) > -1;
        }

        public Input step(int numOfSteps = 1)
        {
            if (this.hasMore(numOfSteps))
            {
                this.position += numOfSteps;
            }
            else
                throw new Exception("There is no more steps");
            return this;
        }

        public Input back(int numOfSteps = 1)
        {
            if (this.hasLess(numOfSteps))
            {
                this.position -= numOfSteps;
            }
            else
                throw new Exception("There is no more steps");
            return this;
        }
        public Input reset() { return this; }
        public char peek(int numOfStep = 1) { return '\0'; }



        static void Main(string[] args)
        {
            Input i = new Input("Tuwaiq Bootcamp programming for .Net ");
            Console.WriteLine(i.step().step(4).step(7).back(3).step(10).back().Character);
        }
    }
}
