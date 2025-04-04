namespace DataAccess.Interfaces
{
	public interface IStudentWorkRepository
	{
		Task<StudentWork?> GetWorkByIdAsync(Guid id);
		Task<List<StudentWork>> GetAllWorksAsync();
		Task<StudentWork> AddNewWorkAsync(StudentWork work);
		Task<bool> DeleteWorkAsync(Guid id);
	}
}
