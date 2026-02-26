// ===== Role =====

// A low-level, internal mechanism to manage collections
// of GeneralTreeNode<T> instances in an n-ary tree structure.

// A GeneralTreeNode<T> is a dumb, low-level container that is wholly managed
// by GeneralTree<T>. This management is in large part structural: it
// concerns which nodes a GeneralTree<T> logically contains, 
// and their position in the tree. However, GeneralTree<T> is also able
// to mutate node Values.
// Still, a GeneralTreeNode<T> instance is fully initialised at creation,
// and its Value is non-nullable. 
// Each GeneralTreeNode<T> instance belongs to at most one GeneralTree<T>.
// Node insertion always generates a new GeneralTreeNode<T> instance. Removed
// nodes become unowned and cannot be added to another list.

// GeneralTree<T> is not intended for direct consumer use.
// Rather, it provides a basic tree structure and mechanism,
// allowing for more specialised trees to sit on top of it.

// GeneralTree<T> presents
// a purely general tree with no positional assumptions. It is an n-ary tree.
// In this repository, it is used as the foundation for the
// ComposedBinaryTree<T> class.



// ===== Invariants =====

// Root and Count:

// Root is a nullable GeneralTreeNode<T> value.
// Count is a non-negative integer.

// At any one time, a GeneralTree<T> instance is in exactly
// one of three states:

// (1) Empty: Root is null, and Count == 0.

// (2) Singleton: Root is non-null, Root.Children.Count == 0, 
// and Count == 1.

// (3) Multi-member: Root is non-null, Root.Children.Count > 0,
// and Count is equal to the number of nodes in the tree.


// Structure:

// With a GeneralTree<T>, nodes have an arbitrary number of children,
// and the tree has no fixed structural shape.

// Duplication: 

// A GeneralTree<T> is allowed to logically contain multiple GeneralTreeNode<T>
// instances with the same Value.
// Restrictions on duplication may be layered on by other specialised trees
// that are implemented with a composition-based implementation using
// a GeneralTree<T> as the background data structure.
// However, a GeneralTree<T> cannot contain the same GeneralTreeNode<T>
// instance in two separate locations.



// ===== Methods =====

// GeneralTree<T> contains a collection of node containment
// methods. Each of these has a GeneralTreeNode<T> parameter, and
// checks whether the argument passed into the method call is present
// in the GeneralTree<T>.
// I include multiple such methods for educational purposes. Each employs
// a different kind of tree traversal.
// They are all set to private as higher levels do not need access to them:


// ContainsBFS(): traverses the tree with an explicit appeal to my Queue<T> class.
// Time complexity O(n); space complexity O(w) (max width w).

// ContainsDFS(): traverses the tree with an explicit appeal to my Stack<T> class.
// Time complexity O(n); space complexity O(n).

// ContainsDFSRecursive(): appeals to a recursive helper method.
// Traverses the tree with implicit appeal to the call stack.
// Time complexity O(n); space complexity O(h) (tree height h).
// Risk of stack overflow.


// For simplicity, whenever an API method requires traversal through the 
// tree, I use BFS. I have demonstrated that other implementations, using
// other traversals, are available.


// Other internal methods:

// SetRoot(T value).
// Time complexity O(1).
// Throws an exception if a Root already exists.

// Clear().
// Time complexity O(1) as we just set Root to null and Count to 0.

// SwapValues(GeneralTreeNode<T> node1, GeneralTreeNode<T> node2).
// Time complexity O(1) as we assume the caller only passes nodes 
// into the parameters if they exist in the tree.
// This is fine as this internal method will only be called
// where this is guaranteed. Otherwise, it would be O(n).

// ContainsValue(T value).
// This is implemented with a BFS traversal.
// Other implementations are available using other traversals,
// but as I have already demonstrated a range of traversals, I have
// not included them here.
// Time complexity O(n).

