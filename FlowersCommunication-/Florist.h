#pragma once

#include <vector>
#include <string>
#include "Wholesaler.h"
#include "DeliveryPerson.h"
#include "FlowerArranger.h"
#include "Person.h"

class Florist :public Person {			//Florist class
private:
	Wholesaler* wholesaler;
	FlowerArranger* flowerArranger;
	DeliveryPerson* deliveryPerson;
public:
	Florist(std::string, Wholesaler*, FlowerArranger*, DeliveryPerson*);
	void acceptOrder(Person*, std::vector<std::string>);
	virtual std::string getName();
};