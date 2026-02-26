// ===== Role =====

// A low-level, internal mechanism to manage sequences
// of ComplexNode<T> instances. This is a bi-directional sequence.
// it ensures the logical order of ComplexNode<T> instances, maintains
// a Head and Tail, and ensures no node appears more than once (no cycles). 

// A ComplexNode<T> is a dumb, low-level container that is wholly managed
// by DoublyLinkedList<T>. This management is purely structural: it
// concerns which nodes a DoublyLinkedList<T> logically contains, 
// and their position. ComplexNode<T> management beyond structural concerns
// is forbidden: a ComplexNode<T> instance is fully initialised at creation,
// and its Value is immutable and non-nullable. 
// Each ComplexNode<T> instance belongs to at most one DoublyLinkedList<T>.
// Node insertion always generates a new ComplexNode<T> instance. Removed
// nodes become unowned and cannot be added to another list.

// Unlike SinglyLinkedList<T>, this class supports O(1) operations at both
// ends, enabling fully symmetric front and back handling, making it
// viable as the mechanism for a Deque implementation.

// DoublyLinkedList<T> is not intended for direct consumer use.
// Rather, it provides a list mechanism on which
// the higher-level, consumer-facing Deque<T> is built.
// T value management by Deque<T> occurs only through
// DoublyLinkedList<T> structural management of ComplexNode<T> instances.



// ===== Invariants ===== 

// Head, Tail, and Count:

// Head and Tail are nullable ComplexNode<T> values.
// Count is a non-negative integer.

// At any one time, a DoublyLinkedList<T> instance is in exactly
// one of three states:

// (1) Empty: Head and Tail are null, and Count is 0.

// (2) Singleton: Head and Tail are non-null 
// and have reference identity. Head.NextNode == null. 
// Tail.PrevNode == null. Count is 1. 

// (3) Multi-member: Head and Tail are non-null, and
// are distinct. Head.NextNode != null and Tail.PrevNode != null.
// Head points to the first node, 
// Tail points to the last node, and Count is equal to 
// the number of nodes in the list (Count > 1).


// Node Linking:

// If Head != null, iterating through the NextNode chain starting
// at Head traverses exactly Count many nodes, and always
// terminates with Tail.

// If Tail != null, iterating through the PrevNode chain starting
// at Tail traverses exactly Count many nodes, and always terminates
// with Head.

// Tail.NextNode is always null.
// Head.PrevNode is always null.

// No cycles exist: iterating through the NextNode chain or the
// PrevNode chain never returns a ComplexNode<T> instance more than once.


// ===== Methods =====

// PeekHead(): 
// Returns the T value of the head without removing it. 
// Time complexity O(1) as we maintain Head/Tail references and don't
// traverse nodes.

// RemoveHead(): 
// Returns the T value of the head and removes it. 
// Time complexity O(1) as we maintain Head/Tail references and don't
// traverse nodes.

// AddHead(T value): 
// Creates a new ComplexNode<T> instance at the head of the list
// with the passed T value.
// Time complexity O(1) as we maintain Head/Tail references and don't
// traverse nodes.

// PeekTail():
// Returns the T value of the tail without removing it.
// Time complexity O(1) as we maintain Head/Tail references and don't
// traverse nodes.

// RemoveTail():
// Returns the T value of the tail and removes it.
// Time complexity O(1) as we maintain Head/Tail references and don't
// traverse nodes.
// This contrasts with the SinglyLinkedList<T> RemoveTail() method, which is O(n).

// AddTail(T value):
// Creates a new ComplexNode<T> instance at the tail of the list with
// the passed T value.
// Time complexity O(1) as we maintain Head/Tail references and don't
// traverse nodes.

// Contains(T value):
// Time complexity O(n) as we linearly search through nodes.

// Clear():
// Time complexity O(1) as we just set Head and Tail to null, and Count to 0.
// Optionally, we could interate through each node and set PrevNode/NextNode to null
// for better garbage collection, but at the cost of O(n) time complexity.

// Note that a DoublyLinkedList<T> has fully symmetric head and tail operations,
// unlike a SinglyLinkedList<T>.



// Additional Private Educational Methods:
// (Not part of the intended mechanism API for higher-level classes)

// RemoveValue(T value):
// Removes every node with a Value equal to the T object passed in.
// Time complexity O(n) as we perform a single linear traversal of the list.


// SwapNodes (T value1, T value2):
// Swaps the first node with a Value of value1 with the first node with
// a Value of value2.
// Time complexity O(n).
// The invariant that DoublyLinkedList<T> manages structure is preserved
// by appropriately adjusting Head, Tail, and PrevNode/NextNode values, not 
// by mutating Values of nodes.



// ===== Design Discussion ===== 

// Access Modifiers:

