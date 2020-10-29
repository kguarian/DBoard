using System;
using System.IO;
using System.Text;
namespace DBoard
{
    public delegate string Del_x_string(); //delegateP: no args to string

    class FileWriter<E>{
        Del_x_string toStringAsync;
        DecTree<E> operand;
        public FileWriter(DecTree<E> operand){
            this.operand = operand;
            toStringAsync = new Del_x_string(operand.ToString);
        }
        public void Write(String filename, String content)
        {
            //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-write-to-a-text-file
            using (Stream s = new FileStream(filename + ".decT", FileMode.Create))
            {
                s.Write(Encoding.Unicode.GetBytes(content));
                s.Close();
            }
        }
    }

    /// <summary>
    /// Developers get freedom here. Swap out "chars" with something else if you want, but the <methodname>Stable</methodname>
    /// </summary>
    class DecString
    {
        DecTree<char> chars;
        int Length;

        public DecString()
        {
            chars = new DecTree<char>();
            Length = 0;
        }

        public void AddChar(char c)
        {
            chars.Add(Length++, c);
        }

        public void GetChar(int index){

        }

        public void AddString(String starter)
        {
            for (int i = 0; i < starter.Length; i++)
            {
                AddChar(starter[i]);
            }
        }

        public DecString(String init)
        {
            chars = new DecTree<char>();
            Length = 0;
            this.AddString(init);
        }

        public override String ToString()
        {
            char[] allChars = new char[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                allChars[i] = chars.Get(i);
            }
            return new string(allChars);
        }

        public static DecTree<DecString> Split(string input){

            if(input.Length == 0){
                DecTree<DecString> r_tree = new DecTree<DecString>();
                r_tree.Add(1, new DecString("\0"));
                return r_tree;
            }

            int stringIndex = 0;
            int charCounter = 0;

            DecTree<DecString> r_tree1 = new DecTree<DecString>();
            DecString opDString = new DecString();
            while(charCounter<input.Length){
                opDString.AddChar(input[charCounter]);
                charCounter++;
                if(input[charCounter]==' '){
                    r_tree1.Add(stringIndex, opDString);
                    stringIndex++;
                    opDString = new DecString();
                    stringIndex = 0;
                }
            }
            if(input[input.Length-1]!=' '){
                r_tree1.Add(stringIndex, opDString);
            }
            return r_tree1;
        }

        #pragma warning disable 168
        public bool Stable(){
            try{
                for (int i = 0; i < this.Length; i++){
                    this.GetChar(i);
                }
                return true;
            } catch (Exception e){
                return false;
            }
        }
        #pragma warning restore 168
    }
}