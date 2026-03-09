namespace LoginNet.Application.Common.Interfaces
{
    public interface IRequest<out TResponse> { }
    public interface IRequest : IRequest<Unit> { }

    public struct Unit 
    { 
        public static readonly Unit Value = new();
        public static Task<Unit> Task => System.Threading.Tasks.Task.FromResult(Value);
    }

    public interface IRequestHandler<in TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public interface IPipelineBehavior<in TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
    }

    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}
