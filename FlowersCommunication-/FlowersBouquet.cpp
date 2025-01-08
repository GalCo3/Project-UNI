#include <string>
#include <vector>
#include "FlowersBouquet.h"

FlowersBouquet::FlowersBouquet(std::vector<std::string> _bouquet) : bouquet(_bouquet), is_arranged(false)
{}

void FlowersBouquet::arrange() {
	this->is_arranged = true;
}