package PolynomialCalculator;

public class Rational implements Scalar{

    private int numerator;
    private int denominator;

    public Rational(int numerator, int denominator) {
        if (denominator<0){
            this.numerator=-numerator;
            this.denominator=-denominator;
        }
        else {
            this.numerator=numerator;
            this.denominator = denominator;
        }


    }
    public Rational() {
        this.numerator = 0;
        this.denominator = 1;
    }


    @Override
    public Scalar add(Scalar s) {

        int[] temp = s.get();

        if (temp.length==1){
            int n = temp[0]*denominator;
            int d = denominator;

            n=n+numerator;

            int gcd = gcd(n,d);

            n=n/gcd;
            d=d/gcd;

            if (d==1)
                return new Integer(n);
            else
                return new Rational(n,d);

        }

        else{
            int gcd = gcd(temp[1],denominator);

            int n = temp[0]*denominator;
            int d = temp[1]*denominator;

            numerator *= temp[1];
            denominator *= temp[1];

            n += numerator;

            int gcd2 = gcd(n,d);

            n=n/gcd2;
            d=d/gcd2;

            if (n==0)
                return new Integer(0);

            if (d==1)
                return new Integer(n);
            else
                return new Rational(n,d);
        }
    }

    @Override
    public Scalar mul(Scalar s) {

        int [] temp = s.get();
        int n;
        int d = denominator;

        if (temp.length==1)
            n = temp[0]*numerator;

        else{
            n =  temp[0]*numerator;
            d = temp[1]*denominator;}

        int gcd = gcd(Math.abs(n),Math.abs(d));

        n /= gcd;
        d /= gcd;

        if (n==0)
            return new Integer(0);
        if (d==1)
            return new Integer(n);
        else
            return new Rational(n,d);

    }


    @Override
    public Scalar neg() {
        return new Rational(numerator*-1,denominator);
    }

    @Override
    public Scalar power(int exponent) {
        red();
        numerator = (int)Math.pow(numerator,exponent);
        denominator = (int)Math.pow(denominator,exponent);

        red();

        if (denominator==1)
            return new Integer(numerator);
        else
            return new Rational(numerator,denominator);
    }

    @Override
    public int[] get() {
        Rational re = reduce();
        int[] ret = new int[2];
        ret[0] = re.numerator;
        ret[1] = re.denominator;
        return ret;
    }

    @Override
    public int sign() {
        if ((numerator>0 & denominator>0)|(numerator<0 &denominator<0))
            return 1;
        else if (numerator==0)
            return 0;
        else
            return -1;
    }

    @Override
    public boolean equals(Scalar s) {
        red();
        int gcd;

        int [] temp = s.get();

        if (numerator==0&temp[0]==0)
            return true;

        if (temp.length ==1 & denominator!=1)
            return false;

        else if ( temp.length==1 & denominator==1 & temp[0] != numerator)
            return false;

        if (temp.length==2 ){
            gcd = gcd(Math.abs(temp[0]),Math.abs(temp[1]));
            temp[0] /= gcd;
            temp[1] /= gcd;
            if (temp[0]!=numerator|temp[1]!=denominator)
                return false;
        }

        return true;
    }

    public void red(){
        int gcd = gcd();

        numerator   /= gcd;
        denominator /= gcd;

    }

    public Rational reduce(){
        red();

        return new Rational(numerator,denominator);
    }

    @Override
    public String toString() {

        String ans ="";
        red();
        ans =  (numerator)+"/"+(denominator);

        return ans;
    }

    private int gcd (){
        int n1 = numerator;
        int n2 = denominator;
        int gcd = 1;
        for (int i = 1; i <= n1 && i <= n2; i++) {
            if (n1 % i == 0 && n2 % i == 0) {
                gcd = i;
            }
        }
        return gcd;
    }

    public int gcd(int n1, int n2){
        n1 = Math.abs(n1);
        n2 = Math.abs(n2);
        int gcd = 1;
        for (int i = 1; i <= n1 && i <= n2; i++) {
            if (n1 % i == 0 && n2 % i == 0) {
                gcd = i;
            }
        }
        return gcd;
    }
}
