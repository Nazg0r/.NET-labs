using Microsoft.Extensions.Configuration;
using Modules.Works.Application.Contracts;
using Modules.Works.Domain.Entities;
using Npgsql;
using System.Text;

namespace Modules.Works.Persistence.Bulk
{
	public class BulkOperations(IConfiguration config) : IBulkOperations
	{
		public async Task InsertAsync(IEnumerable<Work> works, CancellationToken token)
		{
			var connectionString = config.GetConnectionString("Database");
			await using var connection = new NpgsqlConnection(connectionString);
			await connection.OpenAsync(token);

			await using var writer = await connection.BeginBinaryImportAsync(
				"COPY works.\"Works\" (id, file_name, load_date, extension, content, student_id) FROM STDIN (FORMAT BINARY)", token);

			foreach (var work in works)
			{
				await writer.StartRowAsync(token);
				await writer.WriteAsync(work.Id, token);
				await writer.WriteAsync(work.FileName, token);
				await writer.WriteAsync(work.LoadDate, token);
				await writer.WriteAsync(work.Extension, token);
				await writer.WriteAsync(work.Content, token);
				await writer.WriteAsync(work.StudentId, token);
			}

			await writer.CompleteAsync(token);
		}

		public async Task<byte[]> ExportAsync(CancellationToken token)
		{
			var connectionString = config.GetConnectionString("Database");
			await using var connection = new NpgsqlConnection(connectionString);
			await connection.OpenAsync(token);

			using var reader = await connection.BeginTextExportAsync(
				"COPY works.\"Works\" (id, file_name, load_date, extension, content, student_id) TO STDOUT (FORMAT CSV, HEADER true)",
				token); ;

			var csv = await reader.ReadToEndAsync(token);

			return Encoding.UTF8.GetBytes(csv);
		}
	}
}