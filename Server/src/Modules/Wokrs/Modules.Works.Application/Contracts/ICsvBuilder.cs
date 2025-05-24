namespace Modules.Works.Application.Contracts
{
	public interface ICsvBuilder
	{
		public byte[] BuildWorksCsv(IEnumerable<Work> works);
	}
}
