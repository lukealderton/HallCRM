namespace CRM.Core.Medias.Domain;

public sealed class Media
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid OwnerId { get; set; }
    public String FileName { get; set; } = String.Empty;
    public String Key { get; set; } = String.Empty;
    public String ContentType { get; set; } = String.Empty;
    public Int64 SizeBytes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? DeletedUtc { get; set; }
    public Guid? DeletedByUserId { get; set; }

    public Boolean IsImage => ContentType is "image/jpeg" or "image/png" or "image/gif" or "image/webp";
    public Boolean IsVideo => ContentType is "video/mp4" or "video/webm";
    public String Url => $"/api/media/{Id}";
}
