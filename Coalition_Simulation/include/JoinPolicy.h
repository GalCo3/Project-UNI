#pragma once
#include <string>
using std::string;

class Party;
class Simulation;

class JoinPolicy {

    public:
    virtual void join(Party& party,Simulation& sim) = 0; //  gal need to explain
    virtual ~JoinPolicy() = default;
    virtual string getJoinType() = 0;
    virtual JoinPolicy* clone () = 0;
};

class MandatesJoinPolicy : public JoinPolicy 
{
    public:
    JoinPolicy* clone();
    void join(Party& party,Simulation& sim); 
    string getJoinType();
};

class LastOfferJoinPolicy : public JoinPolicy 
{
    public:
    // ~LastOfferJoinPolicy();
    JoinPolicy* clone() ;
    void join(Party& party,Simulation& sim);
    string getJoinType(); // return 'M' to MandatesJoinPolicy or 'L' to LastOfferJoinPolicy
};