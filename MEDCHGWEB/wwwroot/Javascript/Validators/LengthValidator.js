// Define a class to create a LengthCheckValidator object
 class LengthCheckValidator {
     // Define the constructor of the class


    constructor(minLength, maxLength, propertyName) {
        // Assign the parameters to the properties of the object
        this.minLength = minLength;
        this.maxLength = maxLength;
        this.propertyName = propertyName;
        this.errorMessage = "";
    }

    // Define the isValid method of the class
    isValid(value) {
        // Check if the value is a string
        if (typeof value === "string") {
            
            let length = value.length;

            
            if (length < this.minLength || length > this.maxLength) {
                
                this.errorMessage = `${this.propertyName} must be between ${this.minLength} and ${this.maxLength} characters.`;

                
                return false;
            }
        }
        return true;
    }
}
