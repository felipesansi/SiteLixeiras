namespace SiteLixeiras.Sevices
{
    public interface ISeedUserRolesInitial
    {
        Task SeedRolesAsync(); // Método para criar os papéis
        Task SeedUsersAsync(); // Método para criar os usuários
    }
}
