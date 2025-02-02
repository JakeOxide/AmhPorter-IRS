using AmhPorterTest.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AmhPorterTest.Utils
{
    public class SystemManager
    {

        private CorpusManager corpusManager { get; set; }

        private Stemmer stemmer { get; set; }

        public SystemManager()
        {

        }

        public bool CheckCorpus()
        {
            if(corpusManager == null)
            {
                corpusManager = new CorpusManager();
                return true;
            }
            return false;
        }

        public void TriggerCorpusPreprocessing()
        {
            if (CheckCorpus())
            {
                if (corpusManager.CheckCorpusDirectoryContentCount())
                {
                    
                    corpusManager.ReadRawFilesFromDisk();
                    corpusManager.TriggerPunctuationRemoval();
                    corpusManager.TriggerTokenization();
                    corpusManager.TriggerStemmer();
                    corpusManager.TriggerStopwordRemoval();
                    corpusManager.TriggerIndexer(0);

                    // To dump results of operations in system directory
                    //corpusManager.WriteRawFiles();
                    //corpusManager.WriteTokenFiles();
                }
            }
        }

        public ResultParentModel CommenceSearch(string query)
        {
            corpusManager.TriggerQueryProcessor(query);
            return corpusManager.TriggerSearch();
        }
    }
}
