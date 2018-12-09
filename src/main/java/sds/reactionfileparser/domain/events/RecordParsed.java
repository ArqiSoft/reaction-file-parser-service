package sds.reactionfileparser.domain.events;

import java.util.UUID;

import com.fasterxml.jackson.annotation.JsonProperty;
import sds.reactionfileparser.domain.core.Field;
import com.sds.storage.Guid;
import java.util.List;
import sds.messaging.contracts.AbstractContract;

public class RecordParsed extends AbstractContract {

    private UUID id;
    private UUID fileId;
    private String bucket;
    private Guid blobId;
    private long index;
    private List<Field> fields;
    private String timeStamp;
    private UUID userId;

    public RecordParsed() {
        namespace = "Sds.ReactionFileParser.Domain.Events";
        contractName = RecordParsed.class.getSimpleName();
    }

    public void setFileId(UUID FileId) {
        this.fileId = FileId;
    }

    public void setBucket(String Bucket) {
        this.bucket = Bucket;
    }

    public void setBlobId(Guid BlobId) {
        this.blobId = BlobId;
    }

    public void setIndex(long Index) {
        this.index = Index;
    }

    public void setFields(List<Field> Fields) {
        this.fields = Fields;
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

    @JsonProperty("FileId")
    public UUID getFileId() {
        return fileId;
    }

    @JsonProperty("Bucket")
    public String getBucket() {
        return bucket;
    }

    @JsonProperty("BlobId")
    public UUID getBlobId() {
        return blobId.getUUID();
    }

    @JsonProperty("Index")
    public long getIndex() {
        return index;
    }

    @JsonProperty("Fields")
    public List<Field> getFields() {
        return fields;
    }

    /* (non-Javadoc)
     * @see java.lang.Object#toString()
     */
    @Override
    public String toString() {
        return String.format(
                "RecordParsed [id=%s, timeStamp=%s, userId=%s, namespace=%s, contractName=%s, correlationId=%s]",
                id, timeStamp, userId, namespace, contractName, getCorrelationId());
    }

}
