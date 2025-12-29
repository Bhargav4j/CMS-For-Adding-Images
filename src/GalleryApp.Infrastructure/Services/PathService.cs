namespace GalleryApp.Infrastructure.Services;

public interface IPathService
{
    string MapPath(string path);
    string GetWebRootPath();
    string GetContentRootPath();
}

public class PathService : IPathService
{
    private readonly string _webRootPath;
    private readonly string _contentRootPath;

    public PathService(string webRootPath, string contentRootPath)
    {
        _webRootPath = webRootPath;
        _contentRootPath = contentRootPath;
    }

    public string MapPath(string path)
    {
        if (path.StartsWith("~/"))
        {
            path = path.Substring(2);
        }

        return Path.Combine(_webRootPath, path);
    }

    public string GetWebRootPath()
    {
        return _webRootPath;
    }

    public string GetContentRootPath()
    {
        return _contentRootPath;
    }
}
