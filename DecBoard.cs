using System;
using System.IO;
namespace DBoard
{

    class DBoard
    {
        Storage events = new Storage();
        public string GiveInstructions()
        {
            return null;
        }

        /*
            prints the parametrized message, then allows multi-line input
            (ENTER key). Submit input with (ALT+ENTER).
        */
        public string prompt(string message)
        {
            Console.Write($"{message}:");
            DecString retString = new DecString();
            ConsoleKeyInfo nextKey;
            bool ctrlEnter = false;

            while (!ctrlEnter)
            {
                nextKey = Console.ReadKey();
                if (nextKey.Modifiers == ConsoleModifiers.Shift)
                {
                    if (nextKey.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }
                }
                char inputCharacter = nextKey.KeyChar;
                if (inputCharacter != DecString.EndOfLine && inputCharacter != '\0')
                {   //character used in processing
                    if (nextKey.KeyChar == '\r')
                    {                                          //represents ENTER as Newline instead of carriage return.
                        inputCharacter = '\n';
                        Console.WriteLine();
                    }
                    if (nextKey.Key == ConsoleKey.Backspace)
                    {      //implements backspace functionality, except after a newline.
                        retString.RmChar();
                        Console.Write(" \b");
                        continue;
                    }
                    retString.AddChar(inputCharacter);
                }

            }
            return retString.ToString();
        }

        public string Run()
        {
            Console.Write("operation (add, get, rm, save, load, list, delete, clear):");
            string uInput = Console.ReadLine().ToLower();
            if (uInput == "add")
            {
                string[] eventParams = new string[3];
                bool successfulDate = false;

                Console.Write("title:");
                eventParams[0] = Console.ReadLine();
                while (!successfulDate)
                {
                    try
                    {
                        Console.Write("date:");
                        eventParams[1] = Console.ReadLine();
                        if (eventParams[1] == "")
                        {
                            eventParams[1] = DateTime.Now.ToString();
                        }
                        DateTime.Parse(eventParams[1]);
                        successfulDate = true;
                    }
                    catch (FormatException)
                    {
                        string cancelResponse = prompt("Invalid date. Format should be \"(M)M/(D)D/YYYY\". Cancel? (Y/n)");
                        if (cancelResponse == "Y")
                        {
                            return null;
                        }
                    }
                }

                eventParams[2] = prompt("note");

                Event newEvent = new Event(eventParams[0], DecTime.Parse(eventParams[1]), eventParams[2]);

                while (events.GetEvent(eventParams[0]) != null)
                {
                    string removeOrRename = prompt("Duplicate element. (remove) or (rename)?").ToLower();
                    while (removeOrRename != "remove" && removeOrRename != "rename")
                    {
                        removeOrRename = prompt("Invalid input. (remove) or (rename)").ToLower();
                    }
                    if (removeOrRename == "rename")
                    {
                        eventParams[0] = prompt("new name");
                    }
                    else
                    {//only other option is remove
                        events.Rm(eventParams[0]);
                    }
                }
                events.AddEvent(eventParams[0], newEvent);
                return events.GetEvent(eventParams[0]).ToString() != null ? "add succeeded" : "add failed";
            }
            else if (uInput == "rm")
            {
                string alias = prompt("Which event?");
                return events.Rm(alias) ? "rm succeeded" : "rm failed";
            }
            else if (uInput == "get")
            {
                string alias = prompt("Which event?");
                try
                {
                    return events.GetEvent(alias).ToString().Replace('\0', '\n');
                }
                catch (NullReferenceException)
                {
                    return "get failed";
                }
            }
            else if (uInput == "quit")
            {
                return uInput;
            }
            else if (uInput == "save")
            {
                string saveName = prompt("save name or (cancel)");
                if (saveName == "cancel")
                    return "canceled save";
                while (System.IO.File.Exists(saveName))
                {
                    if (prompt("overwrite (Y/n)") == "Y")
                    {
                        events.Export(saveName);
                        return "saved";
                    }
                    else
                    {
                        return "save failed.";
                    }
                }
                events.Export(saveName);
                return "saved";
            }
            else if (uInput == "load")
            {
                System.Collections.Generic.IEnumerable<string> filesInDirectory = System.IO.Directory.EnumerateFiles(".");
                foreach (string file in filesInDirectory)
                {
                    if (file.Contains(".a.dec"))
                        Console.WriteLine(file + "\b\b\b\b\b\b      ");
                }
                Storage loadAttempt;
                try
                {
                    Console.WriteLine("DBoard name");
                    loadAttempt = Storage.Import(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return "load failed";
                }
                events = loadAttempt;
                return "load succeeded";
            }
            else if (uInput == "list")
            {
                long length = events.addresses.Length();
                for (long i = 0; i < length; i++)
                {
                    Console.WriteLine($"{events.addresses.Get(i)}\t\t\t{events.dates.Get(i)}");
                }
                return "listed";
            }
            else if (uInput == "delete")
            {
                System.Collections.Generic.IEnumerable<string> filesInDirectory = System.IO.Directory.EnumerateFiles(".");
                foreach (string file in filesInDirectory)
                {
                    if (file.Contains(".a.dec"))
                        Console.WriteLine(file + "\b\b\b\b\b\b      ");
                }
                Console.WriteLine("DBoard name");
                string DBoardName = Console.ReadLine();
                if (File.Exists($"{DBoardName}.a.dec") && File.Exists($"{DBoardName}.d.dec") && File.Exists($"{DBoardName}.e.dec"))
                {
                    File.Delete($"{DBoardName}.a.dec");
                    File.Delete($"{DBoardName}.d.dec");
                    File.Delete($"{DBoardName}.e.dec");
                    return "deleted";
                }
                return "not deleted. At least one .dec file not found.";
            }
            else if (uInput == "clear")
            {
                Console.Clear();
                return "clear";
            }
            else
            {
                return null;
            }
        }
    }

