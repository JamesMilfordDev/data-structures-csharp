// ===== Role =====

// This is a consumer-facing class that manages T values in a MinHeap system,
// which maintains sorted order of T values and allows efficient access to the 
// min T value.

// ArrayMinHeap<T> was created to compare two composition-based
// MinHeap implementations: 

// (A) A tree-backed MinHeap built using BinaryTree<T>, which explicitly
// models nodes and tree structure.

// (B) An array-backed MinHeap, which implicitly defines tree
// structure via index arithmetic.



// ===== Invariants =====

// Structure and Order by Value:

// Nodes have at most two children.
// The tree is complete (filled left-to-right).
// parent <= child.

// Structure and order are enforced by index arithmetic.


// Duplication:

// The same T value may appear multiple times in the MinHeap.


// Size and MaxSize:

// Size is a non-negative integer that represents logically how many
// T objects are in the MinHeap.
// Size is a logical index pointer: 
// upon logical removal of a T object
// from a MinHeap, MinHeap<T> does not overwrite the leftover
// T reference in the backing T[] to the default T value.
// As such, it may be that the number of non-default T values in the backing
// T[] exceeds Size.

// MaxSize here is a non-nullable positive integer.
// Attempting to construct an ArrayMinHeap<T> instance with a non-positive MaxSize
// throws an exception.

// Size <= MaxSize.



// ===== Methods =====

// ArrayMinHeap<T> contains the same methods as MinHeap<T>.
// See the Design Discussion of this initial comment block
// for a discussion about the efficiency of the
// methods implemented here vs in MinHeap<T>.

// Peek(), Clear(), Contains(), Insert(), Pop().



// ===== Design Discussion =====

// On a Maximum Size:

// The simple approach to a tree-backed MinHeap
// was to not impose a max size
// on the MinHeap. In contrast, with (B), if we use a T[] object
// as the background data structure, the simple approach is to
// impose a max size on a MinHeap. In this case, the T[] object will
// be of that size. This approach is similar to my handling of
// ArrayDeque<T>. 
// Of course, we could augment a tree-backed MinHeap to impose a max size. 
// Further, we could eschew a max size with an array-based MinHeap if we 
// either (i) kept using a T[] object as the background structure, but called
// Array.Resize(), or (ii) used a List<T> object.

// Here, I will use a T[] object as the background data structure,
// and require a max size, for simplicity.


// Properties and Access Modifiers:

// The class and all its API methods are public as they are consumer-facing.
// MaxSize and Size have public getters are these are useful for consumers to 
// access. MaxSize is immutable, and Size has a private setter, as only
// the methods of this class should be able to mutate it.

// Items has a private getter so that ArrayDeque<T> can access the background
// T[]. It has no setter as although elements of the background T[] can be adjusted,
// the Items reference should never be adjusted to a different T[].


// BinaryTree<T> vs an Array as the Background Structure:

// Method comparison: 

// Peek(): O(1) time complexity for each implementation.

// Clear(): O(1) time complexity for each implementation.
// ArrayMinHeap<T> is time complexity O(1) 
// as we don't overwrite left over T objects in 
// the background T[].
// If we wanted to clear the background T[], this would be O(n) as we 
// would be required to perform a linear traversal through the T[],
// setting every index below this.Size to the default T value.

// Contains(): O(n) time complexity for each implementation.
// In the tree-backed implementation, we perform an
// unordered traversal of the tree. In the array-backed implementation,
// we perform a linear search.
// However, the tree-backed implementation will have a higher
// space complexity. For example, if we perform a BFS traversal, this requires
// an additional Queue<BinaryTreeNode<T>> structure. Space complexity
// O(w), for max width w. The linear search of the array-backed
// implementation has space complexity O(1).

// Insert() and Pop():

// Completeness:

// With a tree-backed implementation, structural
// completeness must be actively enforced. This means BFS traversing
// the tree to find the correct node insertion point in Insert(),
// and BFS traversing the tree to find the correct node to remove
// in Pop(). These traversals are O(n) time complexity, O(w) space
// complexity.
// An array-backed implementation guarantees completeness by construction:
// insertion always appends to the logical end of the array and removal
// always deletes the logical last element, with index arithmetic providing
// the implicit parent-child relationships. These insertion/removal
// operations are O(1) time complexity.


// Bubbling:

// When bubbling up in Insert(), and bubbling down in Pop(),
// the tree-backed class traverses node references. The
// array-backed class uses simple index calculations.
// There is no difference in time complexity bubbling 
// between the two classes here,
// assuming BinaryTreeNode<T> stores a Parent reference.
// However, if BinaryTreeNode<T> does not store a Parent reference,
// we have seen that our bubble up becomes more complex,
// resulting in Insert() having a higher time and space complexity.


