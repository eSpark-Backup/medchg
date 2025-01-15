class RequireValidator {
    // Define the constructor of the class
    constructor(isRequired, propertyName) {
        // Assign the parameters to the properties of the object
        this.isRequired = isRequired;
        this.propertyName = propertyName;
        this.errorMessage = "";
    }

    
    isValid(value) {
        
        if (typeof value === "string") {
            
            if (value === "") {
               
                this.errorMessage = `${this.propertyName} is required`;
                
                return false;
            }
            
        }
        return true;
    }
}