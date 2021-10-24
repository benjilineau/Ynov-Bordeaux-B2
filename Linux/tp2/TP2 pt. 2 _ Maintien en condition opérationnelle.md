# TP2 pt. 2 : Maintien en condition opérationnelle

## I. Monitoring

### 1. Principe
La surveillance ou *monitoring* consiste à surveiller la bonne santé d'une entité.  
J'utilise volontairement le terme vague "entité" car cela peut être très divers :

- une machine
- une application
- un lien entre deux machines
- etc.

### 2. Setup

🌞 Setup Netdata 

- Pour toutes les machines à monitorer, j'exécute les commandes suivantes : 

```
# Passez en root pour cette opération
$ sudo su -

# Install de Netdata via le script officiel statique
$ bash <(curl -Ss https://my-netdata.io/kickstart-static64.sh)

# Quittez la session de root
$ exit
```

🌞 Manipulation du service Netdata

- Je vérifie que le service est actif avec ``` sudo systemctl status netdata ```

```
[benji@web ~]$ sudo systemctl status netdata
● netdata.service - Real time performance monitoring
   Loaded: loaded (/usr/lib/systemd/system/netdata.service; enabled; vendor preset: disabled)
   Active: active (running) since Mon 2021-10-11 09:46:22 CEST; 54s ago
```
Nous observons que le service est actif et "enable" donc il est actif dès le boot.

Grâce à la commande `ss -lant`, j'observe que le port qu'utilise netdata est le **19999** .

```
[benji@db ~]$ sudo firewall-cmd --add-port=19999/tcp --permanent
success
``` 

- Je vérifie son fonctionnement sur le navigateur: 

