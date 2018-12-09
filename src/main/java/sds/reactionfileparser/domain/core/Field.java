
package sds.reactionfileparser.domain.core;

import com.fasterxml.jackson.annotation.JsonProperty;

public class Field {
    
    private String Name;
    private String Value;

    public Field(String Name, String Value) {
        this.Name = Name;
        this.Value = Value;
    }
    
    @JsonProperty("Name")
    public String getName() {
        return Name;
    }
    
    @JsonProperty("Value")
    public String getValue() {
        return Value;
    }
    
    
}
