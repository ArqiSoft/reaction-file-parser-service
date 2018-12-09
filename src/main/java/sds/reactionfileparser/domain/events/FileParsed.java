package sds.reactionfileparser.domain.events;

import java.util.UUID;

import com.fasterxml.jackson.annotation.JsonProperty;
import static java.lang.reflect.Array.get;
import java.lang.reflect.Field;
import java.util.List;
import sds.messaging.contracts.AbstractContract;

public class FileParsed extends AbstractContract {

    private UUID id;
    private String timeStamp;
    private UUID userId;
    private long TotalRecords;
    private long ParsedRecords;
    private long FailedRecords;
    private List<String> Fields;

    public FileParsed() {
        namespace = "Sds.ReactionFileParser.Domain.Events";
        contractName = FileParsed.class.getSimpleName();
    }

    public void setTotalRecords(long TotalRecords) {
        this.TotalRecords = TotalRecords;
    }

    public void setParsedRecords(long ParsedRecords) {
        this.ParsedRecords = ParsedRecords;
    }

    public void setFailedRecords(long FailedRecords) {
        this.FailedRecords = FailedRecords;
    }

    public void setFields(List<String> Fields) {
        this.Fields = Fields;
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

    @JsonProperty("TotalRecords")
    public long getTotalRecords() {
        return TotalRecords;
    }

    @JsonProperty("ParsedRecords")
    public long getParsedRecords() {
        return ParsedRecords;
    }

    @JsonProperty("FailedRecords")
    public long getFailedRecords() {
        return FailedRecords;
    }

    @JsonProperty("Fields")
    public List<String> getFields() {
        return Fields;
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
