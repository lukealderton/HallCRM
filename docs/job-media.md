# Job media

Open a saved job and use **Media → Attach media** to upload files. Multiple files can
be selected at once. Each attachment can be downloaded or removed, with a confirmation
before removal. Archived jobs keep their attachments available to view and download;
restore the job to upload or remove files.

Supported formats: JPG, PNG, GIF, WebP, HEIC, HEIF, MP4, MOV, WebM and PDF. JPG, PNG,
GIF and WebP have image previews. MP4 and WebM have video players; playback depends
on the browser's supported codecs. Other formats are available to download.

## Database setup

Apply the `AddJobMedia` migration before using the feature:

```powershell
dotnet ef database update --project CRM.Infrastructure --startup-project CRM.Web --context CRMDbContext
```

The migration adds `T_Media`, with a foreign key to the job, original filename,
private storage key, content type, size, uploader, creation date and removal audit
fields. The migration is included in the project; it is not automatically applied
at application startup.

## Storage configuration

`CRM.Web/appsettings.json` contains these defaults:

```json
{
  "CRM": {
    "MediaSettings": {
      "Root": "App_Data/Media",
      "MaxFileSizeBytes": 104857600,
      "MaxFilesPerUpload": 10
    }
  }
}
```

Relative storage roots resolve from the web application's content directory.
For deployment, configure a persistent directory writable by the application's
service account, outside `wwwroot`. Back up that directory alongside the database.
Multiple application instances need the same shared storage directory.

Files are delivered by `/api/media/{id}` after checking the signed-in CRM user and
job. `?download=true` downloads the original filename. Video requests support byte
ranges. Removed attachments and attachments on deleted jobs are unavailable.

## Implementation and verification

Adapted from the media service in `H:\Unified\TenantExchange2026`: a core service,
EF metadata repository, local storage abstraction, private file keys, signature
checks and atomic writes. The CRM version stores originals and does not require
FFmpeg or a background transcoding service. It uses shared access for enabled CRM
staff, matching the job pages.

Run the regression tests with:

```powershell
dotnet test CRM.Tests/CRM.Tests.csproj -m:1
```

Tests cover upload/read/removal, access controls, missing files, duplicate names,
non-seekable streams, format rejection, size limits, cancellation, database-failure
cleanup, path traversal, authentication, download headers and byte-range responses.
