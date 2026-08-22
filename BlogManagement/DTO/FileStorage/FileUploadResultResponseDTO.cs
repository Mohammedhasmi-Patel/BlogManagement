namespace BlogManagement.DTO.FileStorage;

public class FileUploadResultResponseDTO
{
    public string OriginalFileName { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string FileUrl { get; set; } = null!;

    public string Extension { get; set; } = null!;

    public long FileSize { get; set; }

}
