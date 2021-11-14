# Projet N.A.S GELINEAU Benjamin
## I. Principe
### a. Définition

- Un NAS (Network Attached Storage) est un serveur de fichiers relié à un réseau dont la principale fonction est le stockage de fichier en masse à un endroit centralisé.

### b. Domaine d’utilisation
- Le serveur NAS peut tout aussi bien vous servir pour sauvegarder vos photos directement après la prise de vue qu’a réaliser un serveur de streaming pour regarder vos films/séries que vous appréciez temps.
Il est avantageux car il est accessible par toutes les personnes qui sont autorisées par l’administrateur et depuis n’importe où mais également car il est pratique pour faire des sauvegardes d’un réseau par exemple. C’est très utilisé dans les entreprises et même dans le privé.

## II. Paramétrage du N.A.S sur un micro-ordinateur
### a. Prérequis
- Pour ce projet nous avons décidé de faire tourner le serveur N.A.S sur un micro-ordinateur de type **raspberry pi** avec une distribution linux (**raspbian**) installé au préalable. Il vous faudra par conséquent avoir une **carte sd** pour installer raspbian et des **disques dur/clé usb** pour stocker vos données.Vous pouvez tout aussi bien installer une distribution linux tels Debian sur un vieille ordinateur que vous n’utilisez plus ou plus assez puissant pour vos tâches quotidiennes.

### b. Configuration du Raspberry Pi

Une fois que le matériel est prêt, mettons à jour le Raspberry Pi avec ces commandes : 
```
> sudo apt update
> sudo apt upgrade
```

Maintenant que les mises à jour terminées, créons les dossiers qui seront partagés sur le NAS : 

```
> sudo mkdir /home/shares
> sudo mkdir /home/shares/public
> sudo chown -R root:users /home/shares/public 
> sudo chmod -R ug=rwx,o=rx /home/shares/public
```

### c.  Configuration de Samba

- Le Raspberry Pi étant configuré, il faut maintenant mettre en place le logiciel Samba afin de gérer la mise en réseaux du NAS afin de pouvoir y accéder depuis le réseau.
Il faut donc commencer par installer Samba et éditer le fichier de configuration:

```
> sudo apt install samba samba-common-bin
> sudo nano /etc/samba/smb.conf
```

* Si vous souhaitez mettre en place une authentification, allez à la ligne “##### Authentification #####” et ajoutez la ligne suivante : `security = user`

Maintenant en bas du fichier nous allons ajouter quelques paramètres pour permettre l’accès public : 

```
[public]
    comment= Public Storage path = /home/shares/public
    valid users = @users ( si utilisateur précis alors valid users = nom)
    force group = users create mask = 0660
    directory mask = 0771 (modifiable à votre bon vouloir)
    read only = no
```

📁 Fichier [smb.conf](annexes/smb.conf)

Vous pouvez désormais fermer le fichier et redémarrer Samba avec cette commande : 

```
> sudo /etc/init.d/smbd restart
```

Pour terminer, ajoutons un utilisateur à Samba : 
```
> sudo smbpasswd -a pi
```

### d. Ajouter un périphérique

- Commencez par connecter votre périphérique au Raspberry PI. Regardons maintenant le nom du périphérique avec la commande `> dmesg`, les noms les plus fréquents sont sda1,sdb1...

