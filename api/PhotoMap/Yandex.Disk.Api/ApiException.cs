using System;
using Yandex.Disk.Api.Models;

namespace Yandex.Disk.Api
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