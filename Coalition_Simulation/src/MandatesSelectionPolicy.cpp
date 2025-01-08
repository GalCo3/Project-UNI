#include "../include/SelectionPolicy.h"
#include  "../include/Simulation.h"


void MandatesSelectionPolicy::Select(Simulation& simulation,int partyId,vector<int>& partyIds,int agentId){
    int maxMandats=0;
    int maxId=-1;
    for(int partyIdV: partyIds){
        if(simulation.getParty(partyIdV).getMandates()>maxMandats){
            maxMandats=simulation.getParty(partyIdV).getMandates();
            maxId=partyIdV;
        }
    }
    simulation.getParty(maxId).invite(simulation.getCoalition(simulation.getParty(partyId).getCoalition()),partyId,agentId,simulation.getIterationCounter());//invite the party
    simulation.getCoalition(simulation.getParty(partyId).getCoalition()).addInvite(maxId); // add to coalition the invite
    
}
SelectionPolicy* MandatesSelectionPolicy::clone(){
    return new MandatesSelectionPolicy();
}
string MandatesSelectionPolicy::getSelectionType()
{
    return "M";
}