(Attention, si le périphérique n’est pas formaté avec un système de fichier Li- nux (ext3, ext4, etc…) éxécutez les commandes suivantes :

```
> umount /dev/[Nom du périphérique]
> sudo mkfs.ext4 /dev/[Nom du périphérique]
```

Créez maintenant le répertoire pour pouvoir accéder au périphérique via le NAS et accordez-lui les permissions nécessaires : 

```
> sudo mkdir /home/shares/public/disk1
> sudo chown -R root:users /home/shares/public/disk1 
> sudo chmod -R ug=rwx,o=rx /home/shares/public/disk1
```

Utilisez maintenant la commande suivante pour monter le périphérique dans le dossier : 

```
> sudo mount /dev/sda1 /home/shares/public/disk1
```

Pour monter automatiquement les périphériques au démarrage du Raspberry PI éditez le fichier           et ajoutez à la fin du fichier la ligne suivante : 

```
> sudo nano /etc/fstab
/dev/sda1 /home/shares/public/disk1 auto noatime 0 0
```
### e. Accéder à son N.A.S 

- Bravo ! Votre système N.A.S est prêt à être utilisé dans votre réseaux local. Sur ordinateur il vous suffira de cliquer sur « Connecter un lecteur réseau » en haut à gauche dans votre gestionnaire de fichier et d’ajouter les informations suivantes :

![](https://i.imgur.com/xyWTmC2.png)
Si **\\raspberrypi\public** ne fonctionne pas, essayé **\\ip_nas\public**

Ceci devrait apparaitre (avec un stockage correspondant au disque, le mien était plein) sinon redémarrer votre pc.

![](https://i.imgur.com/sJToHIp.png)

- Sur smartphone, vous pouvez télécharger l'application FE file Explorer

## III. Mise en place du VPN

### a.  Accéder à son N.A.S dans le monde entier

- Avoir accès à son N.A.S dans son serveur local c’est bien mais y avoir accès dans le monde entier c’est mieux. Pour cela nous allons devoir mettre en place un « tunnel » vers votre réseau ou est hébergé votre serveur.
Tout d’abord il va devoir installer l’outil PIVPN avec cette commande : 
```
> curl -L https://install.pivpn.io > pivpn.sh (nous avons fait ça pour vérifier le contenu de pivpn)
> bash pivpn.sh (puis yes pour confirmer l’installation)
```

Vous devriez avoir une chose ressemblant à cela : 
![](https://i.imgur.com/vMppFof.png)


Appuyez sur ok, choissisez l’utilisateur administrateur de votre choix puis valider ensuite choissisez Wireguard, nous laissons le port par défaut (51820), sélectionner Quad9.

### b.	Configuration d’un appareil pour wireguard.

-	Connecter vous en root à votre machine et rentrer la ligne de commande suivante : 

```
root@raspberrypi:/home/pi# pivpn add
Enter a Name for the Client: meow 
::: Client Keys generated
::: Client config generated
::: Updated server config
::: WireGuard reloaded
======================================================================
::: Done! meow.conf successfully created!
::: meow.conf was copied to /home/pi/configs for easy transfer.
::: Please use this profile only on one device and create additional
::: profiles for other devices. You can also use pivpn -qr
::: to generate a QR Code you can scan with the mobile app.
======================================================================
```
-	Télécharger l’application wireguard android/ios/windows/mac/linux, il ne reste plus qu’à scanner le code qr avec votre appareil sur l’application wireguard. Pour un pc il faudra transférer le fichier de configuration de votre client stocké dans « /home/nom admin pivpn/config » dans le logiciel pc.

-	Penser bien à rediriger le port de votre rooter vers l’adresse ip de votre machine (disponible avec la commande « ip a ») pour terminer.


## III.	Mise en place d’un gestionnaire multimédia
- Notre N.A.S est d’ores et déjà prêt à l’emploi, cependant étant cinéphile je souhaite avoir accès à une interface graphique plus agréable que le gestionnaire de fichier par défaut et surtout des fonctionnalités plus poussées comme le cast pour regarder mes films stockés sur mon N.A.S sur ma télévision.
Pour cela il existe plusieurs solutions comme Plex,emby ou jellyfin. Dans notre cas nous allons choisir jellyfin car il propose une expérience plus complète que plex et emby dans sa version gratuite. 

#### a.	Téléchargement

```
>sudo apt install apt-transport-https
>wget -O - https://repo.jellyfin.org/jellyfin_team.gpg.key | sudo apt-key add -
>echo "deb [arch=$( dpkg --print-architecture )] https://repo.jellyfin.org/$( awk -F'=' '/^ID=/{ print $NF }' /etc/os-release ) $( awk -F'=' '/^VERSION_CODENAME=/{ print $NF }' /etc/os-release ) main" | sudo tee /etc/apt/sources.list.d/jellyfin.list
>sudo apt update
>sudo apt install jellyfin
```

-	Ensuite rendez-vous à l’adresse suivante :  http://localhost:8096 
la configuration ensuite est simple.

![](https://i.imgur.com/ZB1HIkK.jpg)

- Je configure le firewall: 
```
pi@raspberrypi:/$ sudo apt-get install ufw -y
pi@raspberrypi:/$ sudo ufw enable
Firewall is active and enabled on system startup
pi@raspberrypi:/ $ sudo ufw allow 8096/tcp
Rule added
Rule added (v6)
```

## IV. Maintien en condition opérationnelle

### a. Principe
La surveillance ou *monitoring* consiste à surveiller la bonne santé d'une entité.  
J'utilise volontairement le terme vague "entité" car cela peut être très divers :

- une machine
- une application
- un lien entre deux machines
- etc.

### b. Setup

```
pi@raspberrypi:~ $ bash <(curl -Ss https://my-netdata.io/kickstart.sh)
```

Il faut maintenant configurer le firewall : 
```
pi@raspberrypi:~/netdata $ sudo ufw allow 19999/tcp
Rule added
Rule added (v6)
pi@raspberrypi:~/netdata $ sudo ufw reload
Firewall reloaded
```

### c. Alerting

- Je crée un webhook sur discord et je colle sont lien dans etc/netdata/health_alarm_notify.conf:

```
###############################################################################
# sending discord notifications

# note: multiple recipients can be given like this:
#                  "CHANNEL1 CHANNEL2 ..."

# enable/disable sending discord notifications
SEND_DISCORD="YES"

# Create a webhook by following the official documentation -
# https://support.discordapp.com/hc/en-us/articles/228383668-Intro-to-Webhooks
DISCORD_WEBHOOK_URL="lien webhook"

# if a role's recipients are not configured, a notification will be send to
# this discord channel (empty = do not send a notification for unconfigured
# roles):
DEFAULT_RECIPIENT_DISCORD="alarms"
```
📁 Fichier [health_alarm_notify.conf](annexes/health_alarm_notify.conf)

Je crée un fichier health.d/ram-usage.conf pour envoyer une notification sur discord a partir d'un certain niveau de ram utilisé:

``` 
alarm: ram_usage
    on: system.ram
lookup: average -1m percentage of used
 units: %
 every: 1m
  warn: $this > 50 
  crit: $this > 80
  info: The percentage of RAM being used by the system.
```

📁 Fichier [ram-usage.conf](annexes/ram-usage.conf)

### d. Firewalling

- Voici les règles de notre firewall ajoutées jusqu'a présent : 

```
pi@raspberry:~ $ sudo ufw show added
Added user rules (see 'ufw status' for running firewall):
ufw allow 8096/tcp
ufw allow 51820/udp
ufw allow 19999/tcp
ufw allow from 192.168.56.1 to any port 22 proto tcp

pi@raspberry:~ $ sudo ufw status
Status: active

To                         Action      From
--                         ------      ----
8096/tcp                   ALLOW       Anywhere
51820/udp                  ALLOW       Anywhere
19999/tcp                  ALLOW       Anywhere
22/tcp                     ALLOW       192.168.56.1
8096/tcp (v6)              ALLOW       Anywhere (v6)
51820/udp (v6)             ALLOW       Anywhere (v6)
19999/tcp (v6)             ALLOW       Anywhere (v6)
```

## V.Script

### a- Mise en place

- Afin de réaliser une grande partie de ce projet en une commande il vous faut télécharger le script fournit dans les annexes sur la machine qui sera votre nas et faire la commande suivant : 
```
sudo bash nas.sh
```
📁 Fichier [nas.sh](annexes/nas.sh)

### b- Pour compléter

- Afin de finir le projet complétement, il vous faudra monter votre disque dur comme expliquer précédement. 
- L'alerting discord manque aussi dans le script