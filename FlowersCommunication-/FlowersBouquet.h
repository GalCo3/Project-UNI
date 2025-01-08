#pragma once
#include <vector>
#include <string>

class FlowersBouquet {						//FlowerBouquet class
private:
	bool is_arranged;
public:
	std::vector<std::string> bouquet;
	FlowersBouquet(std::vector<std::string>);
	void arrange();
	
};
