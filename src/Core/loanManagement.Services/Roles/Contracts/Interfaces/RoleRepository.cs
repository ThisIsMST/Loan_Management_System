namespace loanManagement.Services.Roles.Contracts.Interfaces
{
    public interface RoleRepository
    {
        int AddAdmin();
        int AddCustomer();
        string GetRoleByEmail(string Email);
    }
}
