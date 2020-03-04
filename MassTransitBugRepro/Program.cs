using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using MassTransit.PublishPipeSpecifications;
using Microsoft.ApplicationInsights;

namespace MassTransitBugRepro
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var bus = Bus.Factory.CreateUsingInMemory(sbc =>
			{
				var host = sbc.Host(h =>
				{
				});

				sbc.ReceiveEndpoint("my_queue", endpoint =>
				{
					endpoint.Handler<MyMessage>(async context =>
					{
						await Console.Out.WriteLineAsync($"Received: {context.Message.Value}");
					});
				});

				sbc.UseApplicationInsightsOnPublish(new TelemetryClient());
			});

			await bus.StartAsync();

			// Error happens here
			await bus.Publish(new MyMessage { Value = "Hello, World." });

			Console.ReadLine();

			await bus.StopAsync();
		}

    }

	class MyMessage
	{
		public string Value { get; set; }
	}
}
