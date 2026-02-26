Throughout this repository, to compare the equality of two T objects x and y, I will use EqualityComparer<T>.Default.Equals(x, y), rather than x == y. The issue with == is that it is ambiguous: it may not exist for a certain type T; it may mean reference identity (eg object); it may mean value equality (eg int and string); or it may be defined to mean something entirely different to equality (as its meaning is not enforced by any interface). The meaning of a == instance is resolved at compile time, and any code containing it will only execute if the compiler is able to determine how "==" is defined for the relevant type T. 

Using EqualityComparer<T>.Default.Equals() is the .NET convention in generic data structure classes where equality is required and T is unconstrained.

Under the hood:

EqualityComparer<T> is a generic abstract class containing the static Default property. Default's return value is an instance of a subclass of EqualityComparer<T> (the runtime chooses the best subclass depending on how equality for T is to be handled). So EqualityComparer<T>.Default.Equals() calls the Equals() method on this instance.

Exceptions: 

I will use == in situations where I am explicitly interested in reference identity. For example, comparing whether two node variables point to exactly the same node instance.