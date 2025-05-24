using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Modules.Works.Application.Contracts;
using Modules.Works.Domain.Entities;
using System.Globalization;

namespace Modules.Works.Infrastructure.CsvProcessing
{
	public class CsvParser : ICsvParser
	{
		public List<Work> ParseCsv(IFormFile file)
		{
			using var reader = new StreamReader(file.OpenReadStream());
			using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HeaderValidated = null,
			});

			return csv.GetRecords<Work>().ToList();
		}
	}
}