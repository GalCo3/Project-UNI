from persistence import *

import sys

def main(args : list[str]):
    inputfilename : str = args[1]
    with open(inputfilename) as inputfile:
        for line in inputfile:
            splittedline : list[str] = line.strip().split(", ")
            #TODO: apply the action (and insert to the table) if possible
            prod = Product.find(int(splittedline[0]))
            if int(splittedline[1]) < 0 :
                #sold
                if abs(int(splittedline[1])) <= prod.quantity:
                    #add to activity table
                    Activity.insert(splittedline[0],splittedline[1],splittedline[2],splittedline[3])
                    #update product table
                    Product.update(prod.id,prod.quantity+int(splittedline[1]))
            
            elif int(splittedline[1]) > 0:
                #add to activity
                Activity.insert(splittedline[0],splittedline[1],splittedline[2],splittedline[3])
                #update product table
                Product.update(prod.id,prod.quantity+int(splittedline[1]))
                

if __name__ == '__main__':
    main(sys.argv)