using AmhPorterTest.Models;
using Newtonsoft.Json;

namespace AmhPorterTest.Utils
{
    public class Indexer
    {

        public Dictionary<string, HashSet<Int64>> index { get; set; } = new Dictionary<string, HashSet<Int64>>();


        public void AddDocument(Int64 docId, List<CustomWord> content)
        {
            //string[] words = content.ToLower().Split(new char[] { ' ', '.', ',', '!' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (CustomWord word in content)
            {
                if (!index.ContainsKey(word.word))
                    index[word.word] = new HashSet<Int64>();

                index[word.word].Add(docId);
            }
        }

        public HashSet<Int64> Search(string query)
        {
            string word = query.ToLower();
            return index.ContainsKey(word) ? index[word] : new HashSet<Int64>();
        }


        public void RemoveDocument(int docId)
        {
            foreach (var term in index.Keys)
            {
                index[term].Remove(docId);
                if (index[term].Count == 0)
                    index.Remove(term); // Remove term if no docs left
            }
        }


        // Save index to JSON file
        public void SaveToFile(string filename)
        {
            string json = JsonConvert.SerializeObject(index, Formatting.Indented);
            File.WriteAllText(filename, json);
            Console.WriteLine($"Index saved to {filename}");
        }

        // Load index from JSON file
        public void LoadFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                string json = File.ReadAllText(filename);
                index = JsonConvert.DeserializeObject<Dictionary<string, HashSet<Int64>>>(json);
                Console.WriteLine($"Index loaded from {filename}");
            }
            else
            {
                Console.WriteLine($"File {filename} not found. Starting with an empty index.");
            }
        }


        public void PrintIndex()
        {
            foreach (var entry in index)
            {
                Console.WriteLine($"{entry.Key}: {string.Join(", ", entry.Value)}");
            }
        }

    }
}
