import LinkedListNode from "./LinkedListNode.js";

export default class LinkedList<T> {
    private firstNode: LinkedListNode<T> | null = null;
    private nodesCount: number = 0;

    get first(): LinkedListNode<T> | null {
        return this.firstNode;
    }

    addFirst(value: T): LinkedListNode<T> {
        const node = new LinkedListNode(value);

        if (!this.firstNode) {
            this.firstNode = node;
        } else {
            this.firstNode.previous = node;
            node.next = this.firstNode;
            this.firstNode = node;
        }

        this.nodesCount++;
        return node;
    }

    removeLast(): void {
        if (!this.firstNode) {
            return;
        }

        if (!this.firstNode.next) {
            this.firstNode = null;
            this.nodesCount--;
            return;
        }

        let currentNode = this.firstNode.next;
        while (currentNode.next && currentNode.next.next) {
            currentNode = currentNode.next;
        }

        currentNode.next = null;
        this.nodesCount--;
    }

    asArray(): T[] {
        const array: T[] = [];
        return this.firstNode ? this.appendToArray(array, this.firstNode) : array;
    }

    count(): number {
        return this.nodesCount;
    }

    private appendToArray(array: T[], node: LinkedListNode<T>): T[] {
        array.push(node.value);
        return node.next ? this.appendToArray(array, node.next) : array;
    }
}