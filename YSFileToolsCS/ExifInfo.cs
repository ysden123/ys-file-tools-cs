namespace YSFileToolsCS
{
    internal class ExifInfo
    {
        private readonly Dictionary<string, string> Tags = new()  
        {
            {"Exposure Time", string.Empty },
            {"F-Number", string.Empty },
            {"ISO Speed Ratings", string.Empty },
            {"Exposure Bias Value", string.Empty },
            {"Focal Length", string.Empty },
            {"Focal Length 35", string.Empty },
        };

        public void AddTag(string tagName, string? tagValue)
        {
            if (Tags.ContainsKey(tagName) && tagValue != null)
            {
                Tags[tagName] = tagValue;
            }
        }

        public override string ToString()
        {
            List<string> tagStrings = new();
            foreach (var kvp in Tags)
            {
                tagStrings.Add($"{kvp.Key}: {kvp.Value}");
            }
            return string.Join(", ", tagStrings);
        }
    }
}
