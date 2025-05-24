using System.Globalization;
using Modules.Works.Domain.Entities;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Modules.Works.Application.Contracts;

namespace Modules.Works.Infrastructure.CsvProcessing
{
	public class CsvBuilder : ICsvBuilder
	{
		public byte[] BuildWorksCsv(IEnumerable<Work> works)
		{
			using var stream = new MemoryStream();
			using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);

			using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
				TrimOptions = TrimOptions.Trim,
			});

			csv.WriteRecords(works);

			writer.Flush();

			return stream.ToArray();
		}
	}
}