import RqlPrompt from "../prompt/RqlPrompt.js";

export default class SearchWithRql {
    readonly prompt: RqlPrompt;

    constructor() {
        const inputTextbox = <HTMLInputElement>document.getElementById('commandInputTextbox')!;
        const inputContainer = document.getElementById('commandInputContainer')!;
        this.prompt = new RqlPrompt(
            inputTextbox,
            inputContainer,
            {
                clearOnSubmit: false
            });

        window.hljs.configure({
            cssSelector: '.language-rql',
            languages: ['rql']
        });
    }
}