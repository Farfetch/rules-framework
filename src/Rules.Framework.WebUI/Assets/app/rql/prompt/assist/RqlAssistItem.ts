export default class RqlAssistItem {
    private selectKeys: string[] = ['Enter', 'NumpadEnter', 'Tab'];
    text: string;
    readonly htmlElement: HTMLElement;

    constructor(text: string, onSelected: (text: string) => void) {
        this.text = text;
        this.htmlElement = document.createElement('div');
        this.htmlElement.innerText = this.text;
        this.htmlElement.classList.add('terminal-assist-item', 'hljs', 'language-rql');
        window.hljs.highlightElement(this.htmlElement);
        this.htmlElement.addEventListener('click', (event: MouseEvent) => {
            onSelected.call(this, this.text);
        });
    }

    isSelected(): boolean {
        return this.htmlElement.classList.contains('terminal-assist-item-focused');
    }

    setSelected(): void {
        if (!this.isSelected()) {
            this.htmlElement.classList.add('terminal-assist-item-focused');
        }
    }

    setUnselected(): void {
        this.htmlElement.classList.remove('terminal-assist-item-focused');
    }
}