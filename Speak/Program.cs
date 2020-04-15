using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Speech.Synthesis;

namespace Speak {
    class Program {
        static void Main(string[] args)
        {

            // Initialize a new instance of the SpeechSynthesizer.  
            using (SpeechSynthesizer synth = new SpeechSynthesizer()) {

                // Configure the audio output.   
                synth.SetOutputToDefaultAudioDevice();

                // Create a prompt from a string.  
                Prompt color = new Prompt("What is your favorite color?");

                // Speak the contents of the prompt synchronously.  
                synth.Speak(color);
            }

            using (SpeechSynthesizer synth = new SpeechSynthesizer()) {

                // Configure the audio output.   
                synth.SetOutputToDefaultAudioDevice();

                // Create a PromptBuilder object and append a text string.  
                PromptBuilder song = new PromptBuilder();
                song.AppendText("Say the name of the song you want to hear");

                // Speak the contents of the prompt synchronously.  
                synth.Speak(song);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
