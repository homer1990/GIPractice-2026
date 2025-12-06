namespace GIPractice.Api.Infrastructure;

public class MediaStorageOptions
{
    public string RootPath { get; set; } = "wwwroot/media";
    public string RequestPath { get; set; } = "/media";
}
