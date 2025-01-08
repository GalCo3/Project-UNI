package BusinessLayer.Players;

import BusinessLayer.Abstracts.Player;
import BusinessLayer.Abstracts.Tile;
import UI.MessageCallback;
import java.util.List;
import java.util.Random;

public class Warrior extends Player {

    public Integer abilityCooldown;
    public Integer remainingCooldown;

    public Warrior (String name, Integer health, Integer attack, Integer defense, Integer abilityCooldown, MessageCallback message){
        super(name,health,attack,defense, message);
        this.abilityCooldown = abilityCooldown;
        this.remainingCooldown = 0;
    }

    public Integer getAbilityCooldown() {
        return abilityCooldown;
    }

    public Integer getRemainingCooldown() {
        return remainingCooldown;
    }

    public void setRemainingCooldown(Integer remainingCooldown) {
        this.remainingCooldown = remainingCooldown;
    }

    @Override
    public void levelUp() {
        if(getExperience() >= 50 * getPlayerLevel()) {
            super.levelUp();
            setRemainingCooldown(0);
            setHealthPool(getHealthPool() + 5 * getPlayerLevel());
            setHealthAmount(getHealthPool());
            setAttackPoints(getAttackPoints() + 2 * getPlayerLevel());
            setDefensePoints(getDefensePoints() + getPlayerLevel());
            message.send("Gained " + 15 * getPlayerLevel() + " health" + " " + ", gained " + 6 * getPlayerLevel() + " attack" + ", gained " + 2 * getPlayerLevel() + " defense");
        }
    }

    @Override
    public void processStep() {
        setRemainingCooldown(Math.max(getRemainingCooldown()-1,0));
    }

    @Override
    public void castAbility() {
        if(getRemainingCooldown() > 0){
            return;
        }
        List<Tile> inRangeEnemies = board.getAllEnemiesInRange(2);
        setRemainingCooldown(getAbilityCooldown()+1);
        setHealthAmount(Math.min(getHealthAmount()+(10*getDefensePoints()) , getHealthPool()));
        if (inRangeEnemies.size()==0)
            return;
        Random random = new Random();
        int hitEnemy = random.nextInt(inRangeEnemies.size());
        this.specialAbility(inRangeEnemies.get(hitEnemy),getHealthPool()/10);
    }

    @Override
    public String getInfo() {
        return "Class: Warrior, " + super.getInfo() + ", Ability cooldown: " + getAbilityCooldown() + ", Remaining cooldown: " + getRemainingCooldown()+"\n";
    }

    @Override
    public String printSpecialAbility() {
        if(getRemainingCooldown() == getAbilityCooldown()){
            return ("Avengers Shield on cooldown");
        }
        return getName() + " used Avengers Shield";
    }

    //Equals function for testing only.
    @Override
    public boolean equals(Object _other) {
        if(_other instanceof Warrior) {
            Warrior other = (Warrior) _other;
            if (getTile() == other.getTile() && getPosition().equals(other.getPosition()) && getName().equals(other.getName()) && getHealthAmount().equals(other.getHealthAmount()) && getHealthPool().equals(other.getHealthPool()) && getAttackPoints().equals(other.getAttackPoints()) && getDefensePoints().equals(other.getDefensePoints()) && getExperience().equals(other.getExperience()) && getPlayerLevel().equals(other.getPlayerLevel()) && getAbilityCooldown().equals(other.getAbilityCooldown()) && getRemainingCooldown().equals(other.getRemainingCooldown()))
                return true;
        }
        return false;
    }
}
