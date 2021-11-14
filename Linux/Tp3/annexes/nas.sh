#!/bin/bash
# simple samba server with vpn and jellyfin server
# Benjamin Gelineau 10/11/2021
sudo apt update
sudo apt upgrade
sudo mkdir /home/shares
sudo mkdir /home/shares/public
sudo mkdir /home/shares/public/disk1
sudo chown -R root:users /home/shares/public/disk1
sudo chmod -R ug=rwx,o=rx /home/shares/public/disk1
sudo apt install samba samba-common-bin
echo -e "[public]
comment= Public Storage
path = /home/shares/public/disk1
valid users = @users
force group = users
create mask = 0660
directory mask = 0771
read only = no" >> /etc/samba/smb.conf
sudo /etc/init.d/smbd restart
sudo smbpasswd -a 
echo -e "/dev/sda1 /home/shares/public/ auto noatime 0 0" >> /etc/fstab
curl -L https://install.pivpn.io > pivpn.sh
bash pivpn.sh
Pivpn add
sudo apt install apt-transport-https
wget -O - https://repo.jellyfin.org/jellyfin_team.gpg.key | sudo apt-key add -
echo "deb [arch=$( dpkg --print-architecture )] https://repo.jellyfin.org/$( awk -F'=' '/^ID=/{ print $NF }' /etc/os-release ) $( awk -F'=' '/^VERSION_CODENAME=/{ print $NF }' /etc/os-release ) main" | sudo tee /etc/apt/sources.list.d/jellyfin.list
sudo apt install jellyfin
sudo systemctl enable jellyfin
sudo systemctl daemon-reload
sudo apt-get install ufw -y
sudo ufw enable
sudo ufw allow 8096/tcp
sudo ufw allow 51820/udp
bash <(curl -Ss https://my-netdata.io/kickstart.sh)
sudo ufw allow 19999/tcp
sudo ufw allow 22/tcp
sudo ufw reload

