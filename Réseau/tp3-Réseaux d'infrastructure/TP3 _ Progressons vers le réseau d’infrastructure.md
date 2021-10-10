# TP3 : Progressons vers le réseau d'infrastructure

## I. (mini)Architecture réseau


Je prouve qu'il a une adresse ip dans chaque réseaux qui ce trouve être la passerelle : 
```
[benji@bastion-ovh1fr /]$ ip a
1: lo: <LOOPBACK,UP,LOWER_UP> mtu 65536 qdisc noqueue state UNKNOWN group default qlen 1000
    link/loopback 00:00:00:00:00:00 brd 00:00:00:00:00:00
    inet 127.0.0.1/8 scope host lo
       valid_lft forever preferred_lft forever
    inet6 ::1/128 scope host
       valid_lft forever preferred_lft forever
2: enp0s3: <BROADCAST,MULTICAST,UP,LOWER_UP> mtu 1500 qdisc fq_codel state UP group default qlen 1000
    link/ether 08:00:27:6c:be:a1 brd ff:ff:ff:ff:ff:ff
    inet 10.0.2.15/24 brd 10.0.2.255 scope global dynamic noprefixroute enp0s3
       valid_lft 85518sec preferred_lft 85518sec
    inet6 fe80::a00:27ff:fe6c:bea1/64 scope link noprefixroute
       valid_lft forever preferred_lft forever
3: enp0s8: <BROADCAST,MULTICAST,UP,LOWER_UP> mtu 1500 qdisc fq_codel state UP group default qlen 1000
    link/ether 08:00:27:d9:7c:3d brd ff:ff:ff:ff:ff:ff
    inet 10.3.1.62/26 brd 10.3.1.63 scope global noprefixroute enp0s8
       valid_lft forever preferred_lft forever
    inet6 fe80::a00:27ff:fed9:7c3d/64 scope link
       valid_lft forever preferred_lft forever
4: enp0s9: <BROADCAST,MULTICAST,UP,LOWER_UP> mtu 1500 qdisc fq_codel state UP group default qlen 1000
    link/ether 08:00:27:22:25:19 brd ff:ff:ff:ff:ff:ff
    inet 10.3.1.190/25 brd 10.3.1.255 scope global noprefixroute enp0s9
       valid_lft forever preferred_lft forever
    inet6 fe80::a00:27ff:fe22:2519/64 scope link
       valid_lft forever preferred_lft forever
5: enp0s10: <BROADCAST,MULTICAST,UP,LOWER_UP> mtu 1500 qdisc fq_codel state UP group default qlen 1000
    link/ether 08:00:27:f7:d6:29 brd ff:ff:ff:ff:ff:ff
    inet 10.3.1.206/28 brd 10.3.1.207 scope global noprefixroute enp0s10
       valid_lft forever preferred_lft forever
    inet6 fe80::a00:27ff:fef7:d629/64 scope link
       valid_lft forever preferred_lft forever
```

Je prouve qu'il a internet par un ping 8.8.8.8 : 
```
[benji@bastion-ovh1fr /]$ ping 8.8.8.8
PING 8.8.8.8 (8.8.8.8) 56(84) bytes of data.
64 bytes from 8.8.8.8: icmp_seq=1 ttl=114 time=26.6 ms
```

Puis avec un dig ynov.com, je prouve qu'il a une résolution de nom: 

```
[benji@router /]$ dig ynov.com
;; ANSWER SECTION:
ynov.com.               7442    IN      A       92.243.16.143

;; Query time: 3 msec
;; SERVER: 10.33.10.2#53(10.33.10.2)
;; WHEN: Mon Sep 27 12:35:00 CEST 2021
;; MSG SIZE  rcvd: 53
```

Il porte bien le hostname **router.tp3** : 
```
[benji@router ~]$ hostname
router.tp3
```
J'active le routage : 
```
[benji@router ~]$ sudo firewall-cmd --add-masquerade --zone=public
success
[benji@router ~]$ sudo firewall-cmd --add-masquerade --zone=public --permanent
success
```

## II. Services d'infra


### 1. Serveur DHCP

Le nom de ma machine : 
```
[benji@dhcp /]$ hostname
dhcp.client1.tp3
```
J'ai mis en place le serveur dhcp dans le réseau **10.3.1.128**.
Voir fichier **dhcp.conf**

