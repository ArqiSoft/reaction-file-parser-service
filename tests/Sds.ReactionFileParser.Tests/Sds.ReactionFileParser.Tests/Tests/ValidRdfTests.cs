using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sds.ReactionFileParser.Tests
{
    public class ValidRdfTestFixture
    {
        public Guid UserId { get; } = Guid.NewGuid();
        public Guid BlobId { get; }
        public string Bucket { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public Guid CorrelationId { get; } = Guid.NewGuid();

        public ValidRdfTestFixture(ReactionFileParserTestHarness harness)
        {
            Bucket = UserId.ToString();
            BlobId = harness.UploadBlob(Bucket, "ccr0401.rdf").Result;
            harness.ParseFile(Id, BlobId, Bucket, UserId, CorrelationId).Wait();
        }
    }

    [Collection("Reaction File Parser Test Harness")]
    public class ValidRdfTests : ReactionFileParserTest, IClassFixture<ValidRdfTestFixture>
    {
        private Guid CorrelationId;
        private string Bucket;
        private Guid UserId;
        private Guid Id;

        public ValidRdfTests(ReactionFileParserTestHarness harness, ITestOutputHelper output, ValidRdfTestFixture initFixture) : base(harness, output)
        {
            Id = initFixture.Id;
            CorrelationId = initFixture.CorrelationId;
            Bucket = initFixture.Bucket;
            UserId = initFixture.UserId;
        }

        [Fact]
        public void RdfParsing_ValidRdfFile_ReceivedEventsShouldContainValidData()
        {
            var recordParsedEvents = Harness.GetRecordParsedEventsList(Id);
            recordParsedEvents.Should().HaveCount(75);
            foreach (var evn in recordParsedEvents)
            {
                evn.FileId.Should().Be(Id);
                evn.UserId.Should().Be(UserId);
                evn.CorrelationId.Should().Be(CorrelationId);
            }

            var recordParseFailed = Harness.GetRecordParseFailedEventsList(Id);
            recordParseFailed.Should().HaveCount(0);

            var fileParsedEvn = Harness.GetFileParsedEvent(Id);
            fileParsedEvn.Id.Should().Be(Id);
            fileParsedEvn.UserId.Should().Be(UserId);
            fileParsedEvn.CorrelationId.Should().Be(CorrelationId);
            fileParsedEvn.TotalRecords.Should().Be(75);

            var fileParseFailedEvn = Harness.GetFileParseFailedEvent(Id);
            fileParseFailedEvn.Should().BeNull();
        }

        [Fact]
        public async Task RdfParsing_ValidRdfFile_UploadedBlobsContainNotEmptyData()
        {
            var recordParsedEvents = Harness.GetRecordParsedEventsList(Id);
            foreach (var evn in recordParsedEvents)
            {
                var blobInfo = await Harness.BlobStorage.GetFileInfo(evn.BlobId, Bucket);
                blobInfo.Should().NotBeNull();
                Path.GetExtension(blobInfo.FileName).Should().BeEquivalentTo(".rxn");
                blobInfo.Length.Should().BeGreaterThan(0);
                blobInfo.ContentType.ToLower().Should().BeEquivalentTo("chemical/x-mdl-rxn");
            }
        }

    }
}