// The class and its API methods have the internal access modifier,
// as the class is not for consumer use (public would be inappropriate),
// but is accessed by other consumer-facing classes (private would be inappropriate).
// The modifier protected is also inappropriate as nothing inherits from the class.
// An inheritance-based implementation of higher-level classes, e.g. Deque<T>, would
// be inappropriate, as this would allow consumers to inappropriately
// call DoublyLinkedList<T> methods on a Deque<T> instance.

// The properties Head and Tail are private, as they are purely
// an implementation detail of DoublyLinkedList<T>. No other class needs
// to access them. Consumer-facing classes such as Deque<T> interact
// with the underlying DoublyLinkedList<T> instance only via the internal API methods
// of DoublyLinkedList<T>.

// The Count property has a private setter as only this mechanism-focused
// class should be able to manipulate this reference. However, it
// has an internal getter to enable higher-level abstractions to access it to
// compute their sizes.
// Again, an alternative choice would have been to not store a Count property,
// but instead include a Count() method that performs an O(n) linear traversal.
// However, as Count is important in higher-level abstractions, and will need to
// be frequently accessed, I have decided to accept the memory overhead 
// (storing the reference), and the slight increase in complexity to the logic
// of insertion/removal/clear methods, in order
// to save the time complexity of accessing the count.


// Error Handling:

// Peeking/removing the head/tail when the list is empty throws an exception, 
// rather than returning null.
// Throwing an exception ensures that higher level abstractions cannot silently 
// misinterpret an empty list state.

using System;
using System.Collections.Generic;

namespace DataStructures
{

internal class DoublyLinkedList<T>
{
    private ComplexNode<T>? Head {get; set;}
    private ComplexNode<T>? Tail {get; set;}
    internal int Count {get; private set;}

    internal DoublyLinkedList()
    {
        this.Head = null;
        this.Tail = null;
        this.Count = 0;
    }

    internal T PeekHead()
        {
            if (this.Head == null)              // Handle empty list.
            {
                throw new InvalidOperationException("The list is empty.");
            }

            return this.Head.Value;
        }

    internal T RemoveHead()
        {
            if (this.Head == null)              // Handle empty list.
            {
                throw new InvalidOperationException("The list is empty.");
            }

            T value = this.Head.Value;

            if (this.Head == this.Tail)         // Handle singleton list.
            {
                this.Clear();
            }
            else                                // Handle otherwise.
            {
                this.Head.NextNode.PrevNode = null;
                this.Head = this.Head.NextNode;
                this.Count -=1;
            }

            // Return value.                 
            return value;
        }

    internal void AddHead(T value)
        {
            ComplexNode<T> newHead = new ComplexNode<T>(value);

            if (this.Head == null)              // Handle empty list.
            {
                this.Head = newHead;
                this.Tail = newHead;
            }
            else                                // Handle otherwise.
            {
                this.Head.PrevNode = newHead;
                newHead.NextNode = this.Head;
                this.Head = newHead;
            }

            // Adjust Count.
            this.Count += 1;                    
        }

   internal T PeekTail()
        {
            if (this.Tail == null)              // Handle empty list.
            {
                throw new InvalidOperationException("The list is empty.");
            }

            return this.Tail.Value;
        } 
    internal T RemoveTail()
        {
            if (this.Tail == null)              // Handle empty list.
            {
                throw new InvalidOperationException("The list is empty.");
            }

            T value = this.Tail.Value;

            if (this.Head == this.Tail)         // Handle singleton list.
            {
                this.Clear();
            }
            else                                // Handle otherwise.
            {
                this.Tail.PrevNode.NextNode = null;
                this.Tail = this.Tail.PrevNode;
                this.Count -= 1;
            }

            // Return value.
            return value;

        }
    internal void AddTail(T value)
            {
                ComplexNode<T> newTail = new ComplexNode<T>(value);

                if (this.Tail == null)              // Handle empty list.
                {
                    this.Head = newTail;
                    this.Tail = newTail;
                }
                else                                // Handle otherwise.
                {
                    this.Tail.NextNode = newTail;
                    newTail.PrevNode = this.Tail;
                    this.Tail = newTail;
                }

                // Adjust Count.

                this.Count += 1;                    
            }
    internal bool Contains(T value)
        {
            ComplexNode<T>? currentNode = this.Head;
            while (currentNode != null)
            {
                if (EqualityComparer<T>.Default.Equals(currentNode.Value, value))
                {
                    return true;
                }
                currentNode = currentNode.NextNode;
            }
            return false;
        }

    internal void Clear()
        {
            this.Head = null;
            this.Tail = null;
            this.Count = 0;
        }

