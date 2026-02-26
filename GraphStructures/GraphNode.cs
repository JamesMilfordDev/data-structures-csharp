// ===== Role =====

// A minimal, low-level container.
// This class is not intended for direct consumer use;
// it exists only to support Graph<T>.

// GraphNode<T> contains no methods. Instances are entirely
// managed by Graph<T>.
// A large part of this management is structural: 
// it concerns which nodes a Graph<T> 
// logically contains, and the connections between those nodes
// (the topology of the graph).
// However, unlike with SimpleNode<T> and SinglyLinkedList<T>, and
// unlike with ComplexNode<T> and DoublyLinkedList<T>, but like
// with GeneralTreeNode<T> and GeneralTree<T>, and like with BinaryTreeNode<T>
// and BinaryTree<T>, Graph<T> is able to mutate the Value of a GraphNode<T>.


// Representing Graphs:

// Graph<T> is node-centric:
// Graph<T> represents a Graph by (i) storing a collection of GraphNode<T> objects
// in a simple HashSet<GraphNode<T>>, where (ii) a GraphNode<T> stores a collection
// of edges as a Dictionary<GraphNode<T>, int> object.

// This approach to representing Graphs supports
// both directed and undirected, and weighted and
// unweighted, graphs using only the Graph<T> class:

// Graph<T> will contain the bool
// properties IsWeighted and IsDirected. 
// It will contain two separate edge adding methods: 
// AddWeightedEdge(GraphNode<T> node1, GraphNode<T> node2, int weight); and 
// AddUnweightedEdge(GraphNode<T> node1, GraphNode<T> node2).

// AddWeightedEdge() will throw an exception if the graph is not weighted.
// AddUnweightedEdge() will throw an exception if the graph is weighted.
// For each method, when an exception is not thrown, the method additionally checks
// whether the graph is directed. If the graph is directed, only node1.Neighbours
// has an entry added.
// If the graph is not directed, both node1.Neighbours and node2.Neighbours have
// an entry added.
// In the case of a non-directed graph, I still classify this as only adding one
// edge in the graph (despite adjusting each of the nodes), in terms of edge count.
// In AddUnweightedEdge(), new edge entries are assigned a weight of 1.

// For generality, Graph<T> also allows for graphs with non-positive edge weights.
// A PositiveEdgesOnly bool is set during Graph<T> instance creation.
// Calling certain algorithms on a Graph<T> instance with a false PositiveEdgesOnly
// will throw an exception. For example, Dijkstra-based algorithms.
// A Graph<T> instance cannot be created with false values for both
// IsWeighted and PositiveEdgesOnly.


// ===== Invariants =====

// Each GraphNode<T> instance belongs to at most one Graph<T>.
// Node insertion always generates a new GraphNode<T> instance.
// Removed nodes will become unowned and cannot be added to another Graph.
// This ownership is enforced by Graph<T>.

// Value is non-nullable, but internally mutable.

// The Neighbours Dictionary instance is itself internally mutable, but the 
// the reference cannot be reassigned to a different Dictionary instance.



// ===== Design Discussion =====

// Value Mutability and Nullability:

// The internal mutability of Value is not required for the standard
// mechanisms of Graph<T>. In Graph<T>, the Value of a node is less important
// than the node itself and the connections between nodes. The Value is 
// better considered a piece of metadata. Given this, one could implement 
// Graph<T> with an immutable GraphNode<T> Value. 

// As with my other node implementaions, Value is non-nullable.
// Allowing a nullable Value provides no representational power: a
// Graph<T> can be fully represented without it. Further, 
// allowing a nullable Value actively complicates the Graph<T>.

// Generic vs Non-Generic:

// Again, rather than creating a generic class, an alternative would be
// to create a single GeneralTreeNode class and let Value be of the Object type.
// This approach was rejected to preserve compile-time type safety.


// Access Modifiers:

// The class' Value getter and setter, and its Neighbours getter,
// have the internal access modifier,
// as the class is not for consumer use (public would be inappropriate),
// but is entirely managed by Graph<T> (private would be inappropriate).
// The modifier protected is also inappropriate as nothing inherits from the class.

