// ===== Role =====

// This is a consumer-facing class that manages T values in a Deque
// system. This is a high-level class layered on a T[], employing
// a circular array design.


// ===== Invariants ===== 

// Size and MaxSize:

// Size is a non-negative integer that represents logically how many
// T objects are in the Deque.
// Size is a logical index pointer: 
// upon logical removal of a T object
// from a Deque, ArrayDeque<T> does not overwrite the leftover
// T reference in the backing T[] to the default T value.
// As such, it may be that the number of non-default T values in the backing
// T[] exceeds Size.

// Unlike Deque<T>, MaxSize here is a non-nullable positive integer.
// Attempting to construct an ArrayDeque<T> instance with a non-positive MaxSize
// throws an exception.

// Size <= MaxSize.


// Circularity:

// ArrayDeque<T> employs a circular array design. As such, the field pointers front
// and back are able to move circularly over the T[] indices.
// Note: if Size == 0, it may be that !(front == 0 && back == -1). This occurs after
// certain Add/Remove operations.



// ===== Methods =====

// PeekFront(): 
// Returns the T value of the front element without removing it.
// An exception is thrown if the Deque is empty.
// Time complexity O(1).

// RemoveFront(): 
// Returns the T value of the front element and removes it.
// An exception is thrown for Deque underflow.
// Time complexity O(1).

// AddFront(T value): 
// Adds an element with the passed T value at the front of the Deque.
// An exception is thrown for Deque overflow.
// Time complexity O(1).

// PeekBack():
// Returns the T value of the back element without removing it.
// An exception is thrown if the Deque is empty.
// Time complexity O(1).

// RemoveBack():
// Returns the T value of the back element and removes it.
// An exception is thrown for Deque underflow.
// Time complexity O(1).

// AddBack(T value):
// Adds an element with the passed T value at the back of the Deque.
// An exception is thrown for Deque overflow.
// Time complexity O(1).

// Clear():
// Clears all elements of the Deque.
// Time complexity O(1) as we don't overwrite left over T references in 
// the background T[].
// If we wanted to clear the background T[], this would be O(n) as we 
// would be required to perform a linear traversal through the T[].

// HasSpace():
// Checks to see whether the Deque has space.
// Time complexity O(1). 
// Accesses the Size property of the Deque, and compares it with the Deque
// MaxSize property value.

// Contains(T value):
// Checks to see whether at least one element of the Deque
// has a T value equal to the passed T value.
// Time complexity O(n) as it employs linear traversal through the T[].



// ===== Design Discussion ===== 

// Composition:

// This is a composition-based implementation using
// a T[] as the background data structure.

// I use a "circular array":

// We track the "front" and "back" of the Deque with 
// pointers to indices of the background T[].
// The idea is that when we add to the Deque, we fill up a certain free
// index in background T[] object, and shift the
// front/back pointers as required.

// With a circular array, Add/Remove operations are O(1).

// If instead we kept the front of our Deque as index 0 in the background T[],
// then when we add to the front of the Deque, 
// this would require shifting all elements
// of the Deque over an index in the background T[].
// In which case, Add/Remove operations are O(n).


// DoublyLinkedList vs fixed Array:

// DLL requires higher memory per element (PrevNode and NextNode pointers).
// DLL has a dynamic MaxSize, fixed Array has a fixed MaxSize.
// DLL has O(n) random access, fixed Array has O(1) random access
// (random access is finding the ith element of the Deque (for any i)).
// Fixed Array has better cache locality.
// Fixed Array has better iteration speed (due to better cache locality).


// Access Modifiers:

// The class and all its methods are public as they are consumer-facing.
// MaxSize and Size have public getters. MaxSize has no setter as it should be
// immutable. Size has a private setter to allow methods of this class
// to adjust the Size property.

// Items has a private getter so that ArrayDeque<T> can access the background
// T[]. It has no setter as although elements of the background T[] can be adjusted,
// the Items reference should never be adjusted to a different T[].

// The fields front and back are private as they are a private implementation detail.


// Error Handling:

// I handle deque overflows/underflows in relevant methods
// by throwing an exception, rather than allowing the methods to return null.


using System;

namespace DataStructures
{

public class ArrayDeque<T>
{
    public int MaxSize {get;}
    public int Size {get; private set;}
    private T[] Items {get;}
    private int front;
    private int back;
    
    public ArrayDeque(int maxSize)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSize));
            }

            this.Size = 0;
            this.MaxSize = maxSize;
            this.Items = new T[maxSize];
            this.front = 0;     // Points to where first element would go.
            this.back = -1;     // Signals no valid element at back yet.
        }


    public bool HasSpace()
        {
            return this.Size < this.MaxSize;
        }

    public void Clear()
        {
            this.Size = 0;
            this.front = 0;
            this.back = -1;

            // Any T objects in this.Items remain,
            // but they will be overwritten by future additions.
        }

    public T PeekFront()
        {
            if (this.Size == 0)
            {
                throw new InvalidOperationException("The deque is empty!");
            }
            return this.Items[this.front];
        }

    public T PeekBack()
        {
            if (this.Size == 0)
            {
                throw new InvalidOperationException("The deque is empty!");
            }
            return this.Items[this.back];
        }


    public void AddFront(T value)
        {
            if (!HasSpace())       
            {
                throw new InvalidOperationException("Deque overflow!");
            }
            else
            {
                this.front = (this.front - 1 + this.MaxSize) % this.MaxSize;
                this.Items[this.front] = value;
                this.Size += 1;

                if (this.Size == 1) // Checks if the deque was previously empty.
                {
                    this.back = this.front; // Initialises back.
                }
            }
        }

    public void AddBack(T value)
        {
            if (!HasSpace())       
            {
                throw new InvalidOperationException("Deque overflow!");
            }
            else
            {
                this.back = (this.back + 1) % this.MaxSize;
                this.Items[this.back] = value;
                this.Size += 1;

                // If the deque was previously empty, front and back will
                // now both be set to 0. So no need to check if the deque
                // was previously empty as we do in AddFront().
            }
        }

    public T RemoveFront()
        {
            if (this.Size == 0)     
            {
                throw new InvalidOperationException("Deque underflow!");
            }
            else
            {
                T valueToReturn = this.Items[this.front];
                this.Items[this.front] = default; // See below comment.
                this.front = (this.front + 1) % this.MaxSize;

                // Notice this doesn't remove the T object in the previous
                // front index. Functionally, there is no need to remove the
                // T object at the previous front index.
                // However, this avoids unnecessarily holding references
                // to objects we no longer need to.
                


                this.Size -= 1;
                return valueToReturn;
            }
        }   

    public T RemoveBack()
        {
            if (this.Size == 0)     
            {
                throw new InvalidOperationException("Deque underflow!");
            }
            else
            {
                T valueToReturn = this.Items[this.back];
                this.Items[this.back] = default;
                this.back = (this.back - 1 + this.MaxSize) % this.MaxSize;

                this.Size -= 1;
                return valueToReturn;
            }
        }  

    public bool Contains(T value)
        {
            for (int i = 0; i < this.Size; i++)
            {
                int index = (this.front + i) % this.MaxSize;
                if (EqualityComparer<T>.Default.Equals(this.Items[index], value))
                {
                    return true;
                }
            }

            return false;
        }

}
}