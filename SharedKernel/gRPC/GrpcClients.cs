using Grpc.Net.Client;

namespace SharedKernel.gRPC
{
    public static class GrpcClients
    {
        public static JobPortalGrpc.JobPortalGrpcClient GetJobPortalClient(string url)
        {
            var channel = GrpcChannel.ForAddress(url);
            return new JobPortalGrpc.JobPortalGrpcClient(channel);
        }

        public static MarketplaceGrpc.MarketplaceGrpcClient GetMarketplaceClient(string url)
        {
            var channel = GrpcChannel.ForAddress(url);
            return new MarketplaceGrpc.MarketplaceGrpcClient(channel);
        }

        public static LocalMarketGrpc.LocalMarketGrpcClient GetLocalMarketClient(string url)
        {
            var channel = GrpcChannel.ForAddress(url);
            return new LocalMarketGrpc.LocalMarketGrpcClient(channel);
        }
    }
}