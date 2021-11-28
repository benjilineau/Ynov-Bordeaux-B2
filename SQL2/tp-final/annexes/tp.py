from enum import auto
from faker import Faker
from random import randrange
from random import choice
fake_data = Faker()
import sqlite3
connection = sqlite3.connect('tp.db')
cursor = connection.cursor()

customerId = []
roomId = []

itCust= 0
for i in range(10):
    firstName = fake_data.first_name()
    name = fake_data.last_name()
    itCust +=1
    customerId.append(itCust)
    cursor.execute('INSERT INTO customers VALUES(:customerId,:firstName,:lastName,:phoneNumber,:emailAddress,:breakfast)',
        {'customerId':itCust,'firstName': firstName,'lastName': name,'phoneNumber':fake_data.phone_number(),'emailAddress':firstName + "@calvinleboss.org",'breakfast':fake_data.boolean()}
        )

itEmpl = 0
for i in range(10):
    firstName = fake_data.first_name()
    name = fake_data.last_name()
    itEmpl +=1
    cursor.execute('INSERT INTO employees VALUES(:employeeId,:firstName,:lastName,:phoneNumber,:title,:emailAddress)',
        {'employeeId':itEmpl,'firstName': firstName,'lastName': name,'phoneNumber':fake_data.phone_number(),'title':fake_data.job(),'emailAddress':firstName + "@calvinleboss.org"}
        )


itRoom = 0
for i in range(10):
    itRoom+=1
    roomId.append(itRoom)
    cursor.execute('INSERT INTO rooms VALUES(:roomId,:floor,:price,:bedding)',
        {'roomId':itRoom,'floor':randrange(1,8),'price':randrange(50,80),'bedding':randrange(1,5)}
        )


for i in range(10):
    bookClient = choice(customerId)
    roomClient = choice(roomId)
    customerId.remove(bookClient)
    roomId.remove(roomClient)
    cursor.execute('INSERT INTO bookings VALUES(:bookId,:customerId,:date,:roomId)',
        {'bookId':i,'customerId':bookClient,'roomId':roomClient,'date':fake_data.date()}
        )

connection.execute("""
INSERT INTO menus(menusId,starters,meets,fishes,desserts,drinks,price) VALUES( ?, ?, ?, ?, ?, ?, ?)""", (1, "Saumon", "Boeuf bourguignon", "Dorade","Crepe","Champagne/soda",30))

connection.execute("""
INSERT INTO menus(menusId,starters,meets,fishes,desserts,drinks,price) VALUES( ?, ?, ?, ?, ?, ?, ?)""", (2,"Foie gras", "demi-poulet", "Sole meunière","glace","vin blanc/soda",25))

connection.commit()
connection.close()

print("injection effectuée !")

