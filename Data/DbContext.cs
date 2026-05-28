using MySql.Data.MySqlClient;
using System.Data;

namespace Personal_Sitios.Data
{
    public class DbContext
    {
        private readonly IConfiguration _configuration;

        public DbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            );
        }
    }
}