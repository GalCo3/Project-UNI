from persistence import *

def main():
    #TODO: implement
    c = repo._conn.cursor()

    # print activities table
    c.execute("SELECT * FROM activities ORDER BY date")
    print("Activities")
    for row in c.fetchall():
        print(row)
    print()

    # print branches table
    c.execute("SELECT * FROM branches ORDER BY id")
    print("Branches")
    for row in c.fetchall():
        print(row)
    print()

    # print employees table
    c.execute("SELECT * FROM employees ORDER BY id")
    print("Employees")
    for row in c.fetchall():
        print(row)
    print()

    # print products table
    c.execute("SELECT * FROM products ORDER BY id")
    print("Products")
    for row in c.fetchall():
        print(row)
    print()

    # print suppliers table
    c.execute("SELECT * FROM suppliers ORDER BY id")
    print("Suppliers")
    for row in c.fetchall():
        print(row)
    print()

    # print detailed employees report
    print("Employees report")
    for employee in Employee.find_all():
        emp_branch = Branch.find(employee[3])
        
        c = repo._conn.cursor()
        c.execute("""
            select ABS(SUM(a.quantity * p.price))
            FROM employees e
            JOIN activities a ON e.id = a.activator_id
            JOIN products p ON a.product_id = p.id
            where a.activator_id = ?
         """, [employee[0]])
        sales = c.fetchone()
        if sales[0] == None:
            print("{} {} {} {}".format(employee[1], employee[2], emp_branch[1],0))
        else:
            print("{} {} {} {}".format(employee[1], employee[2], emp_branch[1],sales[0]))
    
    print()
    print("Activities report")
    
    c = repo._conn.cursor()
    c.execute("""
        SELECT a.date, p.description, a.quantity, e.name, s.name
        FROM activities a
        LEFT JOIN employees e ON a.activator_id = e.id
        LEFT JOIN suppliers s ON a.activator_id = s.id
        JOIN products p ON a.product_id = p.id
        ORDER BY a.date
        """)
    activities = c.fetchall()
    for activity in activities:
        print(activity)
    
    
    pass

if __name__ == '__main__':
    main()