using AmhPorterTest.Models;
using System.Text;

namespace AmhPorterTest.Utils
{
    public class StopwordRemover
    {

        public List<CorpusDocument> inputDocuments { get; set; }

        public List<string> stopWords { get; set; }

        public StopwordRemover(List<CorpusDocument> inputDocuments)
        {
            this.inputDocuments = inputDocuments;
        }

        public List<CorpusDocument> RemoveStopWords()
        {
            GetStopWords();
            for (int i = 0; i < stopWords.Count; i++)
            {
                foreach(var doc in  inputDocuments)
                {
                    doc.DocumentTokens.RemoveAll(x => x.word.Equals(stopWords[i]));
                }
            }
            return inputDocuments;
        }

        private void GetStopWords()
        {
            stopWords = File.ReadAllText
                (
                    Path.Combine
                    (
                        Environment.CurrentDirectory, "SystemData", "runtime", "Stopwords.txt"
                    ), 
                    encoding: Encoding.UTF8
                ).Split(',').ToList();
        }
    }
}
