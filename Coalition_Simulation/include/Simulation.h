#pragma once

#include <vector>

#include "Graph.h"
#include "Agent.h"
#include "Coalition.h"


using std::string;
using std::vector;

class Party;
// class Agent;
// class Party;

class Graph;

class Simulation
{
public:
    Simulation(Graph g, vector<Agent> agents);

    void step();
    bool shouldTerminate() const;

    const Graph &getGraph() const;
    Graph& getGraph();
    
    const vector<Agent> &getAgents() const;
    const Party &getParty(int partyId) const;
    const vector<vector<int>> getPartiesByCoalitions() const;
    Coalition &getCoalition(int coalitionId);
    Party &getParty(int partyId);
    int getIterationCounter();
    
    void newAgent (int agentId,int partyId);

    

private:

    Graph mGraph;
    vector<Agent> mAgents;
    vector<Coalition> mCoalition;
    int iterationCounter;
};
