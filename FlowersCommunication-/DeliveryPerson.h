#pragma once

#include <string>
#include "FlowersBouquet.h"
#include "Person.h"

class DeliveryPerson :public Person {		//DeliveryPerson class
public:
	DeliveryPerson(std::string);
	void deliver(Person*, FlowersBouquet*);
	virtual std::string getName();
};
