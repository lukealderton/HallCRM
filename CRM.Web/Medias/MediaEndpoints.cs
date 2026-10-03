using System.Security.Claims;
using CRM.Core.Medias.Abstractions;

namespace CRM.Web.Medias;

public static class MediaEndpoints
{
    public static IEndpointRouteBuilder MapMediaEndpoints(this IEndpointRouteBuilder objEndpoints)
    {
        objEndpoints.MapGet("/api/media/{mediaId:guid}", async (
            Guid mediaId, Boolean? download, ClaimsPrincipal objPrincipal,
            IMediaService objMediaService, HttpContext objContext, CancellationToken objToken) =>
        {
            String? strUserId = objPrincipal.FindFirst("Id")?.Value
                ?? objPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(strUserId, out Guid objRequesterId))
            {
                return Results.Forbid();
            }

            try
            {
                MediaReadResult? objResult = await objMediaService.OpenForReadAsync(mediaId, objRequesterId, objToken);
                if (objResult == null)
                {
                    return Results.NotFound();
                }

                objContext.Response.Headers.CacheControl = "private, no-store";
                objContext.Response.Headers["X-Content-Type-Options"] = "nosniff";
                Boolean blnDownload = download == true || (!objResult.Media.IsImage && !objResult.Media.IsVideo);
                return Results.File(objResult.Content, objResult.Media.ContentType,
                    fileDownloadName: blnDownload ? objResult.Media.FileName : null,
                    enableRangeProcessing: true);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        }).RequireAuthorization();

        return objEndpoints;
    }
}
