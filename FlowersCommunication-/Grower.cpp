#include <iostream>
#include <vector>
#include <string>
#include "Person.h"
#include "Gardener.h"
#include "FlowersBouquet.h"
#include "Grower.h"

Grower::Grower(std::string _name,Gardener* _gardener) : Person(_name), gardener(_gardener)
{}

FlowersBouquet* Grower::prepareOrder(std::vector<std::string> bouquet) {
	std::cout << this->getName() << " forwards order to " << gardener->getName() << std::endl;
	FlowersBouquet* _bouquet = gardener->prepareOrder(bouquet);
	std::cout << gardener->getName() << " returns flowers to " << this->getName() << std::endl;
	return _bouquet;
}

std::string Grower::getName() {
	return "Grower " + Person::getName();
}