    class Event : ConvertibleObject
    {
        public DecTree<object> fields = new DecTree<object>();

        public Event(string title, DecTime date, string notes)
        {
            fields.Add("title", title);
            fields.Add("date", date);
            fields.Add("notes", notes);
        }

        public override string ToString()
        {
            DecString retString = new DecString();
            retString.AddString((string)fields.Get("title"));
            retString.AddChar('\0');

            retString.AddString(((DecTime)fields.Get("date")).ToString());
            retString.AddChar('\0');

            retString.AddString(((string)fields.Get("notes")).Replace('\n', '\0'));     //No Standardized DecTree newline character.
            return retString.ToString();
        }

        public override Event FromString(string input)
        {
            if (input == "")
            {
                return null;
            }

            DecString inputDS = new DecString(input);
            DecString separatedString_ElementComponent = new DecString();
            string[] separatedString = new string[3];       //3 fields: title, datetime, note
            char currChar;
            int currChar_Index = 0;

            for (int i = 0; i < 2; currChar_Index++)
            {
                if ((currChar = inputDS.GetChar(currChar_Index)) != '\0')
                    separatedString_ElementComponent.AddChar(currChar);
                else
                {
                    separatedString[i] = separatedString_ElementComponent.ToString();
                    separatedString_ElementComponent.Clear();
                    i++;
                }
            }

            while ((currChar = inputDS.GetChar(currChar_Index++)) != DecString.EndOfLine)
                separatedString_ElementComponent.AddChar(currChar);

            separatedString[2] = separatedString_ElementComponent.ToString();

            return new Event(separatedString[0], DecTime.Parse(separatedString[1]), separatedString[2].Replace('\0', '\n'));   //No Standardized DecTree newline character.
        }
    }


    class Storage
    {
        public DecTree<string> addresses = new DecTree<string>();
        public DecTree<object> dates = new DecTree<object>();
        public long addresses_counter = 0;
        public DecTree<object> eventHolder = new DecTree<object>();

        public void AddEvent(string address, Event element)
        {
            addresses.Add(addresses_counter, address);
            dates.Add(addresses_counter, (DecTime)element.fields.Get("date"));
            eventHolder.Add(address, element);
            addresses_counter++;
        }

        public Event GetEvent(string address)
        {
            try
            {
                return (Event)eventHolder.Get(address);
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public bool Rm(string address)
        {
            int addressIndex = -1;
            Defragment_Addresses();
            for (int i = 0; i < addresses_counter; i++)
            {
                if (addresses.Get(i) == address)
                {
                    addressIndex = i;
                    break;
                }
            }
            if (addressIndex < 0)
                return false;
            else if (addresses.Rm(addressIndex) && dates.Rm(addressIndex--) && eventHolder.Rm(address))
                return true;
            else
                throw new NullReferenceException();     //If this is thrown, something is just messed up. This should never be thrown.
        }

        public void Export(string path)
        {
            addresses.Export($"{path}.a.dec");
            dates.Export($"{path}.d.dec");
            eventHolder.Export($"{path}.e.dec");
        }

        public static Storage Import(string path)
        {
            Storage retStorage = new Storage();
            retStorage.addresses = DecTree<string>.Import($"{path}.a.dec");
            retStorage.dates = DecTree<string>.Import($"{path}.d.dec", new DecTime(DateTime.Now));
            retStorage.addresses_counter = retStorage.addresses.Length();
            retStorage.eventHolder = DecTree<object>.Import($"{path}.e.dec", new Event(null, DecTime.Convert(DateTime.Now), null));
            return retStorage;
        }

        public void Defragment_Addresses()
        {
            int addressesDefragmented = 0; //number of addresses defragmented
            for (int i = 0; addressesDefragmented < addresses_counter; i++)
            {
                try
                {
                    string address;
                    if ((address = addresses.Get(i)) != null)
                    {
                        addresses.Rm(addressesDefragmented);
                        dates.Rm(addressesDefragmented);
                        addresses.Add(addressesDefragmented, address);
                        dates.Add(addressesDefragmented, address);
                        addressesDefragmented++;
                    }
                }
                catch (NullReferenceException) { continue; }
            }
        }
    }
}