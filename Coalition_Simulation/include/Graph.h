#pragma once
#include <vector>
#include "Party.h"

using std::vector;

class Graph
{
public:
    Graph(vector<Party> vertices, vector<vector<int>> edges);
    int getMandates(int partyId) const;
    int getEdgeWeight(int v1, int v2) const;
    int getNumVertices() const; //total number of parties
    void getPotentialNeighbors(int partyId,vector<int>& vec); //parties we can invite
    const Party &getParty(int partyId) const;
    Party &getParty(int partyId);
    void stepGraph(Simulation& sim);
    bool everyOne_Red() const;

private:
    vector<Party> mVertices;
    vector<vector<int>> mEdges;
};
