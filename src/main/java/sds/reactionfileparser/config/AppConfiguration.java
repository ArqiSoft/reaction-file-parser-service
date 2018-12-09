package sds.reactionfileparser.config;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.concurrent.ArrayBlockingQueue;
import java.util.concurrent.BlockingQueue;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.autoconfigure.EnableAutoConfiguration;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Configuration;

import com.mongodb.MongoClient;
import com.mongodb.MongoClientURI;
import com.npspot.jtransitlight.consumer.Receiver;
import com.npspot.jtransitlight.consumer.ReceiverBusControl;
import com.npspot.jtransitlight.contract.ContractType;
import com.npspot.jtransitlight.publisher.Bus;
import com.npspot.jtransitlight.publisher.IBusControl;
import com.npspot.jtransitlight.transport.JTransitLightTransportException;
import sds.reactionfileparser.domain.commands.ParseFile;
import com.sds.storage.BlobStorage;
import com.sds.storage.gridfs.GridFSBlobStorage;
import com.sds.validation.JsonSchemaValidator;


@Configuration
public class AppConfiguration  {
    private static final Logger LOGGER = LoggerFactory.getLogger(AppConfiguration.class);

    private static final int CONNECTION_TIMEOUT = 30000;
    private static final long HEARTBEAT_INTERVAL = -1;

    @Bean
    BlobStorage getBlobStorage(
            @Value("${spring.data.mongodb.uri}") String mongoConnectionString, 
            @Value("${spring.data.mongodb.grid-fs-database}") String dbName) {
        
        LOGGER.info("Connecting to MongoDB using url {}", mongoConnectionString);
        LOGGER.info("MongoDB database name: {}", dbName);
        
        return new GridFSBlobStorage(new MongoClient(
                new MongoClientURI(mongoConnectionString)).getDatabase(dbName));    
    }
    
    @Bean
    IBusControl getBusControl(
            @Value("${rabbitmq.connectionString}") String rabbitmqConnectionString) 
                    throws JTransitLightTransportException, URISyntaxException {
        
        rabbitmqConnectionString = rabbitmqConnectionString.replace("rabbitmq://", "amqp://");
        
        return Bus.Factory.createUsingRabbitMq(new URI(rabbitmqConnectionString), 
                CONNECTION_TIMEOUT, 
                ContractType.TRANSACTION_TYPE);
    }
    
    @Bean
    ReceiverBusControl getReceiverBus(@Value(
            "${rabbitmq.connectionString}") String rabbitmqConnectionString) 
                    throws JTransitLightTransportException, URISyntaxException {
        
        rabbitmqConnectionString = rabbitmqConnectionString.replace("rabbitmq://", "amqp://");
        
        return Receiver.Factory.createUsingRabbitMq(
                new URI(rabbitmqConnectionString),
                CONNECTION_TIMEOUT);
    }
    
//    @Bean 
//    JsonSchemaValidator getJsonSchemaValidator() {
//        return new JsonSchemaValidator();
//    }
    
    @Bean
    BlockingQueue<ParseFile> getProcessingQueue(
            @Value("${QUEUE_PREFETCH_SIZE:15}") Integer prefetchSize) {
        LOGGER.debug("QUEUE_PREFETCH_SIZE is set to {}", prefetchSize);
        return new ArrayBlockingQueue<ParseFile>(prefetchSize);
    }
}
