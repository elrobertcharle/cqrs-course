namespace Post.Query.Api.Options
{
    public class PostgresDbOptions
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Database { get; set; } = null!;

        public static string GetConnectionString(PostgresDbOptions postgresDbOptions)
        {
            return $"host={postgresDbOptions.Host}:{postgresDbOptions.Port};database={postgresDbOptions.Database};user id={postgresDbOptions.Username};password={postgresDbOptions.Password}";
        }
    }
}