// The Neighbours property has no setter, as although we want Graph<T>
// to be able to mutate the Dictionary instance associated with a GraphNode<T>
// instance, we don't want the Neighbours reference to be mutated.

// One important note: the class GraphNode<T> must itself be public, as
// certain public methods in Graph<T> have a GraphNode<T> return type. See Graph.cs.


// Keeping Nodes Dumb:

// Again, there are two ways in which my GraphNode<T> is dumb:
// (1) The node contains a minimal amount of references.
// (2) The node contains no methods; it is entirely managed by
// Graph<T>.

// This design choice was made to maintain symmetry with my approach
// with linear structures and tree structures. The relationship between
// GraphNode<T> and Graph<T> now closely mirrors the relationships
// between SimpleNode<T> and SinglyLinkedList<T>,
// between ComplexNode<T> and DoublyLinkedList<T>,
// between GeneralTreeNode<T> and GeneralTree<T>,
// and between BinaryTreeNode<T> and BinaryTree<T>.

// In each case,
// we have a dumb, low-level container that is wholly managed
// by another class.

// SimpleNode<T>, ComplexNode<T>, GeneralTreeNode<T>.
// BinaryTreeNode<T>, and GraphNode<T> all merely
// store a Value and pointers to other nodes of the same kind
// (though the pointers differ in arity and directionality).


// Node-centric vs Edge-centric:

// Again, Graph<T> represents a Graph by storing 
// a collection of GraphNode<T> objects.
// Call this approach Option 1. It is node-centric.

// An alternative approach to representing a Graph is to keep a GraphNode<T> as
// merely a container for a Value reference. Then we have two options:

// Option 2: Graph<T> stores an Edges property whose value is
// a data structure that holds (GraphNode<T>, GraphNode<T>, int) tuples, 
// representing edges.

// Option 3: We have a separate GraphEdge<T> class that stores the properties:
// StartNode, EndNode, Weight. Then Graph<T> stores a 
// collection of GraphEdge<T> instances.

// For Options 2 and 3, Graph<T> now stores a collection of 
// (GraphNode<T>, GraphNode<T>, int) tuples.
// This is the edge-centric approach.

// However, with Option 1, specifying a collection of
// nodes is sufficient to specify the graph. This approach is fully general.
// Any kind of topology can be specified.
// With Options 2 and 3, we can specify most kinds of graph by
// specifying a collection of (GraphNode<T>, GraphNode<T>, int) tuples, but
// there is one important exception: 
// we cannot represent a graph with an isolated node.

// So with Option 1, a Graph can be implemented without an explicit Edges
// property. With Options 2 and 3, a Graph can only be implemented
// with both explicit Nodes and Edges properties.

// However, even with Option 1, there is a case for including 
// an Edges property: some Graph algorithms require a list of edges.
// This can be computed ad hoc if we only store a collection of nodes,
// by accessing the Neighbours property of each GraphNode<T> instance,
// but the implementation of the algorithm that requires this 
// ad hoc computation will have an increased time complexity.

// I have chosen Option 1 (to include the Neighbours property in a node),
// to provide symmetry with my other node classes. They all now store 
// a Value reference and pointers to other nodes of the same kind
// (although those pointers are of various arities and directionalities). 
// Further, I will not add an Edges property in Graph<T> as this
// costs extra memory without real reward for my implementation:
// the algorithms I will implement in Graph<T> employ node-centric 
// representation.
// The Graph<T> class can be straightforwardly extended to include an
// Edges property if edge-centric algorithms need to be implemented.


using System;
using System.Collections.Generic;

namespace DataStructures
{
public class GraphNode<T>
{
    internal T Value {get; set;}
    internal Dictionary<GraphNode<T>, int> Neighbours {get;}

    internal GraphNode(T value)
        {
            this.Value = value;
            this.Neighbours = new Dictionary<GraphNode<T>, int>();
        }
}
}