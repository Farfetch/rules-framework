class RqlTerminalHandler {
    keyUpRedirectKeys: string[] = ["Enter", "NumpadEnter", "ArrowUp", "ArrowDown"];
    keyUpClearKeys: string[] = ['Enter', 'NumpadEnter'];
    constructor() {
    }

    handleKeyDown(event: KeyboardEvent, inputTextbox: HTMLInputElement, inputDisplay: HTMLElement) {
        const scrollOffset = inputTextbox.scrollLeft;
        inputDisplay.scrollTo({
            top: 0,
            left: scrollOffset,
            behavior: 'instant'
        });
    }

    handleKeyUp(event: KeyboardEvent, inputTextbox: HTMLInputElement, inputDisplay: HTMLElement) {
        if (this.keyUpRedirectKeys.includes(event.key)) {
            const keyboardEvent = new KeyboardEvent(
                'keyup',
                {
                    key: event.key,
                    ctrlKey: false,
                    altKey: false,
                    shiftKey: false,
                    code: event.key,
                    view: window,
                    bubbles: true,
                    cancelable: true,
                    charCode: 0
                });
            inputDisplay.dispatchEvent(keyboardEvent);
        }

        if (this.keyUpClearKeys.includes(event.key)) {
            inputDisplay.innerText = '';
        }
        else {
            inputDisplay.innerText = inputTextbox.value;
        }

        delete inputDisplay.dataset['highlighted'];
        window.hljs.highlightElement(inputDisplay);
        const scrollOffset = inputTextbox.scrollLeft;
        inputDisplay.scrollTo({
            top: 0,
            left: scrollOffset,
            behavior: 'instant'
        });
    }
}

class RqlTerminal {
    inputTextbox: HTMLInputElement;
    inputDisplay: HTMLElement;
    rqlTerminalHandler: RqlTerminalHandler;
    terminalOutput: HTMLElement;
    constructor() {
        this.inputTextbox = <HTMLInputElement>document.getElementById('commandInputTextbox')!;
        this.inputDisplay = document.getElementById('commandInputDisplay')!;
        this.rqlTerminalHandler = new RqlTerminalHandler();
        this.terminalOutput = <HTMLElement>document.getElementsByClassName('terminal-output').item(0)!;
    }

    initRqlTerminal() {
        this.inputTextbox.addEventListener('keydown', (event: KeyboardEvent) => {
            rqlTerminal.rqlTerminalHandler.handleKeyDown(event, this.inputTextbox, this.inputDisplay);
        });
        this.inputTextbox.addEventListener('keyup', (event: KeyboardEvent) => {
            rqlTerminal.rqlTerminalHandler.handleKeyUp(event, this.inputTextbox, this.inputDisplay);
        });
        window.hljs.configure({
            cssSelector: '.language-rql',
            languages: ['rql']
        });
    }

    focusOnInput() {
        var selection = window.getSelection()!;
        if (selection.type != "Range") {
            this.inputTextbox.focus();
            return false;
        }
    }

    refreshOutputDisplay() {
        window.hljs.highlightAll();
        this.terminalOutput.childNodes.forEach(child => {
            if (child instanceof Text) {
                let textWrapper = new HTMLSpanElement();
                textWrapper.innerText = child.textContent!;
                this.terminalOutput.replaceChild(child, textWrapper);
            }
        });
    }

    refreshInputDisplay(value: string) {
        this.inputDisplay.innerText = value;
        delete this.inputDisplay.dataset['highlighted'];
        window.hljs.highlightElement(this.inputDisplay);
    }

    scrollToLastCommand() {
        let element = document.querySelector<HTMLElement>('.terminal > pre')!;
        element.scrollTo(0, element.scrollHeight);
    }
}

let rqlTerminal = new RqlTerminal();
rqlTerminal.initRqlTerminal();
(<any>window).rqlTerminal = rqlTerminal;
export default rqlTerminal;