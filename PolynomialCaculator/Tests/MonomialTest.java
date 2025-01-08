package Tests;

import PolynomialCalculator.Integer;
import PolynomialCalculator.Monomial;
import PolynomialCalculator.Rational;
import PolynomialCalculator.Scalar;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

public class MonomialTest {

    private Monomial twoPowTwo;
    private Monomial minusTwoPowTwo;
    private Monomial twoPowThree;
    private Monomial halfPowTwo;
    private Monomial minusHalfPowTwo;
    private Monomial halfPowThree;
    private Scalar val;

    @Before
    public void initTest(){
        twoPowTwo = new Monomial(2,new Integer(2));
        minusTwoPowTwo = new Monomial(2,new Integer(-2));
        twoPowThree = new Monomial(3,new Integer(2));
        halfPowTwo = new Monomial(2,new Rational(1,2));
        minusHalfPowTwo = new Monomial(2,new Rational(-1,2));
        halfPowThree = new Monomial(3,new Rational(1,2));
        val = null;
    }

    @Test
    public void testAdd1(){
        twoPowTwo = twoPowTwo.add(new Monomial(2,new Integer(3)));
        Monomial temp = new Monomial(2, new Integer(5));
        Assert.assertTrue(twoPowTwo.equals(temp));
    }
    @Test
    public void testAdd2(){
        twoPowTwo = twoPowTwo.add(new Monomial(2,new Integer(-3)));
        Assert.assertTrue(twoPowTwo.equals(new Monomial(2,new Integer(-1))));
    }
    @Test
    public void testAdd3(){
        twoPowTwo = twoPowTwo.add(halfPowTwo);
        Assert.assertTrue(twoPowTwo.equals(new Monomial(2,new Rational(5,2))));
    }
    @Test
    public void testAdd4(){
        twoPowTwo = twoPowTwo.add(minusHalfPowTwo);
        Assert.assertTrue(twoPowTwo.equals(new Monomial(2,new Rational(3,2))));
    }


    @Test
    public void testMul1(){
        twoPowTwo = twoPowTwo.mul(minusTwoPowTwo);
        Assert.assertTrue(twoPowTwo.equals(new Monomial(4,new Integer(-4))));
    }
    @Test
    public void testMul2(){
        twoPowTwo = twoPowTwo.mul(halfPowTwo);
        Assert.assertTrue(twoPowTwo.equals(new Monomial(4,new Integer(1))));
    }
    @Test
    public void testMul3(){
        twoPowTwo = twoPowTwo.mul(new Monomial(2,new Integer(2)));
        Assert.assertTrue(twoPowTwo.equals(new Monomial(4,new Integer(4))));
    }
    @Test
    public void testMul4(){
        twoPowTwo = twoPowTwo.mul(minusHalfPowTwo);
        Assert.assertTrue(twoPowTwo.equals(new Monomial(4,new Integer(-1))));
    }
    @Test
    public void testMul5(){
        halfPowTwo = halfPowTwo.mul(minusHalfPowTwo);
        Assert.assertTrue(halfPowTwo.equals(new Monomial(4,new Rational(-1,4))));
    }


    @Test
    public void testEvaluate1(){
        val = twoPowTwo.evaluate(new Integer(2));
        Assert.assertTrue(val.equals(new Integer(8)));
    }
    @Test
    public void testEvaluate2(){
        val = minusTwoPowTwo.evaluate(new Integer(-2));
        Assert.assertTrue(val.equals(new Integer(-8)));
    }
    @Test
    public void testEvaluate3(){
        val = twoPowThree.evaluate(new Integer(-2));
        Assert.assertTrue(val.equals(new Integer(-16)));
    }
    @Test
    public void testEvaluate4(){
        val = halfPowTwo.evaluate(new Integer(2));
        Assert.assertTrue(val.equals(new Integer(2)));
    }
    @Test
    public void testEvaluate5(){
        val = minusHalfPowTwo.evaluate(new Integer(2));
        Assert.assertTrue(val.equals(new Integer(-2)));
    }
    @Test
    public void testEvaluate6(){
        val = halfPowThree.evaluate(new Rational(-1,2));
        Assert.assertTrue(val.equals(new Rational(-1,16)));
    }


    @Test
    public void testDerivative1() {
        twoPowTwo = twoPowTwo.derivative();
        Assert.assertTrue(twoPowTwo.equals(new Monomial(1, new Integer(4))));
    }
    @Test
    public void testDerivative2(){
        minusTwoPowTwo = minusTwoPowTwo.derivative();
        Assert.assertTrue(minusTwoPowTwo.equals(new Monomial(1,new Integer(-4))));
    }
    @Test
    public void testDerivative3(){
        halfPowTwo = halfPowTwo.derivative();
        Assert.assertTrue(halfPowTwo.equals(new Monomial(1,new Integer(1))));
    }
    @Test
    public void testDerivative4(){
        minusHalfPowTwo = minusHalfPowTwo.derivative();
        Assert.assertTrue(minusHalfPowTwo.equals(new Monomial(1,new Integer(-1))));
    }
    @Test
    public void testDerivative5(){
        Monomial ans = new Monomial(0,new Integer(3));
        ans = ans.derivative();
        Assert.assertTrue(ans.equals(new Monomial(0,new Integer(0))));
    }


    @Test
    public void testSign1(){
        Assert.assertEquals(twoPowTwo.sign(),1);
    }
    @Test
    public void testSign2(){
        Assert.assertEquals(minusTwoPowTwo.sign(),-1);
    }
    @Test
    public void testSign3(){
        Monomial zero = new Monomial(1,new Integer(0));
        Assert.assertEquals(zero.sign(),0);
    }


    @Test
    public void testToString1(){
        Assert.assertEquals(twoPowTwo.toString(),"2*x^2");
    }
    @Test
    public void testToString2(){
        Assert.assertEquals(minusTwoPowTwo.toString(),"-2*x^2");
    }
    @Test
    public void testToString3(){
        Assert.assertEquals(halfPowTwo.toString(),"1/2*x^2");
    }
    @Test
    public void testToString4(){
        Assert.assertEquals(minusHalfPowTwo.toString(),"-1/2*x^2");
    }
}