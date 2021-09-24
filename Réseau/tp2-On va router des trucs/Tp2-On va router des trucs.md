# Tp2-On va router des trucs

## 1-ARP

### Echange ARP
J'effectue le ping d'une machine à l'autre : 
``` 
[benji@localhost ~]$ ping 10.2.1.12
PING 10.2.1.12 (10.2.1.12) 56(84) bytes of data.
64 bytes from 10.2.1.12: icmp_seq=1 ttl=64 time=0.420 ms
64 bytes from 10.2.1.12: icmp_seq=2 ttl=64 time=0.339 ms
64 bytes from 10.2.1.12: icmp_seq=3 ttl=64 time=0.565 ms
64 bytes from 10.2.1.12: icmp_seq=4 ttl=64 time=0.481 ms
64 bytes from 10.2.1.12: icmp_seq=5 ttl=64 time=0.596 ms
64 bytes from 10.2.1.12: icmp_seq=6 ttl=64 time=0.582 ms
```

J'affiche la table arp de la machine **1** avec la commande **arp -a** : 
```
[benji@localhost ~]$ arp -a
? (10.2.1.12) at 08:00:27:9b:a0:0a [ether] on enp0s8
? (10.33.10.148) at <incomplete> on enp0s8
? (10.33.10.155) at <incomplete> on enp0s8
? (10.2.1.1) at 0a:00:27:00:00:53 [ether] on enp0s8
? (10.33.10.2) at <incomplete> on enp0s8
_gateway (10.0.2.2) at 52:54:00:12:35:02 [ether] on enp0s3
```
J'affiche la table arp de la machine **2** : 
```
[benji@bastion-ovh1fr ~]$ arp -a
? (10.33.10.155) at <incomplete> on enp0s8
? (10.2.1.1) at 0a:00:27:00:00:53 [ether] on enp0s8
_gateway (10.0.2.2) at 52:54:00:12:35:02 [ether] on enp0s3
? (10.33.10.2) at <incomplete> on enp0s8
? (10.2.1.11) at 08:00:27:98:25:b5 [ether] on enp0s8
? (10.33.10.148) at <incomplete> on enp0s8
```

On observe ici que suite au ping, l'adresse ip de la deuxieme VM s'affiche bien dans la table arp de la premiere et inversement:
ARP node1:info node2 :arrow_right: **(10.2.1.12) at 08:00:27:9b:a0:0a [ether] on enp0s8**
Je repère son adresse mac : **08:00:27:9b:a0:0a**

ARP Node 2: info node 1 :arrow_right: **(10.2.1.11) at 08:00:27:98:25:b5 [ether] on enp0s8**
Je repère son adresse mac : **08:00:27:98:25:b5**

Je compare l'adresse mac de node 2 trouvée avec **ip a** et celle affichée dans la table ARP de node 1 : 

Table ARP node 1:
```
08:00:27:9b:a0:0a
```


Node 2:
```
3: enp0s8: <BROADCAST,MULTICAST,UP,LOWER_UP> mtu 1500 qdisc fq_codel state UP group default qlen 1000
    link/ether 08:00:27:9b:a0:0a brd ff:ff:ff:ff:ff:ff
```

J'observe que l'adresse mac observée précedemment sur la table ARP de node 1 est bien celle de node 2 confirmé avec la commande ip a.

### Analyse de trames

Voir fichier dans /capture ARP

| ordre | type trame  | source                   | destination                |
|-------|-------------|--------------------------|----------------------------|
| 1     | Requête ARP | `node1` `PcsCompu_98:25:b5 (08:00:27:98:25:b5)` | Broadcast `FF:FF:FF:FF:FF` |
| 2     | Réponse ARP | `node2` `PcsCompu_9b:a0:0a (08:00:27:9b:a0:0a)` | `node1` `08:00:27:98:25:b5`   |
| 3     | Requête ARP | `node2` `PcsCompu_9b:a0:0a (08:00:27:9b:a0:0a)` | `node1` `PcsCompu_98:25:b5(08:00:27:98:25:b5)`|
| 4     | Réponse ARP | `node1` `08:00:27:98:25:b5` | `node2` `PcsCompu_9b:a0:0a (08:00:27:9b:a0:0a)`   |

## 2-Routage

J'effectue les commandes pour activer le routage: 
```
[benji@router ~]$ sudo firewall-cmd --add-masquerade --zone=public
success
[benji@router ~]$ sudo firewall-cmd --add-masquerade --zone=public --permanent
success
```
#### J'ajoute les routes statiques nécessaires pour que node1.net1.tp2 et marcel.net2.tp2 puissent se ping.

Machine 1 : 
``` 
sudo ip route add 10.2.2.0/24 via 10.2.1.11 dev enp0s8
```
Machine 2 : 
```
sudo ip route add 10.2.1.0/24 via 10.2.2.11 dev enp0s8
```

