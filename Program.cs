// Copyright (c) 2021 Fox Council - https://github.com/FoxCouncil/MicrostonksBot

using Microsoft.Extensions.Hosting;

namespace MicrostonksBot
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .Build();

            host.Run();
        }
    }
}