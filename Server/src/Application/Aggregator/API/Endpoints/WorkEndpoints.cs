using BuildingBlocks.Models;
using Microsoft.AspNetCore.Mvc;
using Modules.Works.Application.Common.Models;
using Modules.Works.Application.UseCases.BulkExport;
using Modules.Works.Application.UseCases.BulkImport;
using Modules.Works.Application.UseCases.DeleteWork;
using Modules.Works.Application.UseCases.GetAllWorks;
using Modules.Works.Application.UseCases.GetSimilarityPercentage;
using Modules.Works.Application.UseCases.GetWorkById;
using Modules.Works.Application.UseCases.UploadWork;

namespace API.Endpoints
{
	public static class WorkEndpoints
	{
		public static IEndpointRouteBuilder MapWorkEndpoints(this IEndpointRouteBuilder endpoints)
		{
			endpoints.MapGet("/api/studentwork/{id:guid}", async (
					[FromRoute] Guid id,
					[FromServices] GetWorkByIdHandler handler,
					CancellationToken cancellationToken) =>
				{
					var result = await handler.HandleAsync(id, cancellationToken);
					return TypedResults.Ok(result);
				})
				.Produces(StatusCodes.Status200OK)
				.ProducesProblem(StatusCodes.Status404NotFound)
				.WithDescription("Endpoint for getting work by workId")
				.WithName("GetWorkById")
				.WithSummary("get work by workId")
				.RequireAuthorization();

			endpoints.MapGet("/api/studentwork", async (
					[FromServices] GetAllWorksHandler handler,
					CancellationToken cancellationToken) =>
				{
					var result = await handler.HandleAsync(Unit.Value, cancellationToken);
					return TypedResults.Ok(result);
				})
				.Produces(StatusCodes.Status200OK)
				.ProducesProblem(StatusCodes.Status404NotFound)
				.WithDescription("Endpoint for getting all works")
				.WithName("GetAllWorks")
				.WithSummary("get all works")
				.RequireAuthorization();

			endpoints.MapGet("/api/studentwork/plagiarism/{id:guid}", async (
					[FromRoute] Guid id,
					[FromServices] GetSimilarityPercentageHandler handler,
					CancellationToken cancellationToken) =>
				{
					var result = await handler.HandleAsync(id, cancellationToken);
					return TypedResults.Ok(result);
				})
				.Produces(StatusCodes.Status200OK)
				.ProducesProblem(StatusCodes.Status404NotFound)
				.WithDescription("Endpoint for getting similarity percentage")
				.WithName("GetSimilarityPercentage")
				.WithSummary("get similarity percentage")
				.RequireAuthorization();

			endpoints.MapDelete("/api/studentwork/{id:guid}", async (
					[FromRoute] Guid id,
					[FromServices] DeleteWorkHandler handler,
					CancellationToken cancellationToken) =>
				{
					var command = new DeleteWorkCommand() { workId = id };
					await handler.HandleAsync(command, cancellationToken);
					return TypedResults.NoContent();
				})
				.Produces(StatusCodes.Status204NoContent)
				.ProducesProblem(StatusCodes.Status404NotFound)
				.WithDescription("Endpoint for deleting work")
				.WithName("DeleteWork")
				.WithSummary("delete work")
				.RequireAuthorization();

			endpoints.MapPost("/api/studentwork/upload/{id}", async (
					[FromRoute] string id,
					[FromForm] IFormFile file,
					[FromServices] UploadWorkHandler handler,
					CancellationToken cancellationToken) =>
				{
					var command = new UploadWorkCommand()
					{
						StudentId = id,
						File = file
					};

					var result = await handler.HandleAsync(command, cancellationToken);
					return TypedResults.Created($"/api/studentwork/{result.Id}", result);
				})
				.DisableAntiforgery()
				.Produces(StatusCodes.Status201Created)
				.ProducesProblem(StatusCodes.Status400BadRequest)
				.WithDescription("Endpoint uploading work")
				.WithName("UploadWork")
				.WithSummary("upload work")
				.RequireAuthorization();

			endpoints.MapGet("/api/studentwork/export", async (
					[FromServices] BulkExportHandler handler,
					CancellationToken cancellationToken) =>
				{
					var csv = await handler.HandleAsync(Unit.Value, cancellationToken);
					return TypedResults.File(csv, "text/csv", "student_works.csv");
				})
				.Produces(StatusCodes.Status200OK)
				.ProducesProblem(StatusCodes.Status404NotFound)
				.WithDescription("Endpoint for works export")
				.WithName("ExportWorks")
				.WithSummary("export work")
				.RequireAuthorization();

			endpoints.MapPost("/api/studentwork/import", async (
					[FromForm] IFormFile file,
					[FromServices] BulkImportHandler handler,
					CancellationToken cancellationToken) =>
				{
					var command = new BulkImportCommand()
					{
						File = file
					};
					await handler.HandleAsync(command, cancellationToken);
				})
				.DisableAntiforgery()
				.Produces(StatusCodes.Status201Created)
				.ProducesProblem(StatusCodes.Status400BadRequest)
				.WithDescription("Endpoint for works import")
				.WithName("ImportWorks")
				.WithSummary("import work")
				.RequireAuthorization();
			return endpoints;
		}
	}
}