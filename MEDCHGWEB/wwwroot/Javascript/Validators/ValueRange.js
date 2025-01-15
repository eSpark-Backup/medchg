// Define a class to create a ValueChecker object
class ValueRange {
    // Define the constructor of the class
    constructor(maxValue, propertyName) {
        // Assign the parameters to the properties of the object
        this.maxValue = maxValue;
        this.propertyName = propertyName;
        this.errorMessage = "";
    }

    // Define the isValid method of the class
    isValid(value) {
        //Convert string to number
        value = Number(value);

        // Check if the value is a number
        if (typeof value === "number") {
            // Check if the value is within the range
            if (value < 1 || value > this.maxValue) {
                // Set the error message
                this.errorMessage = `${this.propertyName} value must be between 1 and ${this.maxValue}.`;
                // Return false if the validation fails
                return false;
            }
        }

        // Return true if the validation passes
        return true;
    }
}