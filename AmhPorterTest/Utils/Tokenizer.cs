using AmhPorterTest.Models;
using System.Reflection.Metadata;

namespace AmhPorterTest.Utils
{
    public class Tokenizer
    {

        List<CorpusDocument> documents { get; set; }

        List<CustomWord> customWords { get; set; }

        public Tokenizer(List<CorpusDocument> documents)
        {
            this.documents = documents;
        }


        public List<CorpusDocument> Tokenize() 
        {
            List<string> tokens = new List<string>();

            for (int i = 0; i < documents.Count; i++)
            {
                customWords = new List<CustomWord>();   
                tokens = documents.ElementAt(i).DocumentContent.Split(new char[] { ' ', '\t', '\n', '\r', }, StringSplitOptions.RemoveEmptyEntries).ToList();
                for (int j = 0; j < tokens.Count; j ++)
                {
                    customWords.Add(new CustomWord(tokens.ElementAt(j)));
                }
                documents.ElementAt(i).DocumentTokens = customWords;
                documents.ElementAt(i).DocumentTokensOriginal = customWords;
            }

            return documents;
        }
    }
}
