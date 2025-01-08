#pragma once

#include <vector>
#include <string>
#include "Grower.h"
#include "FlowersBouquet.h"
#include "Person.h"

class Wholesaler :public Person {			//Wholesaler class
private:
	Grower* grower;
public:
	Wholesaler(std::string, Grower*);
	FlowersBouquet* acceptOrder(std::vector<std::string>);
	virtual std::string getName();
};
