using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Sds.ReactionFileParser.Tests
{
    public class ValidRxnTestFixture
    {
        public Guid UserId { get; } = Guid.NewGuid();
        public Guid BlobId { get; }
        public string Bucket { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public Guid CorrelationId { get; } = Guid.NewGuid();

        public ValidRxnTestFixture(ReactionFileParserTestHarness harness)
        {
            Bucket = UserId.ToString();
            BlobId = harness.UploadBlob(Bucket, "10001.rxn").Result;
            harness.ParseFile(Id, BlobId, Bucket, UserId, CorrelationId).Wait();
        }
    }

    [Collection("Reaction File Parser Test Harness")]
    public class ValidRxnTests : ReactionFileParserTest, IClassFixture<ValidRxnTestFixture>
    {
        private Guid CorrelationId;
        private string Bucket;
        private Guid UserId;
        private Guid Id;

        public ValidRxnTests(ReactionFileParserTestHarness harness, ITestOutputHelper output, ValidRxnTestFixture initFixture) : base(harness, output)
        {
            Id = initFixture.Id;
            CorrelationId = initFixture.CorrelationId;
            Bucket = initFixture.Bucket;
            UserId = initFixture.UserId;
        }

        [Fact]
        public void RxnParsing_ValidRxnFile_ReceivedEventsShouldContainValidData()
        {
            var recordParsedEvent = Harness.GetRecordParsedEventsList(Id).SingleOrDefault();
            recordParsedEvent.Should().NotBeNull();
            recordParsedEvent.FileId.Should().Be(Id);
            recordParsedEvent.UserId.Should().Be(UserId);
            recordParsedEvent.CorrelationId.Should().Be(CorrelationId);

            var recordParseFailed = Harness.GetRecordParseFailedEventsList(Id);
            recordParseFailed.Should().HaveCount(0);

            var fileParsedEvn = Harness.GetFileParsedEvent(Id);
            fileParsedEvn.Id.Should().Be(Id);
            fileParsedEvn.UserId.Should().Be(UserId);
            fileParsedEvn.CorrelationId.Should().Be(CorrelationId);
            fileParsedEvn.TotalRecords.Should().Be(1);

            var fileParseFailedEvn = Harness.GetFileParseFailedEvent(CorrelationId);
            fileParseFailedEvn.Should().BeNull();
        }

        [Fact]
        public async Task RxnParsing_ValidRxnFile_UploadedBlobsContainNotEmptyData()
        {
            var recordParsedEvent = Harness.GetRecordParsedEventsList(Id).SingleOrDefault();
            recordParsedEvent.Should().NotBeNull();
            var blobInfo = await Harness.BlobStorage.GetFileInfo(recordParsedEvent.BlobId, Bucket);
            blobInfo.Should().NotBeNull();
            Path.GetExtension(blobInfo.FileName).Should().BeEquivalentTo(".rxn");
            blobInfo.Length.Should().BeGreaterThan(0);
            blobInfo.ContentType.ToLower().Should().BeEquivalentTo("chemical/x-mdl-rxn");
        }
    }
}
