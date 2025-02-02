using AmhPorterTest.Models;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace AmhPorterTest.Utils
{
    public class CorpusManager
    {

        /* enum Paths
        {
            corpusPath,
            sysDirPath,
            runRawFilePath,
            corpus,
        }*/

        private string? workingDirectory { get; set; }
        private string? sysDirPath { get; set; }
        private string? sysDirStoragePath { get; set; }
        private string? sysDirRuntimePath { get; set; }
        private string? sysDirRuntimeTempPath { get; set; }
        private string? resourceCorpusPath { get; set; }
        private string? resourceCorpusBackupPath { get; set; }
        private string? resourceStopwordsPath { get; set; }
        //private string runRawFilePath { get; set; }
        private string? runTokenFilePath { get; set; }
        private string? corpusPath { get; set; }
        public List<string>? corpus { get; set; }
        public List<CorpusDocument>? documents{ get; set; }
        private Tokenizer? tokenizer { get; set; }
        private StopwordRemover? stopwordRemover { get; set; }
        private Stemmer stemmer { get; set; }
        public Indexer? indexer { get; set; }
        private QueryProcessor? queryProcessor { get; set; }
        public string operation { get; set; }
        private SearchProcessor searchProcessor { get; set; }
        private List<string>? queries { get; set; }
        public Result searchResult { get; set; }    

        private string line = "----------------------------------------------------------------------------------------------------";
        private string lineShort = "--------------------------------------------------";


        public CorpusManager()
        {

            workingDirectory = Environment.CurrentDirectory;
            Directory.CreateDirectory(Path.Combine(workingDirectory, "Resource", "Corpus"));
            Directory.CreateDirectory(Path.Combine(workingDirectory, "Resource", "CorpusBackup"));
            Directory.CreateDirectory(Path.Combine(workingDirectory, "Resource", "Stopwords"));
            resourceCorpusPath = Path.Combine(workingDirectory, "Resource", "Corpus");
            resourceCorpusBackupPath = Path.Combine(workingDirectory, "Resource", "CorpusBackup");
            resourceStopwordsPath = Path.Combine(workingDirectory, "Resource", "Stopwords");
            Directory.CreateDirectory(Path.Combine(workingDirectory, "SystemData", "storage"));
            Directory.CreateDirectory(Path.Combine(workingDirectory, "SystemData", "runtime"));
            Directory.CreateDirectory(Path.Combine(workingDirectory, "SystemData", "runtime", "temp"));
            sysDirPath = Path.Combine(workingDirectory, "SystemData");
            sysDirRuntimePath = Path.Combine(workingDirectory, "SystemData", "runtime");
            sysDirRuntimeTempPath = Path.Combine(workingDirectory, "SystemData", "runtime", "temp");
            sysDirStoragePath = Path.Combine(workingDirectory, "SystemData", "storage");
            corpusPath = Path.Combine(workingDirectory, "Resource", "Corpus");
            //runRawFilePath = Path.Combine(sysDirRuntimeTempPath, "rawData.txt");
            runTokenFilePath = Path.Combine(workingDirectory, "SystemData", "runtime", "temp");
            WriteResources();
        }

        public bool CheckCorpusDirectoryContentCount()
        {
            corpus = Directory.EnumerateFiles(corpusPath).ToList();
            if(corpus.Count > 1) return true;
            return false;
        }

        public void TriggerPunctuationRemoval()
        {
            for (int i = 0; i < documents.Count;i++)
            {
                string extraPunctuation = "!.,؛،፤፣።";          // "፣፤!.,؛،•'።"; 
                string pattern = "[\\p{P}\\p{S}" + Regex.Escape(extraPunctuation) + "\\dA-Za-z]";
                documents.ElementAt(i).DocumentContent = Regex.Replace(documents.ElementAt(i).DocumentContent, pattern, "");
                //Regex.Replace(documents.ElementAt(i).DocumentContent, "[\\p{P}\\p{S}]", "");
            }
        }

        public void TriggerTokenization()
        {
            tokenizer = new Tokenizer(documents);
            documents = tokenizer.Tokenize();
        }

        public void TriggerStopwordRemoval()
        {
            stopwordRemover = new StopwordRemover(documents);
            documents = stopwordRemover.RemoveStopWords();
        }

        public void TriggerStemmer()
        {
            stemmer = new Stemmer(documents);
            documents = stemmer.StemWords();
        }

        public void TriggerIndexer(int action)
        {
            if(action == 0)
            {
                indexer = new Indexer();

                for (int i = 0; i < documents.Count; i++)
                {
                    indexer.AddDocument(documents.ElementAt(i).DocumentID, documents.ElementAt(i).DocumentTokens);
                }
                indexer.SaveToFile(Path.Combine(sysDirRuntimeTempPath, "index.txt"));
            }
            else
            {
                indexer = new Indexer();
                indexer.LoadFromFile(Path.Combine(sysDirRuntimeTempPath, "index.txt"));
            }

        }

        public void TriggerQueryProcessor(string query)
        {
            queryProcessor = new QueryProcessor(query);
            queries = queryProcessor.ProcessQuery().Item1;
            operation = queryProcessor.ProcessQuery().Item2;
        }

        public ResultParentModel TriggerSearch()
        {
            if (queries.Count == 0) return new ResultParentModel(null, new HashSet<Int64>(), new List<(CorpusDocument, double)>());
            searchProcessor = new SearchProcessor(queries, indexer, operation, documents);
            searchResult = searchProcessor.CommenceSearch();
            var result = searchResult.resultDocuments;
            Console.WriteLine("--- Results ---");
            foreach ( var document in result )
            {
                Console.WriteLine($"{document.Item1.DocumentTitle} -> {document.Item2}");
            }

            ResultParentModel rm = new ResultParentModel(searchResult.query, searchResult.resultSet, searchResult.resultDocuments);
            return rm;
        }

















        // -------------------------------------- File Ops ----------------------------------------

        public void ReadRawFilesFromDisk()
        {

            documents = new List<CorpusDocument>();
            for(int i = 0; i < corpus.Count; i++)
            {
                documents.Add
                    (
                        new CorpusDocument
                        (
                            Convert.ToInt64(DateTime.Now.ToString("dfHHmsfffff")), //2,123,456,789
                            //Path.GetFileName(corpus.ElementAt(i))
                            Path.GetFileNameWithoutExtension(corpus.ElementAt(i)),
                            File.ReadAllText(Path.Combine(corpusPath, corpus.ElementAt(i)), Encoding.UTF8)
                        )
                    );
            }
        }

        /*public void ReadRawFilesFromStorage()
        {

            var tempCorpus = Directory.EnumerateFiles(runRawFilePath).ToList();
            if (tempCorpus.Count < 2) return;



            if (documents.Count > 0) documents.Clear();
            documents = new List<CorpusDocument>();
            for (int i = 0; i < tempCorpus.Count; i++)
            {
                documents.Add
                    (
                        new CorpusDocument
                        (
                            tempCorpus.ElementAt(i),
                            //Path.GetFileName(corpus.ElementAt(i))
                            Path.GetFileNameWithoutExtension(tempCorpus.ElementAt(i)),
                            File.ReadAllText(Path.Combine(workingDirectory, tempCorpus.ElementAt(i)), Encoding.UTF8)
                        )
                    );
            }
        }*/

        /* public void WriteFiles()
         {
             for(int i = 0; i < corpus.Count;i++)
             {
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, line, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, documents.ElementAt(i).DocumentTitle.Trim(), Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, lineShort, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, documents.ElementAt(i).DocumentContent, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, lineShort, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(runRawFilePath, Environment.NewLine, Encoding.UTF8);
             }
         }*/

        public void WriteTokenFiles()
         {
             for (int i = 0; i < corpus.Count; i++)
             {
                 string path = Path.Combine(sysDirRuntimeTempPath, $"{documents.ElementAt(i).DocumentTitle}_Tokens.txt");
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, line, Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, documents.ElementAt(i).DocumentTitle.Trim(), Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, lineShort, Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllLines(path, documents.ElementAt(i).DocumentTokens.Select(x => x.word).ToList(), Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, lineShort, Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                 File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
             }
         }

        public void WriteRawFiles()
        {
            for (int i = 0; i < corpus.Count; i++)
            {
                //string path = Path.Combine(sysDirRuntimeTempPath, $"{documents.ElementAt(i).DocumentTitle}_{(i)}.txt");
                string path = Path.Combine(sysDirRuntimeTempPath, $"{documents.ElementAt(i).DocumentTitle}.txt");
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, line, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, documents.ElementAt(i).DocumentTitle.Trim(), Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, lineShort, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, documents.ElementAt(i).DocumentContent, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, lineShort, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
                File.AppendAllText(path, Environment.NewLine, Encoding.UTF8);
            }
        }

        private void WriteResources()
        {


            string stopwords = "ስለዚህ,እኔም,በጣም,ይችላል,ይሆናል,በቃ,አሁን,ራሴ,እኛ,የእኛ,እራሳችን,እና,ስለዚህ,በመሆኑም,ሁሉ,ሆነ,ሌላ,ልክ,ስለ,በቀር,ብቻ," +
                "ና,አንዳች,አንድ,እንደ,ስለሚሆን,እንጂ,ያህል,ይልቅ,ወደ,እኔ,የእኔ,ያደርጋል,አደረገው,መሥራት,እና,ግን,ከሆነ,ወይም,ምክንያቱም," +
                "እንደ,እስከ,ቢሆንም,ጋር,ላይ,መካከል,በኩል,ወቅት,በኋላ,ከላይ,በርቷል,ጠፍቷል,በላይ,አንቺ,የእርስዎ,ራስህ,ራሳችሁ,እሱ,እሱን,የእሱ,ራሱ,እርሷ,የእሷ,ራሷ," +
                "እነሱ,እነሱን,የእነሱ,እራሳቸው,ምንድን,የትኛው,ማንን,ይህ,እነዚህ,እነዚያ,ነኝ,ነው,ናቸው,ነበር,ነበሩ,ሁን,ነበር,መሆን,አለኝ,አለው," +
                "ነበረ,መኖር,ስር,እንደገና,ተጨማሪ,ከዚያ,አንዴ,እዚህ,እዚያ,መቼ,የት,እንዴት,ሁሉም,ማናቸውም,ሁለቱም,እያንዳንዱ,ጥቂቶች,ተጨማሪ,በጣም,ሌላ,አንዳንድ," +
                "አይ,ወይም,አይደለም,ብቻ,የራስ,ተመሳሳይ";

            string stopwordsQuery = "ስለዚህ,እኔም,በጣም,ይችላል,ይሆናል,በቃ,አሁን,ራሴ,እኛ,የእኛ,እራሳችን,እና,ስለዚህ,በመሆኑም,ሁሉ,ሆነ,ሌላ,ልክ,ስለ,በቀር,ብቻ," +
                "ና,አንዳች,አንድ,እንደ,ስለሚሆን,እንጂ,ያህል,ይልቅ,ወደ,እኔ,የእኔ,ያደርጋል,አደረገው,መሥራት,እና,ግን,ከሆነ,ምክንያቱም,እንደ,እስከ,ቢሆንም,ጋር,ላይ,መካከል," +
                "በኩል,ወቅት,በኋላ,ከላይ,በርቷል,ጠፍቷል,በላይ,አንቺ,የእርስዎ,ራስህ,ራሳችሁ,እሱ,እሱን,የእሱ,ራሱ,እርሷ,የእሷ,ራሷ,እነሱ,እነሱን,የእነሱ,እራሳቸው,ምንድን,የትኛው," +
                "ማንን,ይህ,እነዚህ,እነዚያ,ነኝ,ነው,ናቸው,ነበር,ነበሩ,ሁን,ነበር,መሆን,አለኝ,አለው,ነበረ,መኖር,ስር,እንደገና,ተጨማሪ,ከዚያ,አንዴ,እዚህ,እዚያ,መቼ,የት,እንዴት," +
                "ሁሉም,ማናቸውም,ሁለቱም,እያንዳንዱ,ጥቂቶች,ተጨማሪ,በጣም,ሌላ,አንዳንድ,አይ,አይደለም,ብቻ,የራስ,ተመሳሳይ";


            File.WriteAllText(Path.Combine(resourceStopwordsPath, "Stopwords.txt"), stopwords);
            File.WriteAllText(Path.Combine(resourceStopwordsPath, "StopwordsQuery.txt"), stopwordsQuery);
            File.WriteAllText(Path.Combine(sysDirRuntimePath, "Stopwords.txt"), stopwords);
            File.WriteAllText(Path.Combine(sysDirRuntimePath, "StopwordsQuery.txt"), stopwordsQuery);


        }

    }
}
