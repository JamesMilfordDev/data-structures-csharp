// ===== Role =====

// This is a consumer-facing class that manages T values in a MinHeap system,
// which maintains sorted order of T values and allows efficient access to the 
// min T value.

// This is a high-level class layered on my BinaryTree<T>, a low-level,
// internal mechanism that manages collections of BinaryTreeNode<T> instances in
// a binary tree structure.

// With this implementation, BinaryTree<T> provides both
// the main mechanism *and* the structural policy that each node has
// a left child and right child position.

// MinHeap<T> imposes a further structural policy:
// the tree is complete (filled left-to-right).
// That is, the tree cannot be imbalanced, except in the
// restricted sense that the far right of the last level
// can be incomplete.

// MinHeap<T> also imposes an order by Value policy:
// parent <= child.
// Ordering is based on the generic interface IComparable<T>.

// As such, MinHeap<T> imposes:
// stronger structural constraints than BinarySearchTree<T>, 
// but weaker ordering constraints.
// Crucially, MinHeap<T> imposes no ordering of children;
// there is no left/right significance in a MinHeap.

// In BinarySearchTree<T>, ordering is global.
// In MinHeap<T>, ordering is local.

// SPOILER: this node-based implementation is primarily educational, built to
// demonstrate my ability to implement architecture that cleanly separates 
// roles, to highlight the differences between a BST and a MinHeap, and to 
// showcase the tradeoff between node-based and array-based
// implementations of a MinHeap. ArrayMinHeap<T>, found elsewhere in this repository,
// is much more optimal in production.



// ===== Invariants =====

// Structure:

// MinHeap<T> inherits the structural policy
// of BinaryTree<T> that nodes have at most two children.
// MinHeap<T> imposes a further structural policy:
// the tree is complete (filled left-to-right).


// Order by Value:

// parent <= child.


// Duplication:

// In BinarySearchTree<T>, we disallowed 
// duplicates on a broad definition of duplication:
// Two nodes A and B are duplicates iff 
// A.Value.CompareTo(B.Value) == 0.

// This was required given the strict ordering policy:
// left child < parent < right child.

// However, broad duplicates are not a problem in a MinHeap<T>,
// as we only require parent <= child. Indeed, we can also
// allow narrow duplicates: multiple nodes with the same Value.

// As with BinaryTree<T>, a MinHeap<T> cannot contain the same 
// BinaryTreeNode<T> instance in two separate locations.



// ===== Methods =====

// Peek().
// Returns the minimum T value without removing it.
// Throws an exception if the MinHeap is empty.
// Time complexity O(1).

// Clear().
// Calls the BinaryTree<T> method Clear() on the BinaryTree<T> reference
// stored by the MinHeap<T> instance.
// Time complexity O(1).

// Contains(T value).
// As a MinHeap<T> is not globally ordered, unlike BinarySearchTree<T>,
// we do not have access to ordered traversal. As such,
// Contains() here just calls the BinaryTree<T> method Contains()
// on the BinaryTree<T> reference stored by the MinHeap<T> instance.
// That Contains() method uses my Queue<T> class to perform a BFS.
// Time complexity O(n).

// Insert(T value).
// We identify the insertion point for a new node (the far right of
// the bottom layer of the tree), add a node n with a Value of value, 
// then "bubble up" Values from n.
// Time complexity O(n) for the initial traversal, then O(log n) for the bubble up.
// This collapses to O(n).


// Pop().
// We save the original Value of the root: popValue.
// We identify the final node in the tree (the far right of
// the bottom layer of the tree), and set the root's Value to the Value
// of the final node.
// We remove final node from the tree, then "bubble down" Values from Root.
// Throws an exception if the MinHeap is empty.
// Time complexity O(n) for the initial traversal, then O(log n) for the bubble down.
// This collapses to O(n).


// ===== Design Discussion =====

// Consequences of no Parent Reference:

