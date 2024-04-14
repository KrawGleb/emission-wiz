using Autofac;
using EmissionWiz.Models.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EmissionWiz.DataProvider.Database;

internal sealed class DatabaseContext : EmissionWizDbContext, IDatabaseContext
{
    private readonly HashSet<object> _firedObject = new();
    private readonly Dictionary<string, object> _commitTags = new();
    private readonly Dictionary<object, Dictionary<string, object?>> _entityTags = new();

    public DatabaseContext(DbContextOptions<EmissionWizDbContext> options, IComponentContext componentContext) : base(componentContext, options)
    { }

    public async Task BulkInsertAsync<T>(IList<T> entities)
    {
        var entityType = typeof(T);
        var dataTable = new DataTable(entityType.Name);
        var fields = entityType.GetFields().ToList();

        foreach (var field in fields)
        {
            if (field.ReflectedType != null)
                dataTable.Columns.Add(field.Name, field.ReflectedType);
        }

        using var sqlCopy = new SqlBulkCopy(Database.GetDbConnection().ConnectionString);

        sqlCopy.DestinationTableName = dataTable.TableName;
        foreach (var entity in entities)
        {
            var row = dataTable.NewRow();

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var dataTableColumnType = dataTable.Columns[i].DataType;
                var dataTableColumnName = dataTable.Columns[i].ColumnName;
                var property = entityType.GetProperty(dataTableColumnName, dataTableColumnType)
                    ?? throw new InvalidProgramException($"Can't find property with name {dataTableColumnName} and type {dataTableColumnType.FullName}");

                row[i] = property.GetValue(entity);
            }

            dataTable.Rows.Add(row);
        }

        await sqlCopy.WriteToServerAsync(dataTable);
    }

    public async Task<int> CommitChangesAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(cancellationToken);
    }

    public Task ResetChangesAsync()
    {
        _firedObject.Clear();
        _commitTags.Clear();
        _entityTags.Clear();

        //https://github.com/dotnet/efcore/issues/18171
        ChangeTracker.Clear();

        return Task.CompletedTask;
    }
}
