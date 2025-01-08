#pragma once

#include <vector>
#include <string>
#include "Person.h"

class Gardener :public Person {				//Gardener class
public:
	Gardener(std::string);
	FlowersBouquet* prepareOrder(std::vector<std::string>);
	virtual std::string getName();
};
