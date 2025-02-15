import RqlAssistItem from "./RqlAssistItem.js";
import ShowAssistArgs from "./ShowAssistArgs.js";
import Token from "./Token.js";

enum MoveType { None, Previous, Next }

export default class RqlAssist {
    readonly element: HTMLElement;
    private items: RqlAssistItem[];
    private selectedIndex: number;

    constructor() {
        this.element = document.createElement('div');
        this.element.classList.add('terminal-assist', 'bg-dark', 'rounded');
        this.element.style.display = 'none';
        this.items = [];
        this.selectedIndex = -1;
    }

    hide(): void {
        this.element.style.display = 'none';
    }

    getSelectedAssist(): RqlAssistItem | null {
        if (this.items.length > 0) {
            return this.items[this.selectedIndex];
        }

        return null;
    }

    isVisible(): boolean {
        return this.element.style.display != 'none';
    }

    moveNextAssist(): boolean {
        if (this.items.length > 0) {
            if (this.selectedIndex == -1) {
                this.selectedIndex = 0;
                let item = this.items[this.selectedIndex];
                item.setSelected();

                this.refreshScroll(MoveType.Next);
                return true;
            }

            if (this.selectedIndex + 1 < this.items.length) {
                let item = this.items[this.selectedIndex];
                item.setUnselected();

                item = this.items[++this.selectedIndex];
                item.setSelected();

                this.refreshScroll(MoveType.Next);
                return true;
            }
        }

        return false;
    }

    movePreviousAssist(): boolean {
        if (this.items.length > 0) {
            if (this.selectedIndex == -1) {
                this.selectedIndex = 0;
                let item = this.items[this.selectedIndex];
                item.setSelected();

                this.refreshScroll(MoveType.Previous);
                return true;
            }

            if (this.selectedIndex - 1 >= 0) {
                let item = this.items[this.selectedIndex];
                item.setUnselected();

                item = this.items[--this.selectedIndex];
                item.setSelected();

                this.refreshScroll(MoveType.Previous);
                return true;
            }
        }

        return false;
    }

    async show(showAssistArgs: ShowAssistArgs): Promise<void> {
        let currentToken = this.getTokenAtPosition(showAssistArgs.commandText, showAssistArgs.caretPosition);
        let assistSuggestions: string[] = [];
        if (!currentToken.isEmpty()) {
            assistSuggestions = await (<any>window).dotNetPage.invokeMethodAsync('getAssistSuggestions', showAssistArgs.commandText, 1, showAssistArgs.caretPosition);
        }

        this.items = [];
        this.selectedIndex = -1;
        this.element.style.width = '12rem';
        if (!currentToken.isEmpty()) {
            if (assistSuggestions.length > 0) {
                this.element.innerHTML = '';
                assistSuggestions.forEach((assistSuggestion) => {
                    const item = new RqlAssistItem(assistSuggestion, (text) => {
                        showAssistArgs.onSelectedAssistSuggestion.call(null!, text);
                    });

                    this.items.push(item);
                    this.element.appendChild(item.htmlElement);
                });
                this.selectedIndex = 0;
                this.items[0].setSelected();
            } else {
                this.hide();
                return;
            }

            // Valid values only available after showing the assist element.
            this.element.style.display = 'block';
            const assistElementRect = this.element.getBoundingClientRect();
            let maxWidth = 0;
            for (var i = 0; i < this.element.children.length; i++) {
                let childComputedStyles = window.getComputedStyle(this.element.children[i]);
                var childWidth = parseInt(childComputedStyles.paddingLeft) + parseInt(childComputedStyles.paddingRight);
                for (var j = 0; j < this.element.children[i].children.length; j++) {
                    childWidth += this.element.children[i].children[j].clientWidth;
                }

                if (childWidth > maxWidth) {
                    maxWidth = childWidth;
                }
            }

            if (this.element.clientWidth < maxWidth) {
                this.element.style.width = `${maxWidth}px`;
            }

            this.setAssitLocation(showAssistArgs.topPositionPixels - assistElementRect.height, showAssistArgs.leftPositionPixels);
            this.refreshScroll(MoveType.None);
        }
    }

    private getTokenAtPosition(text: string, position: number): Token {
        let beginIndex = position;
        let endIndex = position;
        let tokenDelimiterRegex = new RegExp('\\s', 'i');
        while (beginIndex > 0 && !text[beginIndex - 1].match(tokenDelimiterRegex)) {
            beginIndex--;
        }

        return new Token(text.substring(beginIndex, position), beginIndex, endIndex);
    }

    private setAssitLocation(topPositionPixels: number, leftPositionPixels: number): void {
        this.element.style.top = `${topPositionPixels}px`;
        this.element.style.left = `${leftPositionPixels}px`;
    }

    private refreshScroll(moveType: MoveType) {
        const heightInterval = this.element.clientHeight;
        const currentScrollTop = this.element.scrollTop;
        var heightToSelected = 0;
        var itemHeight = 0;
        for (var i = 0; i < this.items.length; i++) {
            const item = this.items[i];
            heightToSelected += item.htmlElement.clientHeight;
            if (item.isSelected()) {
                itemHeight = item.htmlElement.clientHeight;
                break;
            }
        }

        if (heightToSelected > currentScrollTop && heightToSelected < currentScrollTop + heightInterval) {
            // Selected item is within' assist viewport, do not scroll.
            return;
        }

        var scrollTop;
        if (moveType === MoveType.Previous) {
            scrollTop = heightToSelected - itemHeight;
        } else {
            scrollTop = heightToSelected - heightInterval;
        }

        this.element.scroll({
            top: scrollTop,
            behavior: "instant"
        });
    }
}