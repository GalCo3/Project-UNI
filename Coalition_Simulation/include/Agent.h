#pragma once

#include <vector>
// #include "SelectionPolicy.h"

class Graph;
class SelectionPolicy;
class Simulation;
class Agent
{
public:
    Agent(int agentId, int partyId, SelectionPolicy *selectionPolicy);
    virtual ~Agent();  //rule of 5 agent
    Agent(const Agent &other);
    Agent(Agent && other) noexcept;
    Agent& operator=(const Agent& other);
    Agent& operator=(Agent && other) noexcept;

    void setId(int id);
    void setPartyId(int partyId);
    void setCoalition(int coalitionId);

    int getPartyId() const;
    int getId() const;
    void step(Simulation & sim);
    int getCoalitonId();

private:
    int mAgentId;
    int mPartyId;
    SelectionPolicy *mSelectionPolicy;
    int mCoalitionId;
    

    
};
