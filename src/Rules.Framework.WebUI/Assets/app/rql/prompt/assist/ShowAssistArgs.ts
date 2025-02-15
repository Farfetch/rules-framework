export default class ShowAssistArgs {
    caretPosition: number = 0;
    commandText: string = '';
    leftPositionPixels: number = 0;
    onSelectedAssistSuggestion: (selectedSuggestion: string) => void = (_) => { };
    topPositionPixels: number = 0;
}