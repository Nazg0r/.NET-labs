namespace Shared.Errors
{
	public class NotFoundException : Exception
	{
		public NotFoundException(string item) : base($"Item {item} wasn`t found")
		{
		}
	}
}
