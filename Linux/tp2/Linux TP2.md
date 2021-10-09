# Linux TP2

## I. Un premier serveur web

### 1. Installation

- J'installe le serveur apache avec la commande ``` sudo dnf install httpd ```

Je vérifie que le paquet est bien installé : 
``` 
[benji@web /]$ httpd -v
Server version: Apache/2.4.37 (rocky)
Server built:   Jun 11 2021 15:35:05
```

Je démarre le service avec la commande ``` sudo systemctl start httpd ```

Je vérifie son fonctionnement avec la commande ``` sudo systemctl status httpd ```

```
 httpd.service - The Apache HTTP Server
   Loaded: loaded (/usr/lib/systemd/system/httpd.service; disabled; vendor pres>
   Active: active (running) since Wed 2021-09-29 10:47:21 CEST; 
```

Avec la commande ``` sudo systemctl enable httpd ```, je fais en sorte que le service soit actif dès le boot.

```
Created symlink /etc/systemd/system/multi-user.target.wants/httpd.service → /usr/lib/systemd/system/httpd.service.
```
Afin de connaitre le port qu'utilise httpd, je tape la commande ``` sudo ss -lnpa ```: 
```
tcp        LISTEN       0            128                                                                    *:80                                 *:*           users:(("httpd",pid=1971,fd=4),("httpd",pid=1970,fd=4),("httpd",pid=1969,fd=4),("httpd",pid=1967,fd=4))
```
j'observe que c'est le port 80 qu'utilise le service httpd.

j'ouvre mon port 80 : 
```
[benji@web ~]$ sudo firewall-cmd --add-port=80/tcp --permanent
success
[benji@web ~]$ sudo firewall-cmd --reload
success
[benji@web ~]$ sudo firewall-cmd --list-all
public (active)
  target: default
  icmp-block-inversion: no
  interfaces: enp0s3 enp0s8
  sources:
  services: cockpit dhcpv6-client ssh
  ports: 8888/tcp 80/tcp
  protocols:
  masquerade: no
  forward-ports:
  source-ports:
  icmp-blocks:
  rich rules:

```

#### TEST

Je vérifie que le service est bien démarré et qu'il se lance automatiquement avec la commande `systemctl status httpd `: 

```
[benji@web ~]$ sudo systemctl status httpd
● httpd.service - The Apache HTTP Server
   Loaded: loaded (/usr/lib/systemd/system/httpd.service; enabled; vendor preset: disabled)
   Active: active (running) since Mon 2021-10-04 08:48:23 CEST; 6min ago
```

Nous observons que le service est bien lancé avec **Active** et qu'il démarre automatiquement avec **enable**.


Je vérifie que je peux joindre mon serveur web avec une commande curl localhost

```
[benji@web ~]$ sudo curl localhost
<!doctype html>
<html>
  <head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>HTTP Server Test Page powered by: Rocky Linux</title>
[...]
```

