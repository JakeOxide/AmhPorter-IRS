namespace AmhPorterTest.Models
{
    public class CorpusDocument
    {

        public Int64 DocumentID { get; set; }

        public string DocumentTitle { get; set; }

        public string DocumentContent { get; set; }

        public List<CustomWord> DocumentTokensOriginal { get; set; }

        public List<CustomWord> DocumentTokens { get; set; }

        public CorpusDocument(Int64 DocID, string DocTitle, string DocContent) { 
            
            DocumentID = DocID;
            DocumentTitle = DocTitle;
            DocumentContent = DocContent;

        }

    }
}
