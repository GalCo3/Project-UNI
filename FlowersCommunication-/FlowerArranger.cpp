#include <string>
#include <iostream>
#include "FlowerArranger.h"
#include "FlowersBouquet.h"

FlowerArranger::FlowerArranger(std::string _name) : Person(_name)
{}

void FlowerArranger::arrangeFlowers(FlowersBouquet* bouquet) {
	std::cout << this->getName() << " arranges flowers " << std::endl;
	bouquet->arrange();
}

std::string FlowerArranger::getName() {
	return "Flower Arranger " + Person::getName();
}