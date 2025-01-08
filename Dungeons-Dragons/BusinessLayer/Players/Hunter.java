package BusinessLayer.Players;

import BusinessLayer.Abstracts.Player;
import BusinessLayer.Abstracts.Tile;
import BusinessLayer.Abstracts.Unit;
import BusinessLayer.Enemies.Monster;
import UI.MessageCallback;

import java.util.List;

public class Hunter extends Player {
    private Integer range;
    private Integer arrowsCount;
    private Integer ticksCount;

    public Hunter(String name, Integer health, Integer attack, Integer defense, Integer range, MessageCallback message){
        super(name,health,attack,defense,message);
        this.range = range;
        arrowsCount = 10 * getPlayerLevel();
        ticksCount = 0;
    }

    public Integer getArrowsCount() {
        return arrowsCount;
    }

    public void setArrowsCount(Integer arrowsCount) {
        this.arrowsCount = arrowsCount;
    }

    public Integer getRange() {
        return range;
    }

    public void setTicksCount(Integer ticksCount) {
        this.ticksCount = ticksCount;
    }

    public Integer getTicksCount() {
        return ticksCount;
    }

    @Override
    public void levelUp() {
        if(getExperience() >= 50 * getPlayerLevel()) {
            super.levelUp();
            setArrowsCount(getArrowsCount() + 10 * getPlayerLevel());
            setAttackPoints(getAttackPoints() + 2 * getPlayerLevel());
            setDefensePoints(getDefensePoints() + getPlayerLevel());
            message.send("Gained " + 10 * getPlayerLevel() + " health" + " " + ", gained " + 6 * getPlayerLevel() + " attack" + ", gained " + 2 * getPlayerLevel() + " defense" + ", gained " + 10 * getPlayerLevel() + " arrows");
        }
    }

    @Override
    public void processStep() {
        if(getTicksCount() == 10){
            setArrowsCount(getArrowsCount() + getPlayerLevel());
            setTicksCount(0);
        }
        else setTicksCount(getTicksCount() + 1);
    }

    @Override
    public void castAbility() {
        if(getArrowsCount() == 0) {
            message.send("Not enough arrows to use Shoot");
            return;
        }
        setArrowsCount(getArrowsCount() - 1);
        Tile target = findTarget();
        this.specialAbility(target,getAttackPoints());
    }

    private Tile findTarget()
    {
        List <Tile> tiles = board.getAllEnemiesInRange(getRange());
        if(tiles.size() == 0)
            return this;
        double minRange = Unit.range(tiles.get(0),this);
        Tile target = tiles.get(0);
        for(Tile t: tiles){
            if((Unit.range(t,this))<minRange){
                target = t;
                minRange=Unit.range(t,this);
            }
        }
        return target;
    }

    @Override
    public String getInfo(){
        return "Class: Hunter, " + super.getInfo() + ", Range: " + getRange() + ", Arrows count: " + getArrowsCount()+"\n";
    }

    @Override
    public String printSpecialAbility() {
        return getName() + " used Shoot";
    }

    //Equals function for testing only.
    @Override
    public boolean equals(Object _other) {
        if(_other instanceof Hunter) {
            Hunter other = (Hunter) _other;
            if (getTile() == other.getTile() && getPosition().equals(other.getPosition()) && getName().equals(other.getName()) && getHealthAmount().equals(other.getHealthAmount()) && getHealthPool().equals(other.getHealthPool()) && getAttackPoints().equals(other.getAttackPoints()) && getDefensePoints().equals(other.getDefensePoints()) && getExperience().equals(other.getExperience()) && getPlayerLevel().equals(other.getPlayerLevel()) && getRange().equals(other.getRange()) && getArrowsCount().equals(other.getArrowsCount()))
                return true;
        }
        return false;
    }
}
