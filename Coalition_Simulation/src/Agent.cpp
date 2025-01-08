#include "../include/Agent.h"
#include "../include/Graph.h"
#include "../include/SelectionPolicy.h"
#include "../include/Simulation.h"
#include "../include/Coalition.h"

Agent::Agent(int agentId, int partyId, SelectionPolicy *selectionPolicy) : mAgentId(agentId), mPartyId(partyId), mSelectionPolicy(selectionPolicy),mCoalitionId(-1)
{
 // You can change the implementation of the constructor, but not the signature!
     
}
Agent::~Agent() //destructor
{
    if(mSelectionPolicy){
        delete mSelectionPolicy;
    }
}

Agent::Agent(const Agent& other)://copy constructor
    mAgentId(other.mAgentId),mPartyId(other.mPartyId),mSelectionPolicy(other.mSelectionPolicy->clone()),mCoalitionId(other.mCoalitionId)
{
    
}
//move constructor
Agent::Agent(Agent && other) noexcept:

    mAgentId(other.mAgentId),mPartyId(other.mPartyId),mSelectionPolicy(other.mSelectionPolicy),mCoalitionId(other.mCoalitionId)
{
    other.mSelectionPolicy= nullptr;
}

Agent& Agent::operator=(const Agent& other) //copy assignment operator
{
    mAgentId = other.mAgentId;
    mPartyId = other.mPartyId; 
    
    string temp = other.mSelectionPolicy->getSelectionType();

    if (mSelectionPolicy->getSelectionType()!= temp)
    {
        delete mSelectionPolicy;
        if (temp == "M")
        {
            mSelectionPolicy = new MandatesSelectionPolicy();
        }
        else
        {
            mSelectionPolicy = new EdgeWeightSelectionPolicy();
        }
        
    }
    mCoalitionId = other.mCoalitionId;
    return *this;
}

Agent& Agent::operator=(Agent && other) noexcept // move assignment operator
{
    mAgentId = other.mAgentId;
    mPartyId = other.mPartyId; 

    delete mSelectionPolicy;
    mSelectionPolicy = other.mSelectionPolicy;
    other.mSelectionPolicy=nullptr;
    
    mCoalitionId = other.mCoalitionId;
    return *this;
}

void Agent::setCoalition(int coalitionId) { //update the coalition id from -1 to the coalition id that the agent joined her.
    mCoalitionId = coalitionId;
}
int Agent::getId() const
{
    return mAgentId;
}

int Agent::getPartyId() const
{
    return mPartyId;
}

int Agent::getCoalitonId()
{
    return mCoalitionId;
}

void Agent::setId(int id) // update the agent id if the agent duplicated.
{
    mAgentId = id;
}

void Agent::setPartyId(int partyId)
{
    mPartyId = partyId;
}

void Agent::step(Simulation &sim)
{
    vector<int> potentialNeighbors; // vector that contains the neighbors
    sim.getGraph().getPotentialNeighbors(mPartyId,potentialNeighbors);
    vector<int> potentialNeighborsOut; //vector that contains the neighbors we haven't invited yet.
    for(int newPotential: potentialNeighbors)
    {
        if(!sim.getCoalition(mCoalitionId).isInvited(newPotential)){ // if we haven't invide yet
            potentialNeighborsOut.push_back(newPotential); // add him
        }
    }

    mSelectionPolicy->Select(sim,mPartyId,potentialNeighborsOut,mAgentId); // invite the proper party
    // TODO: implement this method
    //chose party from set via Edge/Weight
    // check that you dont alredy ivite this
    // party --> invite
}
