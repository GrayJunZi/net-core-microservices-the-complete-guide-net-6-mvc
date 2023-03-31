using Mango.Web.DTOs;

namespace Mango.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        ResponseDto response { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
