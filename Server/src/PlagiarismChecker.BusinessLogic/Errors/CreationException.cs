namespace BusinessLogic.Errors
{
	public class CreationException : Exception
	{
		public CreationException(string etityName) : base($"Could not create the {etityName} entity")
		{
		}
	}
}
