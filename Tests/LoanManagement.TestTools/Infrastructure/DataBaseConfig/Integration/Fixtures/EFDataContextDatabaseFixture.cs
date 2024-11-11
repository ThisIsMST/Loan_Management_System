
using LoanManagement.Persistence.EF.DataContext;
using Xunit;

namespace LoanManagement.TestTools.Infrastructure.DataBaseConfig.Integration.Fixtures;

[Collection(nameof(ConfigurationFixture))]
public class EFDataContextDatabaseFixture : DatabaseFixture
{
    public static EFDataContext CreateDataContext(string tenantId)
    {
        var connectionString =
            new ConfigurationFixture().Value.ConnectionString;


        return new EFDataContext(
            $"server=.;database=LoanManagementSystemDB;Trusted_Connection=True;Encrypt=false;TrustServerCertificate=true;");
    }
}