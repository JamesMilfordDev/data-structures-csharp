// ===== Role =====

// Recall that a SimpleNode<T> is a dumb, low-level container wholly managed
// by SinglyLinkedList<T>. This management is purely structural: it
// concerns which nodes a SinglyLinkedList<T> logically contains, 
// and their position. SimpleNode<T> management beyond structural concerns
// is forbidden: a SimpleNode<T> instance is fully initialised at creation,
// and its Value is immutable.
// Each SimpleNode<T> instance belongs to at most one SinglyLinkedList<T>.
// Node insertion always generates a new SimpleNode<T> instance. Removed
// nodes become unowned and cannot be added to another list.

// Queue<T> and Stack<T> are consumer-facing classes layered on
// SinglyLinkedList<T>. They are wholly concerned with T value management,
// which is exclusively achieved via SinglyLinkedList<T> structural
// management of SimpleNode<T> instances.

// However, as SimpleNode<T> does not store a PrevNode reference, 
// RemoveTail() of SinglyLinkedList<T> is O(n) as we must linearly 
// traverse the list to update the NextNode of the penultimate node.
// As neither Queue<T> nor Stack<T> appeal to the RemoveTail() structural
// mechanism, they can be acceptably implemented with SinglyLinkedList<T>.

// However, certain consumer-facing T value management structures like a Deque
// require symmetry between front and back operations. As such, 
// SinglyLinkedList<T> is inappropriate for a composition-based Deque
// implementation.

// ComplexNode<T> provides a richer alternative to SimpleNode<T>. It has
// a similar architectural role to SimpleNode<T>, but also stores a PrevNode
// reference. As such, it will be suitable as the foundation of a Deque.

// ComplexNode<T> is a minimal, low-level container.
// This class is not intended for direct consumer use;
// it exists only to support DoublyLinkedList<T>.

// ComplexNode<T> contains no methods. Instances are entirely
// managed by DoublyLinkedList<T>.
// This management is purely structural: it
// concerns which nodes a DoublyLinkedList<T> logically contains, 
// and their position. 



// ===== Invariants =====

// Each ComplexNode<T> instance belongs to at most one DoublyLinkedList<T>.
// Node insertion always generates a new ComplexNode<T> instance. Removed
// nodes become unowned and cannot be added to another list.

// The Value property is immutable and non-nullable (T not T?).

// NextNode is internally mutable and is either null or references 
// another ComplexNode<T> instance that is logically part of the same
// DoublyLinkedList<T>. A ComplexNode<T> instance with a NextNode null value
// represents the Tail of the DoublyLinkedList<T>.

// PrevNode is internally mutable and is either null or references
// another ComplexNode<T> instance that is logically part of the same
// DoublyLinkedList<T>. A ComplexNode<T> instance with a PrevNode null value
// represents the Head of the DoublyLinkedList<T>.

// Cycles are forbidden by DoublyLinkedList<T>.



// ===== Design Discussion =====

// Value Immutability and Nullability:

// Consumer-facing classes that sit on DoublyLinkedList<T> 
// only concern T value management, and a DoublyLinkedList<T> manages T values
// only through the manipulation of which nodes it logically contains,
// and their position: T value management occurs through structural management.

// Allowing Value to be internally mutable enables DoublyLinkedList<T>
// T value management independently of structural management, breaking this
// invariant.
// Allowing a nullable Value provides no representational power: a
// DoublyLinkedList<T> can be fully represented without it. Further, 
// allowing a nullable Value is actively detrimental: our DoublyLinkedList<T>
// method logic would have to account for nullability, adding noise.


// Generic vs Non-Generic:

// Again, rather than creating a generic class, an alternative would be
// to create a single ComplexNode class and let Value be of the Object type.
// This approach was rejected to preserve compile-time type safety.


// Access Modifiers:

// The class, the getters and setters for the
// NextNode and PrevNode properties, and the getter for the Value property,
// all have the internal access modifier,
// as the class is not for consumer use (public would be inappropriate),
// but is entirely managed by DoublyLinkedList<T> (private would be inappropriate).
// The modifier protected is also inappropriate as nothing inherits from the class.

// Value has no setter as DoublyLinkedList<T> includes
// no methods that require the mutation of node Values.
// This situation contrasts with my GeneralTreeNode<T>
// and BinaryTreeNode<T> classes.

// If we ever needed node Value mutability, an internal setter could be 
// added for Value, but this would weaken the current clear architectural boundaries.


namespace DataStructures
{

internal class ComplexNode<T>
{
    internal T Value  {get;}   

    internal ComplexNode<T>? NextNode {get; set;}
    internal ComplexNode<T>? PrevNode {get; set;}

    internal ComplexNode(T value)
    {
        this.Value = value;
        this.NextNode = null;
        this.PrevNode = null;
    }
}

}

