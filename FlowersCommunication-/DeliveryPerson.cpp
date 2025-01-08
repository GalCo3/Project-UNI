#include <string>
#include <iostream>
#include "Person.h"
#include "FlowersBouquet.h"
#include "DeliveryPerson.h"

DeliveryPerson::DeliveryPerson(std::string _name) : Person(_name)
{}

void DeliveryPerson::deliver(Person* recipient, FlowersBouquet* bouquet)
{
	std::cout << this->getName() << " delivers flowers to " << recipient->getName() << std::endl;
	recipient->acceptFlowers(bouquet);

}

std::string DeliveryPerson::getName() {
	return "Delivery Person " + Person::getName();
}