#### Mettre en place un client dans le réseau client1

Je prouve que **Marcel** a une adresse ip avec la commande ``` sudo systemctl status dhcpd ``` : 

```
Oct 02 15:15:20 dhcp.client1.tp3 dhcpd[1636]: DHCPDISCOVER from 08:00:27:d0:fa:e3 via enp0s8
Oct 02 15:15:21 dhcp.client1.tp3 dhcpd[1636]: DHCPOFFER on 10.3.1.131 to 08:00:27:d0:fa:e3 (marcel) via enp0s8
Oct 02 15:15:22 dhcp.client1.tp3 dhcpd[1636]: DHCPREQUEST for 10.3.1.131 (10.3.1.130) from 08:00:27:d0:fa:e3 (marcel) via enp0s8
Oct 02 15:15:22 dhcp.client1.tp3 dhcpd[1636]: DHCPACK on 10.3.1.131 to 08:00:27:d0:fa:e3 (marcel) via enp0s8
```

On observe bien ici les 4 étapes qu'a réalisé le serveur pour donner l'adresse ip à la machine marcel.

Je prouve que Marcel a accès à sa passerelle avec un ping 8.8.8.8 : 

```
[benji@marcel ~]$ ping 8.8.8.8
PING 8.8.8.8 (8.8.8.8) 56(84) bytes of data.
64 bytes from 8.8.8.8: icmp_seq=1 ttl=113 time=80.8 ms
```
Puis, je prouve que le serveur dhcp lui donne accès un serveur dns : 

```
ynov.com.               9415    IN      A       92.243.16.143
;; Query time: 35 msec
;; SERVER: 1.1.1.1#53(1.1.1.1)
```

Je prouve que marcel.client1.tp3 passe par router.tp3 pour sortir de son réseau à l'aide de traceroute : 

```
[benji@marcel ~]$ traceroute google.com
traceroute to google.com (216.58.214.174), 30 hops max, 60 byte packets
 1  _gateway (10.3.1.190)  1.519 ms  1.463 ms  1.368 ms
 2  10.0.2.2 (10.0.2.2)  1.236 ms  1.094 ms  0.946 ms
```
J'observe que pour sortir du réseau Marcel utilise bien le router (10.3.1.190) qui lui est indiqué par le serveur dhcp.

### 2. Serveur DNS


####  SETUP 

🌞 Mettre en place une machine qui fera office de serveur DNS

Je mets en place un server dns dans le fichier ``` etc/resolv.conf ``` (nameserver 1.1.1.1)

J'installe le package bind avec la commande ``` yum install bind bind-utils -y ``` 

Puis, je crée mes zones forward dans le fichier named.conf : 
``` 
zone "server1.tp3" IN {
        type master;
        file "server1.tp3.forward";
        allow-update { none; };
};

zone "server2.tp3" IN {
        type master;
        file "server2.tp3.forward";
        allow-update { none; };
};
```
Ensuite je crée le 2 fichiers forward et j'y insère ceci : 
```
@   IN  SOA     dns1.server1.tp3. root.server1.tp3. (
         2021080804  ;Serial
         3600        ;Refresh
         1800        ;Retry
         604800      ;Expire
         86400       ;Minimum TTL
)
        ; Set your Name Servers here
@         IN  NS      dns1.server1.tp3.


; Set each IP address of a hostname. Sample A records.
dns1           IN  A       10.3.1.10
router     IN  A       10.3.1.126 (ou .206 pour server2)
```

Maintenant, je modifie le firewall pour que le service dns soit exploitable : 

```
[benji@dns1 /]$ sudo firewall-cmd --add-service=dns --permanent
success
[benji@dns1 /]$ sudo firewall-cmd --reload
success
```

Ensuite j'inclue dans le fichier resolve.conf de mon dns son propre nameserver (10.3.1.10)

Je vérifie que mes zones sont bien configurées ; 
```
[benji@dns1 /]$ sudo named-checkzone server1.tp3 /var/named/server1.tp3.forward
zone server1.tp3/IN: loaded serial 2021080804
OK
```

