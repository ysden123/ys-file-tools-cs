namespace YSFileToolsCS
{
    internal class AppProperties : YSCommon.Properties
    {
        public static readonly string IMAGES_FOLDER = "imagesFolder";
        public static readonly string KEYWORDS_FILE = "keywordsFile";
        public static readonly string IMAGE_TO_CONVERT_FILE = "imageToConvertFile";
        public static readonly string TO_IMAGE_TO_CONVERT_PATH = "toImageToConvertPath";

        public AppProperties() : base("ysfiletoolscs.properties")
        {
        }
    }
}
