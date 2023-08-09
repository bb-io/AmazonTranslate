﻿using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class CreateTerminologyRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] FileContent { get; set; }
    
    [DataSource(typeof(FormatDataHandler))]
    public string Format { get; set; }
}