#include <string>
#include <vector>
#include <iostream>
#include "Wholesaler.h"
#include "Grower.h"
#include "FlowersBouquet.h"
#include "Person.h"

Wholesaler::Wholesaler(std::string _name, Grower* _grower) : Person(_name), grower(_grower)
{}

FlowersBouquet* Wholesaler::acceptOrder(std::vector<std::string> bouquet) {
	std::cout << this->getName() << " forwards order to " << grower->getName() << std::endl;
	FlowersBouquet* _bouquet = grower->prepareOrder(bouquet);
	std::cout <<  grower->getName() << " returns flowers to " << this->getName() << std::endl;
	return _bouquet;
}

std::string Wholesaler::getName() {
	return "Wholesaler " + Person::getName();
}