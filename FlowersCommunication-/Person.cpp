#include <iostream>
#include <vector>
#include <string>
#include "Person.h"
#include "Florist.h"
#include "FlowersBouquet.h"

Person::Person(std::string _name) { name = _name; }

std::string Person::getName() { return name; }

void Person::orderFlowers(Florist* florist, Person* person, std::vector<std::string> bouquet)
{
	std::string Bouquet = "";
	for (auto i = bouquet.begin();i != bouquet.end(); ++i)
		Bouquet = Bouquet + * i + " ";
	std::cout << this->name << " orders flowers to " << person->getName() << " from " << florist->getName() << " : " << Bouquet << std::endl;
	florist->acceptOrder(person, bouquet);
}

void Person::acceptFlowers(FlowersBouquet* bouquet) 
{
	std::string Bouquet = "";
	for (auto i = bouquet->bouquet.begin();i != bouquet->bouquet.end(); ++i)
		Bouquet = Bouquet + *i + " ";
	std::cout << this->name << " recieved flowers" <<  ": " << Bouquet << std::endl;
}