Après avoir modifié le dhcp pour qu'il indique le serveur dns que nous avons crée, je test avec Marcel de trouver l'ip du router grâce au serveur dns : 
```
[benji@marcel /]$ dig router

;; Query time: 4 msec
;; SERVER: 10.3.1.10#53(10.3.1.10)
;; WHEN: Sat Oct 02 17:38:56 CEST 2021
;; MSG SIZE  rcvd: 63
```

Nous observons que c'est bien notre serveur dns qui nous réponds .

#### Get deeper

La clause recursion dans le fichier /etc/named.conf étant déjà activé, je vérifie son foctionnement avec un dig google.com depuis la machine marcel : 

```
[benji@marcel /]$ dig google.com
;; Query time: 3 msec
;; SERVER: 10.3.1.10#53(10.3.1.10)
;; WHEN: Sat Oct 02 17:44:47 CEST 2021
;; MSG SIZE  rcvd: 67
```

Afin que mon serveur dhcp donne notre serveur dns aux utilisateurs connectés, je modifie le dns dans son fichier de configuration : 

```
option domain-name-servers 10.3.1.10;
```

Je crée un nouvel utilisateur et je vérifie qu'il accède au dhcp : 

```
Oct 02 17:56:19 dhcp.client1.tp3 dhcpd[1636]: DHCPREQUEST for 10.3.1.132 from 08:00:27:30:be:2a via enp0s8
Oct 02 17:56:19 dhcp.client1.tp3 dhcpd[1636]: DHCPACK on 10.3.1.132 to 08:00:27:30:be:2a (johnny) via enp0s8
```
Puis avec un dig google.com, je vérifie que le serveur lui a bien donné le bon serveur dns: 
```
[benji@johnny /]$ dig google.com
;; Query time: 3 msec
;; SERVER: 10.3.1.10#53(10.3.1.10)
;; WHEN: Sat Oct 02 18:07:38 CEST 2021
;; MSG SIZE  rcvd: 67
```

## III. Services métier

### 1. Serveur Web

Afin de réalisé au plus vite un petit serveur web, je clone mon serveur web de mon tp2 linux et j'ouvre le port correspondant : 

```
[benji@web /]$ sudo firewall-cmd --add-port=443/tcp --permanent
success
[benji@web /]$ sudo firewall-cmd --reload
success
```

Je test avec un curl depuis **Marcel**

```
[benji@marcel /]$ curl 10.3.1.195:443
<!doctype html>
<html>
  <head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>HTTP Server Test Page powered by: Rocky Linux</title>
```

### 2. Partage de fichiers

- Setup machine NFS

Installation de nfs-utils : 
```
[benji@nfs1 /]$ sudo dnf -y install nfs-utils
```

Je modifie le fichier /etc/idmapd.conf, je décommente et ajoute ceci : `Domain = nfs.server2`

Ensuite je crée le fichier /etc/exports : 
```
/home/nfs_share 10.3.1.192/28(rw,no_root_squash)
```

Je crée le fichier /home/nfs_share: 
```
[benji@nfs1 /]$ sudo mkdir /home/nfs_share
```

Je fais en sorte que le service démarre automatiquement: 
```
[benji@nfs1 /]$ sudo systemctl enable --now rpcbind nfs-server
Created symlink /etc/systemd/system/multi-user.target.wants/nfs-server.service → /usr/lib/systemd/system/nfs-server.service.
```

Je fais en sorte que le firewall autorise le fonctionnement du service : 

```
[benji@nfs1 /]$ sudo firewall-cmd --add-service=nfs
success
[benji@nfs1 /]$ sudo firewall-cmd --add-service={nfs3,mountd,rpc-bind}
success
[benji@nfs1 /]$ sudo firewall-cmd --runtime-to-permanent
success
```

🌞 Configuration du client NFS

J'installe nfs-utils sur la machine web.server2: 
```
[benji@web ~]$ sudo dnf -y install nfs-utils
```

Je modifie le fichier /etc/idmapd.conf, je décommente et ajoute ceci : `Domain = nfs.server2`

