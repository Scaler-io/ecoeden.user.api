﻿namespace Ecoeden.User.Domain.Models.Core
{
    public sealed class FieldLevelError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Field { get; set; }
    }
}
