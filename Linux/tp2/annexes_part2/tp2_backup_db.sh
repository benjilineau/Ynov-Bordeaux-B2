#!/bin/bash
# Saving db
# Gelineau Benjamin- 20/10/2021


  DATE=$(date +%y%m%d_%H%M%S)
  Destination=$1
  Target=nextcloud
  Here=$(pwd)/"hello_${DATE}.tar.gz"

  mysqldump --user=nextcloud --password=meow --databases nextcloud

  tar cvzf "hello_${DATE}.tar.gz" backup_donnees.sql
  rsync -av --remove-source-files ${Here} ${Destination}
  rm -rf backup_donnees.sql
  ls -tp "${Destination}" | grep -v '/$' | tail -n +6 | xargs -I {} rm -- ${Destination}/{}
