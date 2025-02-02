using System.Text;

namespace AmhPorterTest.Utils
{
    public class QueryProcessor
    {

        private string inputQuery { get; set; }

        private List<string> tokens { get; set; }

        public List<string> stopwords { get; set; }
        private string operation { get; set; }

        public QueryProcessor(string query)
        {
            inputQuery = query;
        }

        public (List<string>, string) ProcessQuery()
        {
            if (inputQuery == null || string.IsNullOrEmpty(inputQuery)) return (new List<string>(), "");
            Tokenize();
            RemoveStopWords();
            IdentifyOperation();
            return (tokens, operation);
        }

        private void Tokenize()
        {
            tokens = new List<string>();
            tokens = inputQuery.Split(new char[] { ' ', '\t', '\n', '\r', }, StringSplitOptions.RemoveEmptyEntries).ToList();
            /*
                for (int i = 0; i < documents.Count; i++)
                {
                    tempQuery = new List<string>();
                    for (int j = 0; j < tokens.Count; j++)
                    {
                        customWords.Add(new CustomWord(tokens.ElementAt(j)));
                    }
                    documents.ElementAt(i).DocumentTokens = customWords;
                }
                return documents;
            */
        }

        private void RemoveStopWords()
        {
            GetStopWords();
            for (int i = 0; i < stopwords.Count; i++)
            {
                //for (int j = 0; j < tokens.Count; j++)
                //{
                tokens.RemoveAll(x => x.Equals(stopwords[i]));
                //}
            }
        }

        private void GetStopWords()
        {
            stopwords = File.ReadAllText
                (
                    Path.Combine
                    (
                        Environment.CurrentDirectory, "SystemData", "runtime", "StopwordsQuery.txt"
                    ),
                    encoding: Encoding.UTF8
                ).Split(',').ToList();
        }

        private void IdentifyOperation()
        {
            if (tokens.Contains("እና"))
            {
                operation = "እና";
                return;
            }

            else if (tokens.Contains("ወይም"))
            {
                operation = "ወይም";
                return;
            }

            else if (tokens.Contains("NOT"))
            {
                operation = "NOT";
                return;
            }

            operation = "እና";
        }

    }
}
