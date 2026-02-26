// ===== Role =====

// A minimal, low-level container.
// This class is not intended for direct consumer use;
// it exists only to support SinglyLinkedList<T>.

// SimpleNode<T> contains no methods. Instances are entirely
// managed by SinglyLinkedList<T>.
// This management is purely structural: it
// concerns which nodes a SinglyLinkedList<T> logically contains, 
// and their position. 


// ===== Invariants =====

// Each SimpleNode<T> instance belongs to at most one SinglyLinkedList<T>.
// Node insertion always generates a new SimpleNode<T> instance. Removed
// nodes become unowned and cannot be added to another list.

// The Value property is immutable and non-nullable (T not T?).

// NextNode is internally mutable and is either null or references 
// another SimpleNode<T> instance that is logically part of the same
// SinglyLinkedList<T>. A SimpleNode<T> instance with a NextNode null value
// represents the Tail of the SinglyLinkedList<T>.

// Cycles are forbidden by SinglyLinkedList<T>.


// ===== Design Discussion =====

// Value Immutability and Nullability:

// Consumer-facing classes that sit on SinglyLinkedList<T> 
// only concern T value management, and a SinglyLinkedList<T> manages T values
// only through the manipulation of which nodes it logically contains,
// and their position: T value management occurs through structural management.

// Allowing Value to be internally mutable enables SinglyLinkedList<T>
// T value management independently of structural management, breaking this
// invariant.
// Allowing a nullable Value provides no representational power: a
// SinglyLinkedList<T> can be fully represented without it. Further, 
// allowing a nullable Value is actively detrimental: our SinglyLinkedList<T>
// method logic would have to account for nullability, adding noise.


// Generic vs Non-Generic:

// Rather than creating a generic class, an alternative would be
// to create a single SimpleNode class and let Value be of the Object type.
// This approach was rejected to preserve compile-time type safety.


// Access Modifiers:

// The class, the getter and setter for the
// NextNode property, and the getter for the Value property,
// all have the internal access modifier,
// as the class is not for consumer use (public would be inappropriate),
// but is entirely managed by SinglyLinkedList<T> (private would be inappropriate).
// The modifier protected is also inappropriate as nothing inherits from the class.

// Value has no setter as SinglyLinkedList<T> includes
// no methods that require the mutation of node Values.
// This situation contrasts with my GeneralTreeNode<T>
// and BinaryTreeNode<T> classes.

// If we ever needed node Value mutability, an internal setter could be 
// added for Value, but this would weaken the current clear architectural boundaries.


namespace DataStructures
{

internal class SimpleNode<T>    
{
    internal T Value  {get;}
    internal SimpleNode<T>? NextNode {get; set;}

    internal SimpleNode(T value)
    {
        this.Value = value;
        this.NextNode = null;
    }
}

}

