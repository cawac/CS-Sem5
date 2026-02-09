using Microsoft.Data.Sqlite;
using DeviceLibrary;

public class DatabaseManager
{
    private readonly string _connectionString;

    public DatabaseManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task CreateTablesAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var createManufacturer = @"
            CREATE TABLE IF NOT EXISTS Manufacturer (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Address TEXT NOT NULL,
                IsAChildCompany INTEGER NOT NULL
            );";

        var createPC = @"
            CREATE TABLE IF NOT EXISTS PC (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Model TEXT NOT NULL,
                SerialNumber TEXT NOT NULL,
                PCType TEXT NOT NULL,
                ManufacturerId INTEGER NOT NULL,
                FOREIGN KEY (ManufacturerId) REFERENCES Manufacturer(Id)
            );";

        using var cmd1 = new SqliteCommand(createManufacturer, connection);
        await cmd1.ExecuteNonQueryAsync();

        using var cmd2 = new SqliteCommand(createPC, connection);
        await cmd2.ExecuteNonQueryAsync();
    }

    public async Task SeedDataAsync()
    {
        var manufacturers = DataGenerator.GenerateManufacturers(30);
        var pcs = DataGenerator.GeneratePcs(30);

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        for (int i = 0; i < manufacturers.Count; i++)
        {
            var manufacturerId = await AddManufacturerAsync(manufacturers[i]);
            await AddPCAsync(pcs[i], manufacturerId);
        }
    }

    public async Task<long> AddManufacturerAsync(Manufacturer manufacturer)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var insertManufacturer = @"
            INSERT INTO Manufacturer (Name, Address, IsAChildCompany)
            VALUES (@Name, @Address, @IsAChildCompany);
            SELECT last_insert_rowid();";

        using var cmd = new SqliteCommand(insertManufacturer, connection);
        cmd.Parameters.AddWithValue("@Name", manufacturer.Name);
        cmd.Parameters.AddWithValue("@Address", manufacturer.Address);
        cmd.Parameters.AddWithValue("@IsAChildCompany", manufacturer
            .GetType()
            .GetProperty("IsAChildCompany", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(manufacturer) as bool? == true ? 1 : 0);

        return (long)await cmd.ExecuteScalarAsync();
    }

    public async Task AddPCAsync(PC pc, long manufacturerId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var insertPC = @"
            INSERT INTO PC (Model, SerialNumber, PCType, ManufacturerId)
            VALUES (@Model, @SerialNumber, @PCType, @ManufacturerId);";

        using var cmd = new SqliteCommand(insertPC, connection);
        cmd.Parameters.AddWithValue("@Model", pc.Model);
        cmd.Parameters.AddWithValue("@SerialNumber", pc.SerialNumber);
        cmd.Parameters.AddWithValue("@PCType", pc.PCType.ToString());
        cmd.Parameters.AddWithValue("@ManufacturerId", manufacturerId);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<PC>> GetPCsByManufacturerAsync(string manufacturerName)
    {
        var result = new List<PC>();
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT p.Id, p.Model, p.SerialNumber, p.PCType
            FROM PC p
            INNER JOIN Manufacturer m ON p.ManufacturerId = m.Id
            WHERE m.Name = @Name;";

        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Name", manufacturerName);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            int id = reader.GetInt32(0);
            string model = reader.GetString(1);
            string serial = reader.GetString(2);
            var pcType = Enum.Parse<PCType>(reader.GetString(3));

            result.Add(PC.Create(id, model, serial, pcType));
        }

        return result;
    }
}
