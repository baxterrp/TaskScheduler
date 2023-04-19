namespace TaskScheduler.interfaces
{
    public interface IApiProvider
    {
        Task<TApiResponse> Get<TApiResponse>(string url);
    }
}
