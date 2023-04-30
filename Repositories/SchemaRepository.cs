namespace WebSockets.Repositories
{
    public interface ISchemaRepository
    {
        Task<string> GetSchema(string schema);
    }

    public class SchemaRepository : ISchemaRepository
    {
        public Task<string> GetSchema(string schema)
        {
            var result = @"
            {
                ""type"": ""object"",
                ""properties"": {
                    ""name"": { ""type"": ""string"" },
                    ""age"": { ""type"": ""integer"", ""minimum"": 0 },
                    ""isMarried"": { ""type"": ""boolean"" }
                }
            }";

            return Task.FromResult(schema);
        }
    }
}