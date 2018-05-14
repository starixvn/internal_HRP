namespace HRP.Core.Services
{
    using System.Threading.Tasks;

    public interface IViewRenderService
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}