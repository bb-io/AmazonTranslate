﻿using Blackbird.Applications.Sdk.Common;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public class SmallJobResponse
{
    [Display("Job ID")] public string JobId { get; set; }
    [Display("Job status")] public string JobStatus { get; set; }
}