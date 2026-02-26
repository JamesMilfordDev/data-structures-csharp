// ===== Role =====

// This is a consumer-facing class that manages T values in a Stack
// system (LIFO). This is a high-level class layered on my
// SinglyLinkedList<T>, a low-level, internal mechanism to manage sequences
// of SimpleNode<T> instances in a one-directional list fashion.

// A SimpleNode<T> is a dumb, low-level container wholly managed
// by SinglyLinkedList<T>. This management is purely structural: it
// concerns which nodes a SinglyLinkedList<T> logically contains, 
// and their position. SimpleNode<T> management beyond structural concerns
// is forbidden: a SimpleNode<T> instance is fully initialised at creation,
// and its Value is immutable and non-nullable.
// Each SimpleNode<T> instance belongs to at most one SinglyLinkedList<T>.
// Node insertion always generates a new SimpleNode<T> instance. Removed
// nodes become unowned and cannot be added to another list.

// Stack<T> T value management occurs exclusively via 
// SinglyLinkedList<T> structural management of SimpleNode<T> instances.



// ===== Invariants =====

// LIFO Order:

// The next element to be popped is the last element to be pushed to the Stack<T>.


// Size and MaxSize:

// Size is a non-negative integer, and always equals the Count of Items, 
// the underlying SinglyLinkedList<T> instance. As such, Size invariants
// are enforced by the backing SinglyLinkedList<T>.

// MaxSize is a nullable integer.
// If MaxSize != null, it must be a positive integer. 
// Attempting to construct a Stack<T> instance with a non-positive integer for
// MaxSize throws an exception.

// When MaxSize != null, Size <= MaxSize.



// ===== Methods =====

// Push(T value):
// Adds an element with the passed T value to the top of the Stack.
// An exception is thrown for Stack overflow.
// Time complexity O(1) as it employs AddHead() of SinglyLinkedList<T>,
// which is O(1).

// Pop():
// Removes the top of the Stack and returns its T value.
// An exception is thrown for Stack underflow.
// Time complexity O(1) as it employs RemoveHead() of SinglyLinkedList<T>,
// which is O(1).

// Peek():
// Returns the T value of the head of the Stack without removing it.
// An exception is thrown if the Stack is empty.
// Time complexity O(1) as it employs PeekHead() of SinglyLinkedList<T>,
// which is O(1).

// Clear():
// Clears all elements of the Stack.
// Time complexity O(1) as it employs Clear() of SinglyLinkedList<T>,
// which is O(1).

// HasSpace():
// Checks to see whether the Stack has space.
// Time complexity O(1). Accesses the internal Count property of the 
// underlying SinglyLinkedList<T> object, and compares it with the Stack<T>
// MaxSize property value.

// Contains(T value):
// Checks to see whether at least one element of the Stack
// has a T value equal to the passed T value.
// Time complexity O(n) as it employs Contains() of SinglyLinkedList<T>,
// which is O(n).



// ===== Design Discussion =====

// Inheritance:

// This is a composition-based implementation using
// my SinglyLinkedList<T> as the background data structure.
// We could have had Stack<T>
// inherit from SinglyLinkedList<T>.
// However, this has potential issues: we don't want to be able to
// call SinglyLinkedList<T> methods directly on a Stack<T> instance. 


// Alternative Background Structures:

// I could have implemented a composition-based Stack class
// with either my DoublyLinkedList<T> or an array as the background data structure.

// There are benefits to an array-backed implementation. To see this, compare
// my Deque<T> and ArrayDeque<T> in this repository.
// With a stack, we don't even need a circular array.

// However, a DoublyLinkedList<T>-backed implementation of Stack<T> is strictly
// worse than the current implementation.
// A DoublyLinkedList<T>-backed implementation would take up an extra O(1) space 
// per node, so O(n) space. However, Peek(), Push(), and Pop()
// are all O(1) time complexity already.
// So using a DoublyLinkedList<T> adds space complexity 
// but does not reduce time complexity.

// Strictly speaking, Stack<T> does not even need SinglyLinkedList<T> to store
// a Tail reference, as this is only useful in making the AddTail() 
// method of SinglyLinkedList<T> O(1) not O(n). This was needed by Queue<T>,
// whose Enqueue() calls AddTail() on the underlying SinglyLinkedList<T> instance,
// but Stack<T> methods only employ SinglyLinkedList<T> methods that interact
// with Head.


// Properties:

// The Size property does not have a private backing field as this is unnecessary.
// Rather, Size only has a getter, which directly gets Items.Count.


// Access Modifiers:

// The class and all its methods are public as they are consumer-facing.
// MaxSize and Size have public getters, but no setters, as they should not
// be directly mutable.
// Items has a private getter so that Stack<T> can access its properties,
// but no setter, as Stack<T> methods should not be able to directly mutate Items.


// Error Handling:

// I handle stack overflows/underflows in the Push/Pop methods
// by throwing an exception, rather than allowing the methods to return null.


using System;

namespace DataStructures
{

public class Stack<T>
{
    public int? MaxSize {get;}
    private SinglyLinkedList<T> Items {get;}
    public int Size
        {
            get { return this.Items.Count;}
        }
    public Stack(int? maxSize = null)
        {
            if (maxSize != null && maxSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSize), "MaxSize must be positive.");
            }
            
            this.MaxSize = maxSize;
            this.Items = new SinglyLinkedList<T>();
        }

    public bool HasSpace()
        {
            return MaxSize == null || this.Items.Count < this.MaxSize;
        }

     public T Peek()
        {
            return this.Items.PeekHead();
        }


    public void Push(T value)
        {
            if (!HasSpace())       
            {
                throw new InvalidOperationException("Stack overflow!");
            }
            this.Items.AddHead(value);
        }

    public T Pop()
        {
            return this.Items.RemoveHead();
        }   

    public void Clear()
        {
            this.Items.Clear();
        }

    public bool Contains(T value)
    {
        return this.Items.Contains(value);
    }

}
}
