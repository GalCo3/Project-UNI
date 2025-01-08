#pragma once

#include <string>
#include "FlowersBouquet.h"
#include "Person.h"

class FlowerArranger :public Person {			//FlowerArranger class
public:
	FlowerArranger(std::string);
	void arrangeFlowers(FlowersBouquet*);
	virtual std::string getName();
};

