#pragma once

#include <string>
#include <vector>

class Florist;
class FlowersBouquet;
class Wholesaler;
class Gardener;
class Grower;
class FlowerArranger;
class DeliveryPerson;

class Person {							//Person class
protected:
	std::string name;
public:
	Person(std::string);
	std::string getName();
	void orderFlowers(Florist*, Person*, std::vector<std::string>);
	void acceptFlowers(FlowersBouquet*);
};