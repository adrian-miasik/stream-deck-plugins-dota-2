function resetCounter() {
    var payload = {};
    payload.property_inspector = 'resetCounter';
    sendPayloadToPlugin(payload);
}

// Validates the provided HTML input element (using ID) to always be a number using a regex check.
function validateInput(inputID){
    
    // Grab input elements
    const inputElement = document.getElementById(inputID);

    // Create constraint
    const regularExpression = new RegExp(/\d/g); // Numbers only
    
    // If our input value is not a number...
    if(!regularExpression.test(inputElement.value))
    {
        // Reset value to default
        inputElement.value = 0;
    }
}