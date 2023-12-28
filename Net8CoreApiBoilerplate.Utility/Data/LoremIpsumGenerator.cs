using System;
using System.Text;
using Net8CoreApiBoilerplate.Utility.Extensions;

namespace Net8CoreApiBoilerplate.Utility.Data
{
    public class LoremIpsumGenerator
    {
        private readonly Random _rand;

        public LoremIpsumGenerator(Random rand = null)
        {
            _rand = rand ?? new Random();
        }

        public string Get(int minParagraphs = 1, int maxParagraphs = 3, int minSentencesPerParagraph = 2, int maxSentencesPerParagraph = 8,
            int minWordsPerSentence = 2, int maxWordsPerSentence = 10, bool wrapPTag = false)
        {
            return GetParagraphs(minParagraphs, maxParagraphs, minSentencesPerParagraph, maxSentencesPerParagraph,
                minWordsPerSentence, maxWordsPerSentence, wrapPTag);
        }

        public string GetParagraphs(int min, int max, int minSentences, int maxSentences, int minWordsPerSentence, int maxWordsPerSentence, bool wrapPTag)
        {
            return GetParagraphs(_rand.Next(min, max), minSentences, maxSentences, minWordsPerSentence, maxWordsPerSentence, wrapPTag);
        }

        public string GetParagraphs(int count, int minSentences, int maxSentences, int minWordsPerSentence, int maxWordsPerSentence, bool wrapPTag)
        {
            StringBuilder sb = new StringBuilder();

            for (int p = 0; p < count; p++)
            {
                if (p > 0) sb.Append(Environment.NewLine);
                if (wrapPTag) sb.Append("<p>");

                sb.Append(GetParagraph(minSentences, maxSentences, minWordsPerSentence, maxWordsPerSentence));

                if (wrapPTag) sb.Append("</p>");
            }

            return sb.ToString();
        }

        public string GetParagraph(int minSentences, int maxSentences, int minWordsPerSentence, int maxWordsPerSentence)
        {
            StringBuilder sb = new StringBuilder();
            int sentenceCount = _rand.Next(minSentences, maxSentences);

            for (int s = 0; s < sentenceCount; s++)
            {
                if (s > 0) sb.Append(" ");
                sb.Append(GetSentence(minWordsPerSentence, maxWordsPerSentence));
            }

            return sb.ToString();
        }

        public string GetSentence(int minWords, int maxWords)
        {
            return GetSentence(_rand.Next(minWords, maxWords));
        }

        public string GetSentence(int wordCount)
        {
            string sentence = GetWords(wordCount);
            return char.ToUpper(sentence[0]) + sentence.Substring(1) + '.';
        }

        public string GetWords(int min, int max)
        {
            return GetWords(_rand.Next(min, max));
        }

        public string GetWord()
        {
            int randomIndex = _rand.Next(Words.Length);
            return Words[randomIndex];
        }

        public string GetWords(int count)
        {
            StringBuilder sb = new StringBuilder();

            for (int w = 0; w < count; w++)
            {
                if (w > 0) sb.Append(" ");
                sb.Append(GetWord());
            }

            return sb.ToString();
        }

        public string GetIp()
        {
            const string ipTempate = "{0}.{1}.{2}.{3}";
            return string.Format(ipTempate, _rand.Next(0, 255), _rand.Next(0, 255), _rand.Next(0, 255), _rand.Next(0, 255));
        }

        public string GetEmail()
        {
            string name;
            int number = _rand.Next(1, 3);

            if (number == 2) name = string.Format("{0}.{1}", GetWordsAsParams(2));
            else if (number == 3) name = string.Format("{0}.{1}.{2}", GetWordsAsParams(3));
            else name = GetWords(1);

            string domain = _rand.Next(1, 2) == 1
                ? string.Format("{0}.{1}", GetWordsAsParams(2))
                : string.Format("{0}.{1}.{2}", GetWordsAsParams(3));

            return name + "@" + domain;
        }

        public string GetFullName()
        {
            string[] words = GetArrayOfWords(2);
            string w1 = char.ToUpper(words[0][0]) + words[0].Substring(1);
            string w2 = char.ToUpper(words[1][0]) + words[1].Substring(1);

            return w1 + " " + w2;
        }

        public string GetName()
        {
            string word = GetWord();
            return char.ToUpper(word[0]) + word.Substring(1);
        }

        public string GetName(int minCount)
        {
            string word;
            do
            {
                word = GetWord();

            } while (word.Length < minCount);

            return char.ToUpper(word[0]) + word.Substring(1);
        }

        public int GetNumber(int min = 1, int max = 100)
        {
            return _rand.Next(min, max);
        }

        public string GetPhoneNumber()
        {
            string[] pre = { "061", "062", "063" };
            const string format = "{0}-{1}-{2}";

            return String.Format(format, _rand.NextElement(pre), _rand.Next(100, 999), _rand.Next(100, 999));
        }

