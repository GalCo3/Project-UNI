#pragma once

// #include "Agent.h"
#include <vector>
using std::vector;
class Party;

class Coalition {

    public:

    Coalition(int agentId, Party& party);

    bool isInvited(int partyId);//check if an agent from the same coalition has already invited the party (if so, returns true)
    void addInvite(int partyId); //invite the party only if "isInvited" returned false
    void addParty(Party& party);
    bool shouldTerminate() const; //check if we are done
    int getId();
    int getMandatesSum();
    void getIds(vector<int>& v) const;  //returns a list of ids of all parties in the coalition 
    
    private:

    int coalitionId;
    int mandatesSum;

    vector <int> partysIds; //vector of ids of parties in the coalition
    vector <int> partysIdsInvites; //vector of id's of parties that agents from the coalition have already invited

};