// The notable casualty of not storing a Parent reference in a BinaryTreeNode<T>
// is Insert(). As a result of the simpler lower-level containers,
// our "bubble up" is more complex. Overall, the method ends up with a
// higher time and space complexity.

// Peek(), Clear(), Contains(), and Pop() are not affected by the lack of a Parent
// reference in a BinaryTreeNode<T>.


// Properties and Access Modifiers:

// As with BinarySearchTree<T>, there is a Size property, but it does not have a 
// private backing field as this is unnecessary.
// Rather, Size only has a getter (which is public), 
// which directly gets the Count property of the 
// underlying BinaryTree<T>, because consumers may want to 
// access the size of the MinHeap.

// There is no Root property, as this is unnecessary: consumers do not need to access
// the Root of the underlying tree. I could have added a Root property with a private
// getter that immediately accesses the Root of the underlying BinaryTree<T>, and so
// doesn't have a private backing field, which could then be used in this class, but 
// this is unnecessary: it merely allows us to access the Root with this.Root rather
// than this.Tree.Root.

// If anything, I find the syntax of this.Tree.Size and this.Tree.Root more
// helpful to readers here, as it stresses that MinHeap<T> does not have 
// backing fields for Root/Size.

// The class and all its API methods are public as they are consumer-facing.


// Alternative Background Structures:

// I could have implemented a composition-based MinHeap class
// with an array as the background data structure. See ArrayMinHeap<T>
// in this repository for details and a discussion on trade-offs.

// Again: this node-based implementation is primarily educational, built to
// demonstrate both my ability to implement architecture that cleanly separates 
// roles, to highlight the differences between a BST and a MinHeap, and to 
// showcase the tradeoff between node-based and array-based
// implementations of a MinHeap. ArrayMinHeap<T> is much more optimal in production.


// null! Usage:

// When a variable declared as a non-nullable type cannot be immediately initialised, 
// but is guaranteed by logic to be assigned a value of the correct type before use, 
// it is initialised with null!.
// This tells the compiler that although the variable starts as null, it will be non-null
// when accessed, avoiding definite assignment errors (CS0165).
// The use of var is avoided to keep types explicit.
// This occurs once in Insert() and twice in Pop().


using System;

namespace DataStructures
{

// As with BinarySearchTree, our type T must implement IComparable<T>,
// which importantly for us contains the CompareTo() method.
public class MinHeap<T> where T : IComparable<T>
{
    private BinaryTree<T> Tree {get;}

    public int Size
        {
            get { return this.Tree.Count;}
        }
    public MinHeap()
        {
            this.Tree = new BinaryTree<T>();
        }


    public T Peek()
        {
            if (this.Tree.Root == null)
            {
                throw new InvalidOperationException("The heap is empty.");
            }

            return this.Tree.Root.Value;            
        }

    public void Clear()
        {
            this.Tree.Clear();
        }

    public bool Contains(T value)
        {
             return this.Tree.ContainsValue(value);
        }

