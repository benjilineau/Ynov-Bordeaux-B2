# TP Final SQL Gelineau Benjamin
## Partie 1 : Conception d'une base de données

J'ai fais le choix pour ma database de crée cinq tables : 

### a. Customers

- Ma table customers contient toutes les informations concernant les clients de l'hotel (nom,prenom,téléphone,chambre...). On leur attribut un numéro de client sous forme d'id qu'on lie au reste des informations. De plus, j'ai décidé de rajouter une collone "breakfast" sous la forme d'un boolean pour savoir si le client a payé pour ce service.

```
CREATE TABLE IF NOT EXISTS customers (
    customerId INTEGER PRIMARY KEY AUTOINCREMENT,
    firstName VARCHAR(255) NOT NULL,
    lastName VARCHAR(255) NOT NULL, 
    phoneNumber VARCHAR(255) NOT NULL,
    emailAddress VARCHAR(255) NOT NULL,
    breakfast BOOLEAN
);
```

### b. Employees

- La table employees comme son nom l'indique contient les informations sur les employées
```
CREATE TABLE IF NOT EXISTS employees (
    employeeId INTEGER PRIMARY KEY AUTOINCREMENT,
    firstName VARCHAR(255) NOT NULL, 
    lastName VARCHAR(255) NOT NULL,
    phoneNumber VARCHAR(255) NOT NULL,
    job VARCHAR(255) NOT NULL,
    emailAddress VARCHAR(255) NOT NULL
);
```

### c. bookings

- La table bookings se charge des réservations. Les informations qu'elle contient sont le numéro de la réservation (bookId), le numéro du client (customerId) et bien sur la chambre attribuée (roomId) ainsi que la date. Elle est chargée de faire le lien entre les chambres et le client.

```
CREATE TABLE IF NOT EXISTS bookings (
    bookId INTEGER PRIMARY KEY AUTOINCREMENT,
    customerId INTEGER NOT NULL,
    date DATE NOT NULL,
    roomId INTEGER NOT NULL
);
```

### d. Rooms

- La table rooms répertorie les chambres de l'hotel. Elle indique l'étage ou se trouve la chambre, le prix et le nombre de couchage.

```
CREATE TABLE IF NOT EXISTS rooms (
    roomId INTEGER PRIMARY KEY AUTOINCREMENT,
    floor INTEGER NOT NULL,
    price INTEGER NOT NULL,
    bedding INTEGER NOT NULL
);
```

### e. Menus

- Pour finir la table menus répertorie les menus proposés au restaurant de l'hotel

```
CREATE TABLE  IF NOT EXISTS menus (
    menusId INTEGER NOT NULL,
    starters VARCHAR(255),
    meets VARCHAR(255),
    fishes VARCHAR(255),
    desserts VARCHAR(255),
    drinks VARCHAR(255),
    price INTEGER NOT NULL
);
```

Voir 📁 Fichier [tp.sql](annexes/tp.sql)
## Partie 2 : Remplir la base de données

- Afin de remplir ma base de données, j'ai utilisé le package faker pour générer des données bouchons dans les différentes tables

Voir 📁 Fichier [tp.py](annexes/tp.py)


## Partie 3 : Analyser une base de données


**1** Le prix total des commandes contenant plus de 5 articles différents

```
SELECT OrderDetail.OrderId, COUNT(OrderDetail.ProductID) AS NbProducts, SUM( OrderDetail.UnitPrice* OrderDetail.Quantity* (1-OrderDetail.Discount)) AS TotalPrice 
FROM OrderDetail
GROUP BY OrderDetail.OrderId
HAVING COUNT(OrderDetail.ProductId) > 5
```

![](https://i.imgur.com/Uei75Pk.png)

**2** La liste de tous les territoires de Peacock Margaret : 

```
SELECT TerritoryDescription FROM Territory
INNER JOIN EmployeeTerritory ON EmployeeTerritory.TerritoryId = Territory.Id
INNER JOIN Employee ON Employee.Id = EmployeeTerritory.EmployeeId
WHERE Employee.FirstName = 'Margaret' AND Employee.LastName = 'Peacock'
```

![](https://i.imgur.com/jh8y5Wu.png)


**3** La liste des clients vivant à "London" : 

```
SELECT ContactName FROM Customer
WHERE City = 'London'
```

![](https://i.imgur.com/BWT1PKR.png)

**4** La liste des clients ayant commandé pour une livraison à "London" avant 2013 : 

```
SELECT DISTINCT "ContactName" FROM "Customer"
INNER JOIN "Order" ON "Order"."CustomerId" = "Customer"."Id"
WHERE "ShipCity" = 'London' AND "OrderDate" < '2013-01-01'
```
![](https://i.imgur.com/3a8XD9S.png)

**6** Afficher la valeur totale de la commande avec l'id 10260

```
SELECT SUM(UnitPrice) AS ValeurTotale FROM OrderDetail
WHERE OrderId = 10260
```

![](https://i.imgur.com/kbMHtU7.png)

**7** Afficher la valeur de toutes les commandes infèrieures à la moyenne de toutes les commandes

```
SELECT UnitPrice FROM OrderDetail
GROUP BY UnitPrice
HAVING AVG(UnitPrice) > UnitPrice
```

![](https://i.imgur.com/VFxTbKI.png)
