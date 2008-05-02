svn export http://dell/newrepos/EmbroideryReader/trunk trunktemp
cd trunktemp\install
BuildInstaller.exe
move embreadsetup*.exe ..\..\
move embroideryReader*.zip ..\..\
cd ..\..\
rmdir /Q /S trunktemp
pause