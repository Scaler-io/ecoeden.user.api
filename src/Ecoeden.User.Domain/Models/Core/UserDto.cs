﻿using System.Net;

namespace Ecoeden.User.Domain.Models.Core
{
    public sealed class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public AuthorizationDto AuthorizationDto { get; set; }
    }
}