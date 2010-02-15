svn export https://www.njcrawford.com/svn/EmbroideryReader/branches/1.4 source-zip
echo "Check source-zip folder, make sure there are no unwanted files"
pause
BuildInstaller.exe
rmdir /Q /S source-zip
move embreadsetup*.exe upload-me\
move embroideryReader*.zip upload-me\
copy update.ini upload-me\