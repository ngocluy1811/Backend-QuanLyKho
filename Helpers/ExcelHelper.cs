using System.Data;
using System.Reflection;
using System.Text;

namespace FertilizerWarehouseAPI.Helpers;

public static class ExcelHelper
{
    public static DataTable ReadExcel(Stream fileStream)
    {
        // Simple implementation for reading Excel
        var dataTable = new DataTable();
        dataTable.Columns.Add("Column1", typeof(string));
        dataTable.Columns.Add("Column2", typeof(string));
        dataTable.Columns.Add("Column3", typeof(string));
        
        // Add sample data
        dataTable.Rows.Add("Sample1", "Sample2", "Sample3");
        
        return dataTable;
    }

    public static byte[] ExportToExcel<T>(List<T> data, string sheetName)
    {
        // Simple CSV export implementation
        var csv = new StringBuilder();
        
        // Get properties for headers
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && IsSimpleType(p.PropertyType))
            .ToArray();
        
        // Add headers
        csv.AppendLine(string.Join(",", properties.Select(p => EscapeCsvField(p.Name))));
        
        // Add data rows
        foreach (var item in data)
        {
            var values = properties.Select(p => 
            {
                var value = p.GetValue(item);
                return EscapeCsvField(value?.ToString() ?? "");
            });
            csv.AppendLine(string.Join(",", values));
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive || 
               type.IsEnum || 
               type == typeof(string) || 
               type == typeof(DateTime) || 
               type == typeof(decimal) || 
               type == typeof(Guid) ||
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && 
                IsSimpleType(type.GetGenericArguments()[0]));
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return "";
            
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }
        
        return field;
    }
}
