#include "../include/Simulation.h"

Simulation::Simulation(Graph graph, vector<Agent> agents) : mGraph(graph), mAgents(agents) ,mCoalition(vector<Coalition>()),iterationCounter(0)
{
    // You can change the implementation of the constructor, but not the signature!
    // mCoalition;
    for(const Agent& agent:mAgents)
    {
        mCoalition.push_back(Coalition(agent.getId(),getParty(agent.getPartyId())));
    }

    int i = 0;
    auto it = mAgents.begin();

    while (it != mAgents.end())
    {
        mAgents[i].setCoalition(i);
        it++;
        i++;
    }
    
    
}

void Simulation::step()
{
    
    mGraph.stepGraph(*this); //step for partys -- because this is a private field in graph

    for(Agent& agent : mAgents)
    {
        agent.step(*this);
    }

    iterationCounter++;
}

void Simulation::newAgent(int agentId,int partyId) //create the replicated agent
{
    mAgents.push_back(Agent(mAgents[agentId])); // add to the vector of agents
    mAgents[mAgents.size()-1].setId(mAgents.size()-1);
    mAgents[mAgents.size()-1].setPartyId(partyId);
    mAgents[mAgents.size()-1].setCoalition(mAgents[agentId].getCoalitonId());
}

bool Simulation::shouldTerminate() const
{
    // TODO implement this method
    // check if coalition has 61 mandated or everyone join
    for(const Coalition& coalition : mCoalition)
    {
        if(coalition.shouldTerminate())
        {return true;}
    }
    
    if(mGraph.everyOne_Red())
        return true;

    return false;
}

const Graph &Simulation::getGraph() const
{
    return mGraph;
}

Graph &Simulation::getGraph()
{
    return mGraph;
}

const vector<Agent> &Simulation::getAgents() const
{
    return mAgents;
}

const Party &Simulation::getParty(int partyId) const
{
    return mGraph.getParty(partyId);
}

Party &Simulation::getParty(int partyId)
{
    return mGraph.getParty(partyId);
}

/// This method returns a "coalition" vector, where each element is a vector of party IDs in the coalition.
/// At the simulation initialization - the result will be [[agent0.partyId], [agent1.partyId], ...]
const vector<vector<int>> Simulation::getPartiesByCoalitions() const
{
    // TODO: you MUST implement this method for getting proper output, read the documentation above.
    
    vector<vector<int>> out;
    
    for(const Coalition &c :mCoalition)
    {
        out.push_back(vector<int>());
        c.getIds(out[out.size()-1]);
    }
    return out;
}

Coalition& Simulation::getCoalition(int coalitionId)
{
    return mCoalition[coalitionId];
}

int Simulation::getIterationCounter(){
    return iterationCounter;
}

