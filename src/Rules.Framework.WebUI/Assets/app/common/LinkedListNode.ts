export default class LinkedListNode<T> {
    value: T;
    next: LinkedListNode<T> | null = null;
    previous: LinkedListNode<T> | null = null;

    constructor(value: T) {
        this.value = value;
    }
}