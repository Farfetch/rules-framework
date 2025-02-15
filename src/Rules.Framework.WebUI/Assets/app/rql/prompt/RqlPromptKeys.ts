export default class RqlPromptKeys {
    static readonly assistKeys: string[] = [];
    static readonly assistMovePreviousKey: string = 'ArrowUp';
    static readonly assistMoveNextKey: string = 'ArrowDown';
    static readonly assistAcceptSuggestionKeys: string[] = ['Enter', 'NumpadEnter', 'Tab'];
    static readonly assistHideKey: string = 'Escape';
    static readonly historyKeys: string[] = [];
    static readonly historyNavigateOlderInputKey: string = 'ArrowUp';
    static readonly historyNavigateRecentInputKey: string = 'ArrowDown';
    static readonly sendInputKeys: string[] = ['Enter', 'NumpadEnter'];

    static {
        this.assistKeys.push(this.assistMovePreviousKey, this.assistMoveNextKey, this.assistHideKey);
        this.assistAcceptSuggestionKeys.forEach(key => this.assistKeys.push(key));
        this.historyKeys.push(this.historyNavigateOlderInputKey, this.historyNavigateRecentInputKey);
    }
}