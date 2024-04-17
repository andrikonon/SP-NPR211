namespace Helpers;

public static class Copier
{
    public static void Copy(
        string sourceFilePath,
        string destinationFilePath,
        string destinationFileName = null)
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath))
            throw new ArgumentException("sourceFilePath cannot be null or whitespace", nameof(sourceFilePath));
        
        if (string.IsNullOrWhiteSpace(destinationFilePath))
            throw new ArgumentException("destinationFilePath cannot be null or whitespace", nameof(destinationFilePath));

        var targetDirectoryInfo = new DirectoryInfo(destinationFilePath);

        if (!targetDirectoryInfo.Exists)
            targetDirectoryInfo.Create();

        var fileName = string.IsNullOrWhiteSpace(destinationFileName)
            ? Path.GetFileName(sourceFilePath)
            : destinationFileName;

        File.Copy(sourceFilePath, Path.Combine(destinationFilePath, fileName));
    }
}