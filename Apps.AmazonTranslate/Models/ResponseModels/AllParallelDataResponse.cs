using Blackbird.Applications.Sdk.Common;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public record AllParallelDataResponse([property: Display("Parallel data")] List<FullParallelDataResponse> ParallelData);