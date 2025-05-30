﻿using Hangfire.Server;
using Modules.Works.Application.Common.Models;
using Modules.Works.Application.Contracts;
using Modules.Works.Application.UseCases.UploadWork;
using Modules.Works.Domain.Constants;
using Modules.Works.Domain.Entities;
using Modules.Works.Domain.Exceptions;
using Modules.Works.Infrastructure.Jobs.Shared;

namespace Modules.Works.Infrastructure.Jobs
{
    public class UploadFileJob(
        UploadWorkHandler handler,
        IJobRepository jobRepository)
    {
        public async Task ProcessAsync(string filePath, string studentId) =>
            await ProcessInternalAsync(null, filePath, studentId);

        public async Task ProcessAsync(PerformContext context, string filePath, string studentId) =>
            await ProcessInternalAsync(context, filePath, studentId);

        private async Task ProcessInternalAsync(PerformContext? context, string filePath, string studentId)
        {
            var jobId = context?.BackgroundJob?.Id;
            var fileName = Path.GetFileName(filePath)[Length.GuidLength..];

            await using var stream = ReadFile.Read(filePath);

            var command = new UploadWorkCommand
            {
                FileStream = stream,
                StudentId = studentId,
                FileName = fileName
            };

            var result = await handler.HandleAsync(command, CancellationToken.None);

            var jobResult = new UploadFIleJobResult { JobId = jobId, WorkId = result.Id };
            var isStored = await jobRepository.StoreJobResultAsync(jobResult);

            if (!isStored)
                throw new WorkUploadingFailedException(fileName);

            File.Delete(filePath);
        }
    }
}