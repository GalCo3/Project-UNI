package BusinessLayer.Enemies;

import BusinessLayer.Abstracts.Enemy;
import BusinessLayer.Abstracts.Position;
import BusinessLayer.Abstracts.Tile;
import UI.MessageCallback;
import java.util.Random;

public class Monster extends Enemy {
    public Integer visionRange;

    public Monster(char character, String name, Integer health, Integer attack, Integer defense, Integer experienceValue, Integer visionRange, MessageCallback message){
        super(character,name,health,attack,defense, message);
        this.experienceValue = experienceValue;
        this.visionRange = visionRange;
    }

    public Integer getVisionRange() {
        return visionRange;
    }

    @Override
    public void processStep() {}

    @Override
    public void cast() {}

    @Override
    public void enemyTurn() {
        Tile player = board.getPlayer();
        if (range(this, player) < getVisionRange()) {
            int dx = getPosition().getX() - player.getPosition().getX();
            int dy = getPosition().getY() - player.getPosition().getY();

            if (Math.abs(dx) > Math.abs(dy)) {
                if (dx > 0) {
                    Tile tile1 = board.getTile(getPosition().getX() - 1, getPosition().getY());
                    visit(tile1);
                } else {
                    Tile tile1 = board.getTile(getPosition().getX() + 1, getPosition().getY());
                    visit(tile1);
                }
            } else {
                if (dy > 0) {
                    Tile tile1 = board.getTile(getPosition().getX(), getPosition().getY() - 1);
                    visit(tile1);
                } else {
                    Tile tile1 = board.getTile(getPosition().getX(), getPosition().getY() + 1);
                    visit(tile1);
                }
            }
        } else {
            Random random = new Random();
            int next = random.nextInt(4);
            if (next == 0) {
                Tile t = board.getTile(new Position(getPosition().getX(), getPosition().getY() - 1));
                visit(t);
            } else if (next == 1) {
                Tile t = board.getTile(new Position(getPosition().getX() - 1, getPosition().getY()));
                visit(t);
            } else if (next == 2) {
                Tile t = board.getTile(new Position(getPosition().getX(), getPosition().getY() + 1));
                visit(t);
            } else {
                Tile t = board.getTile(new Position(getPosition().getX() + 1, getPosition().getY()));
                visit(t);
            }
        }
    }

    @Override
    public String getInfo() {
        return "Name: " + getName()+ ", Health: " + getHealthAmount() + "/" + getHealthPool() +", Attack: " + getAttackPoints() + ", Defense: " + getDefensePoints() + ", Experience value: " + getExperienceValue() + ", Vision range: " + getVisionRange();
    }


    //Equals function for testing only.
    @Override
    public boolean equals(Object _other) {
        if(_other instanceof Monster) {
            Monster other = (Monster) _other;
            if (getTile() == other.getTile() && getPosition().equals(other.getPosition()) && getName().equals(other.getName()) && getHealthAmount().equals(other.getHealthAmount()) && getHealthPool().equals(other.getHealthPool()) && getAttackPoints().equals(other.getAttackPoints()) && getDefensePoints().equals(other.getDefensePoints()) && getExperienceValue().equals(other.getExperienceValue()) && getVisionRange().equals(other.getVisionRange()))
                return true;
        }
        return false;
    }
}

