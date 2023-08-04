//The Strategy design pattern is used to define a family of interchangeable algorithms, 
//allowing clients to choose an algorithm from the family without modifying their code.In the context of a front - end ERP system,
//you might use the Strategy pattern to implement different strategies for handling certain tasks or calculations in your user interface.
//Let's explore a detailed example of using the Strategy design pattern in a pure JavaScript front-end ERP system to implement different pricing strategies for products with DOM manipulation.
//Example: Product Pricing Strategies with Strategy Pattern and DOM Manipulation
//In this example, we'll create a product pricing system that allows users to choose different pricing strategies (regular price, sale price) for products and calculates the final price accordingly.


// Strategy: PricingStrategy
class PricingStrategy {
    calculateFinalPrice(price) {
        return price;
    }
}

// Concrete Strategies
class RegularPriceStrategy extends PricingStrategy {
    calculateFinalPrice(price) {
        return price;
    }
}

class SalePriceStrategy extends PricingStrategy {
    constructor(discountPercentage) {
        super();
        this.discountPercentage = discountPercentage;
    }

    calculateFinalPrice(price) {
        return price * (1 - this.discountPercentage / 100);
    }
}

// Context: Product
class Product {
    constructor(name, price, pricingStrategy) {
        this.name = name;
        this.price = price;
        this.pricingStrategy = pricingStrategy;
    }

    setPricingStrategy(pricingStrategy) {
        this.pricingStrategy = pricingStrategy;
    }

    calculateFinalPrice() {
        return this.pricingStrategy.calculateFinalPrice(this.price);
    }
}

// Client Code
const regularPriceStrategy = new RegularPriceStrategy();
const salePriceStrategy = new SalePriceStrategy(20);

const product = new Product('Widget', 100, regularPriceStrategy);

document.getElementById('btn-regular-price').addEventListener('click', () => {
    product.setPricingStrategy(regularPriceStrategy);
    updatePriceDisplay();
});

document.getElementById('btn-sale-price').addEventListener('click', () => {
    product.setPricingStrategy(salePriceStrategy);
    updatePriceDisplay();
});

const updatePriceDisplay = () => {
    const priceElement = document.getElementById('product-price');
    priceElement.textContent = `$${product.calculateFinalPrice().toFixed(2)}`;
};

updatePriceDisplay();




//PricingStrategy: The strategy class that defines the interface for different pricing strategies.
//Concrete Strategies(RegularPriceStrategy, SalePriceStrategy): Concrete strategy classes that implement different pricing algorithms.
//Product: The context class that uses a pricing strategy to calculate the final price of a product.
//Client code: Sets up different pricing strategies, creates a product, and updates the DOM to display the final price based on the selected strategy.
//By using the Strategy pattern, you can encapsulate different algorithms for pricing within separate strategy classes, making it easy to switch between strategies without modifying the client code.
///This is useful in an ERP system where you might need to implement different calculation methods or behaviors for different aspects of the application, such as pricing, shipping, or tax calculations.
//In a real ERP system, you could apply the Strategy pattern to various scenarios, such as handling different payment methods, 
///currency conversions, or discount calculations, while maintaining a consistent interface for interacting with those strategies.