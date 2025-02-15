import LinkedList from "../../common/LinkedList.js";
import LinkedListNode from "../../common/LinkedListNode.js";

export default class RqlPromptHistory {
    private inputHistory: LinkedList<string>;
    private currentInput: LinkedListNode<string> | null;

    constructor() {
        this.inputHistory = new LinkedList<string>();
        this.currentInput = null;
    }

    get current(): string | null {
        return this.currentInput?.value ?? null;
    }

    navigateToRecentInput(): void {
        if (this.currentInput) {
            if (this.currentInput.previous) {
                this.currentInput = this.currentInput.previous;
            } else {
                this.currentInput = null;
            }
        }
    }

    navigateToOlderInput(): void {
        if (this.currentInput) {
            if (this.currentInput.next) {
                this.currentInput = this.currentInput.next;
            }
        } else {
            this.currentInput = this.inputHistory.first;
        }
    }

    pushInputHistory(input: string): void {
        if (this.inputHistory.count() >= 50) {
            this.inputHistory.removeLast();
        }

        this.inputHistory.addFirst(input);
        this.reset();
    }

    reset(): void {
        this.currentInput = null;
    }
}