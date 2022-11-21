namespace Recipe.API.Settings;

public class RecipeDatabaseSettings
{
    public const string Database = "Database";
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }
    public string ConnectionString { get; set; }
}