using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OxfordWordDefinitionTelegramBot
{
    internal class OxfordApiUser
    {
        private readonly Uri _baseAddress = new Uri("https://od-api.oxforddictionaries.com/api/v1/");
        private const string AppId = "15e40921";
        private const string AppKey = "9df2b408307aa00c352472f5ffc9c863";
        private readonly HttpClient _httpClient;
        private readonly Random _rnd;

        private readonly string Name;

        internal OxfordApiUser()
        {
            Name = GetType().Name;

            Logger.LogInformation(Name, "initializing", Logger.Method.Start);

            Logger.LogInformation(Name, "initializing Random", Logger.Method.Start);
            _rnd = new Random();
            Logger.LogInformation(Name, "initializing Random", Logger.Method.End);

            Logger.LogInformation(Name, "initializing HttpClient", Logger.Method.Start);
            _httpClient = new HttpClient
            {
                BaseAddress = _baseAddress
            };
            _httpClient.DefaultRequestHeaders.Add("app_id", AppId);
            _httpClient.DefaultRequestHeaders.Add("app_key", AppKey);
            Logger.LogInformation(Name, "initializing HttpClient", Logger.Method.End);

            Logger.LogInformation(Name, "initializing", Logger.Method.End);
        }

        internal async Task<Word> GetNewWord()
        {
            Logger.LogInformation(Name, "generating new word", Logger.Method.Start);
            var randomWordJson = await GetWordJson(await GetRandomWordListByDomainName(await GetRandomDomainName()));
            var word = new Word(randomWordJson);
            Logger.LogInformation(Name, "generating new word", Logger.Method.End);

            return word;
        }

        private async Task<string> GetWordJson(string givenWord)
        {
            Logger.LogInformation(Name, $"requesting word {{{givenWord}}}", Logger.Method.Start);
            var json = await _httpClient.GetStringAsync($"entries/en/{givenWord}");
            Logger.LogInformation(Name, $"requesting word {{{givenWord}}}", Logger.Method.End);

            return json;
        }


        private async Task<string> GetRandomWordListByDomainName(string domain)
        {
            Logger.LogInformation(Name, $"requesting word list by domain {{{domain}}}", Logger.Method.Start);
            var response = await _httpClient.GetStringAsync($"wordlist/en/domains={domain};lexicalCategory=noun");
            Logger.LogInformation(Name, $"requesting word list by domain {{{domain}}}", Logger.Method.End);

            Logger.LogInformation(Name, "parsing word list JSON", Logger.Method.Start);
            var wordList = JObject.Parse(response)["results"].Select(q => q["word"].ToString()).ToList();
            Logger.LogInformation(Name, $"requested words count = {wordList.Count}", Logger.Method.Info);
            Logger.LogInformation(Name, "parsing word list JSON", Logger.Method.End);

            return wordList.ElementAt(GetRandom(wordList.Count - 1));
        }


        private async Task<string> GetRandomDomainName()
        {
            Logger.LogInformation(Name, "requesting domain list", Logger.Method.Start);
            var response = await _httpClient.GetStringAsync("domains/en");
            Logger.LogInformation(Name, "requesting domain list", Logger.Method.End);

            Logger.LogInformation(Name, "parsing domain list JSON", Logger.Method.Start);
            var domains = JObject.Parse(response)["results"].Select(q => ((JProperty)q).Name).ToList();
            Logger.LogInformation(Name, $"requested domains count = {domains.Count}", Logger.Method.Info);
            Logger.LogInformation(Name, "parsing domain list JSON", Logger.Method.End);

            return domains.ElementAt(GetRandom(domains.Count - 1));
        }

        private int GetRandom(int max)
        {
            Logger.LogInformation(Name, $"random for max = {max}", Logger.Method.Info);

            while (true)
            {
                try
                {
                    return _rnd.Next(0, max);
                }
                catch (Exception)
                {
                    Logger.LogInformation(Name, $"exception caught in GetRandom for max = {max}", Logger.Method.Error);
                }
            }
        }


    }
}
