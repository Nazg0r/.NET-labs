using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Modules.Students.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Exceptions;
using FluentValidation;

namespace API.Infrastructure
{
	[ExcludeFromCodeCoverage]
	public class ErrorHandler : IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
		{
			ProblemDetails problemDetails = new()
			{
				Detail = exception.Message,
				Title = exception.GetType().Name,
				Instance = context.Request.Path
			};

			problemDetails.Status = exception switch
			{
				NotFoundException => StatusCodes.Status404NotFound,
				CreationException => StatusCodes.Status400BadRequest,
				ValidationException => StatusCodes.Status400BadRequest,
				ArgumentException => StatusCodes.Status400BadRequest,
				UnauthorizedException => StatusCodes.Status401Unauthorized,
				_ => StatusCodes.Status500InternalServerError
			};

			if (exception is ValidationException validationException)
				problemDetails.Extensions.Add("ValidationException", validationException.Errors);

			context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

			await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);


			return true;
		}
	}
}
