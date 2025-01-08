using System.Text;

public static class SchemaConverter
{
    public static string ConvertSchemaToString(TableSchema tableSchema)
    {
        if (tableSchema == null)
        {
            throw new ArgumentNullException(nameof(tableSchema), "TableSchema cannot be null.");
        }

        var sb = new StringBuilder();

        // Add table name
        sb.AppendLine($"Table: {tableSchema.TableName}");

        // Add columns and their types
        foreach (var column in tableSchema.Columns)
        {
            sb.AppendLine($"  Column: {column.ColumnName}, Type: {column.DataType}");
        }

        return sb.ToString().Trim();
    }

    public static TableSchema ConvertStringToSchema(string schemaString)
    {
        if (string.IsNullOrWhiteSpace(schemaString))
        {
            throw new ArgumentException("Schema string cannot be null or empty.", nameof(schemaString));
        }

        var lines = schemaString.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0 || !lines[0].StartsWith("Table: "))
        {
            throw new FormatException("Invalid schema string format.");
        }

        var tableName = lines[0].Substring("Table: ".Length).Trim();
        var columns = new List<ColumnSchema>();

        for (int i = 1; i < lines.Length; i++)
        {
            var columnLine = lines[i].Trim();
            if (columnLine.StartsWith("Column: "))
            {
                var parts = columnLine.Substring("Column: ".Length).Split(", Type: ");
                if (parts.Length != 2)
                {
                    throw new FormatException($"Invalid column format: {columnLine}");
                }

                var columnName = parts[0].Trim();
                var dataType = parts[1].Trim();
                columns.Add(new ColumnSchema { ColumnName = columnName, DataType = dataType });
            }
        }

        return new TableSchema { TableName = tableName, Columns = columns };
    }
}
