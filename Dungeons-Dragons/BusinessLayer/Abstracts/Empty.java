package BusinessLayer.Abstracts;

import UI.MessageCallback;

public class Empty extends Tile {

    public Empty(Position p, MessageCallback messageCallback){
        super('.',messageCallback);
         initialize(p);
    }

    @Override
    public boolean isEnemy() {
        return false;
    }
    @Override
    public void visit(Tile other) {}

    @Override
    public void cast() {}

    @Override
    public void accept(Unit other, int attackPoints) {}

    @Override
    public boolean isPlayer() {
        return false;
    }

    @Override
    public void enemyTurn() {}

    @Override
    public void processStep() {}

    @Override
    public void specialAbility(Tile other, int attack) {}

    @Override
    public void accept(Unit other) {
        Position temp = new Position(getPosition().getX(),getPosition().getY());
        setPosition(other.getPosition());
        other.setPosition(temp);
    }

    @Override
    public String getInfo() {
        return null;
    }
}
