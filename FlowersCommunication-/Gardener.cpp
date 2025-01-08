#include <string>
#include <vector>
#include <iostream>
#include "Person.h"
#include "Gardener.h"
#include "FlowersBouquet.h"

Gardener::Gardener(std::string _name) : Person(_name)
{}

FlowersBouquet* Gardener::prepareOrder(std::vector<std::string> bouquet) {
	std::cout << this->getName() << " prepares flowers" << std::endl;
	return new FlowersBouquet(bouquet);
}

std::string Gardener::getName() {
	return "Gardener " + Person::getName();
}