    private void RemoveValue(T valueToRemove)
        {
            // Removes every node in the list with a Value of valueToRemove.
            // Iterates through the nodes with a while loop.
            // When iterating, tracks only currentNode (unlike in SinglyLinkedList<T>'s RemoveValue()).
            // If a node has the matching value, we need to remove it and adjust Count.
            // When removing a node, we have to consider its position in the list.
            // For readability, I have explicitly handled 4 cases separately:
            // currentNode is the head and tail; currentNode is the head only; currentNode
            // is the tail only; currentNode is neither head nor tail.

            ComplexNode<T>? currentNode = this.Head;


            while (currentNode != null)
            {
                if (EqualityComparer<T>.Default.Equals(currentNode.Value, valueToRemove))
                {
                    if (this.Head == currentNode && this.Tail == currentNode)
                    {
                        this.Head = null;
                        this.Tail = null;
                        this.Count -= 1;

                        // The above is equivalent to calling Clear() here.
                        // For readability, I include the code manually.

                        break;
                    }

                    else if (this.Head == currentNode)
                    {
                        this.Head = currentNode.NextNode;
                        this.Head.PrevNode = null;
                        this.Count -= 1;

                        currentNode = currentNode.NextNode;

                        continue;
                    }

                    else if (this.Tail == currentNode)
                    {
                        this.Tail = currentNode.PrevNode;
                        this.Tail.NextNode = null;
                        this.Count -= 1;

                        break;
                    }

                    else                         
                    {
                        currentNode.NextNode.PrevNode = currentNode.PrevNode;
                        currentNode.PrevNode.NextNode = currentNode.NextNode;
                        this.Count -= 1;

                        currentNode = currentNode.NextNode;

                        continue;
                    }
                }
                else    // Move to next node
                {
                    currentNode = currentNode.NextNode;
                }
            }
        }
    private void SwapNodes(T value1, T value2)
        {
            // Swaps the first node with Value of value1
            // with the first node with Value of value2.

            // Handles adjacent nodes, non-adjacent nodes,
            // and updates the Head and Tail correctly.

            if (EqualityComparer<T>.Default.Equals(value1, value2))
            {
                return;
            }

            ComplexNode<T>? node1 = null;
            ComplexNode<T>? node2 = null;

            // Set node1 and node2.

            ComplexNode<T>? currentNode = this.Head;

            while (currentNode != null)
            {
                if (node1 == null && EqualityComparer<T>.Default.Equals(currentNode.Value, value1))
                {
                    node1 = currentNode;    
                }
                else if (node2 == null && EqualityComparer<T>.Default.Equals(currentNode.Value, value2))
                {
                    node2 = currentNode;
                }

                if (node1 != null && node2 != null)
                {
                    break;
                }

                currentNode = currentNode.NextNode;
            }

            // Check neither node1 nor node2 is null.

            if (node1 == null || node2 == null)            
            {
                throw new InvalidOperationException("At least one of those values is not in the list.");
            }

            // Cache node1Prev, node1Next, node2Prev, node2Next for simplicity.

            ComplexNode<T>? node1Prev = node1.PrevNode;
            ComplexNode<T>? node1Next = node1.NextNode;
            ComplexNode<T>? node2Prev = node2.PrevNode;
            ComplexNode<T>? node2Next = node2.NextNode;

            // There are three cases:

            // (a) node1Next is node2 (adjacency).
            // (b) node2Next is node1 (adjacency).
            // (c) the nodes are not adjacent.

            // We must also consider subcases for
            // whether certain nodes are the Head/Tail
            // (this will affect updating node1Prev and node2Prev properties.)

            if (node1Next == node2)                // (a)
            {
                node1.NextNode = node2Next;
                node2.NextNode = node1;

                node1.PrevNode = node2;
                node2.PrevNode = node1Prev;

                if (node1Prev != null)
                {
                    node1Prev.NextNode = node2;
                }

                if (node2Next != null)
                {
                    node2Next.PrevNode = node1;
                }
            }

            else if (node2Next == node1)           // (b) (Same logic as (a))
            {
                node2.NextNode = node1Next;
                node1.NextNode = node2;

                node1.PrevNode = node2Prev;
                node2.PrevNode = node1;

                if (node2Prev != null)
                {
                    node2Prev.NextNode = node1;
                }

                if (node1Next != null)
                {
                    node1Next.PrevNode = node2;
                }
            }

            else                                    // (c)
            {
                node1.NextNode = node2Next;
                node2.NextNode = node1Next;

                node1.PrevNode = node2Prev;
                node2.PrevNode = node1Prev;

                if (node1Prev != null)
                {
                    node1Prev.NextNode = node2;
                }

                if (node1Next != null)
                {
                    node1Next.PrevNode = node2;
                }

                if (node2Prev != null)
                {
                    node2Prev.NextNode = node1;
                }

                if (node2Next != null)
                {
                    node2Next.PrevNode = node1;
                }
            }         
           
            // Finally, we adjust Head and Tail properties.

            if (this.Head == node1)
            {
                this.Head = node2;
            }
            else if (this.Head == node2)
            {
                this.Head = node1;
            }

            if (this.Tail == node1)
            {
                this.Tail = node2;
            }
            else if (this.Tail == node2)
            {
                this.Tail = node1;
            }
        }

}

}