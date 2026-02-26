// ===== Role =====

// A low-level, internal mechanism to manage sequences
// of SimpleNode<T> instances. This is a one-directional sequence.
// It ensures the logical order of SimpleNode<T> instances, maintains
// a Head and Tail, and ensures no node appears more than once (no cycles). 

// A SimpleNode<T> is a dumb, low-level container that is wholly managed
// by SinglyLinkedList<T>. This management is purely structural: it
// concerns which nodes a SinglyLinkedList<T> logically contains, 
// and their position. SimpleNode<T> management beyond structural concerns
// is forbidden: a SimpleNode<T> instance is fully initialised at creation,
// and its Value is immutable and non-nullable. 
// Each SimpleNode<T> instance belongs to at most one SinglyLinkedList<T>.
// Node insertion always generates a new SimpleNode<T> instance. Removed
// nodes become unowned and cannot be added to another list.

// SinglyLinkedList<T> is not intended for direct consumer use.
// Rather, it provides a list mechanism on which
// the higher-level, consumer-facing classes Queue<T> and Stack<T> are built.
// T value management by Queue<T> and Stack<T> occurs only through
// SinglyLinkedList<T> structural management of SimpleNode<T> instances.



// ===== Invariants ===== 

// Head, Tail, and Count:

// Head and Tail are nullable SimpleNode<T> values.
// Count is a non-negative integer.

// At any one time, a SinglyLinkedList<T> instance is in exactly
// one of three states:

// (1) Empty: Head and Tail are null, and Count is 0.

// (2) Singleton: Head and Tail are non-null 
// and have reference identity. Head.NextNode == null. Count is 1. 

// (3) Multi-member: Head and Tail are non-null, and
// are distinct. Head.NextNode != null. Head points to the first node, 
// Tail points to the last node, and Count is equal to 
// the number of nodes in the list (Count > 1).


// Node Linking:

// If Head != null, iterating through the NextNode chain starting
// at Head traverses exactly Count many nodes, and always
// terminates with Tail.

// Tail.NextNode is always null.

// No cycles exist: iterating through the NextNode chain never
// returns a SimpleNode<T> instance more than once.



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
// Creates a new SimpleNode<T> instance at the head of the list
// with the passed T value. 
// Time complexity O(1) as we maintain Head/Tail references and don't
// traverse nodes.

// PeekTail():
// Returns the T value of the tail without removing it. 
// Time complexity O(1) as we maintain Head/Tail references and don't
// traverse nodes.

// RemoveTail():
// Returns the T value of the tail and removes it. 
// Time complexity O(n) as we must linearly traverse the list
// to update the NextNode of the penultimate node.
// Crucially, a SimpleNode<T> instance does not store a PrevNode reference.

// AddTail(T value):
// Creates a new SimpleNode<T> instance at the tail of the list with
// the passed T value. 
// Time complexity O(1) as we maintain Head/Tail references and don't
// traverse nodes.

// Contains(T value):
// Time complexity O(n) as we linearly search through nodes.

// Clear():
// Sets the Head and Tail properties to null, and the Count property to 0.
// Time complexity O(1).
// Optionally, we could iterate through each node and set NextNode to null
// for better garbage collection, but at the cost of O(n) time complexity.


// Additional Private Educational Methods.
// These are not part of the intended mechanism API for higher-level classes:

// RemoveValue(T value):
// Removes every node with a Value equal to the T object passed in.
// Time complexity O(n) as we perform a single linear traversal of the list.


// SwapNodes (T value1, T value2):
// Swaps the first node with a Value of value1 with the first node with
// a Value of value2.
// Time complexity O(n).
// The invariant that SinglyLinkedList<T> manages structure is preserved
// by appropriately adjusting Head, Tail, and NextNode values.




// ===== Design Discussion =====

// Tail Reference:

// A Tail reference is included to reduce the time complexity of PeekTail() and
// AddTail(T value). Without this reference, these operations would be O(n), as
// we would be required to linearly traverse the list.

// This is a choice of a slight increase in structural information for the benefit
// of enabling logically simpler and more efficient methods.

// Note that Head/Tail operations are never fully symmetric: RemoveTail() is always
// O(n) as even with a Tail reference in SinglyLinkedList<T>,
// SimpleNode<T> instances do not store a PrevNode reference, so we need
// to traverse the link to find the old Tail's PrevNode in order to set it 
// as the new Tail.


// Access Modifiers:

// The class and its API methods have the internal access modifier,
// as the class is not for consumer use (public would be inappropriate),
// but is accessed by other consumer-facing classes (private would be inappropriate).
// The modifier protected is also inappropriate as nothing inherits from the class.
// An inheritance-based implementation of higher-level classes, e.g. Stack<T>, would
// be inappropriate, as this would allow consumers to inappropriately
// call SinglyLinkedList<T> methods on a Stack<T> instance.

// The properties Head and Tail are private, as they are purely
// an implementation detail of SinglyLinkedList<T>. No other class needs
// to access them. Consumer-facing classes such as Queue<T> and Stack<T> interact
// with the underlying SinglyLinkedList<T> instance only via the internal API methods
// of SinglyLinkedList<T>.

// The Count property has a private setter as only this mechanism-focused
// class should be able to manipulate this reference. However, it
// has an internal getter to enable higher-level abstractions to access it to
// compute their sizes.
// An alternative choice would have been to not store a Count property,
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

internal class SinglyLinkedList<T>
{
    private SimpleNode<T>? Head {get; set;}
    private SimpleNode<T>? Tail {get; set;}
    internal int Count {get; private set;}

