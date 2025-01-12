#include "../include/StompProtocol.h"
#include "../include/event.h"
#include <vector>
#include <iostream>
#include <fstream>
using std::cerr;
using std::endl;
using std::ofstream;



StompProtocol::StompProtocol():
subId(0),receiptId(0),
isConnected(false),waitForRecipt(false),userName(""),game_subId(),summeryMap(),queue(),con()
{}
std::vector<std::string> StompProtocol::execute(std::string frame)
{
    std::string::size_type pos = frame.find(' ');
    
    std::string action = frame.substr(0,pos);
    
    frame = frame.substr(pos+1);
    std::vector<std::string> out;
    if (!isConnected)
    {
        if (action != "login")
        {
            out.push_back("error - User is not logged in");
            return out;
        }
        
    }
    
    if (action == "login")
    {
        if (isConnected)
        {
            std::cout <<"The client is already logged in, log out before trying again";
            return out;
        }
        else
        {
            out.push_back(connect(frame));
            return out;
        }
        
    }
    else if (action == "join")
    {
        out.push_back(subcscribe(frame));
        return out;
    }
    else if (action == "exit")
    {
        out.push_back(unsubscribe(frame));
        return out;
    }
    else if (action == "report")
    {
        return send(frame);
    }
    else if (action == "summary")
    {
        summary(frame);
        return out;
    }
    else if (action == "logout")
    {
        out.push_back(disconnect(frame));
        return out;
    }
    // queue.push("error");
    out.push_back("error msg");
    return out;
}

std::string StompProtocol::connect(std::string frame)
{
    
    bool found = frame.find(' ') != std::string::npos;
    if(!found)
        //error
    return"error";


    std::string::size_type pos = frame.find(' ');
    std::string host_port = frame.substr(0,pos);
    frame= frame.substr(pos+1);

    pos = host_port.find(':');
    found = pos != std::string::npos;
    std::string host;
    std::string port;
    if (found)
    {
        host = host_port.substr(0,pos);
        port = host_port.substr(pos+1);
        if ((host.length() == 0) | (port.length() == 0))
        {
            //error
            return "error";
        }
        
    }
    else
    {
        //error
        return "error";
    }
    pos = frame.find(' ');
    found = pos != std::string::npos;
    if(!found)
        //error
    return"error";

    std::string username = frame.substr(0,pos);
    frame = frame.substr(pos+1);

    userName = username;
    std::string out("CONNECT\naccept - version:1.2\nhost:stomp . cs . bgu . ac . il\nlogin:"+ username+
    "\npasscode:"+frame+"\n\n");
    
    // std::cout << out;
    con.start(host,stoi(port));

    if(con.connect())
        isConnected = true;
    else
        out = "error - connection problem";

    return out;
}

std::string StompProtocol::subcscribe(std::string frame)
{
    bool found = frame.find('_') != std::string::npos;
    if (!found)
    {
        //error
        return "error";
    }
    
    std::string out("SUBSCRIBE\ndestination:"+frame+"\nid:"+std::to_string(subId) + "\nreceipt:"+std::to_string(receiptId)+"\n\n");

    receiptId = receiptId+1;
    game_subId[frame] = subId;
    queue.push("join "+frame);
    subId = subId+1;
    // std::cout << out;
    return out;

}

std::string StompProtocol::unsubscribe(std::string frame)
{
    bool found = frame.find('_') != std::string::npos;
    if (!found)
    {
        //error
        return "error not legal game name";
    }

    // if(game_subId.find(frame) == game_subId.end())
    // {
    //     //error
    //     return "error not a memeber of channel - "+frame;
    // }

    std::string out("UNSUBSCRIBE\nid:"+std::to_string(game_subId[frame])+"\nreceipt:"+std::to_string(receiptId)+"\n\n");
    receiptId = receiptId+1;
    queue.push("exit "+frame);
    // std::cout << out;
    return out;
}

std::string StompProtocol::disconnect(std::string frame)
{
    if (frame != "logout")
    {
        //error
        return "error";
    }
    
    std::string out("DISCONNECT\nreceipt:"+std::to_string(receiptId)+"\n\n");
    receiptId = receiptId+1;
    queue.push("logout");
    waitForRecipt = true;
    // std::cout << out;
    return out;
}

