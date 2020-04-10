using System;
using Yandex.Disk.Api.Client.Models;

namespace Yandex.Disk.Api.Client
{
    public class ApiException : Exception
    {
        public ApiError ApiError { get; set; }
        
        public ApiException(ApiError apiError) : base($"Yandex API error: {apiError.Description}.")
        {
            ApiError = apiError;
        }
    }
}