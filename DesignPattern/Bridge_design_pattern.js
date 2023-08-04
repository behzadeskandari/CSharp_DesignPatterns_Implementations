//In this example, we'll create a flexible UI component system that allows you to render styled buttons and input fields using the Bridge design pattern.
//We'll use DOM manipulation to apply styles and create the UI elements.

// Implementor: UIStyle
class UIStyle {
    apply(element) { }
}

// Concrete Implementors: LightStyle, DarkStyle
class LightStyle extends UIStyle {
    apply(element) {
        element.style.backgroundColor = 'white';
        element.style.color = 'black';
    }
}

class DarkStyle extends UIStyle {
    apply(element) {
        element.style.backgroundColor = 'black';
        element.style.color = 'white';
    }
}

// Abstraction: UIComponent
class UIComponent {
    constructor(style) {
        this.style = style;
    }

    render() { }
}

// Refined Abstraction: Button
class Button extends UIComponent {
    constructor(style) {
        super(style);
    }

    render() {
        const button = document.createElement('button');
        button.innerText = 'Click Me';
        this.style.apply(button);
        document.body.appendChild(button);
    }
}

// Refined Abstraction: InputField
class InputField extends UIComponent {
    constructor(style) {
        super(style);
    }

    render() {
        const input = document.createElement('input');
        input.type = 'text';
        this.style.apply(input);
        document.body.appendChild(input);
    }
}

// Client Code
const lightStyle = new LightStyle();
const darkStyle = new DarkStyle();

const lightButton = new Button(lightStyle);
const darkButton = new Button(darkStyle);

const lightInput = new InputField(lightStyle);
const darkInput = new InputField(darkStyle);

lightButton.render();
darkButton.render();

lightInput.render();
darkInput.render();


///UIStyle: The implementor class that defines the rendering style interface.
//LightStyle, DarkStyle: Concrete implementors that implement different rendering styles using DOM manipulation.
//UIComponent: The abstraction class representing a UI component that can be rendered with a specific style.
//Button, InputField: Refined abstractions that extend UIComponent to represent specific UI elements.
//By using the Bridge pattern, you can create UI components that are easily stylable with different styles without altering their core rendering logic.
//This allows you to create consistent UI components while maintaining the flexibility to adapt to various design requirements.
//In a real ERP system, you could apply the Bridge pattern to create a variety of styled UI elements, 
//ranging from buttons and input fields to more complex components like modals or navigation menus,
//all while using DOM manipulation to apply styles and create the visual interface.