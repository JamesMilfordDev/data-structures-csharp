// ===== Role =====

// A low-level, internal mechanism to manage collections
// of BinaryTreeNode<T> instances in a binary tree structure.

// A BinaryTreeNode<T> is a dumb, low-level container that is wholly managed
// by BinaryTree<T>. This management is in large part structural: it
// concerns which nodes a BinaryTree<T> logically contains, 
// and their position in the tree. However, BinaryTree<T> is also able
// to mutate node Values.
// Still, a BinaryTreeNode<T> instance is fully initialised at creation,
// and its Value is non-nullable. 
// Each BinaryTreeNode<T> instance belongs to at most one BinaryTree<T>.
// Node insertion always generates a new BinaryTreeNode<T> instance. Removed
// nodes become unowned and cannot be added to another list.

// BinaryTree<T> is not intended for direct consumer use.
// Rather, it provides a basic binary tree structure and mechanism,
// allowing for more specialised trees to sit on top of it.

// BinaryTree<T> presents
// a purely general binary tree with no positional assumptions.
// In this repository, it is used as the foundation for 
// consumer-facing specialised binary trees.



// ===== Invariants =====

// Root and Count:

// Root is a nullable BinaryTreeNode<T> value.
// Count is a non-negative integer.

// At any one time, a BinaryTree<T> instance is in exactly
// one of three states:

// (1) Empty: Root is null, and Count == 0.

// (2) Singleton: Root is non-null, Count == 1,
// Root.LeftChild == null, and Root.RightChild == null

// (3) Multi-member: Root is non-null, 
// and Count is equal to the number of nodes in the tree.


// Structure:

// With a BinaryTree<T>, nodes have at most two children.
// A BinaryTree<T> imposes no further structural policy than this
// (imbalanced trees are allowed).
// We have a fully general notion of a binary tree.


// Order by Value:

// BinaryTree<T> imposes no ordering by Value policy.
// That is the domain of more specialised binary trees.


// Duplication: 

// A BinaryTree<T> is allowed to logically contain multiple BinaryTreeNode<T>
// instances with the same Value.
// Restrictions on duplication may be layered on by other specialised trees
// that are implemented with a composition-based implementation using
// a BinaryTree<T> as the background data structure.
// However, a BinaryTree<T> cannot contain the same BinaryTreeNode<T>
// instance in two separate locations.



// ===== Methods =====

// I have already implemented BFS traversal with appeal to my own Queue<T> class,
// DFS traversal with explicit appeal to my own Stack<T> class,
// and DFS traversal using recursion ie implicit appeal to the call stack.
// See GeneralTree<T>.
// As such, for simplicity, I shall here only use BFS traversal when 
// traversal is required.

// I also do not include a node containment method ie a 
// method with a BinaryTreeNode<T> parameter that
// checks whether the passed node is in the tree. 
// This is not needed.
// One reason to have such a method is to verify that 
// for certain internal methods that pass in a node,
// the passed node is in the tree. For example, in 
// AddLeft(BinaryTreeNode<T> parent, T value).
// However, these methods are internal, and I have ensured that this assumption is 
// guaranteed in the contexts in this repository in which I call these methods.

// SetRoot(T value).
// Time complexity O(1).
// Throws an exception if a Root already exists.

// Clear().
// Time complexity O(1).

// SwapValues(BinaryTreeNode<T> node1, BinaryTreeNode<T> node2).
// Time complexity O(1) as we
// assume the caller only passes nodes 
// into the parameters if they exist in the tree.
// Otherwise, it would be O(n).

// ContainsValue(T Value).
// Time complexity O(n).
// Employs BFS traversal.

// RemoveBranch(BinaryTreeNode<T> node).
// Time complexity O(n).

// AddLeft(BinaryTreeNode<T> parent, T value).
// Throws an exception if a left child already exists.
// Time complexity O(1) as we assume the node passed into a method
// call exists in the tree. Otherwise, it would be O(n).


// AddRight(BinaryTreeNode<T> parent, T value).
// Throws an exception if a right child already exists.
// Time complexity O(1) as we assume the node passed into a method
// call exists in the tree. Otherwise, it would be O(n).

// We additionally add some helper methods in the API. These are all
// time complexity O(1) as we assume the node passed into a method
// call exists in the tree. Otherwise, it would be O(n).

// HasLeft(BinaryTreeNode<T> node).
// HasRight(BinaryTreeNode<T> node).

// GetLeft(BinaryTreeNode<T> node).
// Throws an exception if there is no left child.

// GetRight(BinaryTreeNode<T> node).
// Throws an exception if there is no right child.

// Finally, we add some additional internal methods that will be
// needed by higher level classes to adjust the underlying BinaryTree<T>
// stored by an instance of such a class:

// DecrementCount().
// Reduces the Count by 1.
// Time complexity O(1).

