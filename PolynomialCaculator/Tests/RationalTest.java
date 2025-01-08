package Tests;

import PolynomialCalculator.Integer;
import PolynomialCalculator.Rational;
import PolynomialCalculator.Scalar;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

public class RationalTest {
    private Scalar zero;
    private Scalar half;
    private Scalar minusHalf;

    @Before
    public void initTest(){
        zero = new Integer(0);
        half = new Rational(1,2);
        minusHalf = new Rational(-1,2);
    }

    @Test
    public void testInput1(){
        Rational input = new Rational(-1,-1);
        Assert.assertTrue(input.equals(new Integer(1)));
    }
    @Test
    public void testInput2(){
        Rational input = new Rational(-1,-2);
        Assert.assertTrue(input.equals(new Rational(1,2)));
    }
    @Test
    public void testAdd1() {
        half = half.add(minusHalf);

        Assert.assertTrue(half.equals(zero));
    }
    @Test
    public void testAdd2() {
        half = half.add(new Rational(1,4));
        Assert.assertTrue(half.equals(new Rational(3,4)));
    }
    @Test
    public void testAdd3() {
        half = half.add(new Rational(1,2));
        Assert.assertTrue(half.equals(new Rational(1,1)));
    }
    @Test
    public void testAdd4() {
        minusHalf = minusHalf.add(new Integer(1));
        Assert.assertTrue(minusHalf.equals(half));
    }
    @Test
    public void testAdd5() {
        minusHalf = minusHalf.add(new Rational(1,2));
        Assert.assertTrue(minusHalf.equals(zero));
    }

    @Test
    public void testMul1() {
        half = half.mul(new Rational(1,2));
        Assert.assertTrue(half.equals(new Rational(1,4)));
    }
    @Test
    public void testMul2() {
        half = half.mul(new Integer(2));
        Assert.assertTrue(half.equals(new Integer(1)));
    }
    @Test
    public void testMul3() {
        minusHalf = minusHalf.mul(new Integer(2));
        Assert.assertTrue(minusHalf.equals(new Integer(-1)));
    }
    @Test
    public void testMul4() {
        half = half.mul(minusHalf);
        Assert.assertTrue(half.equals(new Rational(-1,4)));
    }
    @Test
    public void testMul5() {
        half = half.mul(zero);
        Assert.assertTrue(half.equals(new Integer(0)));
    }

    @Test
    public void testNeg1() {
        minusHalf = minusHalf.neg();
        Assert.assertTrue(minusHalf.equals(half));
    }
    @Test
    public void testNeg2() {
        half = half.neg();
        Assert.assertTrue(half.equals(minusHalf));
    }
    @Test
    public void testNeg3() {
        zero = zero.neg();
        Assert.assertTrue(zero.equals(new Rational(0,1)));
    }

    @Test
    public void testPower1() {
        half = half.power(2);
        Assert.assertTrue(half.equals(new Rational(1,4)));
    }
    @Test
    public void testPower2() {
        minusHalf = minusHalf.power(2);
        Assert.assertTrue(minusHalf.equals(new Rational(1,4)));
    }
    @Test
    public void testPower3() {
        minusHalf = minusHalf.power(3);
        Assert.assertTrue(minusHalf.equals(new Rational(-1,8)));
    }
    @Test
    public void testPower4() {
        minusHalf = minusHalf.power(0);
        Assert.assertTrue(minusHalf.equals(new Integer(1)));
    }

    @Test
    public void testSign1() {
        Assert.assertEquals(half.sign(),1);
    }
    @Test
    public void testSign2() {
        Assert.assertEquals(minusHalf.sign(),-1);
    }
    @Test
    public void testSign3() {
        Assert.assertEquals(zero.sign(),0);
    }

    @Test
    public void testReduce1() {
        Assert.assertTrue(half.equals(new Rational(1,2).reduce()));
    }
    @Test
    public void testReduce2() {
        Assert.assertTrue(half.equals(new Rational(2,4).reduce()));
    }
    @Test
    public void testReduce3() {
        Assert.assertTrue(minusHalf.equals(new Rational(-2,4)));
    }

    @Test
    public void testToString1() {
        Assert.assertEquals(half.toString(),"1/2");
    }
    @Test
    public void testToString2() {
        Assert.assertEquals(minusHalf.toString(),"-1/2");
    }
}