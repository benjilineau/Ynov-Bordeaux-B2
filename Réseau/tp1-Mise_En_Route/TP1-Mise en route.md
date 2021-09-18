# TP1-Mise en route
## I. Exploration locale en solo
### 1. Affichage d'informations sur la pile TCP/IP locale

#### Affichez les infos des cartes réseau de votre PC
- Grâce à la commande ```ipconfig``` j'obtiens le nom : ```Carte réseau sans fil Wi-Fi```, son ```Adresse IPv4. . . . . . . . . . . . . .: 10.33.2.217```. L'adresse mac je l'obtiens avec la commande ``` ipconfig /all ``` : ```Adresse physique . . . . . . . . . . . : 04-6C-59-0E-C4-91```
- N'ayant pas de port ethernet je ne peux trouver son nom, son ip... mais si j'en possédais une, je l'aurais trouvé avec la commande ```ipconfig /all``` pour peux que je sois connecté à un réseau grâce au port ethernet.

#### Afficher votre gateway
- Toujours avec la commande ```ipconfig /all```, j'obtiens ma passerelle : ```Passerelle par défaut. . . . . . . . . : 10.33.3.253```.

#### Avec l'interface graphique
![](https://i.imgur.com/gx5Jeuo.png)

#### Question 
- La gateway d'ynov nous permet d'accéder aux autres réseaux.

### Modification des informations

- Afin de changer mon adresse IP, j'ai dû passer en manuel dans mes parametre adressage.

![](https://i.imgur.com/AUSoetl.png)



- Il est possible de perdre la connexion internet car nous possédons peut etre une adresse ip déjà attribuer ou non géré par le réseau.

### Table ARP
#### Exploration de la table ARP

- Pour afficher la table ARP j'utilise la commande `arp -a`
j'identifie l'adresse mac de la passerelle rapidement car son adresse physique n'est pas vide:
 ``` Interface : 10.33.2.217 --- 0xe
  Adresse Internet      Adresse physique      Type
  10.33.2.88            68-3e-26-7c-c3-1a     dynamique
  10.33.3.105           54-14-f3-b5-aa-36     dynamique 
  ``` 
- Afin de remplir ma table arp, j'ai ping des machines du réseau avec la commande : ```ping + ip ```
```Interface : 10.33.2.217 --- 0xe
  Adresse Internet      Adresse physique      Type
  10.33.0.7             9c-bc-f0-b6-1b-ed     dynamique
  10.33.0.27            a8-64-f1-8b-1d-4d     dynamique
  10.33.0.43            2c-8d-b1-94-38-bf     dynamique
  10.33.0.60            e2-ee-36-a5-0b-8a     dynamique
  10.33.0.71            f0-03-8c-35-fe-47     dynamique
  ```



### Nmap

- Après avoir téléchargé nmap, j'ai lancé un ping scan sur le réseau d'ynov pour obtenir les adresses ip occupées. Commande : ```nmap -sn 10.33.0.0/22```
![](https://i.imgur.com/waesWcT.png)

#### Utilisez nmap pour scanner le réseau de votre carte WiFi et trouver une adresse IP libre
- Les adresses ip librent sont celles qui ne figurent pas dans ma table arp ci-dessous.



```Interface : 10.33.2.217 --- 0xe
  Adresse Internet      Adresse physique      Type
  10.33.0.7             9c-bc-f0-b6-1b-ed     dynamique
  10.33.0.27            a8-64-f1-8b-1d-4d     dynamique
  10.33.0.43            2c-8d-b1-94-38-bf     dynamique
  10.33.0.60            e2-ee-36-a5-0b-8a     dynamique
  10.33.0.71            f0-03-8c-35-fe-47     dynamique
  10.33.0.96            ca-4f-f4-af-8f-0c     dynamique
  10.33.0.100           e8-6f-38-6a-b6-ef     dynamique
  10.33.0.119           18-56-80-70-9c-48     dynamique
  10.33.0.135           f8-5e-a0-06-40-d2     dynamique
  10.33.0.140           40-ec-99-8b-11-c2     dynamique
  10.33.0.148           e6-aa-26-ee-23-b7     dynamique
  10.33.0.211           e8-d0-fc-ef-9e-af     dynamique
  10.33.0.242           74-4c-a1-51-1e-61     dynamique
  10.33.0.250           28-cd-c4-dd-db-73     dynamique
  10.33.1.166           1a-41-0b-54-a5-a0     dynamique
  10.33.1.228           34-7d-f6-b2-82-cf     dynamique
  10.33.1.238           50-76-af-88-6c-0b     dynamique
  10.33.1.242           34-7d-f6-5a-20-da     dynamique
  10.33.1.243           34-7d-f6-5a-20-da     dynamique
  10.33.1.248           e8-84-a5-24-94-c9     dynamique
  10.33.2.3             e0-2b-e9-42-5e-91     dynamique
  10.33.2.24            02-24-5e-d2-c9-1e     dynamique
  10.33.2.48            80-91-33-9c-bf-0d     dynamique
  10.33.2.56            c0-3c-59-a9-e0-75     dynamique
  10.33.2.88            68-3e-26-7c-c3-1a     dynamique
  10.33.2.105           ec-2e-98-ca-da-e9     dynamique
  10.33.2.151           10-32-7e-38-50-c3     dynamique
  10.33.2.173           34-2e-b7-47-f9-28     dynamique
  10.33.2.182           f4-4e-e3-c0-ed-29     dynamique
  10.33.2.216           08-d2-3e-35-00-a2     dynamique
  10.33.3.59            02-47-cd-3d-d4-e9     dynamique
  10.33.3.74            40-ec-99-f0-81-b0     dynamique
  10.33.3.80            3c-58-c2-9d-98-38     dynamique
  10.33.3.89            f0-9e-4a-52-94-f0     dynamique
  10.33.3.105           54-14-f3-b5-aa-36     dynamique
  10.33.3.117           d0-c5-d3-8c-6c-f9     dynamique
  ```
### Modification d'adresse IP (part 2)

- Afin de trouver une adresse libre, j'ai lancé un scan nmap puis j'ai sélectionné une adresse comprise entre deux adresses ip bien éloigné. Ensuite j'ai changé mon ip dans les parametres de mon os: 
![](https://i.imgur.com/9yXEzWa.png)
Ensuite j'ai lancé un scan nmap avec la commande ```nmap -sn 10.33.0.0/22```,  voici le résultat :
![](https://i.imgur.com/6fixVIR.png)

Pour prouver que j'ai bien changé d'ip et que ma passerelle est bien définie, j'ai fais ```ipconfig``` suivie d'un ```ping 8.8.8.8``` pour attester de ma connexion à internet :
``` 
C:\Users\Benjamin>ipconfig 
    Carte réseau sans fil Wi-Fi :

   Suffixe DNS propre à la connexion. . . :
   Adresse IPv6 de liaison locale. . . . .: fe80::7c07:df1d:1125:26ca%14
   Adresse IPv4. . . . . . . . . . . . . .: 10.33.3.152
   Masque de sous-réseau. . . . . . . . . : 255.255.252.0
   Passerelle par défaut. . . . . . . . . : 10.33.3.253 
   
C:\Users\Benjamin>ping 8.8.8.8

Envoi d’une requête 'Ping'  8.8.8.8 avec 32 octets de données :
Réponse de 8.8.8.8 : octets=32 temps=18 ms TTL=115
Réponse de 8.8.8.8 : octets=32 temps=18 ms TTL=115
Réponse de 8.8.8.8 : octets=32 temps=18 ms TTL=115
Réponse de 8.8.8.8 : octets=32 temps=21 ms TTL=115
```
## II. Exploration locale en duo
### Modification d'adresse ip
**Réalisé avec ABADIE Arthur**

- Dans les parametres de mon port ethernet, je passe en adressage ip manuel et la configure comme ceci :```ip : 192.168.10.1/30 ``` pour n'avoir que 2 ip disponibles.
Je vérifie le changement avec ```ipconfig```:
```
Carte Ethernet Ethernet 2 :

   Suffixe DNS propre à la connexion. . . :
   Adresse IPv6 de liaison locale. . . . .: fe80::cd8:a4ce:18b3:b5fe%6
   Adresse IPv4. . . . . . . . . . . . . .: 192.168.10.1
   Masque de sous-réseau. . . . . . . . . : 255.255.255.252
```

Test de ping avec mon camarade : 
```
C:\Users\Benjamin>ping 192.168.10.2

Envoi d’une requête 'Ping'  192.168.10.2 avec 32 octets de données :
Réponse de 192.168.10.2 : octets=32 temps<1ms TTL=128
Réponse de 192.168.10.2 : octets=32 temps<1ms TTL=128
Réponse de 192.168.10.2 : octets=32 temps<1ms TTL=128
Réponse de 192.168.10.2 : octets=32 temps<1ms TTL=128

Statistiques Ping pour 192.168.10.2:
    Paquets : envoyés = 4, reçus = 4, perdus = 0 (perte 0%),
Durée approximative des boucles en millisecondes :
    Minimum = 0ms, Maximum = 0ms, Moyenne = 0ms
```

###  Utilisation d'un des deux comme gateway

Afin de partager ma connexion internet a mon camarade, je partage la connexion d'ynov à mon ethernet.
![](https://i.imgur.com/5NpaJCk.png)
Voici la preuve que mon camarade a internet via ma connexion. 
```
C:\Windows\system32>ping 8.8.8.8

Envoi d’une requête 'Ping'  8.8.8.8 avec 32 octets de données :
Réponse de 8.8.8.8 : octets=32 temps=148 ms TTL=114
Réponse de 8.8.8.8 : octets=32 temps=497 ms TTL=114
Réponse de 8.8.8.8 : octets=32 temps=275 ms TTL=114
Réponse de 8.8.8.8 : octets=32 temps=103 ms TTL=114
```
J'utilise la commande : ```traceroute 192.168.10.2```
```
C:\Windows\system32>tracert 192.168.10.2

Détermination de l’itinéraire vers DESKTOP-ON7BJGL [192.168.10.2]
avec un maximum de 30 sauts :

  1    <1 ms    <1 ms    <1 ms  DESKTOP-ON7BJGL [192.168.10.2]

Itinéraire déterminé
```
### 5. Petit chat privé
Afin de communiquer avec mon camarade, je demande à netcat de se mettre en écoute sur le port 8888.
![](https://i.imgur.com/vjyfH6r.png)

#### Pour aller plus loin
Ici je n'autorise que l'ip de mon camarade sur mon port 9999.
```
Cmd line: -l -p 9999 192.168.10.2
I am blue
dabeudi dabeuda
dabeudi dabeuda
dabeudi dabeuda
```

### 6. Firewall

Afin d'autoriser les ping à travers le firewall, je vais mettre en place des exceptions dans le protocole de mon firewall à l'aide des commandes suivantes en mode administrateur dans l'invite de commande : 
```
netsh advfirewall firewall add rule name="ICMP Allow incoming V4 echo request" protocol=icmpv4:8,any dir=in action=allow
```
```
netsh advfirewall firewall add rule name="ICMP Allow incoming V6 echo request" protocol=icmpv6:8,any dir=in action=allow
```

## III. Manipulations d'autres outils/protocoles côté client

### DHCP
Avec ipconfig /all j'obtiens : 
```
    Bail obtenu. . . . . . . . . . . . . . : vendredi 17 septembre 2021 15:03:45
    Bail expirant. . . . . . . . . . . . . : vendredi 17 septembre 2021 17:03:46
    Serveur DHCP . . . . . . . . . . . . . : 10.33.3.254
```

### Dns
Avec ipconfig /all : 
```
Serveurs DNS. . .  . . . . . . . . . . : 10.33.10.2
                                       10.33.10.148
                                       10.33.10.155
```


Je réalise la commande ```nslookup``` avec : 
ynov.com = 92.243.16.143
google.com = 216.58.213.78

L'ip du serveur auquel nous avons fait les requêtes : 10.33.10.2

Maintenant on inverse commande : 
78.74.21.21 = host-78-74-21-21.homerun.telia.com

92.146.54.88 = apoitiers-654-1-167-88.w92-146.abo.wanadoo.fr

### Wireshark

**Réalisé avec CURMI Thomas, LEVEE Dorian, TARENDEAU Gael**

Je mets en évidence le ping entre la passerelle et moi : 
![](https://i.imgur.com/Qrw7nFD.png)

Maintenant un netcat entre mon camarade et moi branché en RJ45 : 
![](https://i.imgur.com/Z0NAPQo.png)

Et pour finir voici la requête vers le serveur DNS : 

![](https://i.imgur.com/49sFBEj.png)