![](https://i.imgur.com/JJSshVt.png)

🌞 Setup Alerting

- Je crée un webhook sur discord et je colle sont lien dans /opt/netdata/etc/netdata/health_alarm_notify.conf:

```
###############################################################################
# sending discord notifications

# note: multiple recipients can be given like this:
#                  "CHANNEL1 CHANNEL2 ..."

# enable/disable sending discord notifications
SEND_DISCORD="YES"

# Create a webhook by following the official documentation -
# https://support.discordapp.com/hc/en-us/articles/228383668-Intro-to-Webhooks
DISCORD_WEBHOOK_URL="https://discord.com/api/webhooks/897054968975069215/uhHUq6VCuzC-UHFS-6ltttEXXC1OFWRTUVamN7PzYiOmTz2G5i2Ga88Q1HUq-LZXn56s"

# if a role's recipients are not configured, a notification will be send to
# this discord channel (empty = do not send a notification for unconfigured
# roles):
DEFAULT_RECIPIENT_DISCORD="alarms"
```

Ensuite je test en faisant : 

```
# become user netdata
sudo su -s /bin/bash netdata

# enable debugging info on the console
export NETDATA_ALARM_NOTIFY_DEBUG=1

# send test alarms to sysadmin
/usr/libexec/netdata/plugins.d/alarm-notify.sh test

# send test alarms to any role
/usr/libexec/netdata/plugins.d/alarm-notify.sh test "ROLE"
```

Résultat :

![](https://i.imgur.com/XxTKIaH.png)

🌞 Config alerting

Je crée un fichier health.d/ram-usage.conf:

```
[benji@web netdata]$ sudo cat health.d/ram-usage.conf 
alarm: ram_usage
    on: system.ram
lookup: average -1m percentage of used
 units: %
 every: 1m
  warn: $this > 50 
  crit: $this > 80
  info: The percentage of RAM being used by the system.
```
puis j'execute cette commande : 

```
sudo sed -i 's/curl=""/curl="\/opt\/netdata\/bin\/curl -k"/' /opt/netdata/etc/netdata/health_alarm_notify.conf
```

Pour finir je restart netdata et lance le stress test: 

```
[benji@web netdata]$ sudo systemctl restart netdata
[benji@web netdata]$ stress --vm 1 --vm-bytes 3512M -t 90s -v
stress: info: [5853] dispatching hogs: 0 cpu, 0 io, 1 vm, 0 hdd
stress: dbug: [5853] using backoff sleep of 3000us
stress: dbug: [5853] setting timeout to 90s
```

## II. Backup

Tout d'abord je commence par réaliser les étapes précédente pour monitorer ma machine de backup.

Ensuite je crée le dossier qui va acceuillir le partage : 

```
[benji@backup ~]$ sudo mkdir /srv/backup/web.tp2.linux
[benji@backup ~]$ sudo mkdir /srv/backup/db.tp2.linux
```
Je donne les droits appropriés: 

```
[benji@backup ~]$ sudo chmod 666 /srv/backup/
```

J'installe les outils nfs : 
```
[benji@backup ~]$ sudo dnf -y install nfs-utils
Last metadata expiration check: 0:44:45 ago on Tue 12 Oct 2021 11:17:06 AM CEST.
Dependencies resolved.srv
```

Je fais en sorte qu'il démarre dès le boot: 
```
[benji@backup ~]$ sudo systemctl enable --now rpcbind nfs-server
Created symlink /etc/systemd/system/multi-user.target.wants/nfs-server.service → /usr/lib/systemd/system/nfs-server.service.
```

J'autorise le service auprès du firewall : 

```
[benji@backu ~]$ sudo firewall-cmd --add-service=nfs --permanent
success
```

- Sur la machine client : 

J'installe nfs : 

```
[benji@web ~]$ sudo dnf -y install nfs-utils
[sudo] password for benji:
Rocky Linux 8 - AppStream                        12 kB/s | 4.8 kB     00:00
Rocky Linux 8 - AppStream                       4.8 MB/s | 9.1 
```
Je modifie le fichier /etc/idmapd.conf : 

```
Domain = tp2.linux
```

TEST 

```
[benji@web ~]$ sudo mount -l | grep backup
10.102.1.13:/srv/backup/web.tp2.linux on /srv/backup type nfs4 (rw,relatime,vers=4.2,rsize=524288,wsize=524288,namlen=255,hard,proto=tcp,timeo=600,retrans=2,sec=sys,clientaddr=10.102.1.11,local_lock=none,addr=10.102.1.13)

```

Je vérifie qu'il reste de la place:

```
[benji@localhost ~]$ df -h
Filesystem           Size  Used Avail Use% Mounted on
devtmpfs             1.9G     0  1.9G   0% /dev
tmpfs                1.9G  348K  1.9G   1% /dev/shm
tmpfs                1.9G   17M  1.9G   1% /run
tmpfs                1.9G     0  1.9G   0% /sys/fs/cgroup
/dev/mapper/rl-root  6.2G  2.3G  4.0G  36% /
/dev/sda1           1014M  241M  774M  24% /boot
tmpfs                374M     0  374M   0% /run/user/1000
```

Je vérifie que je peux écrire

```
[benji@web backup]$ sudo touch bonjour
[benji@web backup]$ ls
bonjour

```


Je fais en sorte que le montage se fasse dès le boot

```
10.102.1.13:/srv/backup/web.tp2.linux /srv/backup               nfs     defaults        0 0
```

🌟 BONUS : partitionnement avec LVM

### 3. Backup de fichiers

#### A. Unité de service

Je crée un fichier tp2_backup.service dans `etc/systemd/system/`: 

```
[Unit]
Description=Our own lil backup service (TP2)

[Service]
ExecStart=/srv/tp2_backup.sh /home/benji/dest /home/benji/test
Type=oneshot
RemainAfterExit=no

[Install]
WantedBy=multi-user.target
```

🌞 Tester le bon fonctionnement

```
[benji@backup ~]$ sudo systemctl start tp2_backup
[sudo] password for benji:
[benji@backup ~]$ ls dest/
hello_211023_171143.tar.gz
```

### B. Timer

Je crée l'unité de service `Timer`:

```
[benji@backup /]$ sudo cat  /etc/systemd/system/tp2_backup.timer
Description=Periodically run our TP2 backup script
Requires=tp2_backup.service
[Timer]
Unit=tp2_backup.service
OnCalendar=*-*-* *:*:00
[Install]
WantedBy=timers.target
```
🌞 Activez le timer

Je démarre le timer: 

```
[benji@backup /]$ sudo systemctl start tp2_backup.timer
[benji@backup /]$ sudo systemctl enable tp2_backup.timer
Created symlink /etc/systemd/system/timers.target.wants/tp2_backup.timer → /etc/systemd/system/tp2_backup.timer.
[benji@backup /]$ sudo systemctl status tp2_backup.timer
● tp2_backup.timer
   Loaded: loaded (/etc/systemd/system/tp2_backup.timer; enabled; vendor>
   Active: active (waiting) since Sat 2021-10-23 17:20:53 CEST; 27s ago
  Trigger: Sat 2021-10-23 17:22:00 CEST; 38s left

Oct 23 17:20:53 backup.tp2.linux systemd[1]: /etc/systemd/system/tp2_bac>
Oct 23 17:20:53 backup.tp2.linux systemd[1]: /etc/systemd/system/tp2_bac>
Oct 23 17:20:53 backup.tp2.linux systemd[1]: Started tp2_backup.timer.
Oct 23 17:21:04 backup.tp2.linux systemd[1]: /etc/systemd/system/tp2_bac>
Oct 23 17:21:04 backup.tp2.linux systemd[1]: /etc/systemd/system/tp2_bac>
```
Le `enable` présent dans le status atteste que le service démarre bien pendant le boot.

🌞 Tests !

```
[benji@backup ~]$ ls dest/
hello_211023_172411.tar.gz  hello_211023_172711.tar.gz
hello_211023_172511.tar.gz  hello_211023_172811.tar.gz
// 1 minutes après
[benji@backup ~]$ ls dest/
hello_211023_172411.tar.gz  hello_211023_172711.tar.gz
hello_211023_172511.tar.gz  hello_211023_172811.tar.gz
hello_211023_172611.tar.gz
```

### C. Contexte
On sauvegarde le dossier qui contient le site NextCloud (quelque part dans /var/)
```
[benji@web /]$ sudo systemctl list-timers
NEXT                          LEFT          LAST                        >
Sun 2021-10-24 03:15:00 CEST  9h left       Sat 2021-10-23 17:52:04 CEST>
Sun 2021-10-24 15:18:03 CEST  21h left      Sat 2021-10-23 15:18:03 CEST>
```
La prochaine sauvegarde va bien se faire à 3h15.

📁 Fichier [/etc/systemd/system/tp2_backup.timer](annexes part2/tp2_backup.timer)
📁 Fichier [/etc/systemd/system/tp2_backup.service](annexes part2/tp2_backup.service)


### Backup de base de données

🌞 Création d'un script /srv/tp2_backup_db.sh

```
#!/bin/bash
# Saving db
# Gelineau Benjamin- 20/10/2021


  DATE=$(date +%y%m%d_%H%M%S)
  Destination=$1
  Target=nextcloud
  Here=$(pwd)/"hello_${DATE}.tar.gz"

  mysqldump --user=benji --password=meow --databases nextcloud

  tar cvzf "hello_${DATE}.tar.gz" backup_donnees.sql
  rsync -av --remove-source-files ${Here} ${Destination}
  rm -rf backup_donnees.sql
  ls -tp "${Destination}" | grep -v '/$' | tail -n +6 | xargs -I {} rm -- ${Destination}/{}
  
  
++ date +%y%m%d_%H%M%S
+ DATE=211024_141846
+ Destination=test/
+ Target=nextCloud
++ pwd
+ Here=/home/benji/hello_211024_141846.tar.gz
+ mysqldump --user=benji --password=meow --databases nextCloud
mysqldump: Got error: 1045: "Access denied for user 'benji'@'localhost' (using password: YES)" when trying to connect
+ tar cvzf hello_211024_141846.tar.gz backup_donnees.sql
backup_donnees.sql
+ rsync -av --remove-source-files /home/benji/hello_211024_141846.tar.gz test/
sending incremental file list
hello_211024_141846.tar.gz

sent 172 bytes  received 43 bytes  524.00 bytes/sec
total size is 123  speedup is 0.44
+ rm -rf backup_donnees.sql
+ tail -n +6
+ ls -tp test/
+ grep -v '/$'
+ xargs -I '{}' rm -- 'test//{}'  
```

Je vérifie: 

```
[benji@db test]$ ls
hello_211024_141846.tar.gz
```
📁 Fichier [/srv/tp2_backup_db.sh](annexes part2/tp2_backup_db.sh)


Je crée l'unité de service tp2_backup_db.service: 

```
[Unit]
Description=Our own lil backup service (TP2)

[Service]
ExecStart=sudo bash /srv/tp2_backup_db.sh /home/benji/test/ nextCloud
Type=oneshot
RemainAfterExit=no

[Install]
WantedBy=multi-user.target
```

Je crée l'unité de service tp2_backup_db.timer: 
```
Description=Periodically run our backup database script
Requires=tp2_backup_db.service

[Timer]
Unit=tp2_backup_db.service
OnCalendar=*-*-* 3:15:00

[Install]
WantedBy=timers.target
```

J'active le timer: 

```
[benji@db /]$ sudo systemctl start tp2_backup_db.timer
[benji@db /]$ sudo systemctl enable tp2_backup_db.timer
Created symlink /etc/systemd/system/timers.target.wants/tp2_backup_db.timer → /et                                                                                                                           c/systemd/system/tp2_backup_db.timer
```

je vérifie: 
```
[benji@db /]$ sudo systemctl list-timers
NEXT                          LEFT       LAST                          PASSED    UNIT                         ACTIVATES
Mon 2021-10-25 03:15:00 CEST  12h left   n/a                           n/a       tp2_backup_db.timer          tp2_backup_db.service
```

📁 Fichier [/etc/systemd/system/tp2_backup_db.timer](annexes part2/tp2_backup_db.timer)
📁 Fichier [/etc/systemd/system/tp2_backup_db.service](annexes part2/tp2_backup_db.service)

## III. Reverse Proxy

🌞 Installer NGINX

```
[benji@front ~]$ sudo dnf install epel-release
[benji@front ~]$ sudo dnf install nginx
```

🌞 Tester !

```
[benji@front ~]$ sudo systemctl start nginx // démarre le service
[benji@front ~]$ sudo systemctl enable nginx // démarre dès le boot
Created symlink /etc/systemd/system/multi-user.target.wants/nginx.service → /usr/lib/systemd/system/nginx.service.
```

J'ouvre le port de nginx: 

```
[benji@front ~]$ sudo firewall-cmd --add-port=80/tcp --permanent
success
[benji@front ~]$  sudo firewall-cmd --reload
success
```

Je vérifie que je peux me connecter avec un `curl 10.102.1.14` depuis mon pc: 

```
C:\Users\Benjamin>curl 10.102.1.14
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
  <head>
    <title>Test Page for the Nginx HTTP Server on Rocky Linux</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
[...]
```

🌞 Explorer la conf par défaut de NGINX

- L'utilisateur que NGINX utilise par défaut est `nginx`.

- Je repère le bloc `server{}` dans le fichier de configuration: 

```
    server {
        listen       80 default_server;
        listen       [::]:80 default_server;
        server_name  _;
        root         /usr/share/nginx/html;

        # Load configuration files for the default server block.
        include /etc/nginx/default.d/*.conf;

        location / {
        }

        error_page 404 /404.html;
            location = /40x.html {
        }

        error_page 500 502 503 504 /50x.html;
            location = /50x.html {
        }
    }
```

- Voici les lignes d'inclusions du fichiers de conf: 

```
include /usr/share/nginx/modules/*.conf;
include /etc/nginx/conf.d/*.conf;
include /etc/nginx/default.d/*.conf;
```

🌞 Modifier la conf de NGINX

- J'ai supprimé le bloc server de base dans le fichier conf de nginx puis j'ai créee une fichier `/etc/nginx/conf.d/web.tp2.linux.conf`:

```
server {
    listen 80;

    server_name web.tp2.linux; 

    location / {
        proxy_pass http://web.tp2.linux;
    }
}
```

## 3. Bonus HTTPS

🌟 Générer la clé et le certificat pour le chiffrement

```
[benji@front ~]$ openssl req -new -newkey rsa:2048 -days 365 -nodes -x509 -keyout server.key -out server.crt
Generating a RSA private key
.....................................+++++
..............+++++
writing new private key to 'server.key'
-----
You are about to be asked to enter information that will be incorporated
into your certificate request.
What you are about to enter is what is called a Distinguished Name or a DN.
There are quite a few fields but you can leave some blank
For some fields there will be a default value,
If you enter '.', the field will be left blank.
-----
Country Name (2 letter code) [XX]:FR
State or Province Name (full name) []:Gironde
Locality Name (eg, city) [Default City]:Bordeaux
Organization Name (eg, company) [Default Company Ltd]:
Organizational Unit Name (eg, section) []:
Common Name (eg, your name or your server's hostname) []:web.tp2.linux
Email Address []:

[benji@front ~]$  sudo mv server.key /etc/pki/tls/private/web.tp2.linux.key
[benji@front ~]$ sudo mv server.crt /etc/pki/tls/certs/web.tp2.linux.crt
[benji@front ~]$ sudo chown root:root /etc/pki/tls/private/web.tp2.linux.key
[benji@front ~]$ sudo chown root:root /etc/pki/tls/certs/web.tp2.linux.crt
[benji@front ~]$ sudo chmod 400 /etc/pki/tls/private/web.tp2.linux.key
[benji@front ~]$ sudo chmod 644 /etc/pki/tls/certs/web.tp2.linux.crt
```

🌟 Modifier la conf de NGINX

```
server {
    # on demande à NGINX d'écouter sur le port 443 avec chiffrement
    listen 443 ssl;

    server_name web.tp2.linux;
    # On indique le chemin du certificat et de la clé
    ssl_certificate     /etc/pki/tls/certs/web.tp2.linux.crt; 
    ssl_certificate_key /etc/pki/tls/private/web.tp2.linux.key;
   
    location / {
        proxy_pass http://web.tp2.linux;
    }
}
```

Puis j'ouvre le port 443: 

```
[benji@front ~]$ sudo firewall-cmd --add-port=443/tcp --permanent
success
[benji@front ~]$  sudo firewall-cmd --reload
success
```
🌟 TEST

Je test la connexion avec un curl: 

```
C:\Users\Benjamin>curl -k 10.102.1.1
<!DOCTYPE html>
<html>
<head>
        <title>Accueil WAMPSERVER</title>
        <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <meta name="viewport" content="width=device-width">
        <link id="stylecall" rel="stylesheet" href="wampthemes/classic/style.css" />
        <link rel="shortcut icon" href="favicon.ico" type="image/ico" />
</head>
```

## IV. Firewalling
### Mise en place
```
[benji@db /]$ sudo firewall-cmd --set-default-zone=drop # on configure la zone "drop" comme zone par défaut
success
[benji@db /]$ sudo firewall-cmd --zone=drop --add-interface=enp0s8 # ajout explicite de l'interface host-only à la zone "drop"
success
The interface is under control of NetworkManager, setting zone to 'drop'.
success
[benji@db /]$ sudo firewall-cmd --new-zone=ssh --permanent #  création d'une nouvelle zone, qui autorisera le trafic lié à l'ip de la machine web.
success
[benji@db /]$ sudo firewall-cmd --zone=ssh --add-source=10.102.1.1/32 --permanent
success
[benji@db /]$ sudo firewall-cmd --zone=ssh --add-port=22/tcp --permanent # autorisation du ssh
success
[benji@db /]$  sudo firewall-cmd --new-zone=db --permanent # Création d'une zone db
success
[benji@db /]$ sudo firewall-cmd --zone=db --add-source=10.102.1.11/32 --permanent # ajout de la machine web dans cette zone
success
[benji@db /]$ sudo firewall-cmd --zone=db --add-port=3306/tcp --permanent
success
[benji@db /]$ sudo firewall-cmd --reload
success
```

🌞 Montrez le résultat de votre conf avec une ou plusieurs commandes firewall-cmd

```
[benji@db /]$ sudo firewall-cmd --get-active-zones
db
  sources: 10.102.1.11/32
drop
  interfaces: enp0s8 enp0s3
mariadb-access
  sources: 10.102.1.11/24
ssh
  sources: 10.102.1.1/32


[benji@db /]$ sudo firewall-cmd --get-active-zones
db
  sources: 10.102.1.11/32
drop
  interfaces: enp0s8 enp0s3
mariadb-access
  sources: 10.102.1.11/24
ssh
  sources: 10.102.1.1/32

[benji@db /]$ sudo firewall-cmd --list-all --zone=ssh
ssh (active)
  target: default
  icmp-block-inversion: no
  interfaces:
  sources: 10.102.1.1/32
  services:
  ports: 22/tcp
  protocols:
  masquerade: no
  forward-ports:
  source-ports:
  icmp-blocks:
  rich rules:


[benji@db /]$ sudo firewall-cmd --list-all --zone=db
db (active)
  target: default
  icmp-block-inversion: no
  interfaces:
  sources: 10.102.1.11/32
  services:
  ports: 3306/tcp
  protocols:
  masquerade: no
  forward-ports:
  source-ports:
  icmp-blocks:
  rich rules:
```

### B. Serveur Web

🌞 Restreindre l'accès au serveur Web web.tp2.linux

Setup : 

```
[benji@web ~]$  sudo firewall-cmd --new-zone=front --permanent
success
[benji@web ~]$ sudo firewall-cmd --zone=front --add-source=10.102.1.1/32 --permanent
success
[benji@web ~]$ sudo firewall-cmd --zone=front --add-port=22/tcp --permanent
success
[benji@web ~]$ sudo firewall-cmd --zone=front --add-source=10.102.1.14/32 --permanent
success
[benji@web ~]$ sudo firewall-cmd --zone=front --add-port=80/tcp --permanent
success
```

🌞 Montrez le résultat de votre conf avec une ou plusieurs commandes firewall-cmd

Ici je n'ai crée qu'une zone `front` pour donner accès au ssh et la machine front.
```
[benji@web ~]$ sudo firewall-cmd --list-all --zone=front                 front (active)
  target: default
  icmp-block-inversion: no
  interfaces:
  sources: 10.102.1.1/32 10.102.1.14/32
  services:
  ports: 22/tcp 80/tcp
  protocols:
  masquerade: no
  forward-ports:
  source-ports:
  icmp-blocks:
  rich rules:
```

### C. Serveur de backup

Accès au ssh : 

```
[benji@backup /]$ sudo firewall-cmd --zone=drop --add-interface=enp0s8 --permanent
The interface is under control of NetworkManager, setting zone to 'drop'.
success
[benji@backup /]$  sudo firewall-cmd --new-zone=ssh --permanent
success
[benji@backup /]$  sudo firewall-cmd --zone=ssh --add-source=10.102.1.1/32 --permanent
success
[benji@backup /]$ sudo firewall-cmd --zone=ssh --add-port=22/tcp --permanent
success
```

Je donne accès aux machines qui effectuent des backup via nfs: 

```
[benji@backup /]$ sudo firewall-cmd --new-zone=nfs --permanent
success
[benji@backup /]$  sudo firewall-cmd --zone=nfs --add-source=10.102.1.11/32 --permanent
success
[benji@backup /]$  sudo firewall-cmd --zone=nfs --add-source=10.102.1.12/32 --permanent
success
[benji@backup /]$  sudo firewall-cmd --zone=nfs --add-port=19999/tcp --permanent
success
[benji@backup /]$  sudo firewall-cmd --zone=nfs --add-service=nfs --permanent
success
```

🌞 Montrez le résultat de votre conf avec une ou plusieurs commandes firewall-cmd

```
[benji@backup /]$  sudo firewall-cmd --get-active-zones
drop
  interfaces: enp0s8 enp0s3
nfs
  sources: 10.102.1.11/32 10.102.1.12/32
ssh
  sources: 10.102.1.1/32


[benji@backup /]$ sudo firewall-cmd --list-all --zone=ssh
ssh (active)
  target: default
  icmp-block-inversion: no
  interfaces:
  sources: 10.102.1.1/32
  services:
  ports: 22/tcp
  protocols:
  masquerade: no
  forward-ports:
  source-ports:
  icmp-blocks:
  rich rules:

[benji@backup /]$ sudo firewall-cmd --list-all --zone=nfs
nfs (active)
  target: default
  icmp-block-inversion: no
  interfaces:
  sources: 10.102.1.11/32 10.102.1.12/32
  services: nfs
  ports: 19999/tcp
  protocols:
  masquerade: no
  forward-ports:
  source-ports:
  icmp-blocks:
  rich rules:

```

### D. Reverse Proxy

🌞 Restreindre l'accès au reverse proxy front.tp2.linux

Accès au ssh: 

```
[benji@front ~]$ sudo firewall-cmd --zone=drop --permanent --add-interface=enp0s8
The interface is under control of NetworkManager, setting zone to 'drop'.
success
[benji@front ~]$ sudo firewall-cmd --permanent --new-zone=ssh
success
[benji@front ~]$ sudo firewall-cmd --zone=ssh --add-source=10.102.1.1/32 --permanent
success
[benji@front ~]$ sudo firewall-cmd --zone=ssh --add-port=22/tcp --permanent
success
```

Accès limité pour le réseau `10.102.1.0` : 

```
[benji@front ~]$  sudo firewall-cmd --new-zone=proxy --permanent
success
[benji@front ~]$ sudo firewall-cmd --zone=proxy --add-source=10.102.1.0/24 --permanent
success
```

🌞 Montrez le résultat de votre conf avec une ou plusieurs commandes firewall-cmd

```
[benji@front ~]$  sudo firewall-cmd --get-active-zones
drop
  interfaces: enp0s8 enp0s3
proxy
  sources: 10.102.1.0/24
ssh
  sources: 10.102.1.1/32
  
[benji@front ~]$ sudo firewall-cmd --list-all --zone=ssh
ssh (active)
  target: default
  icmp-block-inversion: no
  interfaces:
  sources: 10.102.1.1/32
  services:
  ports: 22/tcp
  protocols:
  masquerade: no
  forward-ports:
  source-ports:
  icmp-blocks:
  rich rules:

benji@front ~]$ sudo firewall-cmd --list-all --zone=proxy
proxy (active)
  target: default
  icmp-block-inversion: no
  interfaces:
  sources: 10.102.1.0/24
  services:
  ports:
  protocols:
  masquerade: no
  forward-ports:
  source-ports:
  icmp-blocks:
  rich rules:

```

### E. Tableau récap

🌞 Rendez-moi le tableau suivant, correctement rempli :

| Machine            | IP            | Service                 | Port ouvert | IPs autorisées |
|--------------------|---------------|-------------------------|-------------|---------------|
| `web.tp2.linux`    | `10.102.1.11` | Serveur Web             |80           | 10.102.14     |
| `db.tp2.linux`     | `10.102.1.12` | Serveur Base de Données |3306         | 10.102.1.11   |
| `backup.tp2.linux` | `10.102.1.13` | Serveur de Backup (NFS) |19999        | 10.102.1.11 & 10.102.1.12|
| `front.tp2.linux`  | `10.102.1.14` | Reverse Proxy           |443          | 10.102.1.1 - 10.102.1.254|