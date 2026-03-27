using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Bk.APG.Infrastructure.Service.EndpointBehaviors;

public class HttpClientFactoryBehavior : IEndpointBehavior
{
    private readonly Func<HttpClientHandler, HttpMessageHandler> _messageHandlerFactoryFn;

    public HttpClientFactoryBehavior(string clientName, IHttpMessageHandlerFactory httpMessageHandlerFactory)
    {
        ArgumentNullException.ThrowIfNull(clientName);
        ArgumentNullException.ThrowIfNull(httpMessageHandlerFactory);

        _messageHandlerFactoryFn = _ => httpMessageHandlerFactory.CreateHandler(clientName);
    }

    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
        ArgumentNullException.ThrowIfNull(bindingParameters);

        bindingParameters.Add(_messageHandlerFactoryFn);
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        // interface implementation
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
        // interface implementation
    }

    public void Validate(ServiceEndpoint endpoint)
    {
        // interface implementation
    }
}
