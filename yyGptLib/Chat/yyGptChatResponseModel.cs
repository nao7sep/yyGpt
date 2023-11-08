﻿using System.Text.Json.Serialization;

namespace yyGptLib
{
    public partial class yyGptChatResponseModel
    {
        [JsonPropertyName ("id")]
        public string? Id { get; set; }
        
        [JsonPropertyName ("object")]
        public string? Object { get; set; }
        
        [JsonPropertyName ("created")]
        public int? Created { get; set; }
        
        [JsonPropertyName ("model")]
        public string? Model { get; set; }
        
        [JsonPropertyName ("system_fingerprint")]
        public string? SystemFingerprint { get; set; }

        [JsonPropertyName ("choices")]
        public IList <yyGptChatResponseChoiceModel>? Choices { get; set; }

        [JsonPropertyName ("usage")]
        public yyGptChatResponseUsageModel? Usage { get; set; }
    }
}
