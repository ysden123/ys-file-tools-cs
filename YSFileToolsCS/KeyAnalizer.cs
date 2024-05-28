using System.IO;

namespace YSFileToolsCS
{
    internal class KeyAnalizer
    {
        private record CurrentStatus(int Level, KeyWord? LastKeyWord, KeyWord? ParentKeyWord)
        {
            public KeyWord? FindParentKeyWord(int newLevel)
            {
                KeyWord? theParentKeyWord = ParentKeyWord;
                for (int level = Level; level > newLevel && level > 0; --level)
                {
                    theParentKeyWord = theParentKeyWord != null ? theParentKeyWord.Parent : null;
                }
                return theParentKeyWord;
            }
        }
        private readonly string _path;
        public KeyAnalizer(string path)
        {
            _path = path;
        }

        public List<KeyWord> ReadAllKeys()
        {
            var keyWords = new List<KeyWord>();

            var currentStatus = new CurrentStatus(0, null, null);
            foreach (string line in File.ReadLines(_path))
            {
                if (line.Length == 0) continue;
                ExtractKeyWord(line, out string key, out int level);
                KeyWord keyWord;
                if (level == currentStatus.Level)
                {
                    keyWord = new KeyWord(key, level, currentStatus.ParentKeyWord);
                    currentStatus = new CurrentStatus(level, keyWord, currentStatus.ParentKeyWord);
                }
                else if (level > currentStatus.Level)
                {
                    keyWord = new KeyWord(key, level, currentStatus.LastKeyWord);
                    currentStatus = new CurrentStatus(level, keyWord, currentStatus.LastKeyWord);
                }
                else // level < currentStatus.Level
                {
                    var parentKeyWord = currentStatus.FindParentKeyWord(level);
                    keyWord = new KeyWord(key, level, parentKeyWord);
                    currentStatus = new CurrentStatus(level, keyWord, parentKeyWord);
                }
                keyWords.Add(keyWord);
            }
            var result = keyWords.OrderBy(kw => kw.Key).ToList();
            //var result = keyWords;
            return result;
        }

        public int MaxLevel(List<KeyWord> keyWords)
        {
            int theMaxLevel = 0;
            foreach (var keyWord in keyWords)
            {
                if (keyWord.Level > theMaxLevel) { theMaxLevel = keyWord.Level; }
            }
            return theMaxLevel;
        }

        public Dictionary<string, List<KeyWord>> FindDuplicates(List<KeyWord> keyWords)
        {
            var result = new Dictionary<string, List<KeyWord>>();
            foreach (var keyWord in keyWords)
            {
                List<KeyWord>? existingList;
                result.TryGetValue(keyWord.Key, out existingList);
                if (existingList == null)
                {
                    existingList = new List<KeyWord> { keyWord };
                    result[keyWord.Key] = existingList;
                }
                else
                {
                    existingList.Add(keyWord);
                }
            }

            foreach (var entry in result)
            {
                if (entry.Value.Count < 2)
                    result.Remove(entry.Key);
            }
            return result;
        }
        private static void ExtractKeyWord(string line, out string key, out int level)
        {
            var words = line.Split('\t');
            key = words[words.Length - 1];
            level = words.Length;
        }
    }
}
