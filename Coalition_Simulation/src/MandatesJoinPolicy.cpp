#include  "../include/JoinPolicy.h"
#include  "../include/Party.h"



JoinPolicy* MandatesJoinPolicy::clone()
{
    return new MandatesJoinPolicy();
}

void MandatesJoinPolicy::join(Party& party,Simulation& sim){

    party.join(party.getCoalitionIdMostMandates(), sim,1); // if which==1 --> we select the mostMandates agent
}

string MandatesJoinPolicy::getJoinType()
{
    return "M";
}