// Further considerations:

// The tree-backed implementation requires additional space, as each
// node carries object overhead. The array-backed implementation uses a
// contiguous block of memory and O(1) per element. So, the
// array-backed implementation has better cache locality, and 
// better iteration speed due to better cache locality.

// However, the tree-backed implementation 
// more naturally supports dynamic growth.




using System;
using System.Collections.Generic;

namespace DataStructures
{

public class ArrayMinHeap<T> where T : IComparable<T>
{
    public int MaxSize {get;}
    public int Size {get; private set;}
    private T[] Items {get;}

    public ArrayMinHeap(int maxSize)
        {

            if (maxSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSize));
            }

            this.MaxSize = maxSize;
            this.Size = 0;
            this.Items = new T[maxSize];
        }


    public T Peek()
        {
            if (this.Size == 0)
            {
                throw new InvalidOperationException("The heap is empty.");
            }

            return this.Items[0];       
        }

    public void Clear()
        {
            // No overwriting of T references. If we wanted to include it:
            // for (int i = 0; i < this.Size; i++){this.Items[i] = default;}

            this.Size = 0;
        }
        
    public bool Contains(T value)
        {
            for (int i = 0; i < this.Size; i++)
            {
                if (EqualityComparer<T>.Default.Equals(this.Items[i], value))
                {
                    return true;
                }
            }

            return false;
        }

    public void Insert(T value)
        {
            if (this.Size >= this.MaxSize)
            {
                throw new InvalidOperationException("The heap is full.");
            }

            // Part 1: Insert the new value at the insertion point.
            // No traversal is required here. The insertion point is simply
            // this.Size.
            // So time complexity O(1).

            int currentIndex = this.Size;
            this.Items[currentIndex] = value;
            this.Size++;

            // Part 2: Bubble up.

            while (currentIndex > 0)
            {
                int parentIndex = (currentIndex - 1) / 2;

                if (this.Items[currentIndex].CompareTo(this.Items[parentIndex]) >= 0)
                {
                    break; // Ordering property satisfied
                }

                // Swap child with parent:

                T temp = this.Items[currentIndex];
                this.Items[currentIndex] = this.Items[parentIndex];
                this.Items[parentIndex] = temp;

                currentIndex = parentIndex;
            
            }
        }

    public T Pop()
        {
            // Trivial cases are handled upfront:

            if (this.Size == 0)
            {
                 throw new InvalidOperationException("The heap is empty.");
            }

            T popValue;

            if (this.Size == 1)
            {
                popValue = this.Items[0];
                this.Clear();
                return popValue;
            }

            // All other cases involve at least one child of the root.

            popValue = this.Items[0];

            // Part 1: Swap the root value to the last-member value.

            this.Items[0] = this.Items[Size-1];

            // Part 2: Remove the last-member value.

            this.Size -= 1;

            // We can optionally default the index of the background T[]
            // that is now not strictly part of our MinHeap: 
            
            // this.Items[Size-1] = default;


            // Part 3: Bubble down.
            int currentIndex = 0;

            while ((2 * currentIndex) + 1 < this.Size) 

            // While currentIndex has a child
            // The condition (2 * currentIndex) + 1 < this.Size)
            // checks if a left child exists, and as a MinHeap is complete,
            // this is equivalent to currentIndex having at least one child.
            // We know the root does, so the loop executes at least once.

            {
                int smallestIndex = currentIndex;

                int leftChildIndex = 2 * currentIndex + 1;
                int rightChildIndex = 2 * currentIndex + 2;

                // See which of the parent, left child, and right child is smallest.

                
                if (this.Items[leftChildIndex].CompareTo(this.Items[smallestIndex]) < 0)
                {
                    smallestIndex = leftChildIndex;
                }

                if (rightChildIndex < this.Size && this.Items[rightChildIndex].CompareTo(this.Items[smallestIndex]) < 0)
                {
                    smallestIndex = rightChildIndex;
                }

                // If the parent is the smallest, the bubbling down is complete.

                if (smallestIndex == currentIndex)
                {
                    break;
                }

                // Otherwise, we swap the parent value and smallest child value
                // (or swap with left child if the child values are equal in ordering).

                T temp = this.Items[currentIndex];
                this.Items[currentIndex] = this.Items[smallestIndex];
                this.Items[smallestIndex] = temp;
        
                // We move down the tree:

                currentIndex = smallestIndex;
            }

            // Part 4: Return popValue.

            return popValue;

        }


}

}




