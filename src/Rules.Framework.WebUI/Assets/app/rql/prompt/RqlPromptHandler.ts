import ShowAssistArgs from "./assist/ShowAssistArgs.js";
import Token from "./assist/Token.js";
import RqlPromptKeys from "./RqlPromptKeys.js";
import RqlPrompt from "./RqlPrompt.js";

export default class RqlPromptHandler {
    readonly prompt: RqlPrompt;
    readonly relay: HTMLElement;

    constructor(prompt: RqlPrompt, relay: HTMLElement) {
        this.prompt = prompt;
        this.prompt.addEventListener('keydown', (e: KeyboardEvent) => this.handleKeyDown(e));
        this.prompt.addEventListener('keyup', (e: KeyboardEvent) => this.handleKeyUp(e));
        this.relay = relay;
    }

    handleKeyDown(event: KeyboardEvent): void {
        if (this.prompt.assist.isVisible() && RqlPromptKeys.assistKeys.includes(event.key)) {
            event.preventDefault();
            return;
        }

        this.prompt.inputDisplay.scrollTo({
            top: 0,
            left: this.prompt.scrollOffset,
            behavior: 'instant'
        });
    }

    handleKeyUp(event: KeyboardEvent): void {
        if (this.prompt.assist.isVisible()) {
            if (RqlPromptKeys.assistAcceptSuggestionKeys.includes(event.key)) {
                const selectedAssist = this.prompt.assist.getSelectedAssist();
                if (selectedAssist != null) {
                    this.applyAssistSuggestion(selectedAssist.text);
                    event.preventDefault();
                    return;
                }
            }

            switch (event.key) {
                case RqlPromptKeys.assistMovePreviousKey:
                    this.prompt.assist.movePreviousAssist();
                    event.preventDefault();
                    return;

                case RqlPromptKeys.assistMoveNextKey:
                    this.prompt.assist.moveNextAssist();
                    event.preventDefault();
                    return;

                case RqlPromptKeys.assistHideKey:
                    this.prompt.assist.hide();
                    event.preventDefault();
                    return;

                default:
                    break;
            }
        }

        if (RqlPromptKeys.historyKeys.includes(event.key)) {
            switch (event.key) {
                case RqlPromptKeys.historyNavigateOlderInputKey:
                    this.prompt.history.navigateToOlderInput();
                    if (this.prompt.history.current) {
                        this.prompt.input = this.prompt.history.current;
                    }
                    return;

                case RqlPromptKeys.historyNavigateRecentInputKey:
                    this.prompt.history.navigateToRecentInput();
                    if (this.prompt.history.current) {
                        this.prompt.input = this.prompt.history.current;
                    } else {
                        this.prompt.input = '';
                    }
                    return;

                default:
                    break;
            }
        }

        if (RqlPromptKeys.sendInputKeys.includes(event.key) && this.prompt.input.trim()) {
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
            this.relay.dispatchEvent(keyboardEvent);
            this.prompt.history.pushInputHistory(this.prompt.input);
            if (this.prompt.options.clearOnSubmit) {
                this.prompt.input = '';
            }
        }

        const caretPositionRect = this.getCaretPosition();
        const showAssistArgs = new ShowAssistArgs();
        showAssistArgs.caretPosition = this.prompt.selection;
        showAssistArgs.commandText = this.prompt.input;
        showAssistArgs.leftPositionPixels = caretPositionRect.left;
        showAssistArgs.onSelectedAssistSuggestion = (selected) => {
            this.applyAssistSuggestion(selected);
        };
        showAssistArgs.topPositionPixels = caretPositionRect.top;
        this.prompt.assist.show(showAssistArgs);

        this.prompt.inputDisplay.scrollTo({
            top: 0,
            left: this.prompt.scrollOffset,
            behavior: 'instant'
        });
    }

    private applyAssistSuggestion(assistSuggestion: string): void {
        const originalText = this.prompt.input;
        const tokenToReplace = Token.atIndex(originalText, this.prompt.selection);
        const textBeforeToken = originalText.substring(0, tokenToReplace.beginIndex);
        const textAfterToken = originalText.substring(tokenToReplace.endIndex + 1, originalText.length);
        const newText = `${textBeforeToken}${assistSuggestion}${textAfterToken}`;
        const newSelectionIndex = textBeforeToken.length + assistSuggestion.length;
        this.prompt.input = newText;
        this.prompt.selection = newSelectionIndex;
        this.prompt.assist.hide();
    }

    private getCaretPosition(): DOMRect {
        // Create mirror element to determine the caret position.
        const preformattedElement = document.createElement('pre');
        preformattedElement.className = 'col-12 terminal-input bg-dark text-bg-dark border-0 mb-0 layered-item';
        this.prompt.container.insertBefore(preformattedElement, this.prompt.container.firstChild);

        const inputDisplayMirror = document.createElement('code');
        inputDisplayMirror.className = 'terminal-line mb-1 layered-item';
        preformattedElement.appendChild(inputDisplayMirror);

        const commandText = this.prompt.input;
        const textBeforeCursor = commandText.substring(0, this.prompt.selection);
        const textAfterCursor = commandText.substring(this.prompt.selection);

        // Marking caret position with a non-breaking blank space.
        const pre = document.createTextNode(textBeforeCursor);
        const post = document.createTextNode(textAfterCursor);
        const caretEle = document.createElement('span');
        caretEle.innerHTML = '&nbsp;';

        inputDisplayMirror.innerHTML = '';
        inputDisplayMirror.append(pre, caretEle, post);
        const caretRect = caretEle.getBoundingClientRect();
        this.prompt.container.removeChild(preformattedElement);
        return caretRect;
    }
}