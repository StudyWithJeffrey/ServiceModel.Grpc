# ServiceModel.Grpc service and operation bindings

Only interface implementations are used to bind a list of gRPC operations.

``` c#
[ServiceContract]
public interface IStandardCalculator
{
    [OperationContract]
    int Sum(int x, int y);
}

[ServiceContract]
public interface IScientificCalculator
{
    [OperationContract]
    int Multiply(int x, int y);
}
```

## hosting on server-side

In the following example on a server start-up there will be 2 gRPC services available `IStandardCalculator` and `IScientificCalculator`.
`IDisposable` is ignored as it is not a service contract.

``` c#
internal sealed class CalculatorService : IStandardCalculator, IScientificCalculator, IDisposable
{
    // accept POST /IStandardCalculator/Sum
    public int Sum(int x, int y) { /*...*/ }

    // accept POST /IScientificCalculator/Multiply
    public int Multiply(int x, int y) { /*...*/ }
}

internal sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // enable ServiceModel.Grpc
        services.AddServiceModelGrpc();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            // bind CalculatorService
            endpoints.MapGrpcService<CalculatorService>();
        });
    }
}
```

## call from client-side

``` c#
static readonly IClientFactory MyDefaultClientFactory = new ClientFactory(...);

static void Call()
{
    ChannelBase channel = ... // initialize channel

    // POST /IStandardCalculator/Sum
    MyDefaultClientFactory.CreateClient<IStandardCalculator>(channel).Sum(...);

    // POST /IScientificCalculator/Multiply
    MyDefaultClientFactory.CreateClient<IScientificCalculator>(channel).Multiply(...);
}
```