// ShiftRootDownToUniqueChild().
// Checks to see that there is a Root and the Root has a unique child.
// Throws an exception if this is not the case. If it is the case,
// the Root's child is made the new Root of the BinaryTree<T>, whilst
// maintaining the rest of the structure of the tree. This also involves
// decrementing Count by 1.
// Time complexity O(1).



// ===== Design Discussion =====

// Consequences of no Parent/SubTreeSize References:

// In a similar fashion to RemoveBranch() in GeneralTree<T>,
// if a BinaryTreeNode<T> stored a Parent reference,
// RemoveBranch() would be simpler:
// we could immediately access the parent node of the
// "root" of our branch to be removed, then remove
// that root from the parent node. Next, we would
// perform a single traversal of the removed branch to
// determine its size and therefore adjust
// the Count of the residual tree.

// Finally, again, regardless of whether a BinaryTreeNode<T> stores a 
// Parent reference, we could simplify the second
// part of RemoveBranch() (ie counting the subtree size) by storing
// a SubTreeSize reference in a BinaryTreeNode<T>. In which case,
// we would not need to traverse the subtree to determine its size.
// Again, like storing a Parent reference in a node, this is another example 
// of increased space complexity in our low-level container to
// reduce algorithmic complexity at a higher abstraction layer.

// With both Parent and SubTreeSize references, RemoveBranch() becomes O(1).


// Separate Traversal Methods:

// Again, I could have implemented a BFS() method
// that returns an IEnumerable<BinaryTreeNode<T>>. Then some tree 
// traversals in API methods could have alternatively been 
// implemented by iterating through this IEnumerable<BinaryTreeNode<T>>.
// However, there is no real benefit to this lazy traversal here,
// as traversal is always fully consumed or short-circuited by explicit logic
// in BinaryTree<T> methods.
// Further, we do not have a single uniform type of traversal. For example,
// sometimes when we traverse, we track both the current node and its parent node.
// I *do* implement some lazy traversal in Graph<T> in this repository.


// Access Modifiers:

// The class and its API methods have the internal access modifier,
// as the class is not for consumer use (public would be inappropriate),
// but is accessed by other consumer-facing classes (private would be inappropriate).
// The modifier protected is also inappropriate as nothing inherits from the class.
// An inheritance-based implementation of higher-level classes would
// be inappropriate, as this would allow consumers to inappropriately
// call BinaryTree<T> methods directly on higher-level abstractions.

// Root and Count have internal getters, so that certain higher-level
// abstractions can access these properties, but private setters as no other
// class should be able to directly mutate them.


// Error Handling:

// Again, as all of the methods in this class are internal, I chose
// not to include node containment checks in methods, at the cost of 
// having to manually confirm that whenever a method call that takes a 
// BinaryTreeNode<T> instance as argument, and supposes that node is part 
// of the tree, the context of the 
// call ensures the passed BinaryTreeNode<T> instance is in the tree.




using System;
using System.Collections.Generic; // Only to access EqualityComparer<T>.

namespace DataStructures
{
internal class BinaryTree<T>
{
    internal BinaryTreeNode<T>? Root {get; private set;}
    internal int Count {get; private set;}
    internal BinaryTree()
        {
            this.Root = null;
            this.Count = 0;
        }

    internal void SetRoot(T value)
        {
            if (this.Root != null)
            {
                throw new InvalidOperationException("Tree already has a root.");
            }

            BinaryTreeNode<T> newNode = new BinaryTreeNode<T>(value);
            this.Root = newNode;
            this.Count = 1;
        }

    internal void Clear()
        {
            this.Root = null;
            this.Count = 0;
        }

    internal void SwapValues(BinaryTreeNode<T> node1, BinaryTreeNode<T> node2)
        {
            T temp = node1.Value;
            node1.Value = node2.Value;
            node2.Value = temp;
        }

    internal bool HasLeft(BinaryTreeNode<T> node)
        {
            return node.LeftChild != null;
        }

    internal bool HasRight(BinaryTreeNode<T> node)
        {
            return node.RightChild != null;
        }

    internal BinaryTreeNode<T> GetLeft(BinaryTreeNode<T> node)
        {
            if (node.LeftChild == null)
            {
                throw new InvalidOperationException("That node has no left child.");
            }

            return node.LeftChild;
        }

    internal BinaryTreeNode<T> GetRight(BinaryTreeNode<T> node)
        {
            if (node.RightChild == null)
            {
                throw new InvalidOperationException("That node has no right child.");
            }

            return node.RightChild;
        }

    internal void AddLeft(BinaryTreeNode<T> node, T value)
        {
            if (this.HasLeft(node))
            {
                throw new InvalidOperationException("That node already has a left child.");
            }

            BinaryTreeNode<T> newNode = new BinaryTreeNode<T>(value);
            node.LeftChild = newNode;
            this.Count += 1;
        }

    internal void AddRight(BinaryTreeNode<T> node, T value)
        {
            if (this.HasRight(node))
            {
                throw new InvalidOperationException("That node already has a right child.");
            }

            BinaryTreeNode<T> newNode = new BinaryTreeNode<T>(value);
            node.RightChild = newNode;
            this.Count += 1;
        }