    public void Insert(T value)
        {
            // Part 1:
            // To adhere to the structural policy of completeness, 
            // we first identify the insertion point (ie the far right
            // of the lowest level of the tree).
            // Part 2:
            // To adhere to the ordering policy,
            // we adjust the tree.

            // Part 1:

            // We traverse the tree to identify the insertion point,
            // and insert a new node, newNode, with a Value of value.
            // We use BFS. DFS cannot guarantee left-to-right filling.

            // Part 2:

            // To adjust the tree, we adjust node Values, not the 
            // positions of particular nodes.

            // To adjust the Values of nodes, we "bubble up" 
            // the Value of newNode.
            // Starting with newNode and its parent, we move upwards,
            // checking for each child-parent pair 
            // whether child < parent. If this holds, we swap
            // the Values of child and parent.
            // We continue until child < parent does not hold.
            // At which point, we have correctly shifted the Values of the nodes
            // so that the local ordering policy is satisfied.

            // The main issue with not storing a Parent reference
            // in a BinaryTreeNode is that we have no simple way
            // to traverse up the branch through parent-child combos.

            // There are various options to achieving the bubble up without
            // a direct Parent reference in our nodes, though
            // these alternatives involve more complex logic.
            // Here, I shall discuss two:

            // Option A:
            // Each time we need to access the parent of a node n, we perform
            // a BFS traversal (recording parent and current nodes) until we reach
            // n. However, this adds a time complexity O(n) operation each time
            // we want to access the parent of a node n, which is 
            // very inefficient.

            // Option B:
            // During the initial BFS traversal, we track both parent and
            // current node, and record each parent-child combo moved through.
            // Each combo can be stored as a (BinaryTreeNode<T>, BinaryTreeNode<T>).
            // By the time we reach the insertion point and attach newNode,
            // we have a record of the parents of every node above our newNode
            // in the tree.

            // On space complexity, storing a Parent reference in each node uses
            // O(1) space per node, so O(n) extra space, but this is spread evenly
            // across the structure, and there is no per-operation allocation.

            // In contrast, with Option (B), we have O(n) extra space per Insert() call.

            // On time complexity, with a Parent reference in our nodes, the BFS
            // traversal is O(n), then the bubble-up is O(log n).

            // With Option (B), at each node during the traversal, we perform 
            // a recording operation. During the bubble-up, 
            // we combine the upward-traversal with
            // a look-up at each stage. How long each look up takes depends on the
            // structure used.
            // This whole process simplifies to O(n),
            // but in practice is many more operations than 
            // if nodes stored Parent references.

            // I will implement a version of Option (B).
            // Different versions of Option (B) use different structures to
            // hold the (BinaryTreeNode<T>, BinaryTreeNode<T>) objects.
            // I will use a dictionary, as each lookup is O(1) time.

            if (this.Tree.Root == null)
            {
                this.Tree.SetRoot(value);
                return;
            }


            // Introduce newNode:

            BinaryTreeNode<T> newNode = null!;
            
            // This assignment prevents the compiler from complaining.
            // My logic guarantees that newNode is assigned a BinaryTreeNode<T>.

            // Prepare for the BFS traversal:

            Queue<BinaryTreeNode<T>> queue = new DataStructures.Queue<BinaryTreeNode<T>>();
            Dictionary<BinaryTreeNode<T>, BinaryTreeNode<T>> parentRecord = new  Dictionary<BinaryTreeNode<T>, BinaryTreeNode<T>>();

           // BFS traversal:

            queue.Enqueue(this.Tree.Root);

            while (queue.Size > 0)
            {
                BinaryTreeNode<T> currentNode = queue.Dequeue();

                if (!this.Tree.HasLeft(currentNode))
                {
                    this.Tree.AddLeft(currentNode, value);
                    newNode = currentNode.LeftChild;
                    parentRecord[newNode] = currentNode;
                    break;
                }
                else
                {
                    parentRecord[currentNode.LeftChild] = currentNode;
                    queue.Enqueue(currentNode.LeftChild);
                }

                if (!this.Tree.HasRight(currentNode))
                {
                    this.Tree.AddRight(currentNode, value);
                    newNode = currentNode.RightChild;
                    parentRecord[newNode] = currentNode;
                    break;
                }
                else
                {
                    parentRecord[currentNode.RightChild] = currentNode;
                    queue.Enqueue(currentNode.RightChild);
                }
            }

            // Bubble up:

            BinaryTreeNode<T> current = newNode;

            while (parentRecord.ContainsKey(current)) // Whilst current has a parent.
            {
                BinaryTreeNode<T> parent = parentRecord[current];

                if (parent.Value.CompareTo(current.Value) <= 0)
                {
                    break; // Ordering property satisfied; no need to swap.
                }
            
            // Swap values:

            T temp = current.Value;
            current.Value = parent.Value;
            parent.Value = temp;

            // Move to the next node up:

            current = parent;
            }
        }

