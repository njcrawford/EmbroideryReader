svn export http://medium/newrepos/EmbroideryReader/trunk source-zip
echo "Check source-zip folder, make sure there are no unwanted files"
pause
BuildInstaller.exe
rmdir /Q /S source-zip
move embreadsetup*.exe upload-me\
move embroideryReader*.zip upload-me\
copy update.ini upload-me\