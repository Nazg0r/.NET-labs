namespace BusinessLogic.Errors
{
	public class StudentWorkNotFoundException(Guid id) 
		: NotFoundException($"`student work` with id {id}")
	{
	}
}
