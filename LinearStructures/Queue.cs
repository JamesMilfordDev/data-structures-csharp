// ===== Role =====

// This is a consumer-facing class that manages T values in a Queue
// system (FIFO). This is a high-level class layered on my
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

// Queue<T> T value management occurs exclusively via 
// SinglyLinkedList<T> structural management of SimpleNode<T> instances.



// ===== Invariants =====

// FIFO Order:

// Elements are dequeued in the order they were enqueued.


// Size and MaxSize:

// Size is a non-negative integer, and always equals the Count of Items, 
// the underlying SinglyLinkedList<T> instance. As such, Size invariants
// are enforced by the backing SinglyLinkedList<T>.

// MaxSize is a nullable integer.
// If MaxSize != null, it must be a positive integer. 
// Attempting to construct a Queue<T> instance with a non-positive integer for
// MaxSize throws an exception.

// When MaxSize != null, Size <= MaxSize.





// ===== Methods =====


// Enqueue(T value):
// Adds an element with the passed T value to the end of the Queue.
// An exception is thrown for Queue overflow.
// Time complexity O(1) as it employs AddTail() of SinglyLinkedList<T>,
// which is O(1) given SinglyLinkedList<T> has a Tail property.

// Dequeue():
// Removes the head of the Queue and returns its T value.
// An exception is thrown for Queue underflow.
// Time complexity O(1) as it employs RemoveHead() of SinglyLinkedList<T>,
// which is O(1).

// Peek():
// Returns the T value of the head of the Queue without removing it.
// An exception is thrown if the Queue is empty.
// Time complexity O(1) as it employs PeekHead() of SinglyLinkedList<T>,
// which is O(1).

// Clear():
// Clears all elements of the Queue.
// Time complexity O(1) as it employs Clear() of SinglyLinkedList<T>,
// which is O(1).

// HasSpace():
// Checks to see whether the Queue has space.
// Time complexity O(1). Accesses the internal Count property of the 
// underlying SinglyLinkedList<T> object, and compares it with the Queue<T>
// MaxSize property value.

// Contains(T value):
// Checks to see whether at least one element of the Queue
// has a T value equal to the passed T value.
// Time complexity O(n) as it employs Contains() of SinglyLinkedList<T>,
// which is O(n).



// ===== Design Discussion =====

// Inheritance:

// This is a composition-based implementation using
// my SinglyLinkedList<T> as the background data structure.
// We could have had Queue<T>
// inherit from SinglyLinkedList<T>.
// However, this has potential issues: we don't want to be able to
// call SinglyLinkedList<T> methods directly on a Queue<T> instance.


// Alternative Background Structures:

// I could have implemented a composition-based Queue class
// with either my DoublyLinkedList<T> or an array as the background data structure.

// There are benefits to an array-backed implementation. To see this, compare
// my Deque<T> and ArrayDeque<T> in this repository.

// However, a DoublyLinkedList<T>-backed implementation of Queue<T> is strictly
// worse than the current implementation.
// A DoublyLinkedList<T>-backed implementation would take up an extra O(1) space 
// per node, so O(n) space. However, as my SinglyLinkedList<T> stores a Tail
// reference, Peek(), Dequeue(), and Enqueue() are all O(1) time complexity already.
// So using a DoublyLinkedList<T> adds space complexity 
// but does not reduce time complexity.
// Again, the key is that I included a Tail reference in my SinglyLinkedList<T> class.


// Properties:

// The Size property does not have a private backing field as this is unnecessary.
// Rather, Size only has a getter, which directly gets Items.Count.


// Access Modifiers:

// The class and all its methods are public as they are consumer-facing.
// MaxSize and Size have public getters, but no setters, as they should not
// be directly mutable.
// Items has a private getter so that Queue<T> can access its properties,
// but no setter, as Queue<T> methods should not be able to directly mutate Items.


// Error Handling:

// I handle queue overflows/underflows in the Enqueue/Dequeue methods
// by throwing an exception, rather than allowing the methods to return null.

using System;

namespace DataStructures
{

public class Queue<T>
{
    public int? MaxSize {get;}
    private SinglyLinkedList<T> Items {get;}

    public int Size
        {
            get { return this.Items.Count;}
        }
    public Queue(int? maxSize = null)
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

    public void Enqueue(T value)
        {
            if (!HasSpace())       
            {
                throw new InvalidOperationException("Queue overflow!");
            }
            else
            {
                this.Items.AddTail(value);
            }
        }

    public T Dequeue()
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