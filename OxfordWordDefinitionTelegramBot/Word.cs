using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace OxfordWordDefinitionTelegramBot
{
    internal class Word
    {
        private string Name;

        public string Definition { get; set; }
        public string AudioFile { get; set; }
        public string PhoneticSpelling { get; set; }

        public List<string> Etymologies { get; set; }
        public List<string> Senses { get; set; }
        public List<string> SubSenses { get; set; }

        internal Word(string json)
        {
            Name = GetType().Name;

            Logger.LogInformation(Name, "initializing", Logger.Method.Start);
            Logger.LogInformation(Name, "parsing word description JSON", Logger.Method.Start);

            //shortcuts
            var results = JObject.Parse(json)?["results"]?[0];
            var pronunciations = results?["lexicalEntries"]?[0]?["pronunciations"]?[0];
            var entries = results?["lexicalEntries"]?[0]?["entries"]?[0];


            //main data
            Definition = results?["word"]?.ToString();
            AudioFile = pronunciations?["audioFile"]?.ToString();
            PhoneticSpelling = pronunciations?["phoneticSpelling"]?.ToString();
            Etymologies = entries?["etymologies"]?.Select(q => q?.ToString()).ToList();
            Senses = entries?["senses"]?.Select(q => q["definitions"][0]?.ToString()).ToList();
            SubSenses = entries?["senses"]?[0]?["subsenses"]?.Select(q => q["definitions"]?[0]?.ToString()).ToList();

            Logger.LogInformation(Name, "parsing word description JSON", Logger.Method.End);
            Logger.LogInformation(Name, "initializing", Logger.Method.End);
        }

        internal string GenerateMessage()
        {
            Logger.LogInformation(Name, $"generating message for word {{{Definition}}}", Logger.Method.Start);

            var sb = new StringBuilder();
            var hr = new string('-', 39);

            sb.Append($"{hr}\n");

            if (Definition != null)
            {               
                sb.Append($"\nNEW WORD\n{Definition}\n\n");
                sb.Append($"{hr}\n");
            }

            if (PhoneticSpelling != null)
            {
                sb.Append($"\nPHONETIC SPELLING\n{PhoneticSpelling}\n\n");
                sb.Append($"{hr}\n");
            }

            if (Etymologies?.Any() ?? false)
            {
                sb.Append("\nETYMOLOGIES\n");
                foreach (var etymology in Etymologies)
                    sb.Append($"\u2022 {etymology}\n\n");
                sb.Append($"{hr}\n");
            }

            if (Senses?.Any() ?? false)
            {
                sb.Append("\nSENSES\n");
                foreach (var sense in Senses)
                    sb.Append($"\u2022 {sense}\n\n");
                sb.Append($"{hr}\n");
            }

            if (SubSenses?.Any() ?? false)
            {
                sb.Append("\nSUBSENSES\n");
                foreach (var subSense in SubSenses)
                    sb.Append($"\u2022 {subSense}\n\n");
                sb.Append($"{hr}\n");
            }

            if (AudioFile != null)
            {
                sb.Append($"\nAUDIO\n{AudioFile}\n\n");
                sb.Append($"{hr}\n");
            }
            
            Logger.LogInformation(Name, $"generating message for word {{{Definition}}}", Logger.Method.End);

            return sb.ToString();
        }
    }
}
