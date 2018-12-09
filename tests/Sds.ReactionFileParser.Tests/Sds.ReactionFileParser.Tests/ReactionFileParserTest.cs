using MassTransit;
using Sds.Storage.Blob.Core;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Sds.ReactionFileParser.Tests
{
    [CollectionDefinition("Reaction File Parser Test Harness")]
    public class OsdrTestCollection : ICollectionFixture<ReactionFileParserTestHarness>
    {
    }

    public abstract class ReactionFileParserTest
    {
        public ReactionFileParserTestHarness Harness { get; }

        protected IBus Bus => Harness.BusControl;
        protected IBlobStorage BlobStorage => Harness.BlobStorage;

        public ReactionFileParserTest(ReactionFileParserTestHarness fixture, ITestOutputHelper output = null)
        {
            Harness = fixture;

            if (output != null)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo
                    .TestOutput(output, LogEventLevel.Verbose)
                    .CreateLogger()
                    .ForContext<ReactionFileParserTest>();
            }
        }
    }
}
