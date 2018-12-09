using CQRSlite.Events;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using MassTransit.Scoping;
using MassTransit.Testing.MessageObservers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Sds.MassTransit.Observers;
using Sds.MassTransit.RabbitMq;
using Sds.ReactionFileParser.Domain.Events;
using Sds.Serilog;
using Sds.Storage.Blob.Core;
using Sds.Storage.Blob.GridFs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sds.ReactionFileParser.Tests
{
    public class ReactionFileParserTestHarness : IDisposable
    {
        protected IServiceProvider _serviceProvider;

        public IBlobStorage BlobStorage { get { return _serviceProvider.GetService<IBlobStorage>(); } }
       
        public IBusControl BusControl { get { return _serviceProvider.GetService<IBusControl>(); } }

        private List<ExceptionInfo> Faults = new List<ExceptionInfo>();

        public ReceivedMessageList Received { get; } = new ReceivedMessageList(TimeSpan.FromSeconds(10));

        public ReactionFileParserTestHarness()
        {
            var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", true, true)
                 .AddEnvironmentVariables()
                 .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.ControlledBy(new EnvironmentVariableLoggingLevelSwitch("%OSDR_LOG_LEVEL%"))
                .CreateLogger();

            Log.Information("Staring Imaging tests");

            var services = new ServiceCollection();

            services.AddOptions();
            services.AddSingleton<IEventPublisher, CqrsLite.MassTransit.MassTransitBus>();

            services.AddTransient<IBlobStorage, GridFsStorage>(x =>
            {
                var blobStorageUrl = new MongoUrl(Environment.ExpandEnvironmentVariables(configuration["GridFs:ConnectionString"]));
                var client = new MongoClient(blobStorageUrl);

                return new GridFsStorage(client.GetDatabase(blobStorageUrl.DatabaseName));
            });

            services.AddSingleton<IConsumerScopeProvider, DependencyInjectionConsumerScopeProvider>();

            services.AddSingleton(container => Bus.Factory.CreateUsingRabbitMq(x =>
            {
                IRabbitMqHost host = x.Host(new Uri(Environment.ExpandEnvironmentVariables(configuration["MassTransit:ConnectionString"])), h => { });

                x.RegisterConsumers(host, container, e =>
                {
                    e.UseInMemoryOutbox();
                });

                x.ReceiveEndpoint(host, "processing_fault_queue", e =>
                {
                    e.Handler<Fault>(async context =>
                    {
                        Faults.AddRange(context.Message.Exceptions.Where(ex => !ex.ExceptionType.Equals("System.InvalidOperationException")));

                        await Task.CompletedTask;
                    });
                });

                x.ReceiveEndpoint(host, "processing_update_queue", e =>
                {
                    e.Handler<RecordParsed>(context => { Received.Add(context); return Task.CompletedTask; });
                    e.Handler<RecordParseFailed>(context => { Received.Add(context); return Task.CompletedTask; });
                    e.Handler<FileParsed>(context => { Received.Add(context); return Task.CompletedTask; });
                    e.Handler<FileParseFailed>(context => { Received.Add(context); return Task.CompletedTask; });
                });
            }));

            _serviceProvider = services.BuildServiceProvider();

            var busControl = _serviceProvider.GetRequiredService<IBusControl>();

            busControl.ConnectPublishObserver(new PublishObserver());
            busControl.ConnectConsumeObserver(new ConsumeObserver());

            busControl.Start();
        }

        public List<RecordParsed> GetRecordParsedEventsList(Guid fileId)
        {
            return Received
                .ToList()
                .Where(m => m.Context.GetType().IsGenericType && m.Context.GetType().GetGenericArguments()[0] == typeof(RecordParsed))
                .Select(m => (m.Context as ConsumeContext<RecordParsed>).Message)
                .Where(m => m.FileId == fileId).ToList();
        }

        public FileParsed GetFileParsedEvent(Guid fileId)
        {
            return Received
                .ToList()
                .Where(m => m.Context.GetType().IsGenericType && m.Context.GetType().GetGenericArguments()[0] == typeof(FileParsed))
                .Select(m => (m.Context as ConsumeContext<FileParsed>).Message)
                .Where(m => m.Id == fileId).ToList().SingleOrDefault();
        }

        public FileParseFailed GetFileParseFailedEvent(Guid fileId)
        {
            return Received
                .ToList()
                .Where(m => m.Context.GetType().IsGenericType && m.Context.GetType().GetGenericArguments()[0] == typeof(FileParseFailed))
                .Select(m => (m.Context as ConsumeContext<FileParseFailed>).Message)
                .Where(m => m.Id == fileId).ToList().SingleOrDefault();
        }

        public List<RecordParseFailed> GetRecordParseFailedEventsList(Guid fileId)
        {
            return Received
                .ToList()
                .Where(m => m.Context.GetType().IsGenericType && m.Context.GetType().GetGenericArguments()[0] == typeof(RecordParseFailed))
                .Select(m => (m.Context as ConsumeContext<RecordParseFailed>).Message)
                .Where(m => m.FileId == fileId).ToList();
        }

        public virtual void Dispose()
        {
            var busControl = _serviceProvider.GetRequiredService<IBusControl>();
            busControl.Stop();
        }
    }
}