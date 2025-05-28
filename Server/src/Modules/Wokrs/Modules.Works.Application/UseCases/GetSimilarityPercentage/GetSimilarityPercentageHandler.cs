using System.Text;
using BuildingBlocks.Contracts;
using Modules.Works.Domain.Exceptions;

namespace Modules.Works.Application.UseCases.GetSimilarityPercentage
{
    public class GetSimilarityPercentageHandler(IWorkRepository repository) : IQueryHandler<Guid, List<GetSimilarityPercentageResponse>>
    {
        public async Task<List<GetSimilarityPercentageResponse>> HandleAsync(Guid query, CancellationToken cancellationToken)
        {
            var allWorks = await repository.GetAllWorksAsync();
            var selectedWork = allWorks.Find(w => w.Id == query);

            if (selectedWork is null) throw new StudentWorkNotFoundException(query);

            var matchingWorks = allWorks.Where(w =>
                w.Extension == selectedWork.Extension &&
                !w.Id.Equals(selectedWork.Id));

            var percentages = matchingWorks.Select(w => new GetSimilarityPercentageResponse()
            {
                Id = w.Id,
                Name = w.FileName + w.Extension,
                SimilarityPercentage = CalculatePercentage(
                        Encoding.UTF8.GetString(selectedWork.Content),
                        Encoding.UTF8.GetString(w.Content)
                    )
            })
                .OrderByDescending(p => p.SimilarityPercentage)
                .Take(5)
                .ToList();

            return percentages;
        }

        private double CalculatePercentage(string firsText, string secondText)
        {
            var firstWordDictionary = GetWordFrequency(firsText);
            var secondWordDictionary = GetWordFrequency(secondText);

            var words = firstWordDictionary.Keys
                .Union(secondWordDictionary.Keys)
                .Distinct()
                .ToList();

            double dotProduct = 0;
            double firstMagnitude = 0;
            double secondMagnitude = 0;

            foreach (var word in words)
            {
                firstWordDictionary.TryGetValue(word, out var firstCount);
                secondWordDictionary.TryGetValue(word, out var secondCount);

                dotProduct += firstCount * secondCount;
                firstMagnitude += Math.Pow(firstCount, 2);
                secondMagnitude += Math.Pow(secondCount, 2);
            }
            firstMagnitude = Math.Sqrt(firstMagnitude);
            secondMagnitude = Math.Sqrt(secondMagnitude);

            var result = firstMagnitude * secondMagnitude == 0 ? 0 :
                (dotProduct / (firstMagnitude * secondMagnitude)) * 100;

            return Math.Round(result, 2);
        }

        private Dictionary<string, int> GetWordFrequency(string text)
        {
            var words = text.ToLower().Split([' ', '.', ',', '!', '?', ';', ':', '\n', '\r', '\t'],
                StringSplitOptions.RemoveEmptyEntries);

            return words.GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());
        }
    }
}