package PolynomialCalculator;

public class Monomial  {

    private int exponent;
    private Scalar coefficient;

    public Monomial(int exponent, Scalar coefficient) {
        this.exponent = exponent;
        this.coefficient = coefficient;
    }

    public Monomial add(Monomial m){
        if (exponent!= m.exponent)
            return null;
        Scalar a = coefficient.add(m.coefficient);
        return new Monomial(exponent,a);
    }

    public Monomial mul(Monomial m){
        Scalar n = coefficient.mul(m.coefficient);
        int e = exponent+m.exponent;

        return new Monomial(e,n);
    }

    public Scalar evaluate(Scalar s){
        Scalar a = s.power(exponent);
        a=a.mul(coefficient);
        return a;
    }

    public Monomial derivative(){

        if (exponent==0)
            return new Monomial(0,new Integer(0));

        Scalar c = coefficient.mul(new Integer(exponent));
        int e = exponent-1;

        return new Monomial(e,c);
    }

    public int sign() {
        return coefficient.sign();
    }

    public boolean equals (Monomial m){
        return m.exponent==exponent & m.coefficient.equals(coefficient);
    }


    @Override
    public String toString() {

        String ans="";
        int [] temp = coefficient.get();

            if (temp[0] != 0){

                if ( ((temp.length==1& temp[0]!=1) | ( temp.length==2 && (temp[0]!=temp[1])) ) &  exponent!=0 & exponent !=1 ) // c!=1 & exp (!= 0 & != 1)
                    ans = ans + coefficient.toString()+"*x" +"^"+String.valueOf(exponent);

                else if ( ((temp.length==1& temp[0]!=1) | ( temp.length==2 && (temp[0]!=temp[1])) ) &  exponent==1 ) // c!=1 check & exp ==1
                    ans = ans + coefficient.toString()+"*x";

                else if ( ((temp.length==1 &temp[0]==1) | (temp.length==2 && temp[0]== temp[1]) ) & exponent!=0 & exponent !=1) // c==1 & exp!=0 & != 1
                    ans = ans+"x"+"^"+String.valueOf(exponent);

                else if ( ((temp.length==1& temp[0]==1) | ( temp.length==2 && (temp[0]==temp[1])) ) &  exponent==1 ) // c==1 & exp ==1
                    ans = ans +"x";

                else if(exponent==0) // c!= 1 & exp ==0
                    ans = ans + coefficient.toString();
        }
        //else return "0";

        return ans;
    }

    public Scalar getCoefficient() {
        return coefficient;
    }
}