// AddChild(GeneralTreeNode<T> parent, T value).
// Time complexity O(1) as we assume the caller only passes a node 
// into the parameter if they exist in the tree.
// This is fine as this internal method will only be called
// where this is guaranteed. Otherwise, it would be O(n).

// RemoveBranch(GeneralTreeNode<T> node).
// Traverses the tree looking for node. If it is present,
// node is removed from the tree. The size of the subtree with
// node as root is calculated (by traversing the subtree)
// and this size is subtracted from the Count
// of the GeneralTree<T>.
// Other implementations are available using other
// combinations of two traversals.
// Time complexity O(n), though we perform two O(n) traversals.



// ===== Design Discussion ===== 

// Consequences of no Parent/SubTreeSize References:

// Recall that GeneralTreeNode<T> does not have a Parent property.
// If it did, RemoveBranch() would be simpler:
// we could immediately access the Parent node of the
// root of our branch to be removed, then remove
// that root from the Parent node. Next, we would
// perform a single traversal of the removed branch to
// determine its size and therefore adjust
// the Count of the residual tree.

// Finally, regardless of whether a GeneralTreeNode<T> stores a 
// Parent reference, we could simplify the second
// part of RemoveBranch() (ie counting the subtree size) by storing
// a SubTreeSize reference in a GeneralTreeNode<T>. In which case,
// we would not need to traverse the subtree to determine its size.
// Like storing a Parent reference in a node, this is another example 
// of increased space complexity in our low-level container to
// reduce algorithmic complexity at a higher abstraction layer.

// With both Parent and SubTreeSize references, RemoveBranch() becomes O(1).


// Alternative Implementation:

// As my aim was to implement a purely general tree,
// a composition-based implementation using an array
// as the background data structure would not be elegant.
// For example, there is no straightforward index arithmetic we can
// employ as in binary trees.


// Separate Traversal Methods:

// I could have implemented a BFS() method
// that returns an IEnumerable<GeneralTreeNode<T>>. Then some tree 
// traversals in API methods could have alternatively been 
// implemented by iterating through this IEnumerable<GeneralTreeNode<T>>.
// However, there is no real benefit to this lazy traversal here,
// as traversal is always fully consumed or short-circuited by explicit logic
// in GeneralTree<T> methods.
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
// call GeneralTree<T> methods directly on higher-level abstractions.

// Root and Count have internal getters, so that certain higher-level
// abstractions can access these properties, but private setters as no other
// class should be able to directly mutate them.


// Error Handling:

// I could have added a ContainsBFS() check, or some other node containment check,
// to methods in this class that have a GeneralTreeNode<T> instance passed into
// a call, where this instance is supposed to be part of the tree. For example,
// AddChild(GeneralTreeNode<T> parent, T value). However, adding this check
// would make each such method O(n). 
// Instead, as all of the methods in this class are internal, I chose
// not to include such node containment checks, at the cost of having to manunally
// confirm that whenever a method call of this type is made, the context of the 
// call ensures the passed GeneralTreeNode<T> instance is in the tree.



using System;
using System.Collections.Generic; // Only to access EqualityComparer<T>.

namespace DataStructures
{

internal class GeneralTree<T>
{
    internal GeneralTreeNode<T>? Root {get; private set;}
    internal int Count {get; private set;}

    internal GeneralTree()
        {
            this.Root = null;
            this.Count = 0;
        }


    internal void Clear()
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
            GeneralTreeNode<T> newNode = new GeneralTreeNode<T>(value);
            this.Root = newNode;
            this.Count = 1;
        }

    internal void SwapValues(GeneralTreeNode<T> node1, GeneralTreeNode<T> node2)
        {
            T temp = node1.Value;
            node1.Value = node2.Value;
            node2.Value = temp;
        }

