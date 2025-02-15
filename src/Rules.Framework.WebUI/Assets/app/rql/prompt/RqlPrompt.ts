import RqlAssist from "./assist/RqlAssist.js";
import RqlPromptHistory from "./RqlPromptHistory.js";
import RqlPromptOptions from "./RqlPromptOptions.js";

export default class RqlPrompt {
    readonly assist: RqlAssist;
    readonly container: HTMLElement;
    readonly history: RqlPromptHistory;
    readonly inputDisplay: HTMLElement;
    readonly inputTextbox: HTMLInputElement;
    readonly options: RqlPromptOptions;

    constructor(inputElement: HTMLInputElement, containerElement: HTMLElement, options?: RqlPromptOptions) {
        this.container = containerElement;
        this.inputTextbox = inputElement;
        this.addEventListener('input', (_) => this.refreshInputDisplay());

        this.inputDisplay = document.createElement('code');
        this.inputDisplay.className = 'terminal-line mb-1 layered-item language-rql';

        const inputDisplayPreformatted = document.createElement('pre');
        inputDisplayPreformatted.className = 'col-12 terminal-input bg-dark text-bg-dark border-0 mb-0 layered-item';

        inputDisplayPreformatted.appendChild(this.inputDisplay);
        this.container.insertBefore(inputDisplayPreformatted, this.container.firstChild);

        this.assist = new RqlAssist();
        this.container.appendChild(this.assist.element);

        this.history = new RqlPromptHistory();
        this.options = options ?? new RqlPromptOptions();
    }

    get input(): string {
        return this.inputTextbox.value;
    }

    set input(value: string) {
        this.inputTextbox.value = value;
        this.inputTextbox.dispatchEvent(new Event('change'));
        this.refreshInputDisplay();
    }

    get scrollOffset(): number {
        return this.inputTextbox.scrollLeft;
    }

    get selection(): number {
        return this.inputTextbox.selectionStart!;
    }

    set selection(value: number) {
        this.inputTextbox.selectionStart = value;
        this.inputTextbox.selectionEnd = value;
    }

    addEventListener<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLInputElement, ev: HTMLElementEventMap[K]) => any, options?: boolean | AddEventListenerOptions): void {
        this.inputTextbox.addEventListener(type, listener, options);
    }

    focus(): boolean | undefined {
        var selection = window.getSelection()!;
        if (selection.type != "Range") {
            this.inputTextbox.focus();
            return false;
        }
    }

    private refreshInputDisplay() {
        this.inputDisplay.innerText = this.input;
        delete this.inputDisplay.dataset['highlighted'];
        window.hljs.highlightElement(this.inputDisplay);
        this.inputDisplay.scrollTo({
            top: 0,
            left: this.scrollOffset,
            behavior: 'instant'
        });
    }
}