        public string GetCharacters(int count)
        {
            var result = new StringBuilder();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < count; i++) result.Append(chars[i]);

            return result.ToString();
        }

        public string GetUrl()
        {
            return @"http://www." + GetWords(1) + ".com/" + GetWords(1) + "/" + GetWords(1) + "?" + GetWords(1) + "=" + GetWords(1);
        }

        private string[] GetArrayOfWords(int count)
        {
            string[] array = new string[count];
            for (int i = 0; i < count; i++) array[i] = Words[_rand.Next(Words.Length)];

            return array;
        }

        private object[] GetWordsAsParams(int count)
        {
            object[] array = new object[count];
            for (int i = 0; i < count; i++) array[i] = Words[_rand.Next(Words.Length)];

            return array;
        }

        private static readonly string[] Words =
        {
            "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
            "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
            "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
            "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet",
            "lorem", "ipsum", "dolor", "sit", "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
            "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
            "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
            "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet",
            "lorem", "ipsum", "dolor", "sit", "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod",
            "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua",
            "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita",
            "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "duis",
            "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate", "velit", "esse", "molestie",
            "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at", "vero", "eros", "et",
            "accumsan", "et", "iusto", "odio", "dignissim", "qui", "blandit", "praesent", "luptatum", "zzril", "delenit",
            "augue", "duis", "dolore", "te", "feugait", "nulla", "facilisi", "lorem", "ipsum", "dolor", "sit", "amet",
            "consectetuer", "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet",
            "dolore", "magna", "aliquam", "erat", "volutpat", "ut", "wisi", "enim", "ad", "minim", "veniam", "quis",
            "nostrud", "exerci", "tation", "ullamcorper", "suscipit", "lobortis", "nisl", "ut", "aliquip", "ex", "ea",
            "commodo", "consequat", "duis", "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate",
            "velit", "esse", "molestie", "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at",
            "vero", "eros", "et", "accumsan", "et", "iusto", "odio", "dignissim", "qui", "blandit", "praesent", "luptatum",
            "zzril", "delenit", "augue", "duis", "dolore", "te", "feugait", "nulla", "facilisi", "nam", "liber", "tempor",
            "cum", "soluta", "nobis", "eleifend", "option", "congue", "nihil", "imperdiet", "doming", "id", "quod", "mazim",
            "placerat", "facer", "possim", "assum", "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing",
            "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam",
            "erat", "volutpat", "ut", "wisi", "enim", "ad", "minim", "veniam", "quis", "nostrud", "exerci", "tation",
            "ullamcorper", "suscipit", "lobortis", "nisl", "ut", "aliquip", "ex", "ea", "commodo", "consequat", "duis",
            "autem", "vel", "eum", "iriure", "dolor", "in", "hendrerit", "in", "vulputate", "velit", "esse", "molestie",
            "consequat", "vel", "illum", "dolore", "eu", "feugiat", "nulla", "facilisis", "at", "vero", "eos", "et", "accusam",
            "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita", "kasd", "gubergren", "no", "sea",
            "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
            "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod", "tempor", "invidunt", "ut",
            "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed", "diam", "voluptua", "at", "vero", "eos", "et",
            "accusam", "et", "justo", "duo", "dolores", "et", "ea", "rebum", "stet", "clita", "kasd", "gubergren", "no",
            "sea", "takimata", "sanctus", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
            "amet", "consetetur", "sadipscing", "elitr", "at", "accusam", "aliquyam", "diam", "diam", "dolore", "dolores",
            "duo", "eirmod", "eos", "erat", "et", "nonumy", "sed", "tempor", "et", "et", "invidunt", "justo", "labore",
            "stet", "clita", "ea", "et", "gubergren", "kasd", "magna", "no", "rebum", "sanctus", "sea", "sed", "takimata",
            "ut", "vero", "voluptua", "est", "lorem", "ipsum", "dolor", "sit", "amet", "lorem", "ipsum", "dolor", "sit",
            "amet", "consetetur", "sadipscing", "elitr", "sed", "diam", "nonumy", "eirmod", "tempor", "invidunt", "ut",
            "labore", "et", "dolore", "magna", "aliquyam", "erat", "consetetur", "sadipscing", "elitr", "sed", "diam",
            "nonumy", "eirmod", "tempor", "invidunt", "ut", "labore", "et", "dolore", "magna", "aliquyam", "erat", "sed",
            "diam", "voluptua", "at", "vero", "eos", "et", "accusam", "et", "justo", "duo", "dolores", "et", "ea",
            "rebum", "stet", "clita", "kasd", "gubergren", "no", "sea", "takimata", "sanctus", "est", "lorem", "ipsum"
        };
    }
}
