using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sds.ReactionFileParser.Tests
{
    public class EmptyRdfTestFixture
    {
        public Guid UserId { get; } = Guid.NewGuid();
        public Guid BlobId { get; }
        public string Bucket { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public Guid CorrelationId { get; } = Guid.NewGuid();

        public EmptyRdfTestFixture(ReactionFileParserTestHarness harness)
        {
            Bucket = UserId.ToString();
            BlobId = harness.UploadBlob(Bucket, "empty.rdf").Result;
            harness.ParseFile(Id, BlobId, Bucket, UserId, CorrelationId).Wait();
        }
    }

    [Collection("Reaction File Parser Test Harness")]
    public class EmptyRdfTests : ReactionFileParserTest, IClassFixture<EmptyRdfTestFixture>
    {
        private Guid CorrelationId;
        private string Bucket;
        private Guid UserId;
        private Guid Id;

        public EmptyRdfTests(ReactionFileParserTestHarness harness, ITestOutputHelper output, EmptyRdfTestFixture initFixture) : base(harness, output)
        {
            Id = initFixture.Id;
            CorrelationId = initFixture.CorrelationId;
            Bucket = initFixture.Bucket;
            UserId = initFixture.UserId;
        }

        [Fact]
        public void RxnParsing_EmptyRxnFile_ReceivedEventsShouldContainValidData()
        {
            var recordParsedEvents = Harness.GetRecordParsedEventsList(Id);
            recordParsedEvents.Should().HaveCount(0);

            var recordParseFailed = Harness.GetRecordParseFailedEventsList(Id);
            recordParseFailed.Should().HaveCount(0);

            var fileParsedEvn = Harness.GetFileParsedEvent(Id);
            fileParsedEvn.Should().BeNull();

            var fileParseFailedEvn = Harness.GetFileParseFailedEvent(Id);
            fileParseFailedEvn.Should().NotBeNull();
            fileParseFailedEvn.Id.Should().Be(Id);
            fileParseFailedEvn.UserId.Should().Be(UserId);
            fileParseFailedEvn.CorrelationId.Should().Be(CorrelationId);
            fileParseFailedEvn.RecordsProcessed.Should().Be(0);
        }
    }
}
