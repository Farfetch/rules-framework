import RqlPrompt from '../prompt/RqlPrompt.js';

export default class RqlTerminal {
    readonly prompt: RqlPrompt;
    readonly terminalOutput: HTMLElement;
    constructor() {
        this.terminalOutput = <HTMLElement>document.getElementsByClassName('terminal-output').item(0)!;

        const inputTextbox = <HTMLInputElement>document.getElementById('commandInputTextbox')!;
        const inputContainer = document.getElementById('commandInputContainer')!;
        this.prompt = new RqlPrompt(inputTextbox, inputContainer);

        window.hljs.configure({
            cssSelector: '.language-rql',
            languages: ['rql']
        });
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

    scrollToLastCommand() {
        let element = document.querySelector<HTMLElement>('.terminal > pre')!;
        element.scrollTo(0, element.scrollHeight);
    }
}