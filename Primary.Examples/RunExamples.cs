using System.Threading.Tasks;

namespace Primary.Examples
{
    internal static class RunExamples
    {
        private static async Task Main()
        {
            await GetInstrumentsAndDataExample.Run();
            //await OrderExample.Run();
            //await WebSocketExample.Run();
        }
    }
}
