package sds.reactionfileparser.domain.events;

import java.util.UUID;

import com.fasterxml.jackson.annotation.JsonProperty;
import sds.messaging.contracts.AbstractContract;

public class RecordParseFailed extends AbstractContract {

    private UUID fileId;
    private long index;
    private String message;
    private UUID id;
    private UUID userId;
    private String timeStamp;
    
    public RecordParseFailed() {
        namespace = "Sds.ReactionFileParser.Domain.Events";
        contractName = RecordParseFailed.class.getSimpleName();
    }

    public void setMessage(String Message) {
        this.message = Message;
    }  

    /**
     * @return the id
     */
    @JsonProperty("Id")
    public UUID getId() {
        return id;
    }

    /**
     * @param id the id to set
     */
    public void setId(UUID id) {
        this.id = id;
    }

    /**
     * @return the timeStamp
     */
    @JsonProperty("TimeStamp")
    public String getTimeStamp() {
        return timeStamp;
    }

    /**
     * @param timeStamp the timeStamp to set
     */
    public void setTimeStamp(String timeStamp) {
        this.timeStamp = timeStamp;
    }

    /**
     * @return the userId
     */
    @JsonProperty("UserId")
    public UUID getUserId() {
        return userId;
    }

    /**
     * @param userId the userId to set
     */
    public void setUserId(UUID userId) {
        this.userId = userId;
    }

    public void setFileId(UUID FileId) {
        this.fileId = FileId;
    }

    public void setIndex(long Index) {
        this.index = Index;
    }
    
    @JsonProperty("FileId")
    public UUID getFileId() {
        return fileId;
    }
    @JsonProperty("Index")
    public long getIndex() {
        return index;
    }

    @JsonProperty("Message")
    public String getMessage() {
        return message;
    }
    
    /* (non-Javadoc)
     * @see java.lang.Object#toString()
     */
    @Override
    public String toString() {
        return String.format(
                "FailureEvent [id=%s, timeStamp=%s, userId=%s, namespace=%s, contractName=%s, correlationId=%s]",
                id, timeStamp, userId, namespace, contractName, getCorrelationId());
    }
    
    

}