    internal SinglyLinkedList()
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
                this.Head = this.Head.NextNode;
                this.Count -=1;
            }

            // Return value.                 
            return value;
        }

    internal void AddHead(T value)
        {
            SimpleNode<T> newHead = new SimpleNode<T>(value);

            if (this.Head == null)              // Handle empty list.
            {
                this.Head = newHead;
                this.Tail = newHead;
            }
            else                                // Handle otherwise.
            {
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
            else
            {                                   // Handle otherwise.

                // Find the previous node to the Tail:
                
                SimpleNode<T>? prev = null;
                SimpleNode<T> current = this.Head;

                while (current.NextNode != null)
                {
                    prev = current;
                    current = current.NextNode;
                }

                // Adjust the list, then return value:

                this.Tail = prev;
                this.Tail.NextNode = null;

                // Adjust Count manually:

                this.Count -= 1;
            }

            // Return value.

            return value;
            
        }

    internal void AddTail(T value)
        {
            SimpleNode<T> newTail = new SimpleNode<T>(value);

            if (this.Tail == null)              // Handle empty list.
            {
                this.Head = newTail;
                this.Tail = newTail;
            }
            else                                // Handle otherwise.
            {
                this.Tail.NextNode = newTail;
                this.Tail = newTail;
            }

            // Adjust Count.

            this.Count += 1;                    
        }

    internal bool Contains(T value)
        {
            SimpleNode<T>? currentNode = this.Head;
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

    // Educational methods (not part of the mechanism API for higher-level classes):
    private void RemoveValue(T valueToRemove)
        {
            // Removes every node in the list with a Value of valueToRemove.
            // Iterates through the nodes with a while loop.
            // When iterating, tracks currentNode and prevNode.
            // If a node has the matching Value, we need to remove it and adjust Count.
            // When removing a node, we have to consider its position in the list.
            // For readability, I have explicitly handled 4 cases separately:
            // currentNode is the head and tail; currentNode is the head only; currentNode
            // is the tail only; currentNode is neither head nor tail.

            SimpleNode<T>? currentNode = this.Head;
            SimpleNode<T>? prevNode = null;

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

                        currentNode = null;

                        // No need to reassign prevNode.

                        continue;
                    }

                    else if (this.Head == currentNode)
                    {
                        this.Head = currentNode.NextNode;
                        this.Count -= 1;

                        currentNode = currentNode.NextNode;

                        // No need to reassign prevNode (it remains null).

                        continue;
                    }

                    else if (this.Tail == currentNode)
                    {
                        this.Tail = prevNode;
                        this.Tail.NextNode = null;
                        this.Count -= 1;

                        currentNode = null;

                        // No need to reassign prevNode.

                        continue;
                    }

                    else                         
                    {
                        this.Count -= 1;
                        prevNode.NextNode = currentNode.NextNode;

                        currentNode = currentNode.NextNode;

                        // No need to reassign prevNode.

                        continue;
                    }
                }
                else    // Move to next node
                {
                    prevNode = currentNode;
                    currentNode = currentNode.NextNode;
                }
            }
        }

    private void SwapNodes(T value1, T value2)
        {
            // Swaps the first node with Value of value1
            // with the first node with Value of value2.

            // Handles adjacent nodes, non-adjacent nodes,
            // and updates the Head correctly.

            if (EqualityComparer<T>.Default.Equals(value1, value2))
            {
                return;
            }

            SimpleNode<T>? node1 = null;
            SimpleNode<T>? node1Prev = null;
            SimpleNode<T>? node2 = null;
            SimpleNode<T>? node2Prev = null;

            // Set node1, node1Prev, node2, node2Prev.

            SimpleNode<T>? currentNode = this.Head;
            SimpleNode<T>? prevNode = null;

            while (currentNode != null)
            {
                if (node1 == null && EqualityComparer<T>.Default.Equals(currentNode.Value, value1))
                {
                    node1 = currentNode;
                    node1Prev = prevNode;
                
                }
                else if (node2 == null && EqualityComparer<T>.Default.Equals(currentNode.Value, value2))
                {
                    node2 = currentNode;
                    node2Prev = prevNode;
                }

                if (node1 != null && node2 != null)
                {
                    break;
                }

                prevNode = currentNode;
                currentNode = currentNode.NextNode;
            }

            // Check neither node1 nor node2 is null.

            if (node1 == null || node2 == null)            
            {
                throw new InvalidOperationException("At least one of those values is not in the list.");
            }

            // Cache node1Next and node2Next for simplicity.

            SimpleNode<T>? node1Next = node1.NextNode;
            SimpleNode<T>? node2Next = node2.NextNode;

            // There are three cases:

            // (a) node1Next is node2 (adjacency).
            // (b) node2Next is node1 (adjacency).
            // (c) the nodes are not adjacent.

            // We must also consider
            // whether certain nodes are the Head/Tail
            // (this will affect updating node1Prev and node2Prev NextNodes.)


            if (node1Next == node2)                // (a)
            {
                node1.NextNode = node2Next;
                node2.NextNode = node1;

                if (node1Prev != null)
                {
                    node1Prev.NextNode = node2;
                }
            }
            else if (node2Next == node1)           // (b)
            {
                node2.NextNode = node1Next;
                node1.NextNode = node2;

                if (node2Prev != null)
                {
                    node2Prev.NextNode = node1;
                }
            }
            else                                        // (c)
            {   
                
                node1.NextNode = node2Next;
                node2.NextNode = node1Next;

                if (node1Prev != null)
                {
                    node1Prev.NextNode = node2;
                }
                if (node2Prev != null)
                {
                    node2Prev.NextNode = node1;
                }                
            }

            // Finally, we adjust the Head/Tail properties.

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