Je vérifie que les machines peuvent se ping : 

Machine 1 : 
```
[benji@node1 ~]$ ping 10.2.2.12
PING 10.2.2.12 (10.2.2.12) 56(84) bytes of data.
64 bytes from 10.2.2.12: icmp_seq=1 ttl=63 time=0.778 ms
64 bytes from 10.2.2.12: icmp_seq=2 ttl=63 time=1.06 ms
```

Machine 2 : 
```
[benji@marcel ~]$ ping 10.2.1.12
PING 10.2.1.12 (10.2.1.12) 56(84) bytes of data.
64 bytes from 10.2.1.12: icmp_seq=1 ttl=63 time=0.501 ms
64 bytes from 10.2.1.12: icmp_seq=2 ttl=63 time=1.27 ms
```
| ordre | type trame  | IP source | MAC source                | IP destination | MAC destination            |
|-------|-------------|-----------|---------------------------|----------------|----------------------------|
| 1     | Requête ARP |10.2.1.12  |`node1` `08:00:27:98:25:b5`|10.2.1.11       | Broadcast `FF:FF:FF:FF:FF` |
| 2     | Réponse ARP |10.2.1.11  |`routeur` `08:00:27:1e:76:bd`|10.2.1.12     | `node1` `08:00:27:98:25:b5`|
| 3     | Requête ARP |10.2.2.11  | `routeur` `08:00:27:1e:76:bd`|10.2.2.11    | Broadcast `FF:FF:FF:FF:FF` |
| 4     | Réponse ARP |10.2.2.12  | `marcel` `08:00:27:cd:a3:68 `|10.2.1.12    | `Router` `08:00:27:1e:76:bd`|
| 5     | Ping        |10.2.1.12  | `node1` `08:00:27:98:25:b5`  |10.2.2.12    |`marcel` `08:00:27:cd:a3:68 `|
| 6     | Pong        |10.2.2.12  |`marcel` `08:00:27:cd:a3:68 ` |10.2.1.12    |`node1` `08:00:27:98:25:b5`  |

## 3. Accès internet

#### Donnez un accès internet à vos machines
J'ajoute une route par défaut à mes machines afin qu'elles aient accès à internet : 

Machine 1 : 
``` 
sudo ip route add default via 10.2.1.11 dev enp0s8
```
Machine 2 : 
```
sudo ip route add default via 10.2.2.11 dev enp0s8
```

- Test ping : 

Machine 1: 
```
[benji@node1 ~]$ ping 8.8.8.8
PING 8.8.8.8 (8.8.8.8) 56(84) bytes of data.
64 bytes from 8.8.8.8: icmp_seq=1 ttl=113 time=23.7 ms
64 bytes from 8.8.8.8: icmp_seq=2 ttl=113 time=20.3 ms
```
Machine 2: 
```
[benji@marcel ~]$ ping 8.8.8.8
PING 8.8.8.8 (8.8.8.8) 56(84) bytes of data.
64 bytes from 8.8.8.8: icmp_seq=1 ttl=113 time=20.5 ms
64 bytes from 8.8.8.8: icmp_seq=2 ttl=113 time=20.3 ms
```

Après leurs avoir données un serveur dns en modifiant le fichier **/etc/resolv.conf** (1.1.1.1), je test avec un **ping google.com**

Machine 1: 
```
[benji@node1 ~]$ ping google.com
PING google.com (142.250.75.238) 56(84) bytes of data.
64 bytes from par10s41-in-f14.1e100.net (142.250.75.238): icmp_seq=1 ttl=113 time=19.2 ms

```

Machine 2: 
```
[benji@marcel ~]$ ping google.com
PING google.com (216.58.198.206) 56(84) bytes of data.
64 bytes from par10s27-in-f14.1e100.net (216.58.198.206): icmp_seq=1 ttl=113 time=17.8 ms
```

#### Analyse de trames

| ordre | type trame | IP source           | MAC source               | IP destination | MAC destination |
|-------|------------|---------------------|--------------------------|----------------|-----------------|
| 1     | ping       | `node1` `10.2.1.12` | `node1` `AA:BB:CC:DD:EE` | `8.8.8.8`      |`08:00:27:1e:76:bd `| 
| 2     | pong       | 8.8.8.8             | 08:00:27:1e:76:bd        | 10.2.1.12      |08:00:27:98:25:b5| 

## 4.DHCP

Après avoir téléchargé le server dhcp, je rajouté ceci dans son fichier de configuration : 
``` 
default-lease-time 900;
max-lease-time 10800;
ddns-update-style none;
authoritative;
subnet 10.2.1.0  netmask 255.255.255.0 {
  range 10.2.1.10 10.2.1.30;
  option routers 10.2.1.11;
  option subnet-mask 255.255.255.0;
  option domain-name-servers 8.8.8.8;

}
```

Je vérifie sur ma machine node 2 configuré pour recevoir le dhcp que celui-ci est bien fonctionnel : 

