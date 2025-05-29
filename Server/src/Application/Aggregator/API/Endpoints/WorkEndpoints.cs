using BuildingBlocks.Models;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Modules.Works.Application.Common.Models;
using Modules.Works.Application.UseCases.BulkExport;
using Modules.Works.Application.UseCases.BulkImport;
using Modules.Works.Application.UseCases.DeleteWork;
using Modules.Works.Application.UseCases.GetAllWorks;
using Modules.Works.Application.UseCases.GetSimilarityPercentage;
using Modules.Works.Application.UseCases.GetUploadedWork;
using Modules.Works.Application.UseCases.GetWorkById;
using Modules.Works.Application.UseCases.UploadWork;
using Modules.Works.Domain.Exceptions;
using Modules.Works.Infrastructure.Jobs;

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
                    [FromServices] IBackgroundJobClient backgroundJobClient,
                    [FromServices] IConfiguration conf,
                    CancellationToken cancellationToken) =>
                {
                    var filePath = await SaveTempFile(file, conf, cancellationToken);

                    var jobId = backgroundJobClient.Enqueue<UploadFileJob>(job =>
                        job.ProcessAsync(null!, filePath, id));

                    return TypedResults.Created("/api/studentwork/upload/{id}",
                        new { message = "Upload job has been queued.", jobId = jobId });
                })
                .DisableAntiforgery()
                .Produces(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithDescription("Endpoint uploading work")
                .WithName("UploadWork")
                .WithSummary("upload work")
                .RequireAuthorization();

            endpoints.MapGet("/api/studentwork/upload/status/{jobId}", async Task<IResult> (
                    [FromRoute] string jobId,
                    [FromServices] GetUploadedWorkHandler handler,
                    CancellationToken cancellationToken) =>
                {
                    var jobData = JobStorage.Current.GetMonitoringApi().JobDetails(jobId);
                    if (jobData == null || jobData.History.Count == 0)
                        throw new JobResultNotFoundException(jobId);

                    var state = jobData.History[0].StateName;

                    if (state != "Succeeded")
                        return TypedResults.Accepted($"/api/studentwork/upload/status/{jobId}",
                            new
                            {
                                jobId,
                                status = state,
                                message = "The job is still in progress. Please try again later."
                            });

                    var result = await handler.HandleAsync(jobId, cancellationToken);

                    return TypedResults.Ok(result);
                })
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithDescription("Endpoint for getting uploaded work")
                .WithName("GetUploadedWork")
                .WithSummary("get uploaded work")
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
                    [FromServices] IBackgroundJobClient backgroundJobClient,
                    [FromServices] IConfiguration conf,
                    CancellationToken cancellationToken) =>
                {
                    var filePath = await SaveTempFile(file, conf, cancellationToken);

                    backgroundJobClient.Enqueue<ImportDataJob>(job =>
                        job.ProcessAsync(filePath));

                    return TypedResults.Created("/api/studentwork/import",
                        new { message = "Import job has been queued." });
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

        private static async Task<string> SaveTempFile(IFormFile file, IConfiguration conf,
            CancellationToken cancellationToken)
        {
            var uploadDirPath = conf["UploadDir:Path"]!;
            Directory.CreateDirectory(uploadDirPath);

            var filePath = Path.Combine(uploadDirPath, $"{Guid.NewGuid()}_{file.FileName}");

            await using var stream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(stream, cancellationToken);

            return filePath;
        }
    }
}