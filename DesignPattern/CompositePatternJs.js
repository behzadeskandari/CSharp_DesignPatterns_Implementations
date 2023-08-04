////1 - Define the DashboardComponent Interface in JavaScript:


class DashboardComponent {
    constructor(name = '', description = '', isVegetarian = false, price = 0) {
        this.name = name;
        this.description = description;
        this._isVegetarian = isVegetarian;
        this.price = price;
    }

    getName() {
        return this.name;
    }

    getDescription() {
        return this.description;
    }

    getPrice() {
        return this.price;
    }

    isVegetarian() {
        return this._isVegetarian;
    }

    print() {
        this.shouldBeOverwritten();
    }

    add() {
        this.shouldBeOverwritten();
    }

    remove() {
        this.shouldBeOverwritten();
    }

    getChild() {
        this.shouldBeOverwritten();
    }

    shouldBeOverwritten() {
        throw new Error('This method should be overridden by subclasses.');
    }
}

export default DashboardComponent;
    //2 - Create Leaf and Composite Classes for Dashboard Sections and Items:
    class DashboardItem extends DashboardComponent {
        constructor(name, description, price, isVegetarian) {
            super(name, description, isVegetarian, price);
        }

        print() {
            console.log(`Item: ${this.name} - ${this.description}, Price: ${this.price}, Vegetarian: ${this.isVegetarian()}`);
        }
    }

class DashboardSection extends DashboardComponent {
    constructor(name, description) {
        super(name, description, false, 0);
        this.items = [];
    }

    print() {
        console.log(`Section: ${this.name} - ${this.description}`);
        console.log('Items:');
        this.items.forEach(item => item.print());
    }

    add(item) {
        this.items.push(item);
    }

    remove(item) {
        const index = this.items.indexOf(item);
        if (index !== -1) {
            this.items.splice(index, 1);
        }
    }

    getChild(index) {
        return this.items[index];
    }
}

//3-Integrate the Composite Design Pattern into the Dashboard Menu:

const mainDashboard = new DashboardSection('Main Dashboard', 'Welcome to the dashboard!');

const salesSection = new DashboardSection('Sales', 'Sales reports and analytics');
const financeSection = new DashboardSection('Finance', 'Financial overview');
const hrSection = new DashboardSection('HR', 'Employee management');

const salesItem1 = new DashboardItem('Sales Report 1', 'Monthly sales report', 0, false);
const salesItem2 = new DashboardItem('Sales Report 2', 'Quarterly sales report', 0, false);
const financeItem1 = new DashboardItem('Profit Analysis', 'Financial analysis for the year', 0, false);
const hrItem1 = new DashboardItem('Employee List', 'List of all employees', 0, false);

salesSection.add(salesItem1);
salesSection.add(salesItem2);
financeSection.add(financeItem1);
hrSection.add(hrItem1);

mainDashboard.add(salesSection);
mainDashboard.add(financeSection);
mainDashboard.add(hrSection);

// Print the entire dashboard hierarchy
mainDashboard.print();

//In this example, we applied the Composite Design Pattern to create a dashboard menu in the front-end JavaScript context.
// The DashboardComponent interface serves as the component interface,
//and the DashboardItem and DashboardSection classes represent the leaf and composite components, respectively.
//This design allows us to build a structured dashboard menu with sections and items and print the hierarchy in a consistent manner.
import MenuComponent from './MenuComponent';

class Menu extends MenuComponent {
    constructor(name, description) {
        super();
        this.menuComponents = [];
        this.name = name;
        this.description = description;
    }

    add(menuComponent) {
        this.menuComponents.push(menuComponent);
    }

    remove(menuComponent) {
        this.menuComponents = this.menuComponents.filter(component => {
            return component !== component;
        });
    }

    getChild(index) {
        return this.menuComponents[index];
    }

    getName() {
        return this.name; 
       
    }

    getDescription() {
        return this.description;
    }

    print() {
        console.log(this.getName() + ": " + this.getDescription());
        console.log("--------------------------------------------");
        this.menuComponents.forEach(component => {
            component.print();
        });
    }
}

export default Menu;




class MenuItem extends MenuComponent {
    print() {
        console.log(this.name + ": " + this.description + ", " + this.price + "euros");
    }
}

export default MenuItem;