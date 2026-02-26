// ===== Role =====

// A minimal, low-level container.
// This class is not intended for direct consumer use;
// it exists only to support GeneralTree<T>.

// GeneralTreeNode<T> contains no methods. Instances are entirely
// managed by GeneralTree<T> in a tree fashion. 
// A large part of this management is structural: 
// it concerns which nodes a GeneralTree<T> 
// logically contains, and their position in the tree.
// However, unlike with SimpleNode<T> and SinglyLinkedList<T>, and
// unlike with ComplexNode<T> and DoublyLinkedList<T>,
// GeneralTree<T> is able to mutate GeneralTreeNode<T> Values.

// Each node supports an arbitrary number of children: it is an n-ary
// tree node.



// ===== Invariants =====

// Each GeneralTreeNode<T> instance belongs to at most one GeneralTree<T>.
// Node insertion always generates a new GeneralTreeNode<T> instance.
// Removed nodes become unowned and cannot be added to another tree.

// The Value property is internally mutable but non-nullable (T not T?).

// Children is a List<GeneralTreeNode<T>>. The list itself is internally
// mutable, but the Children reference cannot be reassigned to a different list.
// Modification of the List<GeneralTreeNode<T>> only occurs through
// appropriate methods of higher-level internal mechanism classes.

// Invariants concerning node ownership and management are enforced by GeneralTree<T>.



// ===== Design Discussion =====

// Value Mutability and Nullability:

// The internal mutability of Value is required for GeneralTree<T> to
// provide a full range of mechanisms.

// As with SimpleNode<T> and ComplexNode<T>, Value is non-nullable.
// Allowing a nullable Value provides no representational power: a
// GeneralTree<T> can be fully represented without it. Further, 
// allowing a nullable Value is actively detrimental: our GeneralTree<T>
// method logic would have to account for nullability, adding noise.


// Generic vs Non-Generic:

// Again, rather than creating a generic class, an alternative would be
// to create a single GeneralTreeNode class and let Value be of the Object type.
// This approach was rejected to preserve compile-time type safety.


// Access Modifiers:

// The class and its properties have the internal access modifier,
// as the class is not for consumer use (public would be inappropriate),
// but is entirely managed by GeneralTree<T> (private would be inappropriate).
// The modifier protected is also inappropriate as nothing inherits from the class.


// Keeping Nodes Dumb:

// There are two ways in which my GeneralTreeNode<T> is dumb:
// (1) The node contains a minimal amount of references.
// (2) The node contains no methods; it is entirely managed by
// GeneralTree<T>.

// On (1), GeneralTreeNode<T> node does *not* contain a Parent reference.
// Such references are not required for correctness:
// a tree can be represented, and many tree operations
// can be executed, using only child references.
// Further, including a Parent reference increases space complexity.

// However, the benefit of a Parent reference is a significant reduction in
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
// GeneralTreeNode<T> and GeneralTree<T> now closely mirrors the 
// relationships between SimpleNode<T> and SinglyLinkedList<T>, and
// between ComplexNode<T> and DoublyLinkedList<T>. In each case,
// we have a dumb, low-level container that is wholly managed
// by another class. This other class is still a low-level
// internal mechanism to manage the structure of a collection
// of the dumb, low-level container nodes. The other class is not 
// intended for direct consumer use, but rather provides a mechanism
// on which higher-level, consumer-facing classes are built.

// SimpleNode<T>, ComplexNode<T>, and GeneralTreeNode<T> all merely
// store a Value and pointers to other nodes of the same kind, differing
// only in arity and directionality.

// Again, in practice, tree nodes will often combine additional
// references, such as a Parent and SubTreeSize, with some self-managing
// methods. For example, we might include AddChild(T value) which
// adjusts both the Children reference and the SubTreeSize reference.
// This approach again increases the complexity of the foundational node
// for the benefit of reduced complexity at a higher abstraction layer.


using System;
using System.Collections.Generic;

namespace DataStructures
{
internal class GeneralTreeNode<T>
{
    internal T Value {get; set;}
    internal List<GeneralTreeNode<T>> Children {get; set;}

    internal GeneralTreeNode(T value)
        {
            this.Value = value;
            this.Children = new List<GeneralTreeNode<T>>();
        }
}
}