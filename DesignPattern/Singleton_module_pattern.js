// Singleton module pattern
const AppConfig = (function () {
    // Private instance variable
    let instance;

    // Private configuration variables
    const privateApiEndpoint = 'https://api.example.com';
    const privateThemeColor = 'blue';

    // Private constructor
    function AppConfig() {
        if (instance) {
            return instance;
        }
        instance = this;

        // Public configuration methods
        this.getApiEndpoint = function () {
            return privateApiEndpoint;
        };

        this.getThemeColor = function () {
            return privateThemeColor;
        };
    }

    // Public getInstance method
    AppConfig.getInstance = function () {
        if (!instance) {
            instance = new AppConfig();
        }
        return instance;
    };

    return AppConfig;
})();

// Client code
const appConfig = AppConfig.getInstance();

document.getElementById('api-endpoint').textContent = appConfig.getApiEndpoint();
document.getElementById('theme-color').textContent = appConfig.getThemeColor();

//We define the AppConfig class using a module pattern to create a private scope. The instance and configuration variables are private and not accessible from outside the module.

//We provide public methods(getApiEndpoint and getThemeColor) to access the configuration data in a controlled manner.

//The getInstance method provides controlled access to the singleton instance.It ensures that only one instance is created and returned, regardless of how many times it's called.

//The client code accesses the singleton instance through the getInstance method, ensuring that the same instance is used throughout the application.

//By using this approach, you can manage the scope and lifetime of the singleton AppConfig instance effectively.This helps prevent potential issues with dependency management, avoids global variables, and promotes a clean and well - structured design in your ERP system or any JavaScript application.