using AmhPorterTest.Models;
using System;

namespace AmhPorterTest.Utils
{
    public class SearchProcessor
    {

        public List<string> query { get; set; }
        public Indexer indexer { get; set; }
        public string operation { get; set; }  
        public Result result { get; set; } = new Result();
        public List<CorpusDocument> documents { get; set; } 



        public SearchProcessor(List<string> query, Indexer indexer, string operation, List<CorpusDocument> documents)
        {
            this.query = query;
            this.indexer = indexer; 
            this.operation = operation;
            this.documents = documents;
        }


        public Result CommenceSearch()
        {
            if(string.IsNullOrEmpty(operation))
            {
                operation = "እና";
            }

            result.resultSet = BooleanSearch();

            var rankingResult = RankByTFIDF();

            foreach(var rank in rankingResult)
            {
                result.resultDocuments.Add((documents.Find(x => x.DocumentID.Equals(rank.Key)), rank.Value));
            }

            result.query = query;

            return result;

        }

        // Boolean Search

        // Boolean Search: AND, OR, NOT
        private HashSet<Int64> BooleanSearch()
        {
            if (query.Count < 1) return new HashSet<Int64>(); // Need at least 2 words

            HashSet<Int64> resultSet = indexer.index.ContainsKey(query[0]) ? new HashSet<Int64>(indexer.index[query[0]]) : new HashSet<Int64>();

            for (int i = 1; i < query.Count; i++)
            {
                if (!indexer.index.ContainsKey(query[i])) continue;

                HashSet<Int64> termDocs = new HashSet<Int64>(indexer.index[query[i]]);

                if (operation == "እና")
                    resultSet.IntersectWith(termDocs); // Common docs
                else if (operation == "ወይም")
                    resultSet.UnionWith(termDocs); // Combine docs
                else if (operation == "NOT")
                    resultSet.ExceptWith(termDocs); // Exclude docs
            }

            return resultSet;
        }


        // TF

        // Compute Term Frequency (TF)
        private double ComputeTF(string term, List<string> words)
        {
            int termCount = words.Count(w => w == term);
            return (double)termCount / words.Count;
        }


        // IDF

        // Compute Inverse Document Frequency (IDF)
        private double ComputeIDF(string term)
        {
            if (!indexer.index.ContainsKey(term)) return 0;
            int docCount = indexer.index[term].Count;
            double result = (double)documents.Count / docCount;
            return Math.Log10(result);
        }
        

        // Rank using Tf & IDF

        // TF-IDF Ranking *after* Boolean Search
        private Dictionary<Int64, double> RankByTFIDF()
        {
            Dictionary<Int64, double> scores = new Dictionary<Int64, double>();



            foreach (var docId in result.resultSet) // Only rank the filtered docs
            {
                var doc = documents.Find(x => x.DocumentID == docId);

                double score = 0;
                for (int i = 0; i < query.Count; i++) 
                {
                    double tf = ComputeTF(query[i], doc.DocumentTokens.Select(x => x.word).ToList());
                    double idf = ComputeIDF(query[i]);
                        
                    score += tf * idf;
                }
                scores[docId] = score;
            }

            return scores.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }


    }
}
