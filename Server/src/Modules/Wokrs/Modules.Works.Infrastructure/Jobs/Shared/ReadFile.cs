namespace Modules.Works.Infrastructure.Jobs.Shared
{
    public static class ReadFile
    {
        public static Stream Read(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("The specified file does not exist.", filePath);

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return stream;
        }
    }
}