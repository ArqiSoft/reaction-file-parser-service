using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sds.ReactionFileParser.Tests
{
    public class NonRdfTestFixture
    {
        public Guid UserId { get; } = Guid.NewGuid();
        public Guid BlobId { get; }
        public string Bucket { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public Guid CorrelationId { get; } = Guid.NewGuid();

        public NonRdfTestFixture(ReactionFileParserTestHarness harness)
        {
            Bucket = UserId.ToString();
            BlobId = harness.UploadBlob(Bucket, "non-rdf.rdf").Result;
            harness.ParseFile(Id, BlobId, Bucket, UserId, CorrelationId).Wait();
        }
    }

    [Collection("Reaction File Parser Test Harness")]
    public class NonRdfTests : ReactionFileParserTest, IClassFixture<NonRdfTestFixture>
    {
        private Guid CorrelationId;
        private string Bucket;
        private Guid UserId;
        private Guid Id;

        public NonRdfTests(ReactionFileParserTestHarness harness, ITestOutputHelper output, NonRdfTestFixture initFixture) : base(harness, output)
        {
            Id = initFixture.Id;
            CorrelationId = initFixture.CorrelationId;
            Bucket = initFixture.Bucket;
            UserId = initFixture.UserId;
        }

        [Fact]
        public void RdfParsing_NonRdfFile_ReceivedEventsShouldContainValidData()
        {
            var recordParsedEvent = Harness.GetRecordParsedEventsList(Id).SingleOrDefault();
            recordParsedEvent.Should().BeNull();

            var recordParseFailed = Harness.GetRecordParseFailedEventsList(Id).SingleOrDefault();
            recordParseFailed.Should().NotBeNull();
            recordParseFailed.FileId.Should().Be(Id);
            recordParseFailed.UserId.Should().Be(UserId);
            recordParseFailed.CorrelationId.Should().Be(CorrelationId);

            var fileParsedEvn = Harness.GetFileParsedEvent(Id);
            fileParsedEvn.Should().NotBeNull();
            fileParsedEvn.Id.Should().Be(Id);
            fileParsedEvn.UserId.Should().Be(UserId);
            fileParsedEvn.CorrelationId.Should().Be(CorrelationId);
            fileParsedEvn.TotalRecords.Should().Be(1);

            var fileParseFailedEvn = Harness.GetFileParseFailedEvent(Id);
            fileParseFailedEvn.Should().BeNull();
        }
    }
}
