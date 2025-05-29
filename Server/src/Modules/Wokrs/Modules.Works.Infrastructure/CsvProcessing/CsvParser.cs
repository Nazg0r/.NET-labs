using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Modules.Works.Application.Contracts;
using Modules.Works.Domain.Entities;

namespace Modules.Works.Infrastructure.CsvProcessing
{
    public class CsvParser : ICsvParser
    {
        public List<Work> ParseCsv(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
            });

            return csv.GetRecords<Work>().ToList();
        }
    }
}