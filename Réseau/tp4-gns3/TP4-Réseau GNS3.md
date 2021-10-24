# TP4-Réseau GNS3

## I. Dumb switch

### 1. Topologie 1

![](https://i.imgur.com/AerB5kt.png)

### 2. Adressage topologie 1

Je setup les adresses ip des deux vpcs: 

```
PC1> ip 10.1.1.1 /24
Checking for duplicate address...

PC1 : 10.1.1.1 255.255.255.0
```

Je vérifie que les 2 vpcs peuvent se ping : 
```
PC2> ping 10.1.1.1

84 bytes from 10.1.1.1 icmp_seq=1 ttl=64 time=10.009 ms
84 bytes from 10.1.1.1 icmp_seq=2 ttl=64 time=15.293 ms
84 bytes from 10.1.1.1 icmp_seq=3 ttl=64 time=13.041 ms
```

## II. VLAN

### 1. Topologie 2

![](https://i.imgur.com/3IW6Qo0.png)

### 2. Adressage topologie 2

| Node  | IP            | VLAN |
|-------|---------------|------|
| `pc1` | `10.1.1.1/24` | 10   |
| `pc2` | `10.1.1.2/24` | 10   |
| `pc3` | `10.1.1.3/24` | 20   |

### 3. Setup topologie 2
🌞 Adressage

Je vérifie que tout le monde peut se ping : 

```
PC2> ping 10.1.1.1

84 bytes from 10.1.1.1 icmp_seq=1 ttl=64 time=10.009 ms
84 bytes from 10.1.1.1 icmp_seq=2 ttl=64 time=15.293 ms
84 bytes from 10.1.1.1 icmp_seq=3 ttl=64 time=13.041 ms
```

```
PC2> ping 10.1.1.3

84 bytes from 10.1.1.3 icmp_seq=1 ttl=64 time=11.233 ms
84 bytes from 10.1.1.3 icmp_seq=2 ttl=64 time=14.805 ms
```

🌞 Configuration des VLANs

Je crée les deux vlans: 

```
Switch>enable
Switch#conf t
Switch(config)#vlan 10
Switch(config-vlan)#name vlan10
Switch(config-vlan)#exit
Switch(config)#vlan 20
Switch(config-vlan)#name vlan20
Switch(config-vlan)#exit
```

Je configure les vlans: 

```
Switch(config)#interface GigabitEthernet0/0
Switch(config-if)#switchport mode access
Switch(config-if)#switchport access vlan 10
Switch(config-if)#exit
Switch(config)#interface GigabitEthernet0/1
Switch(config-if)#switchport mode access
Switch(config-if)#switchport access vlan 10
Switch(config-if)#exit
Switch(config)#interface GigabitEthernet0/2
Switch(config-if)#switchport mode access
Switch(config-if)#switchport access vlan 20
Switch(config-if)#exit
```

🌞 Vérif

Je vérifie que le pc 2 puisse ping le pc 1 et pas le pc 3: 

```
PC2> ping 10.1.1.3
host (10.1.1.3) not reachable
PC2> ping 10.1.1.1
84 bytes from 10.1.1.1 icmp_seq=1 ttl=64 time=17.408 ms
84 bytes from 10.1.1.1 icmp_seq=2 ttl=64 time=14.320 ms
84 bytes from 10.1.1.1 icmp_seq=3 ttl=64 time=13.003 ms
```

## III. Routing

### 1. Topologie 3

![](https://i.imgur.com/mEHqiBW.png)

- Création des vlans: 

```
Switch>en
Switch#conf t
Enter configuration commands, one per line.  End with CNTL/Z.
Switch(config)#vlan 11
Switch(config-vlan)#name clients
Switch(config-vlan)#exit


VLAN Name                             Status    Ports
---- -------------------------------- --------- -------------------------------
1    default                          active    Gi0/0, Gi0/1, Gi0/2, Gi0/3
                                                Gi1/0, Gi1/1, Gi1/2, Gi1/3
                                                Gi2/0, Gi2/1, Gi2/2, Gi2/3
                                                Gi3/0, Gi3/1, Gi3/2, Gi3/3
10   vlan10                           active
11   clients                          active
12   admins                           active
13   servers                          active
```
Adressage vlan port: 
```
Switch(config)#interface GigabitEthernet 0/0
Switch(config-if)#switchport mode access
Switch(config-if)#switchport access vlan 11
Switch(config-if)#no shutdown
Switch(config-if)#exit
Switch(config)#interface GigabitEthernet 0/1
Switch(config-if)#switchport mode access
Switch(config-if)#switchport access vlan 11
Switch(config-if)#no shutdown
Switch(config-if)#exit
Switch(config-if)#interface GigabitEthernet 0/2
Switch(config-if)#switchport mode access
Switch(config-if)#switchport access vlan 12
Switch(config-if)#no shutdown
Switch(config-if)#exit
Switch(config)#interface GigabitEthernet 1/3
Switch(config-if)#switchport mode access
Switch(config-if)#switchport access vlan 13
Switch(config-if)#no shutdown
Switch(config-if)#exit
```

Je trunk le router : 

```
Switch(config)# interface gigaBitEthernet0/0
Switch(config-if)#switchport trunk encapsulation dot1q
Switch(config-if)#switchport mode trunk
Switch(config-if)#switchport trunk allowed vlan add 11,12,13

Switch#show interface trunk

Port        Mode             Encapsulation  Status        Native vlan
Gi0/0       on               802.1q         trunking      1

Port        Vlans allowed on trunk
Gi0/0       1-4094

Port        Vlans allowed and active in management domain
Gi0/0       1,10-13,20

Port        Vlans in spanning tree forwarding state and not pruned
Gi0/0       1,10-13,20
```

🌞 Config du routeur

Je créer les 3 interfaces de sous réseau.

```
R1(config)#interface fastEthernet0/0.10
R1(config-subif)#encapsulation dot1Q 11
R1(config-subif)#ip addr 10.1.1.254 255.255.255.0
R1(config-subif)#exit
R1(config)#interface fastEthernet0/0.20
R1(config-subif)#encapsulation dot1Q 12
R1(config-subif)#ip addr 10.2.2.254 255.255.255.0
R1(config-subif)#exit
R1(config)#interface fastEthernet0/0.30
R1(config-subif)#encapsulation dot1Q 13
R1(config-subif)#ip addr 10.3.3.254 255.255.255.0
```

🌞 Vérif

Je test ping le router

pc1: 
```
PC1> ping 10.1.1.254

84 bytes from 10.1.1.254 icmp_seq=1 ttl=255 time=28.610 ms
84 bytes from 10.1.1.254 icmp_seq=2 ttl=255 time=19.368 ms
84 bytes from 10.1.1.254 icmp_seq=3 ttl=255 time=18.528 ms
```

admin:
```
admin> ping 10.2.2.254
84 bytes from 10.2.2.254 icmp_seq=1 ttl=255 time=28.210 ms
84 bytes from 10.2.2.254 icmp_seq=2 ttl=255 time=19.633 ms
```
web: 


## IV. NAT

### Topologie 4


![](https://i.imgur.com/bO5xTdF.png)

### Setup topologie 4

🌞 Ajoutez le noeud Cloud à la topo

- Après avoir ajouter le cloud à la topo, je configure mon router afin qu'il reçoive une adresse ip en dhcp: 

```
R1(config)#interface f1/0
R1(config-if)#ip address dhcp
R1(config-if)#no shut
```
Je test avec un ping 1.1.1.1: 

```
R1#ping 1.1.1.1
Type escape sequence to abort.
Sending 5, 100-byte ICMP Echos to 1.1.1.1, timeout is 2 seconds:
.!!!!
Success rate is 80 percent (4/5), round-trip min/avg/max = 20/23/32 ms
```

🌞 Configurez le NAT

Je configure les nat en fonction de ma topologie: 

```
R1(config)# interface fastEthernet 0/0
R1(config-if)#ip nat inside
R1(config-if)#exit
R1(config)#interface fastEthernet 1/0
R1(config-if)#ip nat outside
R1(config-if)#exit
```

🌞 Test

J'ajoute les routes par défaut: 

```
pc1> ip 10.1.1.1/24 10.1.1.254
admin> ip 10.2.2.1/24 10.2.2.254
[benji@web ~]$ sudo ip route add default via 10.3.3.254 dev enp0s3
```

Je configure les vpcs pour utiliser le serveur dns 1.1.1.1: 

```
admin> ip dns 8.8.8.8
[benji@web /]$ cat etc/resolve.conf
nameserver 8.8.8.8
```

Je test avec un `ping google.com`: 

```
PC1> ping google.com
google.com resolved to 142.250.179.78
84 bytes from 142.250.179.78 icmp_seq=1 ttl=113 time=39.918 ms
84 bytes from 142.250.179.78 icmp_seq=2 ttl=113 time=32.744 ms
84 bytes from 142.250.179.78 icmp_seq=3 ttl=113 time=59.487 ms
```

## V. Add a building

### Topologie 5

🌞 Mettre en place un serveur DHCP dans le nouveau bâtiment

Pour mettre en place le serveur dhcp, j'ai utilisé un serveur utilisé dans un des tp précédent. J'ai modifié son fichier de conf pour donner ceci: 

```
# DHCP Server Configuration file.
#   see /usr/share/doc/dhcp-server/dhcpd.conf.example
#   see dhcpd.conf(5) man page
#
default-lease-time 900;
max-lease-time 10800;
ddns-update-style none;
authoritative;
subnet 10.1.1.253  netmask 255.255.255.0 {
  range 10.1.1.0 10.1.1.252;
  option routers 10.1.1.254;
  option subnet-mask 255.255.255.0;
  option domain-name-servers 8.8.8.8;
}
```

🌞 Vérification

- un client récupère une IP en DHCP: 

```
PC3> ip dhcp
DDORA IP 10.1.1.1/24 GW 10.1.1.254
```

- il peut ping le serveur Web: 

```
PC3> ping 10.3.3.1
84 bytes from 10.3.3.1 icmp_seq=1 ttl=64 time=23.999 ms
84 bytes from 10.3.3.1 icmp_seq=2 ttl=64 time=27.165 ms
84 bytes from 10.3.3.1 icmp_seq=3 ttl=64 time=13.383 ms
```

- il peut ping le serveur 8.8.8.8: 

```
PC3> ping 8.8.8.8
64 bytes from 8.8.8.8: icmp_seq=1 ttl=115 time=17.4 ms
64 bytes from 8.8.8.8: icmp_seq=2 ttl=115 time=19.3 ms
```

- il peut ping le serveur google.com:

```
PC3> ping google.com
64 bytes from par10s34-in-f14.1e100.net (216.58.206.238): icmp_seq=1 ttl=114 tim                                                                            e=16.5 ms
64 bytes from par10s34-in-f14.1e100.net (216.58.206.238): icmp_seq=2 ttl=114 tim                                                                            e=21.2 ms
64 bytes from par10s34-in-f14.1e100.net (216.58.206.238): icmp_seq=3 ttl=114 tim                                                                            e=17.1 ms
```