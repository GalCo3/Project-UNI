#include "Person.h"
#include "Wholesaler.h"
#include "Florist.h"
#include "DeliveryPerson.h"
#include "FlowerArranger.h"
#include "Gardener.h"
#include "Grower.h"

int main(int argc, char** argv) {
	Person* chris = new Person("Chris");
	Person* robin = new Person("Robin");
	Gardener* garret = new Gardener("Garret");
	Grower* gray = new Grower("Gray", garret);
	Wholesaler* watson = new Wholesaler("Watson",gray);
	DeliveryPerson* dylan = new DeliveryPerson("Dylan");
	FlowerArranger* flora = new FlowerArranger("Flora");
	Florist* fred = new Florist("Fred", watson, flora, dylan);
	chris->orderFlowers(fred, robin, { "Roses","Violets","Gladiolus"});

	delete chris;
	delete robin;
	delete garret;
	delete gray;
	delete watson;
	delete dylan;
	delete flora;
	delete fred;
}