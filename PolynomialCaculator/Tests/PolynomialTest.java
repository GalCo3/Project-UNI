package Tests;

import PolynomialCalculator.Integer;
import PolynomialCalculator.Monomial;
import PolynomialCalculator.Polynomial;
import PolynomialCalculator.Rational;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

public class PolynomialTest {

    private Polynomial a;
    private Polynomial b;
    private Polynomial c;
    private Monomial[] d;

    @Before
    public void initTest(){
        a = null;
        b = null;
        c = null;
        d = null;
    }

    @Test
    public void testBuild1(){
        a = Polynomial.build("1 0 0 2");
        d = new Monomial[]
                {new Monomial(0,new Integer(1)), new Monomial(1,new Integer(0)), new Monomial(2,new Integer(0)),new Monomial(3,new Integer(2))};
        Polynomial te = new Polynomial(d);
        Assert.assertTrue(a.equals(te));
    }
    @Test
    public void testBuild2(){
        a = Polynomial.build("0 0 0 0 0 4");
        d = new Monomial[]
                {new Monomial(0,new Integer(0)),new Monomial(1,new Integer(0)),new Monomial(2,new Integer(0)),new Monomial(3,new Integer(0)),new Monomial(4,new Integer(0)),new Monomial(5,new Integer(4))};
        Polynomial te = new Polynomial(d);
        Assert.assertTrue(a.equals(te));
    }
    @Test
    public void testBuild3(){
        a = Polynomial.build("1/2 -1/2 1/4 -2");
        d = new Monomial[]
                {new Monomial(0,new Rational(1,2)),new Monomial(1,new Rational(-1,2)),new Monomial(2,new Rational(1,4)),new Monomial(3,new Integer(-2))};
        Polynomial te = new Polynomial(d);
        Assert.assertTrue(a.equals(te));
    }
    @Test
    public void testBuild4(){
        a = Polynomial.build("0 0 0 0 0 0 0 0 0 0 100");
        d = new Monomial[]
                {new Monomial(0,new Integer(0)),new Monomial(1,new Integer(0)),new Monomial(2,new Integer(0)),new Monomial(3,new Integer(0)),new Monomial(4,new Integer(0)),new Monomial(5,new Integer(0)),new Monomial(6,new Integer(0)),new Monomial(7,new Integer(0)),new Monomial(8,new Integer(0)),new Monomial(9,new Integer(0)),new Monomial(10,new Integer(100))};
        Polynomial te = new Polynomial(d);
        Assert.assertTrue(a.equals(te));
    }
    @Test
    public void testBuild5(){
        a = Polynomial.build("1 0      0       1");
        d = new Monomial[]
                {new Monomial(0,new Integer(1)),new Monomial(1,new Integer(0)),new Monomial(2,new Integer(0)),new Monomial(3,new Integer(1))};
        Polynomial te = new Polynomial(d);
        Assert.assertTrue(a.equals(te));
    }

    @Test
    public void testAdd1(){
        a = Polynomial.build("1 0 0 2");
        a = a.add(Polynomial.build("2 0 0 4"));

        b = Polynomial.build("3 0 0 6");
        Assert.assertTrue(a.equals(b));
    }
    @Test
    public void testAdd2(){
        a = Polynomial.build("1 0 0 2");
        a = a.add(Polynomial.build("-3 0 0 -1"));
        b = Polynomial.build("-2 0 0 1");
        Assert.assertTrue(a.equals(b));
    }
    @Test
    public void testAdd3(){
        a = Polynomial.build("1/2 0 4");
        a = a.add(Polynomial.build("2 0 -1/2"));
        b = Polynomial.build("5/2 0 7/2");
        Assert.assertTrue(a.equals(b));
    }
    @Test
    public void testAdd4(){
        a = Polynomial.build("1 0 0 2");
        a = a.add(Polynomial.build("0 0 2 -1"));
        b = Polynomial.build("1 0 2 1");
        Assert.assertTrue(a.equals(b));
    }

    @Test
    public void testMul1(){
        a = Polynomial.build("4 2 1/3 3");
        b = Polynomial.build("2");
        a = a.mul(b);
        c = Polynomial.build("8 4 2/3 6");
        Assert.assertTrue(a.equals(c));
    }
    @Test
    public void testMul2(){
        a = Polynomial.build("5 4 1");
        b = Polynomial.build("1 2 3");
        a = a.mul(b);
        c = Polynomial.build("5 14 24 14 3");
        Assert.assertTrue(a.equals(c));
    }
    @Test
    public void testMul3(){
        a = Polynomial.build("4 2 1/3 -3");
        b = Polynomial.build("1/2 -1 3 -1/3");
        a = a.mul(b);
        c = Polynomial.build("2 -3 61/6 17/6 10/3 -82/9 1");
        Assert.assertTrue(a.equals(c));
    }
    @Test
    public void testMul4(){
        a = Polynomial.build("4 2 1/3 3");
        b = Polynomial.build("0");
        a = a.mul(b);
        c = Polynomial.build("0 0 0 0");
        Assert.assertTrue(a.equals(c));
    }
    @Test
    public void testMul5(){
        a = Polynomial.build("4 2 1/3 3");
        b = Polynomial.build("1");
        a = a.mul(b);
        c = Polynomial.build("4 2 1/3 3");
        Assert.assertTrue(a.equals(c));
    }

    @Test
    public void testEvaluate1(){
        a = Polynomial.build("3 2 1 0 4");
        Assert.assertTrue(a.evaluate(new Integer(2)).equals(new Integer(75)));
    }
    @Test
    public void testEvaluate2(){
        a = Polynomial.build("3 2 1 0 4");
        Assert.assertTrue(a.evaluate(new Rational(1,2)).equals(new Rational(9,2)));
    }
    @Test
    public void testEvaluate3(){
        a = Polynomial.build("3 2 1 0 4");
        Assert.assertTrue(a.evaluate(new Integer(-2)).equals(new Integer(67)));
    }
    @Test
    public void testEvaluate4(){
        a = Polynomial.build("1 1/4 1 -1 -1/2");
        Assert.assertTrue(a.evaluate(new Rational(-1,2)).equals(new Rational(39,32)));
    }


    @Test
    public void testDerivative1(){
        a = Polynomial.build("3");
        a = a.derivative();
        Assert.assertTrue(a.equals(Polynomial.build("0")));
    }
    @Test
    public void testDerivative2(){
        a = Polynomial.build("3 2 1 0 4");
        a = a.derivative();
        b = Polynomial.build("2 2 0 16");
        Assert.assertTrue(a.equals(b));
    }
    @Test
    public void testDerivative3(){
        a = Polynomial.build("0 1 -1 1/2 -1/4");
        a = a.derivative();
        b = Polynomial.build("1 -2 3/2 -1");
        Assert.assertTrue(a.equals(b));
    }


    @Test
    public void testToString1(){
        a = Polynomial.build("1 0 0 2");
        Assert.assertEquals(a.toString(),"1 + 2*x^3");
    }
    @Test
    public void testToString2(){
        a = Polynomial.build("0 0 0 0 0 4");
        Assert.assertEquals(a.toString(),"4*x^5");
    }
    @Test
    public void testToString3(){
        a = Polynomial.build("1/2 -1/2 1/4 -2");
        Assert.assertEquals(a.toString(),"1/2 -1/2*x + 1/4*x^2 -2*x^3");
    }
    @Test
    public void testToString4(){
        a = Polynomial.build("0 0 0 0 0 0 0 0 0 0 100");
        Assert.assertEquals(a.toString(),"100*x^10");
    }
}