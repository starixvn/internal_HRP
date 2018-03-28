namespace HRP.Core.Services
{
    using System.Threading.Tasks;

    public interface IViewRenderService
    {
        Task<string> RenderViewToStringAsync(string viewName, object model);
    }
}