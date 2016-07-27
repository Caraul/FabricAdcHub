using System;
using System.Threading;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace FabricAdcHub.Sender
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                ActorRuntime.RegisterActorAsync<Sender>(
                   (context, actorType) => new ActorService(context, actorType, () => new Sender())).GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
