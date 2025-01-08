#pragma once
#include <vector>
#include <string>
class Simulation;
using std::string;
using std::vector;

class SelectionPolicy 
{
    public:
    virtual void Select(Simulation& simulation,int partyId,vector<int>& partyIds,int agentId) = 0; 
    virtual ~SelectionPolicy() = default; // default destrctor
    //party ids after filter partys that someone from coalition already invited
    virtual string getSelectionType() = 0;
    virtual SelectionPolicy* clone () = 0;
};


class MandatesSelectionPolicy: public SelectionPolicy
{
    public:
    void Select(Simulation& simulation,int partyId,vector<int>& partyIds,int agentId);
    string getSelectionType();
    SelectionPolicy* clone();
};

class EdgeWeightSelectionPolicy: public SelectionPolicy
{ 
    public:
    void Select(Simulation& simulation,int partyId,vector<int>& partyIds,int agentId);
    string getSelectionType();
    SelectionPolicy* clone();
};