namespace Modules.Works.Domain.Exceptions
{
    public class WorkUploadingFailedException(string fileName)
        : Exception($"Failed to upload file: \"{fileName}\"");
}