package PolynomialCalculator;

public interface Scalar {

    Scalar add(Scalar s); /*accepts a scalar argument and returns a new scalar which is the sum of
    the current scalar and the argument.*/

    Scalar mul(Scalar s); /*accepts a scalar argument and returns a new scalar which is the multiplication
    of the current scalar and the argument. */

    Scalar neg(); /*returns a new scalar which is the multiplication of the current scalar with (-1).*/

    Scalar power(int exponent); /*accepts a scalar argument and returns a new scalar which is the
    scalar raised to the power of the exponent argument.*/

    int [] get ();

    int sign(); /*returns 1 for positive scalar, -1 for negative and 0 for 0.*/

    boolean equals (Scalar s);


}
