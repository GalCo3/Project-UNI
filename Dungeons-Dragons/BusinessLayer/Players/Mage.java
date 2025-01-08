package BusinessLayer.Players;

import BusinessLayer.Abstracts.HeroicUnit;
import BusinessLayer.Abstracts.Player;
import BusinessLayer.Abstracts.Tile;
import UI.MessageCallback;

import java.util.List;
import java.util.Random;

public class Mage extends Player {

    public Integer manaPool;
    public Integer currentMana;
    public Integer manaCost;
    public Integer spellPower;
    public Integer hitsCount;
    public Integer abilityRange;

    public Mage(String name, Integer health, Integer attack, Integer defense, Integer manaPool, Integer manaCost, Integer spellPower, Integer hitsCount, Integer abilityRange, MessageCallback message) {
        super(name,health,attack,defense,message);
        this.manaPool = manaPool;
        this.currentMana = manaPool/4;
        this.manaCost = manaCost;
        this.spellPower = spellPower;
        this.hitsCount = hitsCount;
        this.abilityRange = abilityRange;
    }

    public Integer getCurrentMana() {
        return currentMana;
    }

    public void setCurrentMana(Integer currentMana) {
        this.currentMana = currentMana;
    }

    public Integer getManaCost() {
        return manaCost;
    }

    public Integer getManaPool() {
        return manaPool;
    }

    public Integer getAbilityRange() {
        return abilityRange;
    }

    public Integer getSpellPower() {
        return spellPower;
    }

    public void setSpellPower(Integer spellPower) {
        this.spellPower = spellPower;
    }

    public void setManaPool(Integer manaPool) {
        this.manaPool = manaPool;
    }

    public Integer getHitsCount() {
        return hitsCount;
    }

    @Override
    public void levelUp() {
        if (getExperience() >= 50 * getPlayerLevel()) {
            super.levelUp();
            setManaPool(getManaPool() + 25 * getPlayerLevel());
            setCurrentMana(Math.min(getCurrentMana() + (getManaPool() / 4), getManaPool()));
            setSpellPower(getSpellPower() + 10 * getPlayerLevel());
            message.send("Gained " + 10 * getPlayerLevel() + " health" + " " + ", gained " + 4 * getPlayerLevel() + " attack" + ", gained " + getPlayerLevel() + " defense" + ", gained " + 25 * getPlayerLevel() + " max mana" + ", gained " + 10 * getPlayerLevel() + " spellpower");
        }
    }

    @Override
    public void castAbility() {
        if (getCurrentMana()<getManaCost()) {
            return;
        }
        setCurrentMana(getCurrentMana()-getManaCost());
        int hits = 0;
        List<Tile> enemies = board.getAllEnemiesInRange(getAbilityRange());
        Random rnd = new Random();
        Tile enemy;
        while (hits < getHitsCount() & enemies.size()>0 ){
            enemy = enemies.get(rnd.nextInt(enemies.size()));
            this.specialAbility(enemy,getSpellPower());
            hits++;
            enemies = board.getAllEnemiesInRange(getAbilityRange());
        }
        return;
    }

    @Override
    public void processStep() {
        setCurrentMana(Math.min(getManaPool(), getCurrentMana() + getPlayerLevel()));
    }

    @Override
    public String getInfo() {
        return "Class: Mage, " + super.getInfo() + ", Mana: " + getCurrentMana() + "/" + getManaPool() + ", Mana cost: " + getManaCost() + ", Spellpower: " + getSpellPower() + ", Ability range: " + getAbilityRange() +"\n";
    }

    @Override
    public String printSpecialAbility() {
        if (getCurrentMana()<getManaCost()) {
            return ("Not enough mana");
        }
        return getName() + " used Blizzard";
    }

    //Equals function for testing only.
    @Override
    public boolean equals(Object _other) {
        if(_other instanceof Mage) {
            Mage other = (Mage) _other;
            if (getTile() == other.getTile() && getPosition().equals(other.getPosition()) && getName().equals(other.getName()) && getHealthAmount().equals(other.getHealthAmount()) && getHealthPool().equals(other.getHealthPool()) && getAttackPoints().equals(other.getAttackPoints()) && getDefensePoints().equals(other.getDefensePoints()) && getExperience().equals(other.getExperience()) && getPlayerLevel().equals(other.getPlayerLevel()) && getManaPool().equals(other.getManaPool()) && getCurrentMana().equals(other.getCurrentMana()) && getManaCost().equals(other.getManaCost()) && getSpellPower().equals(other.getSpellPower()) && getHitsCount().equals(other.getHitsCount()) && getAbilityRange().equals(other.getAbilityRange()))
                return true;
        }
        return false;
    }
}
