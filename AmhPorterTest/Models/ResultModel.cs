namespace AmhPorterTest.Models
{
    public record ResultParentModel(List<string> query, HashSet<Int64> resultSet, List<(CorpusDocument, double)> resultDocuments);

}
