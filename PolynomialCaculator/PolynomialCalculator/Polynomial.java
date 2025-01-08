package PolynomialCalculator;

public class Polynomial {

    private Monomial[] m;


    public Polynomial(Monomial[] mm ){
        this.m = mm;

    }


    public static Polynomial build(String input) {
        input = input.replaceAll("\\s+", " ");
        Monomial [] a = new Monomial[Count(input)];
        String [] s = input.split(" ");
        String [] sp;

        for (int i = 0; i < a.length; i++) {

            if(s[i].contains("/")){

                sp = s[i].split("/");
                int n = toInt(sp[0]);
                int d = toInt(sp[1]);
                a[i]=new Monomial(i,new Rational(n,d));
            }
            else{
                a[i] = new Monomial(i,new Integer(toInt(s[i])));
            }
        }
        return new Polynomial(a);
    }


    public Polynomial add(Polynomial p) {

        Monomial[] m1 = p.get();
        int maxL = Math.max(m.length,m1.length);
        Monomial [] a = new Monomial[maxL];

        for (int i = 0; i <maxL ; i++) {
            a[i] = m[i].add(m1[i]);
        }

        return new Polynomial(a);
    }

    public Polynomial mul(Polynomial p) {
        Monomial[] m1 = p.get();
        int len = m1.length+m.length-1;
        Monomial [] a = new Monomial[len];
        for (int i = 0; i < a.length; i++) {
            a[i] = new Monomial(i,new Integer(0));
        }

        for (int i = 0; i <m.length ; i++) {
            for (int j = 0; j< m1.length; j++){
                a[i+j] = a[i+j].add(m[i].mul(m1[j]));
            }
        }
        return new Polynomial(a);
    }

    public Scalar evaluate(Scalar s) {

        Scalar ans=new Rational();
        Scalar add;

        int [] temp = s.get();
        boolean tr = temp.length==2;

        for (int i = 0; i < m.length; i++) {
            add = m[i].evaluate(s);
            ans = ans.add(add);
            if (tr)
                s = new Rational(temp[0],temp[1]);
            else
                s = new Integer(temp[0]);
        }

        return ans;
    }

    public Polynomial derivative() {


        int count = 0;

        Monomial [] de = new Monomial[m.length-1];

        if(de.length==0){
            Monomial [] m1 = new Monomial[1];
            m1[0] = new Monomial(0,new Integer(0));
            return new Polynomial(m1);
        }

        int j = 0;
        for (int i = 1; i < m.length; i++) {
            de[j] = m[i].derivative();
            j++;
        }
        Polynomial ans = new Polynomial(de);
        return ans;
    }

    Monomial [] get (){
        return m;
    }


    public String toString() {
        String ans = "";
        String [] num;
        int count = 0;
        for (int i = 0; i < m.length; i++) {

            if (!(m[i].toString().equals("")))
                if (count == 0){
                    ans = ans + m[i].toString();
                    count++;
                }
                else if (m[i].toString().charAt(0)!='-')
                        ans = ans +" + "+ m[i].toString();
                else
                    ans = ans +" "+ m[i].toString();

        }
        return ans;
    }

    private static int toInt(String s){

        return java.lang.Integer.parseInt(s);
    }

    public boolean equals (Polynomial p){
        Monomial [] m1 = p.get();
        if (m1.length!=m.length)
            return false;
        for (int i = 0; i < m.length; i++) {
            if (!(m[i].equals(m1[i])))
                return false;
        }
        return true;
    }

    private static int Count(String s) {
        int count = 0;
        for (int i =0; i<s.length();i++){
            if (s.charAt(i) == ' ')
                count++;
        }

        return count+1;
    }
    public int getLen (){
        return m.length;
    }

}
