namespace BlogManagement.Configurations;

public class FileUploadSettings
{
    public long MaxFileSize { get; set; } = 5 * 1024 * 1024;
    public int MaxFileCount { get; set; } = 10;

    public string[] AllowedFileExtensions { get; set; } =
    [
        ".jpg",
        ".jpeg",
        ".png"
    ];


}