    private bool ContainsBFS(GeneralTreeNode<T> node)
        {
            // ContainsBFS() performs a BFS tree traversal.
            // Time complexity: O(n).
            // Space complexity: O(w), where w is max tree width.
            // I have implemented this with my own Queue<T> data structure
            // defined elsewhere in this repository.

            if (this.Root == null)
            {
                return false;
            }

            Queue<GeneralTreeNode<T>> queue = new DataStructures.Queue<GeneralTreeNode<T>>();
            queue.Enqueue(this.Root);

            while (queue.Size > 0)
            {
                GeneralTreeNode<T> currentTreeNode = queue.Dequeue();
                
                if (currentTreeNode == node)
                {
                    return true;
                }

                foreach (GeneralTreeNode<T> child in currentTreeNode.Children)
                {
                    queue.Enqueue(child);
                }
            }

            return false;
        }

    private bool ContainsDFS(GeneralTreeNode<T> node)
        {
            // ContainsDFS() performs a DFS tree traversal.
            // Time complexity: O(n).
            // Space complexity: O(n) [more on this in the next method].
            // I have implemented this with my own Stack<T> data structure
            // defined elsewhere in this repository.
            // This provides an interesting sister to ContainsBFS.

            if (this.Root == null)
            {
                return false;
            }

            Stack<GeneralTreeNode<T>> stack = new DataStructures.Stack<GeneralTreeNode<T>>();
            stack.Push(this.Root);

            while (stack.Size > 0)
            {
                GeneralTreeNode<T> currentTreeNode = stack.Pop();
                
                if (currentTreeNode == node)
                {
                    return true;
                }

                for (int i = currentTreeNode.Children.Count - 1; i >= 0; i--)
                {
                    stack.Push(currentTreeNode.Children[i]);
                }

                // With this approach to pushing to the stack,
                // for each parent, the first child is examined first.
                // That is, we have left-to-right examination.
                // Alternatively, we could have simply used:
                // foreach (GeneralTreeNode<T> child in currentTreeNode.Children)
                // {stack.Push(child);}
                // This would give us right-to-left examination.
                // It does not matter for my search algorithm, 
                // but I used the more complex push syntax to 
                // match the more intuitive order in which nodes
                // should be pushed.

            }

            return false;
        }


    private bool ContainsDFSRecursive(GeneralTreeNode<T> node)
        {
            // This is presented as an alternative to ContainsDFS().
            // The method uses a recursive helper method to achieve
            // a DFS of the tree.

            // Time complexity: O(n).

            // ContainsDFSRecursive() still uses a stack structure
            // to process nodes of the tree, as we employ recursion.
            // However, this stack (the call stack)
            // is managed implicitly in the background,
            // rather than explicitly, as with my ContainsDFS().

            // Although both DFS methods use stacks, they are employed
            // in slightly different ways.
            // In ContainsDFSRecursive(), the important elements of the stack
            // are calls to ContainsDFSHelper(), where each such call
            // is concerned with a different GeneralTreeNode<T> as currentNode.
            // Further, the order in which the GeneralTreeNode<T> objects 
            // of the tree are moved through this stack is different to
            // my ContainsDFS(). For example, if the root has three children
            // [A, B, C], then for ContainsDFS(), we add each of A, B, and C to
            // the stack immediately. In contrast, with ContainsDFSRecursive(),
            // calls to ContainsDFSHelper() with B or C as the currentNode
            // will only be added to the implicit stack once the whole of the
            // A branch has been examined.

            // TLDR: ContainsDFS() schedules immediately;
            // ContainsDFSRecursive() schedules later.

            // As a result, for ContainsDFSRecursive():
            // Space complexity: O(h), where h is the tree height.

            // However, the call stack has a fixed maximum size, so
            // there is a risk of stack overflow with ContainsDFSRecursive(),
            // but not with ContainsDFS().

            if (this.Root == null)
            {
                return false;
            }

            return ContainsDFSHelper(this.Root, node);           
        }

    private bool ContainsDFSHelper(GeneralTreeNode<T> currentNode, GeneralTreeNode<T> targetNode)
        {
            if (currentNode == targetNode)
            {
                return true;
            }

            foreach (GeneralTreeNode<T> child in currentNode.Children)
            {
                if (ContainsDFSHelper(child, targetNode))
                {
                    return true;
                }
            }

            // This produces left-to-right-examination of children.            

            return false;
        }

