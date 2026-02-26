// ===== Role =====

// Recall that GeneralTree<T> is a low-level, internal mechanism
// to manage collections of GeneralTreeNode<T> instances in a purely general
// tree structure with no positional assumptions.

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
// Rather, it provides a basic n-ary tree structure and mechanism,
// allowing for more specialised trees to sit on top of it.

// ComposedBinaryTree<T> sits on top of GeneralTree<T> as a mid-level
// abstraction that layers structural policy on top of mechanism. The
// class is not consumer-facing, but provides a foundation for
// specialised trees to sit on.

// GeneralTree<T> is the mechanism.
// ComposedBinaryTree<T> is the structural policy.

// While some layers in this class hierarchy may appear thin, they provide
// clear architectural boundaries.

// This class does *not* impose a node ordering policy according to Value. 
// That is the domain of more specialised binary trees that sit on
// top of it.

// In particular, the structural constraints are:
// each node has at most two children (a left child and right child).
// Further, a right child can be inserted on a node only if that node
// has a left child.

// Note that this last condition is stricter than a truly general binary tree.
// It is a consequence of layering ComposedBinaryTree<T> on
// GeneralTree, which employs the GeneralTreeNode<T> class.
// Here, a node stores its children in a simple List object in insertion order and
// does not take null values. Further, the Value property of a GeneralTreeNode<T>
// is not nullable. As such, to represent a right child, a left child must exist.

// For this reason, this class is an educational endpoint, and is not used
// as an underlying data structure in other composition-based implementations
// in this repository.
// My consumer-facing specialised trees are
// alternatively built on a node-based binary tree implementation, BinaryTree<T>.
// However, one could, for example, straightforwardly layer a minheap implementation
// on ComposedBinaryTree<T>, as the structural policy of a minheap is 
// consistent with the structural policy of ComposedBinaryTree<T>.

// On its API, ComposedBinaryTree<T> provides the mechanism of GeneralTree<T> by
// implementing methods that call relevant GeneralTree<T> methods
// on the GeneralTree<T> Tree reference the ComposedBinaryTree<T> stores.
// The only difference is that it separates the AddChild() of GeneralTree<T>
// into AddLeft() and AddRight(), though each method still calls
// AddChild() on the private GeneralTree<T> Tree reference the 
// ComposedBinaryTree<T> stores.



// ===== Invariants =====

// Children:

// Recall that the GeneralTree<T> class employs the GeneralTreeNode<T> class, which
// stores its children as a simple List object. As such:
// for any node n, n.Children[0] is the left child (if it exists),
// and n.Children[1] is the right child (if it exists).

// Structure:

// Each node has at most two children (a left child and right child).
// Further, a right child can be inserted on a node only if that node
// has a left child.



// ===== Methods =====

// SetRoot(T value).
// Time complexity O(1) as we call the GeneralTree<T> SetRoot(),
// which is O(1).

// Clear().
// Time complexity O(1) as we call the GeneralTree<T> Clear(),
// which is O(1).

// SwapValues(GeneralTreeNode<T> node1, GeneralTreeNode<T> node2).
// Time complexity O(1) as we call the GeneralTree<T> SwapValues(),
// which is O(1).

// ContainsValue(T value).
// Time complexity O(n) as we call the GeneralTree<T> ContainsValue(),
// which is O(n).

// RemoveBranch(GeneralTreeNode<T> node).
// Time complexity O(n) as well call the GeneralTree<T> RemoveBranch(),
// which is O(n) as a GeneralTreeNode<T> does not store both Parent and
// SubTreeSize references.

// AddLeft(GeneralTreeNode<T> parent, T value)
// Time complexity O(1) as we assume the node passed into a method
// call exists in the tree. Otherwise, it would be O(n).
// Throws an exception if a left child already exists.

// AddRight(GeneralTreeNode<T> parent, T value)
// Time complexity O(1) as we assume the node passed into a method
// call exists in the tree. Otherwise, it would be O(n).
// Throws an exception if a right child already exists.

// We additionally add some helper methods in the API. These are all
// time complexity O(1) as we assume the node passed into a method
// call exists in the tree. Otherwise, they would be O(n).

// HasLeft(GeneralTreeNode<T> node).
// HasRight(GeneralTreeNode<T> node).

// GetLeft(GeneralTreeNode<T> node).
// Throws an exception if a left child does not exist.

// GetRight(GeneralTreeNode<T> node).
// Throws an exception if a right child does not exist.


// ===== Design Discussion =====

// Properties and Access Modifiers:

// The class and its API methods have the internal access modifier,
// as the class is not for consumer use (public would be inappropriate),
// but is accessed by other consumer-facing classes (private would be inappropriate).
// The modifier protected is also inappropriate as nothing inherits from the class.
// An inheritance-based implementation of higher-level classes would
// be inappropriate, as this would allow consumers to inappropriately
// call ComposedBinaryTree<T> methods directly on higher-level abstractions.

// Tree has a private getter and no setter as it should only be directly accessed
// here and should not be reference mutable here.

// Count has no private backing field. It only has an internal getter which accesses
// this.Tree.Count.




using System;

namespace DataStructures
{

internal class ComposedBinaryTree<T>
{
    private GeneralTree<T> Tree {get;}
    internal int Count
        {
            get {return this.Tree.Count;}
        }
        
    internal ComposedBinaryTree()
        {
            this.Tree = new GeneralTree<T>();
        }

    internal void SetRoot(T value)
        {
            this.Tree.SetRoot(value);
        }

    internal void Clear()
        {
            this.Tree.Clear();
        }
        
    internal void SwapValues(GeneralTreeNode<T> node1, GeneralTreeNode<T> node2)
        {
            this.Tree.SwapValues(node1, node2);
        }
    
    internal bool ContainsValue(T value)
        {
            return this.Tree.ContainsValue(value);
        }

    internal void RemoveBranch(GeneralTreeNode<T> node)
        {
            this.Tree.RemoveBranch(node);
        }


    internal void AddLeft(GeneralTreeNode<T> parent, T value)
        {
            if (parent.Children.Count > 0)
            {
                throw new InvalidOperationException("That node already has a left child.");
            }
            this.Tree.AddChild(parent, value);
        }

    internal void AddRight(GeneralTreeNode<T> parent, T value)
        {
             if (parent.Children.Count >= 2)
            {
                throw new InvalidOperationException("That node already has a right child.");
            }

            if (parent.Children.Count == 0)
            {
                throw new InvalidOperationException("That node has no left child yet.");
            }

            this.Tree.AddChild(parent, value);
        }

    internal GeneralTreeNode<T> GetLeft(GeneralTreeNode<T> parent)
        {
            if (parent.Children.Count == 0)
            {
                throw new InvalidOperationException("That node has no left child.");
            }

            return parent.Children[0];
        }
    
    internal GeneralTreeNode<T> GetRight(GeneralTreeNode<T> parent)
        {
            if (parent.Children.Count <= 1)
            {
                throw new InvalidOperationException("That node has no right child.");
            }

            return parent.Children[1];
        }

    internal bool HasLeft(GeneralTreeNode<T> parent)
        {
            return parent.Children.Count > 0;
        }
    
    internal bool HasRight(GeneralTreeNode<T> parent)
        {
            return parent.Children.Count > 1;
        }
}

}