    internal void DecrementCount()
        {
            this.Count -= 1;
        }

    internal void ShiftRootDownToUniqueChild()
        {
            if (this.Root == null)
            {
                throw new InvalidOperationException("The tree has no root.");
            }
            if (!this.HasLeft(this.Root) && !this.HasRight(this.Root))
            {
                throw new InvalidOperationException("The root has no child.");
            }
            if (this.HasLeft(this.Root) && this.HasRight(this.Root))
            {
                throw new InvalidOperationException("The root has two children.");
            }

            // Save the unique child:

            BinaryTreeNode<T> child;
            if (this.HasLeft(this.Root))
            {
                child = this.Root.LeftChild;
            }
            else
            {
                child = this.Root.RightChild;
            }

            this.Root = child;
            this.Count -= 1;
        }



    internal bool ContainsValue(T value)
        {
            if (this.Root == null)
            {
                return false;
            }

            Queue<BinaryTreeNode<T>> queue = new DataStructures.Queue<BinaryTreeNode<T>>();
            queue.Enqueue(this.Root);

            while (queue.Size > 0)
            {
                BinaryTreeNode<T> currentTreeNode = queue.Dequeue();
                
                if (EqualityComparer<T>.Default.Equals(currentTreeNode.Value, value))
                {
                    return true;
                }

                if (this.HasLeft(currentTreeNode))
                {
                    queue.Enqueue(currentTreeNode.LeftChild);
                }
                if (this.HasRight(currentTreeNode))
                {
                    queue.Enqueue(currentTreeNode.RightChild);
                }
            }

            return false;
        }  

    internal void RemoveBranch(BinaryTreeNode<T> node)
        {
            if (this.Root == null)
            {
                throw new InvalidOperationException("The tree is empty.");    
            }

            // As BinaryTreeNode<T> objects don't keep a reference to 
            // the Parent,
            // we will need to traverse the tree, keeping track of currentParent and
            // currentNode.
            // At each node, we check whether currentNode == node. If it does, we
            // remove currentNode as the left/right child
            // from currentParent. Next, we need to
            // determine how large the branch from currentNode is, to determine how
            // much to decrement the Count of the tree by.

            // There are multiple ways
            // to traverse the tree, with their own pros/cons.
            // For simplicity, I will just implement a BFS.

            // Special case: node == this.Root.
            // this.Root has no parent, so we handle it separately:

            if (this.Root == node)
            {
                this.Clear();
                return;
            }

            Queue<(BinaryTreeNode<T>, BinaryTreeNode<T>)> queue = new DataStructures.Queue<(BinaryTreeNode<T>, BinaryTreeNode<T>)>();
            
            if (this.HasLeft(this.Root))
                {
                    queue.Enqueue((this.Root, this.Root.LeftChild));
                }
            if (this.HasRight(this.Root))
                {
                    queue.Enqueue((this.Root, this.Root.RightChild));
                }

            while (queue.Size > 0)
            {
                (BinaryTreeNode<T> currentParent, BinaryTreeNode<T> currentNode) = queue.Dequeue();
                
                if (currentNode == node)
                {
                    // currentNode is our node!
                    // Now we need to:
                    // (a) remove it from currentParent;
                    // (b) compute the size of the branch
                    // to adjust the Count of the tree down; and
                    // (c) return.

                    // (a)
                    // We need to check whether currentNode is the left or right child.
                    
                    if (currentParent.LeftChild == currentNode)
                        {
                            currentParent.LeftChild = null;
                        }
                    else
                        {
                            currentParent.RightChild = null;
                        }

                    // (b) In order to know how much to subtract from Count,
                    // we need to know the size of the branch from currentNode.
                    // This requires a traversal of the branch.
                    // Let's use BFS again:

                    int branchSize = 0;
                    Queue<BinaryTreeNode<T>> branchQueue = new DataStructures.Queue<BinaryTreeNode<T>>();
                    branchQueue.Enqueue(currentNode);

                    while (branchQueue.Size > 0)
                    {
                        BinaryTreeNode<T> n = branchQueue.Dequeue();
                        branchSize += 1;

                        if (this.HasLeft(n))
                        {
                            branchQueue.Enqueue(n.LeftChild);
                        }
                        if (this.HasRight(n))
                        {
                            branchQueue.Enqueue(n.RightChild);
                        }
                    }

                    this.Count -= branchSize;
                    
                    // (c)

                    return;
                }

                // currentNode is not our node. We enqueue its children and move on.

                if (this.HasLeft(currentNode))
                    {
                        queue.Enqueue((currentNode, currentNode.LeftChild));
                    }
                if (this.HasRight(currentNode))
                    {
                        queue.Enqueue((currentNode, currentNode.RightChild));
                    }
            }
            

            // If node isn't found in the tree, we will throw an exception as usual:

            throw new InvalidOperationException("That node is not in the tree.");
            

        }
}

}