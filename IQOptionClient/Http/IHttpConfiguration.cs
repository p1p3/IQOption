using System;

namespace IQOptionClient.Http
{
    public interface IHttpConfiguration
    {
        Uri BaseUrl { get; }
    }
}