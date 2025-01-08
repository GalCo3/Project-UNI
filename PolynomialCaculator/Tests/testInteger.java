package Tests;

import PolynomialCalculator.Integer;
import PolynomialCalculator.Rational;
import PolynomialCalculator.Scalar;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

public class testInteger{
    private Scalar zero;
    private Scalar one;
    private Scalar six;
    private Scalar minusOne;
    private Scalar minusSix;
    private Scalar rat;

    @Before
    public void initTest(){
        six = new Integer(6);
        one = new Integer(1);
        minusOne = new Integer(-1);
        minusSix = new Integer(-6);
        rat = new Rational(5,1);
        zero = new Integer(0);
    }

    @Test
    public void testAdd1() {
        one = one.add(new Integer(5));
        Assert.assertTrue(one.equals(six));
    }
    @Test
    public void testAdd2() {
        one = one.add(new Integer(-2));
        Assert.assertTrue(one.equals(minusOne));
    }
    @Test
    public void testAdd3() {
        minusOne = minusOne.add(new Integer(7));
        Assert.assertTrue(minusOne.equals(six));
    }
    @Test
    public void testAdd4() {
        one = one.add(rat);
        Assert.assertTrue(one.equals(six));
    }


    @Test
    public void testMul1() {
        one = one.mul(six);
        Assert.assertTrue(one.equals(six));
    }
    @Test
    public void testMul2() {
        minusOne = minusOne.mul(six);
        Assert.assertTrue(minusOne.equals(minusSix));
    }
    @Test
    public void testMul3() {
        minusOne = minusOne.mul(minusSix);
        Assert.assertTrue(minusOne.equals(six));
    }
    @Test
    public void testMul4() {
        one = minusOne.mul(zero);
        Assert.assertTrue(one.equals(zero));
    }


    @Test
    public void testNeg1() {
        one = one.neg();
        Assert.assertTrue(one.equals(minusOne));
    }
    @Test
    public void testNeg2() {
        minusOne = minusOne.neg();
        Assert.assertTrue(minusOne.equals(one));
    }
    @Test
    public void testNeg3() {
        zero = zero.neg();
        Assert.assertTrue(zero.equals(new Integer(0)));
    }


    @Test
    public void testPower1() {
        six = six.power(2);
        Assert.assertTrue(six.equals(new Integer(36)));
    }
    @Test
    public void testPower2() {
        minusOne = minusOne.power(2);
        Assert.assertTrue(minusOne.equals(one));
    }
    @Test
    public void testPower3() {
        minusOne = minusOne.power(3);
        Assert.assertTrue(minusOne.equals(new Integer(-1)));
    }
    @Test
    public void testPower4() {
        six = six.power(0);
        Assert.assertTrue(six.equals(one));
    }

    @Test
    public void testSign1() {
        Assert.assertEquals(one.sign(),1);
    }
    @Test
    public void testSign2() {
        Assert.assertEquals(minusOne.sign(),-1);
    }
    @Test
    public void testSign3() {
        Assert.assertEquals(zero.sign(),0);
    }

    @Test
    public void testToString1() {
        Assert.assertEquals(one.toString(),"1");
    }
    @Test
    public void testToString2() {
        Assert.assertEquals(minusOne.toString(),"-1");
    }

}