Je vérifie l'accès depuis mon navigateur : 
![](https://i.imgur.com/J8SmJTf.png)

### 2. Avancer vers la maîtrise du service

Avec la commande ``` sudo systemctl enable httpd ```, je fais en sorte que le service soit actif dès le boot.

Je vérifie que le service est bien démarré et qu'il se lance automatiquement avec la commande `systemctl status httpd `: 

```
[benji@web ~]$ sudo systemctl status httpd
● httpd.service - The Apache HTTP Server
   Loaded: loaded (/usr/lib/systemd/system/httpd.service; enabled; vendor preset: disabled)
   Active: active (running) since Mon 2021-10-04 08:48:23 CEST; 6min ago
```

Nous observons que le service est bien lancé avec **Active** et qu'il démarre automatiquement avec **enable**.

Voici le contenu du fichier httpd.service : 

```
# See httpd.service(8) for more information on using the httpd service.

# Modifying this file in-place is not recommended, because changes
# will be overwritten during package upgrades.  To customize the
# behaviour, run "systemctl edit httpd" to create an override unit.

# For example, to pass additional options (such as -D definitions) to
# the httpd binary at startup, create an override unit (as is done by
# systemctl edit) and enter the following:

#       [Service]
#       Environment=OPTIONS=-DMY_DEFINE

[Unit]
Description=The Apache HTTP Server
Wants=httpd-init.service
After=network.target remote-fs.target nss-lookup.target httpd-init.service
Documentation=man:httpd.service(8)

[Service]
Type=notify
Environment=LANG=C

ExecStart=/usr/sbin/httpd $OPTIONS -DFOREGROUND
ExecReload=/usr/sbin/httpd $OPTIONS -k graceful
# Send SIGWINCH for graceful stop
KillSignal=SIGWINCH
KillMode=mixed
```

Voila la ligne qui définit l'utilisateur utilisé par le service définit dans son fichier de configuration : 
```
User apache
Group apache
``` 

Avec la commande ` ps -ef ` je vérifie que le service tourne bien sous l'utilisateur désigné dans le fichier de configuration : 
```
apache       856     836  0 08:48 ?        00:00:00 /usr/sbin/httpd -DFOREGROUND
apache       858     836  0 08:48 ?        00:00:00 /usr/sbin/httpd -DFOREGROUND
apache       859     836  0 08:48 ?        00:00:00 /usr/sbin/httpd -DFOREGROUND
apache       861     836  0 08:48 ?        00:00:00 /usr/sbin/httpd -DFOREGROUND
```
Nous observons que c'est bien l'utilisateur **apache** qui utilise le service.

Avec un `ls -al` je vérifie que l'utilisateur évoqué précédemment a accès au dossier: 

```
drwxr-xr-x.  4 root root   33 Sep 29 10:36 .
drwxr-xr-x. 22 root root 4096 Sep 29 10:36 ..
drwxr-xr-x.  2 root root    6 Jun 11 17:35 cgi-bin
drwxr-xr-x.  2 root root    6 Jun 11 17:35 html
```

L'utilisateur peut éxecuter le programme.

Je crée l'utilisateur **papou** inspiré d'apache : 
```
[benji@web /]$ sudo useradd -d /usr/share/httpd papou
[benji@web /]$ sudo usermod papou --shell=/sbin/nologin

[benji@web /]$ sudo cat /etc/passwd
apache:x:48:48:Apache:/usr/share/httpd:/sbin/nologin
papou:x:1001:1001::/usr/share/httpd:/sbin/nologin
```

Voici les changements effectués dans le fichier conf : 

```
User papou
Group papou
``` 





Je vérifie qu'ils ont pris effet après avoir redémarré apache avec la commande ` ps -U papou `: 
```
[benji@web ~]$ ps -U papou
    PID TTY          TIME CMD
    858 ?        00:00:00 httpd
    859 ?        00:00:00 httpd
    860 ?        00:00:00 httpd
    861 ?        00:00:00 httpd
```
Nous observons que c'est bien le nouvel utilisateur papou qui gère maintenant le service apache.

Voici la ligne que je modifie pour changer le port d'écoute : ` Listen 90 `
Je ferme mon port 80: 
```
[benji@web ~]$ sudo firewall-cmd --remove-port=80/tcp --permanent
```
Puis j'ouvre mon port 443 : 

```
[benji@web /]$ sudo firewall-cmd --add-port=443/tcp --permanent
success
[benji@web /]$ sudo firewall-cmd --reload
success
```
Je vérifie que je peux joindre le serveur avec le nouveau port grâce à un `curl 10.102.1.11:443` : 
```
[benji@web /]$ curl 10.102.1.11:443
<!doctype html>
<html>
  <head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>HTTP Server Test Page powered by: Rocky Linux</title>
    <style type="text/css">
```



Je vérifie que je peux joindre le serveur avec le nouveau port depuis mon navigateur : 

![](https://i.imgur.com/CmNzHOM.png)

## II. Une stack web plus avancée 

### A. Serveur Web et NextCloud


- Sur la machine web: 

J'installe EPEL avec la commande ` dnf install epel `, ensuite je ` dnf update ` pour vérifier d'avoir la dernière version d'epel.

- Ensuite, j'installe ` dnf install https://rpms.remirepo.net/enterprise/remi-release-8.rpm `.

- Puis je run ` dnf module list php `: 

```
php       7.2 [d]      common [d], devel, minimal     PHP scripting language
php       7.3          common [d], devel, minimal     PHP scripting language
php       7.4          common [d], devel, minimal     PHP scripting language

Remi's Modular repository for Enterprise Linux 8 - x86_64
Name      Stream       Profiles                       Summary
php       remi-7.2     common [d], devel, minimal     PHP scripting language
php       remi-7.3     common [d], devel, minimal     PHP scripting language
php       remi-7.4     common [d], devel, minimal     PHP scripting language
php       remi-8.0     common [d], devel, minimal     PHP scripting language
php       remi-8.1     common [d], devel, minimal     PHP scripting language
```

J'enable la dernière version de php afin qu'il soit oppérationel dès le boot.

je run `dnf module list php`

J'installe les outils nécessaire avec la commande : ```dnf install httpd mariadb-server vim wget zip unzip libxml2 openssl php74-php php74-php-ctype php74-php-curl php74-php-gd php74-php-iconv php74-php-json php74-php-libxml php74-php-mbstring php74-php-openssl php74-php-posix php74-php-session php74-php-xml php74-php-zip php74-php-zlib php74-php-pdo php74-php-mysqlnd php74-php-intl php74-php-bcmath php74-php-gmp```

Mon service httpd étant déjà configuré pour démarré automatiquement avec la commande ` systemctl enable httpd `, je passe à l'étape suivante.

Je crée les dossiers qui vont acceuillir les fichiers web : 

```
mkdir /etc/httpd/sites-enabled
mkdir /etc/httpd/sites-available
```
Nous avons également besoin d'un répertoire où nos sites vont résider. Ce répertoire peut être n'importe où, mais une bonne façon de garder les choses organisées est de créer un répertoire appelé **"sub-domains"**.
`mkdir /var/www/sub-domains/`


Je modifie/crée le fichier `  /etc/httpd/sites-available/web.tp2.linux `, je y insère ceci: 

```
<VirtualHost *:80>
  DocumentRoot /var/www/sub domains/web.tp2.linux/html/
  ServerName  nextcloud.yourdomain.com

  <Directory /var/www/sub-domains/web.tp2.linux/html/>
    Require all granted
    AllowOverride All
    Options FollowSymLinks MultiViews

    <IfModule mod_dav.c>
      Dav off
    </IfModule>
  </Directory>
</VirtualHost>
```

Ensuite je lie les dossiers /etc/httpd/sites-available/web.tp2.linux et /etc/httpd/sites-enabled/ avec la commande : 

```
ln -s /etc/httpd/sites-available/com.yourdomain.nextcloud /etc/httpd/sites-enabled/
```

Je crée le dossier "racine": `mkdir -p /var/www/sub-domains/web.tp2.linux/html`. Ce sera là que notre instance de nextcloud résidera.

Après avoir trouvé ma "timezone" avec la commande `timedatectl`, je modifie le fichier /etc/opt/remi/php74/php.ini : `;date.timezone =" Europe/Paris"`

Je télécharge Nextcloud avec la commande : `wget https://download.nextcloud.com/server/releases/nextcloud-22.2.0.zip`

Puis le dézip avec la commande : `unzip nextcloud-22.2.0.zip`

Dans le dossier /Nextcloud je copie les éléments de notre "racine" : `cp -Rf * /var/www/sub-domains/web.tp2.linux/html/`

Je donne les droits à apache afin que le service lui soit accessible : ``` chown -Rf apache.apache /var/www/sub-domains/web.tp2.linux/html ```

Pour des raisons de sécurité, nous voulons aussi déplacer le dossier "data" de l'intérieur à l'extérieur du DocumentRoot. 
```
mv /var/www/sub-domains/web.tp2.linux/html/data /var/www/sub-domains/web.tp2.linux/
```

Je rédemarre le service `sudo systemctl restart httpd` et je vérifie son fonctionnement dans mon navigateur.

![](https://i.imgur.com/KUAhTg3.png)

### B. Base de données
- Sur la machine database: 

Je commence avec une `dnf update` puis un `dnf install mariadb-server` afin d'installer mariadb.

Ensuite, afin de démarré le service automatiquement, je fais `systemctl enable mariadb` puis je démarre le service avec ` systemctl start mariadb `.

Afin d'installé correctement mysql, je lance `mysql_secure_installation`, je crée un nouveau mot de passe pour l'utilisateur root, je supprime les utilisateurs anonymes puis je presse entrée jusqu'a la fin du setup.

Avec la commande `ss -antpl` j'observe que le port utilisé par mariadb est le port **3306**. Je l'ouvre donc avec la commande : `[benji@db ~]$ sudo firewall-cmd --add-port=3306/tcp --permanent
`

- Sur la Machine Nextcloud

Je vérifie le fonctionnement de l'accèssibilité de la base de donnée avec la commande ci-dessous:
```
[benji@web ~]$ mysql -u nextcloud -h 10.102.1.12 -p
Enter password:
Welcome to the MySQL monitor.  Commands end with ; or \g.
Your MySQL connection id is 9
Server version: 5.5.5-10.3.28-MariaDB MariaDB Server
```
- trouver une commande qui permet de lister tous les utilisateurs de la base de données :
```SELECT User FROM mysql.user;```
### C. Finaliser l'installation de NextCloud

- modifiez votre fichier hosts 
 Je modifie mon fichier host situé dans C:\Windows\System32\drivers\etc, je lui ajoute ceci : 
 `10.102.1.11 web.tp2.Linux`.
 
🌞 Exploration de la base de données
```
[benji@db ~]$ sudo mysql -u root -p nextcloud -e "SHOW TABLES;"
Enter password: 
+-----------------------------+
| Tables_in_nextcloud         |
+-----------------------------+
| oc_accounts                 |
| oc_accounts_data            |
| oc_activity                 |
| oc_activity_mq              |
[...]
```


| Machine         | IP            | Service                 | Port ouvert | IP autorisées |
|-----------------|---------------|-------------------------|-------------|---------------|
| `web.tp2.linux` | `10.102.1.11` | Serveur Web             | 80          | any           |
| `db.tp2.linux`  | `10.102.1.12` | Serveur Base de Données | 3306        | `10.102.1.11` 
