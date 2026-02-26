// ===== Role =====

// This is a consumer-facing class that manages T values in a Deque
// system. This is a high-level class layered on my
// DoublyLinkedList<T>, a low-level, internal mechanism to manage sequences
// of ComplexNode<T> instances in a bi-directional list fashion.

// A ComplexNode<T> is a dumb, low-level container wholly managed
// by DoublyLinkedList<T>. This management is purely structural: it
// concerns which nodes a DoublyLinkedList<T> logically contains, 
// and their position. ComplexNode<T> management beyond structural concerns
// is forbidden: a ComplexNode<T> instance is fully initialised at creation,
// and its Value is immutable and non-nullable.
// Each ComplexNode<T> instance belongs to at most one DoublyLinkedList<T>.
// Node insertion always generates a new ComplexNode<T> instance. Removed
// nodes become unowned and cannot be added to another list.

// Deque<T> T value management occurs exclusively via 
// DoublyLinkedList<T> structural management of ComplexNode<T> instances.

// DoublyLinkedList<T> is an appropriate foundation for Deque<T> as its
// structural mechanisms provide symmetric front and back operations.
// This is not the case for SinglyLinkedList<T>, whose RemoveTail() is O(n),
// as a linear traversal is required to set the NextNode reference of the
// penultimate node to null.



// ===== Invariants =====

// Size and MaxSize:

// Size is a non-negative integer, and always equals the Count of Items, 
// the underlying DoublyLinkedList<T> instance. As such, Size invariants
// are enforced by the backing DoublyLinkedList<T>.

// MaxSize is a nullable integer.
// If MaxSize != null, it must be a positive integer. 
// Attempting to construct a Deque<T> instance with a non-positive integer for
// MaxSize throws an exception.

// When MaxSize != null, Size <= MaxSize.



// ===== Methods =====

// PeekFront(): 
// Returns the T value of the front element without removing it.
// An exception is thrown if the Deque is empty.
// Time complexity O(1) as it employs PeekHead() of DoublyLinkedList<T>,
// which is O(1).

// RemoveFront(): 
// Returns the T value of the front element and removes it.
// An exception is thrown for Deque underflow.
// Time complexity O(1) as it employs RemoveHead() of DoublyLinkedList<T>,
// which is O(1).

// AddFront(T value): 
// Adds an element with the passed T value at the front of the Deque.
// An exception is thrown for Deque overflow.
// Time complexity O(1) as it employs AddHead() of DoublyLinkedList<T>,
// which is O(1).

// PeekBack():
// Returns the T value of the back element without removing it.
// An exception is thrown if the Deque is empty.
// Time complexity O(1) as it employs PeekTail() of DoublyLinkedList<T>,
// which is O(1).

// RemoveBack():
// Returns the T value of the back element and removes it.
// An exception is thrown for Deque underflow.
// Time complexity O(1) as it employs RemoveTail() of DoublyLinkedList<T>,
// which is O(1).

// AddBack(T value):
// Adds an element with the passed T value at the back of the Deque.
// An exception is thrown for Deque overflow.
// Time complexity O(1) as it employs AddTail() of DoublyLinkedList<T>,
// which is O(1).

// Clear():
// Clears all elements of the Deque.
// Time complexity O(1) as it employs Clear() of DoublyLinkedList<T>,
// which is O(1).

// HasSpace():
// Checks to see whether the Deque has space.
// Time complexity O(1). Accesses the internal Count property of the 
// underlying DoublyLinkedList<T> object, and compares it with the Deque<T>
// MaxSize property value.

// Contains(T value):
// Checks to see whether at least one element of the Deque
// has a T value equal to the passed T value.
// Time complexity O(n) as it employs Contains() of DoublyLinkedList<T>,
// which is O(n).



// ===== Design Discussion ===== 

// Inheritance:

// This is a composition-based implementation using
// my DoublyLinkedList<T> as the background data structure.
// We could have had Deque<T>
// inherit from DoublyLinkedList<T>.
// However, this has potential issues: we don't want to be able to
// call DoublyLinkedList<T> methods directly on a Deque<T> instance. 

// Alternative Background Structures:

// I could have implemented a composition-based Deque class
// with an array as the background data structure. See ArrayDeque<T>
// in this repository.

// In one sense, I could have implemented a composition-based Deque class
// with SinglyLinkedList<T>.
// However, this wouldn't strictly be a Deque, as we would have
// an asymmetry between front and back operations. This is because
// in SinglyLinkedList<T>, RemoveBack() is O(n) as nodes do not store a
// PrevNode reference, so we must linearly traverse the list to find the previous
// node to Tail to set its NextNode to null.

// Properties:

// The Size property does not have a private backing field as this is unnecessary.
// Rather, Size only has a getter, which directly gets Items.Count.

// Access Modifiers:

// The class and all its methods are public as they are consumer-facing.
// MaxSize and Size have public getters, but no setters, as they should not
// be directly mutable.
// Items has a private getter so that Deque<T> can access its properties,
// but no setter, as Deque<T> methods should not be able to directly mutate Items.

// Error Handling:

// I handle deque overflows/underflows in relevant methods
// by throwing an exception, rather than allowing the methods to return null.

using System;

namespace DataStructures
{

public class Deque<T>
{
    public int? MaxSize {get;}
    private DoublyLinkedList<T> Items {get;}
    public int Size
        {
            get { return this.Items.Count;}
        }
    public Deque(int? maxSize = null)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSize), "MaxSize must be positive.");
            }
            
            this.MaxSize = maxSize;
            this.Items = new DoublyLinkedList<T>();
        }


    public bool HasSpace()
        {
            return MaxSize == null || this.Items.Count < this.MaxSize;
        }


    public T PeekFront()
        {
            return this.Items.PeekHead();
        }

    public T PeekBack()
        {
            return this.Items.PeekTail();
        }


    public void AddFront(T value)
        {
            if (!HasSpace())       
            {
                throw new InvalidOperationException("Deque overflow!");
            }
            
            this.Items.AddHead(value);
        }

    public void AddBack(T value)
        {
            if (!HasSpace())       
            {
                throw new InvalidOperationException("Deque overflow!");
            }
            
            this.Items.AddTail(value);
        }

    public T RemoveFront()
        {
            return this.Items.RemoveHead();
        }   

    public T RemoveBack()
        {
            return this.Items.RemoveTail();
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