# Tp1 Linux Gelineau Benjamin

## 0- Préparation de la machine

### Setup de deux machines Rocky Linux configurées de façon basique.

#### un accès internet (via la carte NAT)
- Voici la preuve que mes machines ont accès à internet : 

Machine 1: ![](https://i.imgur.com/QvoWepH.png)

Machine 2: ![](https://i.imgur.com/5W9am9Q.png)

#### un accès à un réseau local

- Voici la preuve que mes machines ont un réseau local et peuvent se ping : 

Machine 1 à machine 2 : 
```
[benji@node1 ~]$ ping 10.101.1.12
PING 10.101.1.12 (10.101.1.12) 56(84) bytes of data.
64 bytes from 10.101.1.12: icmp_seq=1 ttl=64 time=0.359 ms
64 bytes from 10.101.1.12: icmp_seq=2 ttl=64 time=0.585 ms
64 bytes from 10.101.1.12: icmp_seq=3 ttl=64 time=0.608 ms
```

Machine 2 à machine 1 : 
```
[benji@node2 ~]$ ping 10.101.1.11
PING 10.101.1.11 (10.101.1.11) 56(84) bytes of data.
64 bytes from 10.101.1.11: icmp_seq=1 ttl=64 time=0.276 ms
64 bytes from 10.101.1.11: icmp_seq=2 ttl=64 time=0.595 ms
64 bytes from 10.101.1.11: icmp_seq=3 ttl=64 time=0.585 ms
```
Toutes ses manipulations sont réalisées en ssh.

#### les machines doivent avoir un nom

Avec la commande ```sudo hostnamectl set-hostname node1.tp1.b2```, je change le nom de mes machines.

Machine 1: 
```
[benji@node1 ~]$ hostname
node1.tp1.b2
```

Machine 2: 
```
[benji@node2 ~]$ hostname
node2.tp1.b2
```
#### utiliser 1.1.1.1 comme serveur DNS

- Avec la commande ```sudo nano /etc/resolv.conf```, je rajoute le **nameserver 1.1.1.1**

Je vérifie son fonctionnement avec un **dig ynov.com** : 

```
[benji@node1 ~]$ dig ynov.com

;; ANSWER SECTION:
ynov.com.               6079    IN      A       **92.243.16.143**

;; Query time: 21 msec
;; SERVER: 1.1.1.1#53(1.1.1.1)
```
Nous observons bien ici l'ip du nom demandé (92.243.16.143) et le serveur qui m'a répondu (1.1.1.1).

#### les machines doivent pouvoir se joindre par leurs noms respectifs

Après avoir lié l'ip de la machine 2 à son nom dans le fichier **/etc/hosts** et inversement, je vérifie que je peux le ping grâce à son nom: 

Machine 1 : 
```
[benji@node1 ~]$ ping node2
PING node2 (10.101.1.12) 56(84) bytes of data.
64 bytes from node2 (10.101.1.12): icmp_seq=1 ttl=64 time=0.283 ms
```



Machine 2 : 
```
[benji@node2 ~]$ ping node1
PING node1 (10.101.1.11) 56(84) bytes of data.
64 bytes from node1 (10.101.1.11): icmp_seq=1 ttl=64 time=0.260 ms
```
## I. Utilisateurs

### 1. Création et configuration

Je crée l'utilisateur **test** avec les options demandées : 
``` [benji@node1 ~]$ sudo useradd test -m -s /bin/bash ```

Je crée le groupe **admins** 
``` [benji@node1 ~]$ sudo groupadd admins ```

Afin de donner les droits de root via sudo aux utilisateurs du groupe admins, j'ai modifié le fichier ``` /etc/sudoers ``` en ajoutant ceci : 

``` %admins       ALL=(ALL)       NOPASSWD: ALL ```

Puis j'ajoute l'utilisateur **test* au groupe **admins** .
```
[benji@node1 ~]$ sudo usermod -aG admins test
[benji@node1 ~]$ groups test
test : test admins
```

### 2. SSH

Je génère une clé ssh avec mon ordinateur avec la commande ``` ssh-keygen -t rsa -b 4096 ``` puis je l'importe dans ma vm : 

```
[test@node1 ~]$ cd .ssh/
[test@node1 .ssh]$ nano authorized_keys
[test@node1 .ssh]$ sudo chmod 600 authorized_keys
[test@node1 .ssh]$ cd ..
[test@node1 .ssh]$ sudo chmod 700 .shh
[test@node1 .ssh]$ cat authorized_keys
ssh-rsa [...] wiRY7rqlcRbpZn3iUBmDhqxkARJqLNDm24zmX4JmA6pnykzFE0tST55r4jRpg9rvTCm6dOuqp6Qmgkf220tFPqA//TlUT0tSupT1LWivSJQ== benjamin@DESKTOP-ON7BJGL
```

Voici la preuve que je peux me connecter en ssh sans mot de passe : 
![](https://i.imgur.com/Hzj79Ep.png)

## II. Partitionnement

J'agrege les deux disques en un seul volume group : 
```
[benji@node1 ~]$ sudo pvcreate /dev/sdb
  Physical volume "/dev/sdb" successfully created.
[benji@node1 ~]$ sudo pvcreate /dev/sdc
  Physical volume "/dev/sdc" successfully created.
[benji@node1 ~]$ sudo vgcreate data /dev/sdb /dev/sdc
  Volume group "data" successfully created
```

Je crée 3 logical volumes de 1 Go chacun : 
```
[benji@node1 ~]$  sudo lvcreate -L 1G data -n ma_data_1
  Logical volume "ma_data_1" created.
[benji@node1 ~]$  sudo lvcreate -L 1G data -n ma_data_2
  Logical volume "ma_data_2" created.
[benji@node1 ~]$  sudo lvcreate -L 1G data -n ma_data_3
  Logical volume "ma_data_3" created.
```

Je formate ces partitions en **ext4** : 
```
[benji@node1 ~]$ sudo mkfs -t ext4 /dev/data/ma_data_1
mke2fs 1.45.6 (20-Mar-2020)
Creating filesystem with 262144 4k blocks and 65536 inodes
Filesystem UUID: b55d762f-11be-44d4-a970-8db65751b1c1
Superblock backups stored on blocks:
        32768, 98304, 163840, 229376

Allocating group tables: done
Writing inode tables: done
Creating journal (8192 blocks): done
Writing superblocks and filesystem accounting information: done
```

Après avoir crée les dossiers **/mnt/part1(2,3)**, je monte les partitions qui leurs sont attribuées : 

```
mount /dev/data/ma_data_1 /mnt/part1
```

Je vérifie le montage : 
```
/dev/mapper/data-ma_data_1 on /mnt/part1 type ext4 (rw,relatime,seclabel)
/dev/mapper/data-ma_data_2 on /mnt/part2 type ext4 (rw,relatime,seclabel)
/dev/mapper/data-ma_data_3 on /mnt/part3 type ext4 (rw,relatime,seclabel)
```

Après avoir ajouté ceci :
```
/dev/data/ma_data_1 /mnt/part1 ext4 defaults 0 0
/dev/data/ma_data_2 /mnt/part2 ext4 defaults 0 0
/dev/data/ma_data_3 /mnt/part3 ext4 defaults 0 0
```

dans le dossier **/etc/fstab**, je démonte une de mes partitions pour vérifier le bon fonctionnement du boot automatique : 
```
[benji@node1 ~]$ sudo umount /mnt/part1
[benji@node1 ~]$ sudo mount -av
/                        : ignored
/boot                    : already mounted
none                     : ignored
mount: /mnt/part1 does not contain SELinux labels.
       You just mounted an file system that supports labels which does not
       contain labels, onto an SELinux box. It is likely that confined
       applications will generate AVC messages and not be allowed access to
       this file system.  For more details see restorecon(8) and mount(8).
/mnt/part1               : successfully mounted
/mnt/part2               : already mounted
/mnt/part3               : already mounted
```

Nous observons que ma partition **part 1** c'est monté de nouveau avec succès.

## III. Gestion de services

### 1. Interaction avec un service existant

Avec la commande ```  systemctl is-enabled firewalld ```, j'observe si l'utilitaire **firewalld** est démarrée et activée lors du boot : 

``` 
[benji@node1 ~]$ systemctl is-enabled firewalld enabled
```
Mon utilitaire est démarré et activée lors du boot.

### 2. Création de service

#### A. Unité simpliste

Après avoir crée le fichier web.service dans **/etc/systemd/system**, je démarre le service web et je vérifie son fonctionnement avec un curl: 


```
[benji@node1 system]$ curl 10.101.1.11:8888
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>Directory listing for /</title>
</head>
<body>
```

#### B. Modification de l'unité

Je crée l'utilisateur web : ``` [benji@node1 ~]$ sudo useradd web ```

Je modifie le fichier demandé : 
```
[Service]
ExecStart=/bin/python3 -m http.server 8888
User=web
WorkingDirectory= /srv/web
```

Je crée un dossier **web** dans le dossier /srv puis ajoute avec une commande touch un fichier test dans le dossier web.

Après un chmod 700 sur le dossier web, je lance le serveur avec l'utilisateur web et je vérifie son fonctionnement : 
```
[web@node1 web]$ sudo systemctl start web
[web@node1 web]$ sudo systemctl status web
● web.service - Very simple web service
   Loaded: loaded (/etc/systemd/system/web.service; enabled; vendor preset: disabled)
   Active: active (running) since Fri 2021-09-24 12:20:20 CEST; 31min ago
 Main PID: 794 (python3)
    Tasks: 1 (limit: 23671)
   Memory: 12.6M
   CGroup: /system.slice/web.service
           └─794 /bin/python3 -m http.server 8888

Sep 24 12:20:20 localhost.localdomain systemd[1]: Started Very simple web service.
[web@node1 web]$ curl 10.101.1.11:8888
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title>Directory listing for /</title>
</head>
<body>

```