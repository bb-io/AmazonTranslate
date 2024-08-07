﻿using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class CreateTerminologyRequest
{
    public string Name { get; set; }

    public string Description { get; set; }
    
    [Display("File", Description = "File in CSV, TMX, or TSV format.")]
    public FileReference File { get; set; }
    
    [StaticDataSource(typeof(FormatDataHandler))]
    public string Format { get; set; }
    
    [StaticDataSource(typeof(TerminologyDirectionalityDataHandler))]
    public string? Directionality { get; set; }
}