std::vector<std::string> StompProtocol::send(std::string frame)
{
    std::vector<std::string>ans =std::vector<std::string>();
    bool found = frame.find(".json") != std::string::npos;
    if (!found)
    {
        //error
        ans.push_back("error not legal json file");
        return ans;
    }

    
    names_and_events events1=parseEventsFile(frame);
    // std::vector<Event>::iterator it;
    unsigned int vecSize = events1.events.size();
    std::string msg="";
    for(unsigned int i = 0; i < vecSize; i++)
    {
        std::string gameName = events1.events[i].get_team_a_name()+"_"+events1.events[i].get_team_b_name();
        if(game_subId.find(gameName) == game_subId.end())
        {
            //error
            ans.push_back("error not a memeber of channel - "+gameName);
            return ans;
        }
    
        msg=
        "SEND\ndestination:"+events1.team_a_name+"_"+events1.team_b_name+"\n\n"

        +"user:"+userName+"\n"
        +"team a: "+events1.team_a_name+"\n"
        +"team b: " +events1.team_b_name+"\n"
        +"event name: "+events1.events[i].get_name()+"\n"
        +"time: "+std::to_string(events1.events[i].get_time())+"\n"
        +"general game updates:\n";
        
        auto map1 = events1.events[i].get_game_updates();
        std::map<std::string, std::string>::iterator it = map1.begin();

        while (it != map1.end())
        {
            msg = msg+"\t"+it->first+": "+it->second+"\n";
            ++it;
        }

        msg = msg+"team a updates:\n";
        
        map1 = events1.events[i].get_team_a_updates();
        it = map1.begin();
        while (it != map1.end())
        {
            msg = msg+"\t"+it->first+": "+it->second+"\n";
            ++it;
        }

        msg = msg+"team b updates:\n";
        
        map1 = events1.events[i].get_team_b_updates();
        it = map1.begin();
        while (it != map1.end())
        {
            msg = msg+"\t"+it->first+": "+it->second+"\n";
            ++it;
        }
        msg = msg+"description:\n" + events1.events[i].get_discription()+"\n";
        // std::cout << msg;
        ans.push_back(msg);
    }
    return ans;   
}

void StompProtocol::summary(std::string frame)
{
    std::string::size_type pos = frame.find(' ');
    bool found = pos != std::string::npos;
    if(!found)
        //error
        return ;
    std::string gameName = frame.substr(0,pos);

    frame = frame.substr(pos+1);

    pos = gameName.find('_');
    found = pos != std::string::npos;
    if (!found)
    {
        //error
        return;
    }
    
    pos = frame.find(' ');
    found = pos != std::string::npos;
    if (!found)
    {
        //error
        return;
    }
    std::string user = frame.substr(0,pos);
    frame = frame.substr(pos+1);

    pos = frame.find(".txt");
    found = pos != std::string::npos;
    if (!found)
    {
        //error
        return;
    }
    //frame holds the file name
    std::vector<Event>& events  = summeryMap[std::pair<std::string,std::string>(user,gameName)];
    // events[events.size()]
    std::map<std::string,std::string> m;
    std::map<std::string,std::string> a;
    std::map<std::string,std::string> b;
    std::map<std::string,std::string>::iterator it;
    
    unsigned int vecSize = events.size();
	
    for(unsigned int i = 0; i < vecSize; i++)
    {
        auto game = events[i].get_game_updates();
        for (it = game.begin(); it != game.end(); it++)
        {
            m[it->first] = it->second;
        }

        game = events[i].get_team_a_updates();
        for (it = game.begin(); it != game.end(); it++)
        {
            a[it->first] = it->second;
        }

        game = events[i].get_team_b_updates();
        for (it = game.begin(); it != game.end(); it++)
        {
            b[it->first] = it->second;
        }
    }

    std::string msgOut;
    if (vecSize>0)
        msgOut =events.front().get_team_a_name() + " vs "+events.front().get_team_b_name()+"\nGame stats:\nGeneral stats:\n";
    
    for (it = m.begin(); it != m.end(); it++)
    {
        msgOut += it->first+": "+it->second+"\n";
    }

    

    msgOut+= events.front().get_team_a_name()+" stats:\n";
    for (it = a.begin(); it != a.end(); it++)
    {
        msgOut += it->first+": "+it->second+"\n";
    }

    msgOut+= events.front().get_team_b_name()+" stats:\n";
    for (it = b.begin(); it != b.end(); it++)
    {
        msgOut += it->first+": "+it->second+"\n";
    }

    msgOut+="Game event reports:\n";
    for(unsigned int i = 0; i < vecSize; i++)
    {
        msgOut+=std::to_string(events[i].get_time())+" - " + events[i].get_name()+":\n\n";
        msgOut+=events[i].get_discription()+"\n\n\n";
    }

    std::ofstream blabla(frame);
    if (blabla.is_open())
    {
        blabla << msgOut;
        blabla.close();
        std::cout<<msgOut;
    }
    

}

void StompProtocol::connect()
{
    isConnected=true;
}

void StompProtocol::disconnect()
{
    isConnected=false;
}

std::string StompProtocol::getLastCommand()
{
    std::string out = queue.front();
    queue.pop();
    return out;
}

void StompProtocol::reset()
{
    subId=0;
    receiptId =0;
    isConnected=false;
    while (!queue.empty())
    {
        queue.pop();
    }
    
    // queue = std::queue<std::string>(); //new empty queue
    userName = "";
    con.close();
    game_subId=std::map<std::string, int>(); //new Map
    summeryMap = std::map<std::pair<std::string,std::string>,std::vector<Event>>();
}

ConnectionHandler* StompProtocol::getConnection()
{
    return &con;
}

bool StompProtocol::is_Connected(){ return isConnected;}

void StompProtocol::addMessage(std::pair<std::string,std::string> user_game,Event event)
{
    summeryMap[user_game].push_back(event);
}

void StompProtocol::recived()
{
    waitForRecipt=false;
}

bool StompProtocol::getWait()
{
    return waitForRecipt;
}