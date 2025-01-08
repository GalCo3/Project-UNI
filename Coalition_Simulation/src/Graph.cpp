#include "../include/Graph.h"

Graph::Graph(vector<Party> vertices, vector<vector<int>> edges) : mVertices(vertices), mEdges(edges) 
{
    // You can change the implementation of the constructor, but not the signature!
}

int Graph::getMandates(int partyId) const //return the number of mandates of a party
{
    return mVertices[partyId].getMandates();
}

int Graph::getEdgeWeight(int v1, int v2) const //return the weight of edge
{
    return mEdges[v1][v2];
}

int Graph::getNumVertices() const //total number of parties
{
    return mVertices.size();
}

void Graph::getPotentialNeighbors(int partyId,vector<int>& vec)
{
    for (Party party:mVertices) //go over all the parties in the graph
    {
        if (!(party.getId() == partyId) & !(mEdges[partyId][party.getId()] == 0) ) //if the party is not me and I am also its neighbor
        {
            if (mVertices[party.getId()].getState()!=State::Joined) //if the party is not "joined"
            {
                vec.push_back(party.getId()); //add it to the potential invite
            }
            
        }
        
    }
    
}

const Party &Graph::getParty(int partyId) const
{
    return mVertices[partyId]; 
}

Party &Graph::getParty(int partyId)
{
    return mVertices[partyId];
}

void Graph::stepGraph(Simulation& sim)
{
    for(Party& party : mVertices)
    {
        party.step(sim); //party step
    }
}


bool Graph::everyOne_Red() const{ //check whether the status of the party is "joined"
    for(const Party& party: mVertices){
        if(party.getState()!=State::Joined)
            return false;
    }
    return true;
}