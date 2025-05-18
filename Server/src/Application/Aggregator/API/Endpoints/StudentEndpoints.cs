using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Modules.Students.Application.Common.Models;
using Modules.Students.Application.UseCases.GetStudentByUsername;
using Modules.Students.Application.UseCases.GetStudentByWorkId;
using Modules.Students.Application.UseCases.Login;
using Modules.Students.Application.UseCases.Register;

namespace API.Endpoints
{
	public static class StudentEndpoints
	{
		public static IEndpointRouteBuilder MapStudentEndpoints(this IEndpointRouteBuilder endpoints)
		{
			endpoints.MapPost("/api/student/register", async (
					[FromBody] RegisterStudentCommand command,
					[FromServices] RegisterStudentHandler handler,
					[FromServices] IValidator<RegisterStudentCommand> validator,
					CancellationToken cancellationToken) =>
				{
					await validator.ValidateAndThrowAsync(command, cancellationToken);
					await handler.HandleAsync(command, cancellationToken);
					return TypedResults.Created();
				})
				.Produces(StatusCodes.Status201Created)
				.ProducesProblem(StatusCodes.Status400BadRequest)
				.WithDescription("Endpoint for student registration")
				.WithName("Register")
				.WithSummary("student registration");

			endpoints.MapPost("/api/student/login", async (
					[FromBody] LoginStudentCommand command,
					[FromServices] LoginStudentHandler handler,
					[FromServices] IValidator<LoginStudentCommand> validator,
					CancellationToken cancellationToken) =>
				{
					await validator.ValidateAndThrowAsync(command, cancellationToken);
					var result = await handler.HandleAsync(command, cancellationToken);
					return TypedResults.Ok(result);
				})
				.Produces(StatusCodes.Status200OK)
				.ProducesProblem(StatusCodes.Status400BadRequest)
				.WithDescription("Endpoint for student logining")
				.WithName("Login")
				.WithSummary("student logining");

			endpoints.MapGet("/api/student/{username}", async (
					[FromRoute] string username,
					[FromServices] GetStudentByUsernameHandler handler,
					CancellationToken cancellationToken) =>
				{
					var result = await handler.HandleAsync(username, cancellationToken);
					return TypedResults.Ok(result);
				})
				.Produces(StatusCodes.Status200OK)
				.ProducesProblem(StatusCodes.Status400BadRequest)
				.WithDescription("Endpoint for getting student by username")
				.WithName("GetByUsername")
				.WithSummary("get student by username")
				.RequireAuthorization();

			endpoints.MapGet("/api/student/work/{workId:guid}", async (
					[FromRoute] Guid workId,
					[FromServices] GetAuthorByWorkIdHandler handler,
					CancellationToken cancellationToken) =>
				{
					var result = await handler.HandleAsync(workId, cancellationToken);
					return TypedResults.Ok(result);
				})
				.Produces(StatusCodes.Status200OK)
				.ProducesProblem(StatusCodes.Status400BadRequest)
				.WithDescription("Endpoint for getting student by work workId")
				.WithName("GetByWorkId")
				.WithSummary("get student by work workId")
				.RequireAuthorization();

			return endpoints;
		}
	}
}