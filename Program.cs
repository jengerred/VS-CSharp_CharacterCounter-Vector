using System;
using System.Collections; // Required for ArrayList
using System.IO;         // Required for File operations

namespace JenniferGerred_CharacterCounter_Vector
{
    // Represents a character and its frequency count
    public class CharacterFrequency
    {
        private char _ch;       // The character being counted
        private int _frequency; // Number of times the character appears

        // Constructor - creates a new CharacterFrequency object for the given character
        // The frequency starts at 0 and is incremented when the character is found
        public CharacterFrequency(char character)
        {
            _ch = character;
            _frequency = 0; 
        }

        // Gets the character
        public char GetCharacter() => _ch;

        // Sets the character
        public void SetCharacter(char character) => _ch = character;

        // Gets how many times the character appears
        public int GetFrequency() => _frequency;

        // Sets the frequency count for the character
        public void SetFrequency(int frequency) => _frequency = frequency;

        // Adds one to the frequency count
        public void Increment() => _frequency++;

        // Checks if two CharacterFrequency objects represent the same character
        // Returns true if they have the same character, false otherwise
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            CharacterFrequency other = (CharacterFrequency)obj;
            return _ch == other._ch;
        }

        // Returns a hash code based on the character value
        //public override int GetHashCode() => _ch.GetHashCode();

        // Returns a string in the format: Character(ASCIIValue)[tab]Frequency
        // For control characters, only shows: (ASCIIValue)[tab]Frequency
        public override string ToString()
        {
            int asciiValue = (int)_ch;
            string charDisplay = (asciiValue < 32 || asciiValue == 127) ? "" : _ch.ToString();
            return (charDisplay == "")
                ? $"        ({asciiValue})\t{_frequency}"      // Control character: no char shown
                : $"        {charDisplay}({asciiValue})\t{_frequency}"; // Printable character
        }
    }

    // Main program class for counting character frequencies in a text file
    class Program
    {
        // Main entry point - reads input file, counts characters, writes to output file
        // Command line arguments: inputFile outputFile
        static void Main(string[] args)
        {
            // Debug: Show current directory
            Console.WriteLine($"Current Directory: {Environment.CurrentDirectory}");
            Console.WriteLine($"Input file path: {args[0]}\n");
            Console.WriteLine("\nVector - Character(ascii)  Frequency\n");

            // Validate command-line arguments
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: JenniferGerred_CharacterCounter_Vector.exe <inputFile> <outputFile>");
                Console.WriteLine("Example: JenniferGerred_CharacterCounter_Vector.exe wap.txt wap_output.txt");
                return; // Exit if incorrect arguments are provided
            }

            string inputFile = args[0];
            string outputFile = args[1];

            // TEMPORARY TEST: Simulate input "Hello." without reading from a file
            string testInput = "Hello.\r\n"; // In txt file "\r \n" is automatic
            ArrayList testFrequencies = new ArrayList();

            foreach (char c in testInput)
            {
                CharacterFrequency tempCf = new CharacterFrequency(c);
                int index = testFrequencies.IndexOf(tempCf);

                if (index != -1)
                {
                    CharacterFrequency existingCf = (CharacterFrequency)testFrequencies[index]!;
                    existingCf.Increment();
                }
                else
                {
                    tempCf.Increment();
                    testFrequencies.Add(tempCf);
                }
            }

            // Output results for the test input
            Console.WriteLine("Test Output for input: \"Hello.\"");
            foreach (CharacterFrequency cf in testFrequencies)
            {
                Console.WriteLine(cf.ToString());
            }
            Console.WriteLine("End of test output\n");
            // End of test

            // ArrayList to store CharacterFrequency objects, as required by the assignment for C#.
            // ArrayList in C# is equivalent to vector in C++ or Vector in Java - all are dynamic array implementations.
            // This approach uses a dynamic list (ArrayList) to store only the characters
            // that actually appear in the input file. This differs from a strategy
            // that might use a fixed-size array (e.g., size 256) to map directly to ASCII values.
            // Using ArrayList allows for flexible storage and relies on the Equals() method
            // of CharacterFrequency for finding existing characters.
            ArrayList charFrequencies = new ArrayList();

            try
            {
                // Step 1: Check if the input file exists
                if (!File.Exists(inputFile))
                {
                    Console.WriteLine($"Error: Input file '{inputFile}' not found.");
                    return; // Exit if input file does not exist
                }

                // Step 2: Read the input file character by character
                // Using statement ensures the StreamReader is properly disposed of
                using (StreamReader reader = new StreamReader(inputFile))
                {
                    int charCode; // To store the integer representation of the character
                    // Read characters until the end of the file is reached (reader.Read() returns -1)
                    while ((charCode = reader.Read()) != -1) 
                    {
                        char character = (char)charCode; // Convert the integer back to a char
                        
                        // Create a temporary CharacterFrequency object for searching in the ArrayList
                        CharacterFrequency tempCf = new CharacterFrequency(character);
                        
                        // Check if this character is already in our ArrayList.
                        // This search is necessary because ArrayList does not allow direct indexing by character/ASCII value
                        // in the same way a pre-sized array mapped to ASCII values would.
                        // Instead, IndexOf() iterates and uses the Equals() method of CharacterFrequency.
                        int index = charFrequencies.IndexOf(tempCf);

                        if (index != -1)
                        {
                            // Character exists: retrieve it and increment its frequency
                            CharacterFrequency existingCf = (CharacterFrequency)charFrequencies[index]!; // Non-null assertion
                            existingCf.Increment();
                        }
                        else
                        {
                            // If the character is new, it's added to the ArrayList.
                            // Unlike a fixed array indexed by ASCII, where all possible characters have a slot,
                            // here we only store objects for characters encountered.
                            tempCf.Increment(); 
                            charFrequencies.Add(tempCf);
                        }
                    }
                } // StreamReader is automatically closed here

                // Step 3: Write the character frequencies to the output file
                // Using statement ensures the StreamWriter is properly disposed of
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    writer.WriteLine("Vector - Character(ascii)  Frequency\n");

                    // Iterate through the ArrayList and write each CharacterFrequency object
                    // The ToString() method of CharacterFrequency handles the required output format
                    foreach (CharacterFrequency cf in charFrequencies)
                    {
                        string line = cf.ToString();
                        writer.WriteLine(line);    // Write to file
                        Console.WriteLine(line);   // Display on console
                    }
                } // StreamWriter is automatically closed here

                // Confirmation messages: Print results to console for quick viewing
                Console.WriteLine("\n  * Note: Printed Count.txt file output to Console for Quick Viewing");
                Console.WriteLine("  * Args: " + string.Join(", ", args));
            }
            catch (IOException ioEx)
            {
                // Handle file I/O specific errors
                Console.WriteLine($"A file I/O error occurred: {ioEx.Message}");
            }
            catch (UnauthorizedAccessException uaEx)
            {
                // Handle errors related to file permissions
                Console.WriteLine($"File access denied: {uaEx.Message}");
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            // Keep console window open until user presses Enter
            Console.ReadLine();
        }
    }
}