    internal bool ContainsValue(T value)
        {
            // I will use a simple BFS traversal here.
            // Other traversal methods are available.

            if (this.Root == null)
            {
                return false;
            }

            Queue<GeneralTreeNode<T>> queue = new DataStructures.Queue<GeneralTreeNode<T>>();
            queue.Enqueue(this.Root);

            while (queue.Size > 0)
            {
                GeneralTreeNode<T> currentTreeNode = queue.Dequeue();
                
                if (EqualityComparer<T>.Default.Equals(currentTreeNode.Value, value))
                {
                    return true;
                }

                foreach (GeneralTreeNode<T> child in currentTreeNode.Children)
                {
                    queue.Enqueue(child);
                }
            }

            return false;
        }

    internal void AddChild(GeneralTreeNode<T> parent, T value)
        {
            GeneralTreeNode<T> newNode = new GeneralTreeNode<T>(value);
            parent.Children.Add(newNode);
            this.Count += 1;
        }


    internal void RemoveBranch(GeneralTreeNode<T> node)
        {
            if (this.Root == null)
            {
                throw new InvalidOperationException("The tree is empty.");    
            }

            // As GeneralTreeNode<T> objects don't keep a reference to 
            // the Parent,
            // we will need to traverse the tree, keeping track of currentParent and
            // currentNode.
            // At each node, we check whether currentNode == node. If it does, we
            // remove currentNode from currentParent.Children. Next, we need to
            // determine how large the branch from currentNode is, to determine how
            // much to decrement the Count of the tree by.

            // As with my Contains methods, there are multiple ways
            // to traverse the tree, with their own pros/cons.
            // For simplicity, I will just implement a BFS.

            // Special case: this.Root, which has no parent, so we handle it separately:

            if (this.Root == node)
            {
                this.Clear();
                return;
            }

            Queue<(GeneralTreeNode<T>, GeneralTreeNode<T>)> queue = new DataStructures.Queue<(GeneralTreeNode<T>, GeneralTreeNode<T>)>();
            
            foreach (GeneralTreeNode<T> child in this.Root.Children)
            {
                queue.Enqueue((this.Root, child));
            }


            while (queue.Size > 0)
            {
                (GeneralTreeNode<T> currentParent, GeneralTreeNode<T> currentNode) = queue.Dequeue();
                
                if (currentNode == node)
                {
                    // currentNode is our node!
                    // Now we need to:
                    // (a) remove it from currentParent.Children;
                    // (b) compute the size of the branch
                    // to adjust the Count of the tree down; and
                    // (c) return.

                    // (a)
                    currentParent.Children.Remove(currentNode);

                    // (b) In order to know how much to subtract from Count,
                    // we need to know the size of the branch from currentNode.
                    // This requires a traversal of the branch.
                    // Let's use BFS again:

                    int branchSize = 0;
                    Queue<GeneralTreeNode<T>> branchQueue = new DataStructures.Queue<GeneralTreeNode<T>>();
                    branchQueue.Enqueue(currentNode);

                    while (branchQueue.Size > 0)
                    {
                        GeneralTreeNode<T> n = branchQueue.Dequeue();
                        branchSize += 1;

                        foreach (GeneralTreeNode<T> child in n.Children)
                        {
                            branchQueue.Enqueue(child);
                        }
                    }

                    this.Count -= branchSize;

                    // (c)
                    
                    return;
                }

                // currentNode is not our node. We enqueue its children and move on.

                foreach (GeneralTreeNode<T> child in currentNode.Children)
                {
                    queue.Enqueue((currentNode, child));
                }
            }
            

            // If node isn't found in the tree, we will throw an exception:

            throw new InvalidOperationException("That node is not in the tree.");
            

        }

    

}

}