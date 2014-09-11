using System.Threading.Tasks;

namespace NetPing_modern.Services.Confluence
{
    public interface IConfluenceClient
    {
        Task<string> GetContenAsync(int id);
    }
}
