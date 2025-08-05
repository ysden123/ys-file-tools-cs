namespace YSFileToolsCS
{
    internal class AppProperties : YSCommon.Properties
    {
        public static readonly string IMAGES_FOLDER = "imagesFolder";
        public static readonly string KEYWORDS_FILE = "keywordsFile";

        public AppProperties() : base("ysfiletoolscs.properties")
        {
        }
    }
}