    public T Pop()
        {
            // Trivial cases are handled upfront:

            if (this.Tree.Root == null)
            {
                throw new InvalidOperationException("The heap is empty.");
            }

            T popValue;

            if (this.Tree.Count == 1)
            {
                popValue = this.Tree.Root.Value;
                this.Tree.Clear();
                return popValue;
            }

            // All other cases involve this.Tree.Root having at least one child.

            // Part 1:
            // BFS traversal down to finalNode ie the node at 
            // the far right of the lowest level.
            // We do not need to employ a Dict object
            // as with Insert().
            // However, we will need to save the parent of finalNode.
            // to remove it in Part 3.

            // Part 2:
            // Save Value of this.Tree.Root as the popValue, then
            // adjust the Value of this.Tree.Root to finalNode.Value.

            // Part 3:
            // Remove finalNode from the tree.
            // This includes adjusting the Count property of this.Tree.

            // Part 4:
            // "Bubble down" the new Value of this.Tree.Root.

            // Part 5:
            // Return popValue.


            // Part 1:


            Queue<(BinaryTreeNode<T>, BinaryTreeNode<T>)> queue = new DataStructures.Queue<(BinaryTreeNode<T>, BinaryTreeNode<T>)>();


            // Before our main while loop, we enqueue the root's children:

            if (this.Tree.HasLeft(this.Tree.Root)) 
            // Count > 1, so we have at least one child; check retained for defensive clarity.
            {
                queue.Enqueue((this.Tree.Root, this.Tree.Root.LeftChild));

                if (this.Tree.HasRight(this.Tree.Root))
                // A node has a right child only if
                // it has a left child, as the tree is complete.
                // So, we check for a right child within the
                // block that executes if a left child exists.
                {
                    queue.Enqueue((this.Tree.Root, this.Tree.Root.RightChild));
                }
            
            }
            
            BinaryTreeNode<T> parentNode = null!;
            BinaryTreeNode<T> finalNode = null!;

            // These assignments prevent the compiler from complaining.
            // My logic guarantees that parentNode and finalNode will be
            // immediately reassigned in the following Dequeue().

            (parentNode, finalNode) = queue.Dequeue();
            
            while (queue.Size > 0)
            {
                (parentNode, finalNode) = queue.Dequeue();

                if (this.Tree.HasLeft(finalNode))
                {
                    queue.Enqueue((finalNode, finalNode.LeftChild));

                    
                
                    if (this.Tree.HasRight(finalNode))
                    {
                        queue.Enqueue((finalNode, finalNode.RightChild));
                    }
                }
            }

            // Part 2:

            popValue = this.Tree.Root.Value;
            this.Tree.Root.Value = finalNode.Value;

            // Part 3:

            // Two cases:
            // (A) finalNode is the LeftChild of parentNode.
            // (B) finalNode is the RightChild of parentNode.

            if (finalNode == parentNode.LeftChild)
            {
                parentNode.LeftChild = null;
            }
            else
            {
                parentNode.RightChild = null;
            }

            this.Tree.DecrementCount();

            // Part 4:

            BinaryTreeNode<T> current = this.Tree.Root;
            

            while (this.Tree.HasLeft(current)) 

            // Ie while current has at least one child.
            // We know the Root does, so the loop executes at least once.

            {
                BinaryTreeNode<T> smallest = current;

                // See which of the parent, left child, and right child is smallest.
                
                if (current.LeftChild.Value.CompareTo(smallest.Value) < 0)
                {
                    smallest = current.LeftChild;
                }

                if (this.Tree.HasRight(current) && current.RightChild.Value.CompareTo(smallest.Value) < 0)
                {
                    smallest = current.RightChild;
                }

                // If the parent is the smallest, the bubbling down is complete.

                if (smallest == current)
                {
                    break;
                }

                // Otherwise, we swap the parent Value and smallest child Value
                // (or swap with left child if the two child Values are equal in ordering).

                T temp = current.Value;
                current.Value = smallest.Value;
                smallest.Value = temp;

                // We move down the tree:

                current = smallest;
            }


            // Part 5:

            return popValue;
        }



}

}