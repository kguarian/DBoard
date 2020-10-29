using System;

namespace DBoard
{
    class DecBoard
    {
        DecTree<Event> Schedule;


        //Constructor empty. AddEvent functionality moved to funtion "AddEvent."
        public DecBoard()
        {
            Schedule = new DecTree<Event>();
        }

        //FIXME: rename AddEvent, create empty constructor, check if index is already filled (throw exception if so), document changes, erase comment

        /*
        void return type because method does not modify inputs.
        There should already be variables for this stuff.
        */
        public void AddEvent(string Index, string Title, string Date, String Notes)
        {
            try
            {
                Schedule.Get(Index);
                //this is where the NullReferenceException should be thrown

                //since it wasn't thrown, we need to throw the exception. The index should be free.
                throw new Exception("Duplicate Index. Undefined behavior.");
            }
            catch (NullReferenceException e)
            {
            }
            Schedule.Add(Index, new Event(Title, Date, Notes));
        }
        public void PrintDecTree(int[] indices, DecTree<object> subject)
        {
            if (indices.Length == 0)
            {
                return;
            }
            try
            {
                foreach (int index in indices)
                {
                    Console.Write(subject.Get(indices[index]));
                }
            }
            catch (NullReferenceException e)
            {
                Console.Write("index subject disagreement. Something was wrong with the DecTree's Ordering.");
                e.Equals(e);
            }
        }

        class Event
        {
            public DateTime Date;
            public String Name;
            DecTree<DecString> Note;
            long NoteSize = 0;
#nullable enable
            public Event(String Title, String Date, String Notes)
            {
                char TERMCHAR = '<';
                this.Date = DateTime.Parse(Date);
                this.Name = Title;
                Note = new DecTree<DecString>();
                if (!(Notes == null))
                {
                    String[] noteArray = Notes.Split();
                    bool notDone = true;
                    //Yes. Used in If statement a few lines down. Ignore.
                    int noteArrayLength = noteArray.Length;

                    int noteArrayIndex = 0;
                    for (noteArrayIndex = 0; noteArrayIndex < noteArrayLength; noteArrayIndex++)
                    {
                        DecString subNote = new DecString();
                        if (noteArrayIndex == noteArrayLength)
                        {
                            notDone = false;
                            break;
                        }
                        //Debug
                        //okay, what am I trying to say?
                        //get the token we're on, check if its last char is ?
                        //zero length case?
                        //left for documentation
                        if (noteArray[noteArrayIndex].Length == 0)
                        {
                            continue;
                        }
                        else if (noteArray[noteArrayIndex][noteArray[noteArrayIndex].Length - 1] == TERMCHAR)
                        {
                            Note.Add(subNote);
                        }
                        else
                        {
                            subNote.AddString($" {noteArray[noteArrayIndex]}");
                            //change the formatting at your happiest hour. The space is
                            //at the beginning so that we have words between our strings.
                        }
                    }
                }
            }
#nullable disable
        }
        public static void Main(string[] args)
        {
            string title = args[0];
            string date = args[1];
            string subtitle = args[2];

            DecString notes = new DecString();

            for (int i = 3; i < args.Length; i++)
            {
                //doesn't run if no arguments. That's checked already.
                //this loop body adds the next string in the args array. That's it.
                notes.AddString(args[i]);
                notes.AddChar(' ');
            }
            //lowkey a neural net, but these are just a rudimentary parsing of args elements.

            DecBoard dec = new DecBoard();
            dec.AddEvent(title, subtitle, date, notes.ToString());
            Console.WriteLine(dec.Schedule.Get(title).Date.ToString());
        }
    }
    class Logger
    {
        System.IO.BinaryWriter FileWriter;
        public Logger()
        {
            System.IO.Directory.CreateDirectory("./logs");
            System.IO.File.Create("./log.txt");
            
        }
    }
}
