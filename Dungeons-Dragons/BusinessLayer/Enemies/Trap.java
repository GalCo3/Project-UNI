package BusinessLayer.Enemies;

import BusinessLayer.Abstracts.Enemy;
import BusinessLayer.Abstracts.Tile;
import UI.MessageCallback;

public class Trap extends Enemy {
    private final Integer visibilityTime;
    private final Integer invisibilityTime;
    private Integer ticksCount;
    private boolean visible;
    private final char trapChar;

    public Trap(char character, String name, Integer health, Integer attack, Integer defense, Integer experience, Integer visibilityTime, Integer invisibilityTime, MessageCallback message){
        super(character,name,health,attack,defense, message);
        trapChar = character;
        experienceValue = experience;
        this.visibilityTime = visibilityTime;
        this.invisibilityTime = invisibilityTime;
        ticksCount = 0;
        visible = true;
    }

    public void setTicksCount(Integer ticksCount) {
        this.ticksCount = ticksCount;
    }

    public Integer getTicksCount() {
        return ticksCount;
    }

    public Integer getVisibilityTime() {
        return visibilityTime;
    }

    public Integer getInvisibilityTime() {
        return invisibilityTime;
    }

    public char getTrapChar() {
        return trapChar;
    }

    public boolean getVisible(){
        return visible;
    }

    public void setVisible(boolean value){
        this.visible = value;
    }

    @Override
    public void cast() {}

    @Override
    public void enemyTurn() {
        setVisible(getTicksCount() < getVisibilityTime());
        if(getVisible())
            setTile(getTrapChar());
        else
            setTile('.');
        if(getTicksCount() == (getVisibilityTime()+getInvisibilityTime()))
            setTicksCount(0);
        else setTicksCount(getTicksCount()+1);
        Tile p = board.getPlayer();
        if (range(this,p )< 2)
            visit(p);
    }

    @Override
    public void processStep() {}

    @Override
    public String getInfo() {
        return "Name: " + getName()+ ", Health: " + getHealthAmount() + "/" + getHealthPool() +", Attack: " + getAttackPoints() + ", Defense: " + getDefensePoints() + ", Experience value: " + getExperienceValue() + ", Visibility time: " + getVisibilityTime() + ", Invisibility time: " + getInvisibilityTime() + "\n";
    }


    //Equals function for testing only.
    @Override
    public boolean equals(Object _other) {
        if(_other instanceof Trap){
            Trap other = (Trap) _other;
            if (getTile() == other.getTile()) {
                if (getPosition().equals(other.getPosition())) {
                    if (getName().equals(other.getName())) {
                        if (getHealthAmount().equals(other.getHealthAmount())) {
                            if (getHealthPool().equals(other.getHealthPool())) {
                                if (getAttackPoints().equals(other.getAttackPoints())) {
                                    if (getDefensePoints().equals(other.getDefensePoints())) {
                                        if (getExperienceValue().equals(other.getExperienceValue())) {
                                            if (getVisibilityTime().equals(other.getVisibilityTime())) {
                                                if (getInvisibilityTime().equals(other.getInvisibilityTime())) {
                                                    if (getTicksCount().equals(other.getTicksCount())) {
                                                        if (getVisible() == other.getVisible()) {
                                                            if (getTrapChar() == other.getTrapChar())
                                                                return true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        return false;
    }
}
