package BusinessLayer.Abstracts;

public class Position  {
    private int x;
    private int y;

    public Position(int x, int y){
        this.x = x;
        this.y = y;
    }

    public void setX(int x){ this.x = x;}

    public void setY(int y){ this.y = y;}

    public int getX(){ return x;}

    public int getY(){ return y;}

    public int compareTo(Position p) {

        if (y>p.y)
            return 1;
        if (y<p.y)
            return -1;
        if (x>p.x)
            return 1;
        if (x<p.x)
            return -1;
        return 0;
    }

    //Equals function for testing only.
    public boolean equals(Object _other){
        if(_other == null) return true;
        if(_other instanceof Position){
            Position other = (Position) _other;
            if(getX() == other.getX()){
                if(getY() == other.getY())
                    return true;
            }
        }
        return false;
    }
}