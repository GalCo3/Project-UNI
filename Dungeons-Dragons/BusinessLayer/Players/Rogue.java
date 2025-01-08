package BusinessLayer.Players;

import BusinessLayer.Abstracts.Player;
import BusinessLayer.Abstracts.Tile;
import UI.MessageCallback;
import java.util.List;

public class Rogue extends Player {

    public Integer cost;
    public Integer currentEnergy;

    public Rogue (String name , Integer health, Integer attack, Integer defense, Integer cost, MessageCallback message){
        super(name,health,attack,defense,message);
        this.cost = cost;
        this.currentEnergy = 100;
    }

    public void setCurrentEnergy(Integer currentEnergy) {
        this.currentEnergy = currentEnergy;
    }

    public Integer getCurrentEnergy() {
        return currentEnergy;
    }

    public Integer getCost() {
        return cost;
    }

    @Override
    public void levelUp() {
        if (getExperience() >= 50 * getPlayerLevel()) {
            super.levelUp();
            setCurrentEnergy(100);
            setAttackPoints(getAttackPoints() + 3 * getPlayerLevel());
            message.send("Gained " + 10 * getPlayerLevel() + " health" + " " + ", gained " + 7 * getPlayerLevel() + " attack" + ", gained " + getPlayerLevel() + " defense");
        }
    }

    @Override
    public void castAbility(){
        if (getCurrentEnergy()<getCost()) {
            return;
        }
        setCurrentEnergy(getCurrentEnergy()-getCost());
        List<Tile> inRangeEnemies = board.getAllEnemiesInRange(1);
        for(Tile enemy: inRangeEnemies) this.specialAbility(enemy,getAttackPoints());
    }

    @Override
    public void processStep() {
        setCurrentEnergy(Math.min(getCurrentEnergy() + 10, 100));
    }

    @Override
    public String getInfo() {
        return "Class: Rogue, " + super.getInfo() + ", Cost: " + getCost() + ", Energy: " + getCurrentEnergy()+"\n";
    }

    @Override
    public String printSpecialAbility() {
        if(getCurrentEnergy()<getCost()){
            return "Fan of Knives Shield on cooldown";
        }
        return getName() + " used Fan of Knives";
    }

    //Equals function for testing only.
    @Override
    public boolean equals(Object _other) {
        if(_other instanceof Rogue) {
            Rogue other = (Rogue) _other;
            if (getTile() == other.getTile() && getPosition().equals(other.getPosition()) && getName().equals(other.getName()) && getHealthAmount().equals(other.getHealthAmount()) && getHealthPool().equals(other.getHealthPool()) && getAttackPoints().equals(other.getAttackPoints()) && getDefensePoints().equals(other.getDefensePoints()) && getExperience().equals(other.getExperience()) && getPlayerLevel().equals(other.getPlayerLevel()) && getCost().equals(other.getCost()) && getCurrentEnergy().equals(other.getCurrentEnergy()))
                return true;
        }
        return false;
    }
}
