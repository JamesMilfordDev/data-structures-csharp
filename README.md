# Data Structures Library

A documentation-first C# library implementing
foundational linear, tree, and graph structures.

## Purpose

The library is designed to emphasise: 

- Clear and deliberate documentation.

- Foundational design principles: 
    - architectural boundaries
    - clear separation of responsibilities between layers
    - appropriate encapsulation

- Explicit consideration of design tradeoffs.

- Explicit consideration of time and space complexity.


To support this, the library deliberately includes non-standard implementations of 
certain structures, and each source file includes an in-depth documentation block.

## Documentation Format

Each source file documentation block includes the following sections:

- Role
- Invariants
- Methods (if the class includes any)
- Design Discussion 

## Library Structure

The library divides into three areas:

- (A) Linear structures
- (B) Tree structures
- (C) Graph structures

Each area is structured as one or more conceptual flows.

(A) Linear structures:

- (A.i) `SimpleNode<T>` => `SinglyLinkedList<T>` => `Queue<T>` and `Stack<T>`
- (A.ii) `ComplexNode<T>` => `DoublyLinkedList<T>` => `Deque<T>`
- (A.iii) `ArrayDeque<T>`


(B) Tree structures:

- (B.i) `GeneralTreeNode<T>` => `GeneralTree<T>` => `ComposedBinaryTree<T>`
- (B.ii) `BinaryTreeNode<T>` => `BinaryTree<T>` => `BinarySearchTree<T>` and `MinHeap<T>`
- (B.iii) `ArrayMinHeap<T>`


(C) Graph structures:

- (C.i) `GraphNode<T>` => `Graph<T>`

## Suggested Reading Order

The library was developed in the order presented here. Whilst each flow can
be considered independently, the source files are most naturally read
sequentially. Design tradeoffs in earlier flows often inform later
implementation decisions.

## Library-Wide Design Choices

### Equality vs Identity:

- To check equality of `T` objects `x` and `y`,
`EqualityComparer<T>.Default.Equals(x, y)` is used.

- To check identity of node references, `==` is used.

### Node Design:

- Each node class is a **minimal, low-level container**. It contains no methods
and is entirely managed by a certain higher-level class. A node consists of a 
**Value** and pointers to other nodes of the same kind (pointers vary 
in arity and directionality, depending on the node type).

- Each node class instance belongs to at most **one** instance of the relevant higher-level class. Removed nodes become unowned and cannot be added to another instance of the higher-level class.

### Handling Nullability:

- Compiler warnings concerning nullability have been disabled. Logic guarantees that whenever an expression is required to be non-null, it is.

- In production, the null-forgiving operator `!` is recommended. This is avoided here to reduce noise.

- Variables declared as non-nullable but initialised later are set with `null!` to avoid 
definite assignment errors (CS0165).

- `var` is avoided to keep types explicit.





