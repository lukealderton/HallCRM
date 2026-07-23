using CRM.Core.Users.Abstraction.Repositories;
using CRM.Core.Users.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Users.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<CRMDbContext> _dbContextFactory;

        public UserRepository(
            IDbContextFactory<CRMDbContext> objDbContextFactory)
        {
            _dbContextFactory = objDbContextFactory;
        }

        public async Task<Dictionary<Guid, String>> GetDisplayNamesByUserIdsAsync(
            IReadOnlyCollection<Guid> colUserIds,
            CancellationToken objToken = default)
        {
            Guid[] colCleanUserIds = [.. colUserIds
                .Where(x => x != Guid.Empty)
                .Distinct()];

            if (!colCleanUserIds.Any())
            {
                return [];
            }

            await using CRMDbContext objDbContext = await _dbContextFactory.CreateDbContextAsync(objToken);

            var colProfiles = await objDbContext.UserProfiles
                .AsNoTracking()
                .Where(x => colCleanUserIds.Contains(x.Id))
                .Select(x => new
                {
                    UserId = x.Id,
                    x.Forename,
                    x.Surname
                })
                .ToListAsync(objToken);

            var colAccounts = await objDbContext.Users
                .AsNoTracking()
                .Where(x => colCleanUserIds.Contains(x.DomainUserId))
                .Select(x => new
                {
                    UserId = x.DomainUserId,
                    x.Email,
                    x.UserName
                })
                .ToListAsync(objToken);

            Dictionary<Guid, String> colDisplayNamesByUserId = new();

            foreach (var objProfile in colProfiles)
            {
                String strDisplayName = BuildDisplayName(
                    objProfile.Forename,
                    objProfile.Surname,
                    null,
                    null);

                if (!String.IsNullOrWhiteSpace(strDisplayName))
                {
                    colDisplayNamesByUserId[objProfile.UserId] = strDisplayName;
                }
            }

            foreach (var objAccount in colAccounts)
            {
                if (colDisplayNamesByUserId.ContainsKey(objAccount.UserId))
                {
                    continue;
                }

                String strDisplayName = BuildDisplayName(
                    null,
                    null,
                    objAccount.Email,
                    objAccount.UserName);

                if (!String.IsNullOrWhiteSpace(strDisplayName))
                {
                    colDisplayNamesByUserId[objAccount.UserId] = strDisplayName;
                }
            }

            return colDisplayNamesByUserId;
        }

        public async Task<User?> GetUserAsync(
            Guid objUserId,
            CancellationToken objToken = default)
        {
            if (objUserId == Guid.Empty)
            {
                return null;
            }

            await using CRMDbContext objDbContext = await _dbContextFactory.CreateDbContextAsync(objToken);

            return await GetUserQuery(objDbContext)
                .FirstOrDefaultAsync(x => x.Id == objUserId, objToken);
        }

        private static IQueryable<User> GetUserQuery(
            CRMDbContext objContext)
        {
            return objContext.Users
                .AsNoTracking()
                .Select(x => new User
                {
                    Id = x.DomainUserId,
                    Username = x.UserName,
                    Email = x.Email,
                    Forename = x.Forename,
                    Surname = x.Surname,
                    //AccessLevel = x.AccessLevel,
                    EmailConfirmed = x.EmailConfirmed,
                    IsLockedOut =
                        x.LockoutEnd.HasValue &&
                        x.LockoutEnd > DateTimeOffset.UtcNow,
                    Enabled = x.Enabled,
                    CreatedUtc = x.CreatedUtc,
                    LastLoginUtc = x.LastLoginUtc,
                    UpdatedUtc = x.UpdatedUtc
                });
        }


        private static String BuildDisplayName(
            String? strFirstName,
            String? strSurname,
            String? strEmail,
            String? strUserName)
        {
            List<String> colNameParts = [];

            if (!String.IsNullOrWhiteSpace(strFirstName))
            {
                colNameParts.Add(strFirstName.Trim());
            }

            if (!String.IsNullOrWhiteSpace(strSurname))
            {
                colNameParts.Add(strSurname.Trim());
            }

            String strName = String.Join(" ", colNameParts);

            if (!String.IsNullOrWhiteSpace(strName))
            {
                return strName;
            }

            if (!String.IsNullOrWhiteSpace(strEmail))
            {
                return strEmail.Trim();
            }

            if (!String.IsNullOrWhiteSpace(strUserName))
            {
                return strUserName.Trim();
            }

            return "Unknown user";
        }
    }
}