package PolynomialCalculator;

public class Integer implements Scalar {

    private int number;

    public Integer(int number) {
        this.number = number;
    }

    public Integer() {
        this.number = 0;
    }

    @Override
    public Scalar add(Scalar s) {

        int[] temp = s.get();
        Scalar ans;

        if (temp.length == 1) {
            ans = new Integer(temp[0] + number);
        } else {
            int gcd1 = gcd(temp[0], temp[1]);
            temp[0] /= gcd1;
            temp[1] /= gcd1;

            if (temp[1] == 1)
                ans = new Integer(temp[0] + number);

            else {
                int d = temp[1];
                int n = d * number;

                n = n + temp[0];

                int gcd = gcd(n, d);

                n = n / gcd;
                d = d / gcd;

                if (d == 1)
                    ans = new Integer(n);

                else
                    ans = new Rational(n, d);
            }
        }

        return ans;
    }

    @Override
    public Scalar mul(Scalar s) {
        int[] temp = s.get();
        int ans = number;
        if (temp.length == 1) {
            ans *= temp[0];
        }
        if (temp.length == 2) {
            ans *= temp[0];
            int gcd = gcd(ans, temp[1]);
            ans /= gcd;
            temp[1] /= gcd;
        }
        if (temp.length == 2 && temp[1] != 1)
            return new Rational(ans, temp[1]);


        return new Integer(ans);
    }

    @Override
    public Scalar neg() {
        return new Integer(number * -1);
    }

    @Override
    public Scalar power(int exponent) {
        int ans = (int) Math.pow(number, exponent);
        return new Integer(ans);
    }

    @Override
    public int[] get() {
        int[] ret = new int[1];
        ret[0] = number;
        return ret;
    }

    @Override
    public int sign() {
        if (number > 0)
            return 1;
        else if (number == 0)
            return 0;
        else
            return -1;
    }

    @Override
    public boolean equals(Scalar s) {

        int[] temp = s.get();

        if (temp.length == 1)
            return number == temp[0];

        else {
            int gcd = gcd(temp[0], temp[1]);
            temp[0] /= gcd;
            temp[1] /= gcd;

            if (temp[0] == 0)
                return number == 0;

            if (temp[1] == 1)
                return number == temp[1];
            else
                return false;
        }

    }

    @Override
    public String toString() {
        String ans = String.valueOf(number);
        return ans;
    }

    public int gcd(int n1, int n2) {
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
