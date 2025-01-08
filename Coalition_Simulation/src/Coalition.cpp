#include "../include/Coalition.h"
#include "../include/Party.h"
// #include <iostream>
// #include <vector>
// #include <algorithm>


Coalition::Coalition(int id, Party& party): coalitionId(id),mandatesSum(party.getMandates()),partysIds(),partysIdsInvites()
{   
    partysIds.push_back(party.getId());
    party.setState(State::Joined);
    party.setCoalition(coalitionId);
}

bool Coalition:: shouldTerminate() const 
{
    return mandatesSum>= 61;
}

void Coalition::addInvite(int partyId) //add the party we invited to the vector
{
    partysIdsInvites.push_back(partyId);
}

 void Coalition::getIds(vector<int>& v) const
{
    for(int a :partysIds)  //go through all the id's parties and add to the vector
    {
        v.push_back(a);
    }
}


bool Coalition::isInvited(int partyId) // gal need to explain
{
    auto it = partysIdsInvites.begin();

    while (it != partysIdsInvites.end()) 
    {
        if (*it == partyId) //if we invited the party
        {
            return true;
        }
        it++;
        /* code */
    }
    
    // for(const int& partyIdV:partysIdsInvites)
    // {
        
    //     if (partyIdV == partyId)
    //     {
    //         return true;
    //         // partysIdsInvites.
    //     }
        
    // }
    return false;
}


void Coalition::addParty(Party& party)
{
    mandatesSum = mandatesSum + party.getMandates(); //add the number of mandates to the total number of mandates of the coalition
    partysIds.push_back(party.getId()); //add the id of the new party to the vector
    party.setCoalition(coalitionId);
}
int Coalition::getId(){
    return coalitionId;
}
int Coalition::getMandatesSum(){ //return the total amount of mandates
    return mandatesSum;
}
