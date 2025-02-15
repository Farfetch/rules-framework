import RqlPromptHandler from '../prompt/RqlPromptHandler.js';
import RqlTerminal from './RqlTerminal.js';

export function initialize(dotNetPage: any): void {
    (<any>window).dotNetPage = dotNetPage;

    const rqlTerminal = new RqlTerminal();
    const relayInputTextbox = document.getElementById('relayInputTextbox')!;
    const rqlPromptHandler = new RqlPromptHandler(rqlTerminal.prompt, relayInputTextbox);

    (<any>window).rqlTerminal = rqlTerminal;
    (<any>window).rqlPromptHandler = rqlPromptHandler;
}