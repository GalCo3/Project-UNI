package BusinessLayer.Abstracts;

import UI.MessageCallback;

public class Wall extends Tile {
    public Wall(Position position, MessageCallback messageCallback){
        super('#',messageCallback);
        setPosition(position);
    }

    @Override
    public boolean isEnemy() { return false; }

    @Override
    public boolean isPlayer() { return false; }

    @Override
    public void visit(Tile other) {}

    @Override
    public void accept(Unit other) {}

    @Override
    public void accept(Unit other, int attackPoints) {}

    @Override
    public void enemyTurn() {}

    @Override
    public void processStep() {}

    @Override
    public void specialAbility(Tile other, int attack) {}

    @Override
    public void cast() {}

    @Override
    public String getInfo() {
        return null;
    }
}
