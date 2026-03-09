using LoginNet.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace LoginNet.Infrastructure.Services
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly ConcurrentDictionary<(Type RequestType, Type ResponseType), (Type HandlerType, MethodInfo HandleMethod, Type BehaviorsType)> _cache = new();

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var requestType = request.GetType();
            var responseType = typeof(TResponse);

            var (handlerType, handleMethod, behaviorsType) = _cache.GetOrAdd((requestType, responseType), static types =>
            {
                var hType = typeof(IRequestHandler<,>).MakeGenericType(types.RequestType, types.ResponseType);
                var mInfo = hType.GetMethod("Handle")!;
                var bType = typeof(IPipelineBehavior<,>).MakeGenericType(types.RequestType, types.ResponseType);
                return (hType, mInfo, bType);
            });

            var handler = _serviceProvider.GetRequiredService(handlerType);
            var behaviors = _serviceProvider.GetServices(behaviorsType)
                                            .Cast<dynamic>()
                                            .ToList();

            RequestHandlerDelegate<TResponse> handlerDelegate = () => (Task<TResponse>)handleMethod.Invoke(handler, [request, cancellationToken])!;

            if (behaviors.Any())
            {
                int currentBehaviorIndex = 0;

                RequestHandlerDelegate<TResponse> ProcessBehavior()
                {
                    if (currentBehaviorIndex < behaviors.Count)
                    {
                        var behavior = behaviors[currentBehaviorIndex++];
                        return () => behavior.Handle((dynamic)request, (dynamic)ProcessBehavior(), cancellationToken);
                    }
                    return handlerDelegate;
                }

                return await ProcessBehavior()();
            }

            return await handlerDelegate();
        }
    }
}
