from persistence import *

import sys
import os

def add_branche(splittedline : list[str]):
    #TODO: add the branch into the repo
    cursor = repo._conn.cursor()
    sqlite_insert_with_param = '''INSERT INTO branches(id,location,number_of_employees) VALUES(?,?,?)'''
    cursor.execute(sqlite_insert_with_param, splittedline)
    repo._conn.commit()
    pass

def add_supplier(splittedline : list[str]):
    #TODO: insert the supplier into the repo
    cursor = repo._conn.cursor()
    sqlite_insert_with_param = '''INSERT INTO suppliers(id,name,contact_information) VALUES(?,?,?)'''
    cursor.execute(sqlite_insert_with_param, splittedline)
    repo._conn.commit()
    pass

def add_product(splittedline : list[str]):
    #TODO: insert product
    cursor = repo._conn.cursor()
    sqlite_insert_with_param = '''INSERT INTO products(id,description,price,quantity) VALUES(?,?,?,?)'''
    cursor.execute(sqlite_insert_with_param, splittedline)
    repo._conn.commit()
    pass

def add_employee(splittedline : list[str]):
    #TODO: insert employee
    cursor = repo._conn.cursor()
    sqlite_insert_with_param = '''INSERT INTO employees(id,name,salary,branche) VALUES(?,?,?,?)'''
    cursor.execute(sqlite_insert_with_param, splittedline)
    repo._conn.commit()
    pass

adders = {  "B": add_branche,
            "S": add_supplier,
            "P": add_product,
            "E": add_employee}

def main(args : list[str]):
    inputfilename = args[1]
    # delete the database file if it exists
    repo._close()
    # uncomment if needed
    if os.path.isfile("bgumart.db"):
        os.remove("bgumart.db")
    repo.__init__()
    repo.create_tables()
    with open(inputfilename) as inputfile:
        for line in inputfile:
            splittedline : list[str] = line.strip().split(",")
            adders.get(splittedline[0])(splittedline[1:])
    

if __name__ == '__main__':
    main(sys.argv)