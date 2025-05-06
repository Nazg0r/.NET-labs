using System.Text;
using DataAccess.Entities;

namespace TestTools
{
	public static class SharedTestsData
	{
		public static readonly List<Student> TestStudents =
		[
			new Student
			{
				UserName = "johnDoe",
				Name = "John",
				Surname = "Doe",
				Group = "IM-00",
				Works =
				[
					new StudentWork
					{
						Id = Guid.NewGuid(),
						Content = Encoding.UTF8.GetBytes("hello world"),
						FileName = "lab1",
						LoadDate = DateTime.UtcNow,
						Extension = ".cs"
					},

					new StudentWork
					{
						Id = Guid.NewGuid(),
						Content = Encoding.UTF8.GetBytes("Change the world"),
						FileName = "homework",
						LoadDate = DateTime.UtcNow,
						Extension = ".js"
					},

					new StudentWork
					{
						Id = Guid.NewGuid(),
						Content = Encoding.UTF8.GetBytes("hello my friend"),
						FileName = "email",
						LoadDate = DateTime.UtcNow,
						Extension = ".txt"
					},

					new StudentWork
					{
						Id = Guid.NewGuid(),
						Content = Encoding.UTF8.GetBytes("a lot of code"),
						FileName = "program",
						LoadDate = DateTime.UtcNow,
						Extension = ".cs"
					}
				]
			},

			new Student
			{
				UserName = "walterWhite",
				Name = "Walter",
				Surname = "White",
				Group = "IM-01",
				Works =
				[
					new StudentWork
					{
						Id = Guid.NewGuid(),
						Content = Encoding.UTF8.GetBytes("hello world"),
						FileName = "myWork",
						LoadDate = DateTime.UtcNow,
						Extension = ".cs"
					},

					new StudentWork
					{
						Id = Guid.NewGuid(),
						Content = Encoding.UTF8.GetBytes("I do it tomorrow"),
						FileName = "course",
						LoadDate = DateTime.UtcNow,
						Extension = "java"
					},

					new StudentWork
					{
						Id = Guid.NewGuid(),
						Content = Encoding.UTF8.GetBytes("hello world 2"),
						FileName = "lab2",
						LoadDate = DateTime.UtcNow,
						Extension = ".cs"
					},

					new StudentWork
					{
						Id = Guid.NewGuid(),
						Content = Encoding.UTF8.GetBytes(""),
						FileName = "empty",
						LoadDate = DateTime.UtcNow,
						Extension = ".txt"
					}
				]
			}
		];

		public static readonly List<StudentWork> WorksWithSameExtension =
		[
			new StudentWork
			{
				Id = Guid.NewGuid(),
				Content = Encoding.UTF8.GetBytes("home work"),
				FileName = "original",
				Extension = ".cs",
				LoadDate = DateTime.Now,
				StudentId = Guid.NewGuid().ToString()
			},

			new StudentWork
			{
				Id = Guid.NewGuid(),
				Content = Encoding.UTF8.GetBytes("my home work"),
				FileName = "copy",
				Extension = ".cs",
				LoadDate = DateTime.Now,
				StudentId = Guid.NewGuid().ToString()
			},

			new StudentWork
			{
				Id = Guid.NewGuid(),
				Content = Encoding.UTF8.GetBytes("task"),
				FileName = "irrelevant",
				Extension = ".cs",
				LoadDate = DateTime.Now,
				StudentId = Guid.NewGuid().ToString()
			},

			new StudentWork
			{
				Id = Guid.NewGuid(),
				Content = Encoding.UTF8.GetBytes("home task"),
				FileName = "extra",
				Extension = ".cs",
				LoadDate = DateTime.Now,
				StudentId = Guid.NewGuid().ToString()
			}
		];

		public static readonly List<StudentWork> WorksWithDifferentExtension =
		[
			new StudentWork
			{
				Id = Guid.NewGuid(),
				Content = Encoding.UTF8.GetBytes("home work"),
				FileName = "original",
				Extension = ".cs",
				LoadDate = DateTime.Now,
				StudentId = Guid.NewGuid().ToString()
			},

			new StudentWork
			{
				Id = Guid.NewGuid(),
				Content = Encoding.UTF8.GetBytes("my home work"),
				FileName = "copy",
				Extension = ".txt",
				LoadDate = DateTime.Now,
				StudentId = Guid.NewGuid().ToString()
			},

			new StudentWork
			{
				Id = Guid.NewGuid(),
				Content = Encoding.UTF8.GetBytes("task"),
				FileName = "irrelevant",
				Extension = ".cs",
				LoadDate = DateTime.Now,
				StudentId = Guid.NewGuid().ToString()
			},

			new StudentWork
			{
				Id = Guid.NewGuid(),
				Content = Encoding.UTF8.GetBytes("home task"),
				FileName = "extra",
				Extension = ".c",
				LoadDate = DateTime.Now,
				StudentId = Guid.NewGuid().ToString()
			}
		];
	}
}
