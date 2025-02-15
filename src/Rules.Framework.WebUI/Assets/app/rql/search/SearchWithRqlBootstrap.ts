import RqlPromptHandler from '../prompt/RqlPromptHandler.js';
import SearchWithRql from './SearchWithRql.js';

export function initialize(dotNetPage: any): void {
    (<any>window).dotNetPage = dotNetPage;

    const searchWithRql = new SearchWithRql();
    const relayInputTextbox = document.getElementById('relayInputTextbox')!;
    const rqlPromptHandler = new RqlPromptHandler(searchWithRql.prompt, relayInputTextbox);

    (<any>window).searchWithRql = searchWithRql;
    (<any>window).rqlPromptHandler = rqlPromptHandler;
}