Je monte le serveur nfs dans /srv/nfs: 
```
[benji@web /]$ sudo mount -t nfs 10.3.1.200:/home/nfs_share /srv/nfs
[benji@web /]$  df -hT
Filesystem                 Type      Size  Used Avail Use% Mounted on
devtmpfs                   devtmpfs  1.9G     0  1.9G   0% /dev
tmpfs                      tmpfs     1.9G     0  1.9G   0% /dev/shm
tmpfs                      tmpfs     1.9G  8.5M  1.9G   1% /run
tmpfs                      tmpfs     1.9G     0  1.9G   0% /sys/fs/cgroup
/dev/mapper/rl-root        xfs       6.2G  2.0G  4.3G  32% /
/dev/sda1                  xfs      1014M  182M  833M  18% /boot
tmpfs                      tmpfs     374M     0  374M   0% /run/user/1000
nfs.server2:/home/nfs_share nfs4      6.2G  2.0G  4.3G  32% /srv/nfs
```

Je fais en sorte que le montage se fasse automatiquement dès le boot:
```
[benji@web /]$ sudo nano etc/fstab

nfs.server2:/home/nfs_share /srv/nfs               nfs     defaults        0 0

```

Je vérifie que je peux lire et écrire dans le dossier nfs: 

```
[benji@web nfs]$ sudo touch ilovecatgif
[benji@web nfs]$ ls
ilovecatgif
```
Je vérifie que le fichier **ilovecatgif** se trouve aussi dans srv/nfs_share : 
```
[benji@nfs1 nfs_share]$ ls
ilovecatgif
```

## IV. Un peu de théorie : TCP et UDP

#### 🌞 Déterminer, pour chacun de ces protocoles, s'ils sont encapsulés dans du TCP ou de l'UDP : 

- Le **SSH**,l'**hhtp** et le **nfs**, sont encapsulés dans du **TCP**. Ici se que nous voulons c'est que les trames arrivent et en bon état, nous préférons donc une connexion plus fiable que rapide et c'est se que propose le tcp.

- Le **DNS** utilise l'**UDP**, ici c'est la rapidité qui prime car l'état des trames est "négligable" étant donné son faible chiffre (2),une nouvelle requête sera envoyée si les paquets ont été corrompue. 

📁 **Captures réseau [tp3_ssh.pcap](captures/tp3_ssh.pcap), [tp3_http.pcap](captures/tp3_http.pcap), [tp3_dns.pcap](captures/tp3_dns.pcap) et [tp3_nfs.pcap](captures/tp3_nfs.pcap)**

📁 **Capture réseau [tp3_3way.pcap](captures/tp3_3way.pcap)**

## V. El final

- Tableau des réseaux

| Nom du réseau | Adresse du réseau | Masque        | Nombre de clients possibles | Adresse passerelle | Adresse broadcast |
|---------------|-------------------|---------------|-----------------------------|--------------------|----------------------------------------------------------------------------------------------|
| `serveur1`     | `10.3.1.0`        | `255.255.255.128` | 126                           | `10.3.1.126`         | `10.3.1.127`                                                                                   |
| `Client1`     | `10.3.1.128`        | `255.255.255.192` | 62                           | `10.3.1.190`         | `10.3.1.191`                                                                                   |
| `server2`     | `10.3.1.192`        | `255.255.255.240` | 14                         | `10.3.1.206`         | `10.3.1.207` |


- Tableau d'adressage

| Nom machine  | Adresse IP `client1` | Adresse IP `server1` | Adresse IP `server2` | Adresse de passerelle |
|--------------|----------------------|----------------------|----------------------|-----------------------|
| `router.tp3` | `10.3.1.190/26`      | `10.3.1.126/25`     | `10.3.1.206/28`      | Carte NAT             |
| `dhcp.tp3`   | `10.3.1.130/26`      |            X        |             X        | `router.tp3` `10.3.1.190/26`|
|`marcel.tp3`  | ` dynamic` `10.3.1.131/26`| X               |             X        | `router.tp3` `10.3.1.190/26`|
|`dns1.tp3`    | X                    | `10.3.1.10/25`       | X                    | `router.tp3` `10.3.1.126/25`|
| `jonnhy.tp3` | `dynamic` `10.3.1.132/26`|            X     |             X        | `router.tp3` `10.3.1.190/26`|
| `web.tp3`    | X                        |            X     | `10.3.1.195/28`      | `router.tp3` `10.3.1.206/28`|
| `nfs.tp3`    | X                        |            X     | `10.3.1.200/28`      | `router.tp3` `10.3.1.206/28`|

- Le schéma (port 22 ouvert sur toutes les machines):

![](https://i.imgur.com/HSKS57K.png)

