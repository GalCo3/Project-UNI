#include <iostream>
#include <vector>
#include <string>
#include "Florist.h"
#include "Wholesaler.h"
#include "FlowerArranger.h"
#include "DeliveryPerson.h"
#include "Person.h"
#include "FlowersBouquet.h"

Florist::Florist(std::string name, Wholesaler* _wholesaler, FlowerArranger* _flowerArranger, DeliveryPerson* _deliveryPerson) : Person(name), wholesaler(_wholesaler), flowerArranger(_flowerArranger), deliveryPerson(_deliveryPerson)
{}

void Florist::acceptOrder(Person* person, std::vector<std::string> bouquet)
{
	std::cout <<  this->getName() << " forwards order to " << wholesaler->getName() << std::endl;
	FlowersBouquet* _bouquet = wholesaler->acceptOrder(bouquet);
	std::cout <<  wholesaler->getName() << " returns flowers to " << this->getName() << std::endl;
	std::cout <<  this->getName() << " requests flower arrangement from " << flowerArranger->getName() << std::endl;
	flowerArranger->arrangeFlowers(_bouquet);
	std::cout << flowerArranger->getName() << " returns arranged flowers to " << this->getName() << std::endl;
	std::cout << this->getName() << " forwards flowers to " << deliveryPerson->getName() << std::endl;
	deliveryPerson->deliver(person, _bouquet);
} 

std::string Florist::getName() {
	return "Florist " + Person::getName();
}