``` 
2: enp0s8: <BROADCAST,MULTICAST,UP,LOWER_UP> mtu 1500 qdisc fq_codel state UP group default qlen 1000
    link/ether 08:00:27:a7:76:18 brd ff:ff:ff:ff:ff:ff
    inet 10.2.1.10/24 brd 10.2.1.255 scope global dynamic noprefixroute enp0s8
```

Nous observons que l'ip de ma machine est bien la première des ip proposées par mon serveur dhcp.

### 1- Améliorer la configuration du DHCP

Afin de donner une route par défaut au utilisateur du serveur, j'ai ajouté ceci dans le fichier de configuration : 
```
option routers 10.2.1.11;
```
et ceci pour le serveur dns : 
```
option domain-name-servers 8.8.8.8;
```

J'obtiens une nouvelle adresse ip grâce à la commande ``` sudo dhclient -v enp0s8 ```  puis je fais un **ip a** pour confirmer le changement: 
```
2: enp0s8: <BROADCAST,MULTICAST,UP,LOWER_UP> mtu 1500 qdisc fq_codel state UP group default qlen 1000
    link/ether 08:00:27:a7:76:18 brd ff:ff:ff:ff:ff:ff
    inet 10.2.1.13/24 brd 10.2.1.255 scope global dynamic  enp0s8
```
Afin de prouver que cette ip a bien été affecté à ma machine grâce au serveur dhcp, je fais ``` sudo systemctl status dhcpd ``` afin d'afficher les mac et ip qui sont attribuées par le serveur : 

``` 
Sep 24 17:46:42 node1.net1.tp2 dhcpd[38486]: DHCPREQUEST for 10.2.1.13 from 08:00:27:a7:76:18 via enp0s8
Sep 24 17:46:42 node1.net1.tp2 dhcpd[38486]: DHCPACK on 10.2.1.13 to 08:00:27:a7:76:18 via enp0s8
```
Nous observons ici que l'ip attribué ainsi que la mac lié est bien celle de ma machine 2 (voir ci-dessus).De plus nous observons la requête effectué par la machine 2 afin de changer son ip. Tout cela nous confirme que l'adresse ip de la machine 2 a été attribué par le serveur dhcp.


Je vérifie que la gateway indiqué par le dhcp est bien celle qu'utilise ma machine : 
```
[benji@localhost ~]$ route -n
Kernel IP routing table
Destination     Gateway         Genmask         Flags Metric Ref    Use Iface
0.0.0.0         10.2.1.11       0.0.0.0         UG    100    0        0 enp0s8
10.2.1.0        0.0.0.0         255.255.255.0   U     100    0        0 enp0s8
```

Nous observons que la gateway est bien celle indiqué par le serveur dhcp 10.2.1.11 (ip router avant changement tp)

Je vérifie que la machine peux ping sa passerelle : 
```
[benji@node2 ~]$ ping 10.2.1.11
PING 10.2.1.11 (10.2.1.11) 56(84) bytes of data.
64 bytes from 10.2.1.11: icmp_seq=1 ttl=64 time=0.249 ms
64 bytes from 10.2.1.11: icmp_seq=2 ttl=64 time=0.577 ms
```


Je vérifie le fonctionnement du dns avec un dig ynov.com : 
```
[benji@node2 ~]$ dig ynov.com
;; ANSWER SECTION:
ynov.com.               10529   IN      A       92.243.16.143

```



Je vérifie le fonctionnement avec un ping google.com : 
```
[benji@node2 ~]$ ping google.com
PING google.com (216.58.198.206) 56(84) bytes of data.
64 bytes from par10s27-in-f206.1e100.net (216.58.198.206): icmp_seq=1 ttl=114 time=18.7 ms
64 bytes from par10s27-in-f206.1e100.net (216.58.198.206): icmp_seq=2 ttl=114 time=18.3 ms
```

Nous avons bien accès à internet. J'en conclue que la connexion entre le router, le serveur dhcp et le pc est fonctionnel.

### 2. Analyse de trames

Voir capture dans /capture arp

| ordre | type trame | IP source           |  IP destination | MAC destination |
|-------|------------|---------------------|----------------|-----------------|
| 1     |dhcp Discover| `node2` `0.0.0.0`  |  255.255.255.255|Broadcast        |
| 2     |ARP         | `serveur` `10.2.1.12`|10.2.1.20     |Broadcast        |
| 3     |dhcp offer  | `serveur` `10.2.1.12`| 10.2.1.20    |Broadcast        |
| 4     |dhcp request| `node2` `0.0.0.0` | 255.255.255.255 |Broadcast        |
| 5     |ARP         | `serveur` `10.2.1.12`| 10.2.1.20    |Broadcast        |
| 6     |dhcp AKN    | `serveur` `10.2.1.12`| 10.2.1.20    |`node2`          |
