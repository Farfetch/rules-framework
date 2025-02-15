export default class Token {
    readonly beginIndex: number;
    readonly beginPosition: number;
    readonly endIndex: number;
    readonly endPosition: number;
    readonly lexeme: string;

    constructor(lexeme: string, beginIndex: number, endIndex: number) {
        this.beginIndex = beginIndex;
        this.beginPosition = beginIndex + 1;
        this.endIndex = endIndex;
        this.endPosition = endIndex + 1;
        this.lexeme = lexeme;
    }

    isEmpty(): boolean {
        return this.lexeme.length === 0;
    }

    static atIndex(text: string, index: number): Token {
        let beginIndex = index;
        let endIndex = index;
        let tokenDelimiterRegex = new RegExp('\\s', 'i');
        while (beginIndex > 0 && !text[beginIndex - 1].match(tokenDelimiterRegex)) {
            beginIndex--;
        }

        while (endIndex < text.length && !text[beginIndex - 1].match(tokenDelimiterRegex)) {
            endIndex++;
        }

        return new Token(text.substring(beginIndex, endIndex), beginIndex, endIndex);
    }
}