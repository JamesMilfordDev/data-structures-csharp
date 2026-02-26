// ===== Role =====

// On tree structures, I have so far implemented:

// GeneralTreeNode<T>.
// GeneralTree<T>.
// ComposedBinary<T>.

// GeneralTreeNode<T> is a minimal, low-level container.
// This class is not intended for direct consumer use;
// it exists only to support GeneralTree<T>. GeneralTreeNode<T> 
// contains no methods. Instances are entirely
// managed by GeneralTree<T>.

// GeneralTree<T> is a low-level, internal mechanism
// to manage collections
// of GeneralTreeNode<T> instances in a purely general
// tree structure with no positional assumptions.

// ComposedBinaryTree<T> sits on top of GeneralTree<T> as a mid-level
// abstraction that layers structural policy on top of mechanism. The
// class is not consumer-facing, but provides a foundation for
// specialised trees to sit on.

// GeneralTree<T> is the mechanism.
// ComposedBinaryTree<T> is the structural policy.

// More specialised classes may be layered on ComposedBinaryTree<T>,
// including consumer-facing ones that are concerned with T object management.

// However, ComposedBinaryTree<T> imposes a structural policy that
// is too strict for the class to represent a truly general
// binary tree.
// In particular, the structural constraints are:
// each node has at most two children (a left child and right child).
// Further, a right child can be inserted on a node only if that node
// has a left child.

// Again, this is a consequence of layering ComposedBinaryTree<T> on
// GeneralTree, which employs the GeneralTreeNode<T> class.
// Here, a node stores its children in a simple List object in insertion order and
// does not take null values. Further, the Value property of a GeneralTreeNode<T>
// is not nullable. As such, to represent a right child, a left child must exist.

// As such, I now alternatively build a truly general binary tree structure
// on which certain consumer-facing specialised binary trees will sit 
// in this repository. This begins with BinaryTreeNode<T>.

// As with GeneralTreeNode<T>, BinaryTreeNode<T> is a minimal, low-level container.
// This class is not intended for direct consumer use;
// it exists only to support BinaryTree<T>.

// BinaryTreeNode<T> contains no methods. Instances are entirely
// managed by BinaryTree<T> in a binary tree fashion. 
// A large part of this management is structural: 
// it concerns which nodes a BinaryTree<T> 
// logically contains, and their position in the tree.
// However, like with GeneralTreeNode<T> and GeneralTree<T>, and
// unlike with SimpleNode<T> and SinglyLinkedList<T>, and
// unlike with ComplexNode<T> and DoublyLinkedList<T>,
// BinaryTree<T> is also able to mutate BinaryTreeNode<T> Values.



// ===== Invariants =====

// Each BinaryTreeNode<T> instance belongs to at most one BinaryTree<T>.
// Node insertion always generates a new BinaryTreeNode<T> instance.
// Removed nodes become unowned and cannot be added to another tree.

// The Value property is internally mutable but non-nullable (T not T?).

// LeftChild and RightChild are nullable BinaryTreeNode<T> references.

// Invariants concerning node ownership and management are enforced by BinaryTree<T>.



// ===== Design Discussion ===== 


// Value Mutability and Nullability:

// The internal mutability of Value is required for BinaryTree<T> to
// provide a full range of mechanisms.

// As with SimpleNode<T>, ComplexNode<T>, and GeneralTreeNode<T>,
// Value is non-nullable.
// Allowing a nullable Value provides no representational power: a
// BinaryTree<T> can be fully represented without it. Further, 
// allowing a nullable Value is actively detrimental: our BinaryTree<T>
// method logic would have to account for nullability, adding noise.


// Generic vs Non-Generic:

// Again, rather than creating a generic class, an alternative would be
// to create a single BinaryTreeNode class and let Value be of the Object type.
// This approach was rejected to preserve compile-time type safety.


// Access Modifiers:

// The class and its properties have the internal access modifier,
// as the class is not for consumer use (public would be inappropriate),
// but is entirely managed by BinaryTree<T> (private would be inappropriate).
// The modifier protected is also inappropriate as nothing inherits from the class.

// An internal setter for Value allows for the BinaryTree<T> to 
// include a method that mutates node Values. This is required,
// as we will need to swap node Values.


// Keeping Nodes Dumb:

// As with GeneralTreeNode<T>,
// there are two ways in which my BinaryTreeNode<T> is dumb:
// (1) The node contains a minimal amount of references.
// (2) The node contains no methods; it is entirely managed by
// BinaryTree<T>.

// The discussion mirrors that on GeneralTreeNode<T>:

// On (1), BinaryTreeNode<T> node does *not* contain a Parent reference.
// Such references are not required for correctness:
// a tree can be represented, and many tree operations
// can be executed, using only child references.
// Further, including a Parent reference increases space complexity.

// However, the benefit is a significant reduction in
// the complexity of many methods in tree classes
// implemented using these node classes
// (such as upward traversal)
// both in terms of time complexity and the logic of the methods.
// In production libraries, the inclusion of a Parent reference
// is usually worth it. Here, I do not include them to
// demonstrate my ability to implement the more complex methods.

// The design choice of whether to include a 
// Parent reference or not is a matter of
// where to include complexity:
// additional structural information being stored to
// enable logically simpler and more efficient methods at
// higher abstraction layers.

// Another reference I could have added was a SubTreeSize reference.
// This is another example 
// of increased space complexity in our low-level container to
// reduce algorithmic complexity at a higher abstraction layer
// (for example, we would not need to calculate subtree sizes on the fly
// with a traversal).

// On (2), this decision was made to maintain symmetry with my approach
// with the linear data structures. The relationship between
// BinaryTreeNode<T> and BinaryTree<T> 
// now closely mirrors the 
// relationships between GeneralTreeNode<T> and GeneralTree<T>, 
// between SimpleNode<T> and SinglyLinkedList<T>, and
// between ComplexNode<T> and DoublyLinkedList<T>. In each case,
// we have a dumb, low-level container that is wholly managed
// by another class. This other class is still a low-level
// internal mechanism to manage the structure of a collection
// of the dumb, low-level container nodes. The other class is not 
// intended for direct consumer use, but rather provides a mechanism
// on which higher-level, consumer-facing classes are built.

// SimpleNode<T>, ComplexNode<T>, GeneralTreeNode<T>, and now
// BinaryTreeNode<T> all merely
// store a Value and pointers to other nodes of the same kind, though
// with pointers of different arities and directionality.

// Again, in practice, tree nodes will often combine additional
// references, such as a Parent and SubTreeSize, with some self-managing
// methods. For example, we might include AddLeft(T value) which
// adjusts both the LeftChild reference and the SubTreeSize reference.
// This approach again increases the complexity of the foundational node
// for the benefit of reduced complexity at a higher abstraction layer.


using System;

namespace DataStructures
{
internal class BinaryTreeNode<T>
{
    internal T Value {get; set;}
    internal BinaryTreeNode<T>? LeftChild {get; set;}
    internal BinaryTreeNode<T>? RightChild {get; set;}

    internal BinaryTreeNode(T value)
        {
            this.Value = value;
            this.LeftChild = null;
            this.RightChild = null;
        }
}

}