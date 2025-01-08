import sqlite3
import atexit
from dbtools import Dao
 
# Data Transfer Objects:
class Employee(object):
    def __init__(self, id: int, name: str, salary: float, branche: int):
        self.id = id
        self.name = name
        self.salary = salary
        self.branche = branche
    
    def find_all():
        c = repo._conn.cursor()
        c.execute("""
             SELECT * FROM employees
             ORDER By name
         """)
        # return Product(*c.fetchall())
        return c.fetchall()

class Supplier(object):
    def __init__(self, id: int, name: str, contact_information: str):
        self.id = id
        self.name = name
        self.contact_information = contact_information

class Product(object):
    def __init__(self, id: int, description: str, price: float, quantity: int):
        self.id = id
        self.description = description
        self.price = price
        self.quantity = quantity

    def find(product_id):
         c = repo._conn.cursor()
         c.execute("""
             SELECT * FROM products WHERE id = ?
         """, [product_id])
         return Product(*c.fetchone())
    
    def update(id,quantity):
        repo._conn.execute("""
               UPDATE products SET quantity=(?) WHERE id=(?)
           """, [quantity,id])

class Branch(object):
    def __init__(self, id: int, location: str, number_of_employees: int):
        self.id = id
        self.location = location
        self.number_of_employees = number_of_employees

    def find(id):
        c = repo._conn.cursor()
        c.execute("""
             SELECT * FROM branches WHERE id = ?
         """, [id])
        return c.fetchone()


class Activity(object):
    def __init__(self, product_id: int, quantity: int, activator_id: int, date: str):
        self.product_id = product_id
        self.quantity = quantity
        self.activator_id = activator_id
        self.date = date

    def insert(product_id,quantity,activator_id,date):
         repo._conn.execute("""
             INSERT INTO activities (product_id, quantity, activator_id,date ) VALUES (?, ?, ?, ?)
         """, [product_id, quantity,activator_id,date])

    def find_all():
        c = repo._conn.cursor()
        c.execute("""
             SELECT * FROM activities
             order by date
         """)
        # return Product(*c.fetchall())
        return c.fetchall()
 
#Repository
class Repository(object):
    def __init__(self):
        self._conn = sqlite3.connect('bgumart.db')
        self._conn.text_factory = str #str
        self.employees = Dao(Employee,self._conn)
        self.supplieres = Dao(Supplier,self._conn)
        self.products = Dao(Product,self._conn)
        self.branches = Dao(Branch,self._conn)
        self.activities = Dao(Activity,self._conn)
        self.activities._table_name = "activities"
        
        #TODO: complete


 
    def _close(self):
        self._conn.commit()
        self._conn.close()
 
    def create_tables(self):
        self._conn.executescript("""
            CREATE TABLE employees (
                id              INT         PRIMARY KEY,
                name            TEXT        NOT NULL,
                salary          REAL        NOT NULL,
                branche    INT REFERENCES branches(id)
            );
    
            CREATE TABLE suppliers (
                id                   INTEGER    PRIMARY KEY,
                name                 TEXT       NOT NULL,
                contact_information  TEXT
            );

            CREATE TABLE products (
                id          INTEGER PRIMARY KEY,
                description TEXT    NOT NULL,
                price       REAL NOT NULL,
                quantity    INTEGER NOT NULL
            );

            CREATE TABLE branches (
                id                  INTEGER     PRIMARY KEY,
                location            TEXT        NOT NULL,
                number_of_employees INTEGER
            );
    
            CREATE TABLE activities (
                product_id      INTEGER REFERENCES products(id),
                quantity        INTEGER NOT NULL,
                activator_id    INTEGER NOT NULL,
                date            TEXT    NOT NULL
            );
        """)

    def execute_command(self, script: str) -> list:
        return self._conn.cursor().execute(script).fetchall()
 
# singleton
repo = Repository()
atexit.register(repo._close)