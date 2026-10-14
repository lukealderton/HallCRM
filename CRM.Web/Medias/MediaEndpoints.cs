using System.Security.Claims;
using CRM.Core.Medias.Abstractions;
using CRM.Core.Users.Abstraction.Services;
using CRM.Core.Users.Domain;
using CRM.Web.Users.Services;

namespace CRM.Web.Medias;

public static class MediaEndpoints
{
    public static IEndpointRouteBuilder MapMediaEndpoints(this IEndpointRouteBuilder objEndpoints)
    {
        objEndpoints.MapGet("/api/media/{mediaId:guid}", async (
            Guid mediaId, Boolean? download, ClaimsPrincipal objPrincipal,
            IMediaService objMediaService, IUserService objUsers, HttpContext objContext, CancellationToken objToken) =>
        {
            User? objUser = await objPrincipal.GetCrmUserAsync(objUsers, objToken);
            if (objUser?.Enabled != true)
            {
                return Results.Forbid();
            }

            try
            {
                MediaReadResult? objResult = await objMediaService.OpenForReadAsync(mediaId, objUser.Id, objToken);
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
