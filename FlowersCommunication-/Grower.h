#pragma once

#include <vector>
#include <string>
#include "Person.h"
#include "FlowersBouquet.h"
#include "Gardener.h"

class Grower :public Person {				//Grower class
private:
	Gardener* gardener;
public:
	Grower(std::string, Gardener*);
	FlowersBouquet* prepareOrder(std::vector<std::string>);
	virtual std::string getName();
};

