namespace AmhPorterTest.Models
{
    public class Result
    {

        public List<string> query { get; set; } = new List<string>();

        public HashSet<Int64> resultSet { get; set; } = new HashSet<Int64>();

        public List<(CorpusDocument, double)> resultDocuments { get; set; } = new List<(CorpusDocument, double)> { };

    }
}
