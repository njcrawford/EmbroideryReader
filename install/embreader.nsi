; example2.nsi
;
; This script is based on example1.nsi, but it remember the directory, 
; has uninstall support and (optionally) installs start menu shortcuts.
;
; It will install example2.nsi into a directory that the user selects,

!include LogicLib.nsh
!include "DotNET.nsh"
#define DOTNET_VERSION "2.0"

;--------------------------------

; The name of the installer
Name "Embroidery Reader"

; The file to write
OutFile "${outfile}"

; The default installation directory
InstallDir "$PROGRAMFILES\Embroidery Reader"
; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Embroidery Reader" "Install_Dir"

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "Embroidery Reader ${VERSION} (required)"

  SectionIn RO

  ;check for .net 2
  !insertmacro CheckDotNet "2.0"
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
   File "embroideryReader.exe"
   File "nc_settings.dll"
   File "PesFile.dll"
   File "nc_Updater.dll"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM "SOFTWARE\Embroidery Reader" "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Embroidery Reader" "DisplayName" "Embroidery Reader"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Embroidery Reader" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Embroidery Reader" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Embroidery Reader" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\Embroidery Reader"
  CreateShortCut "$SMPROGRAMS\Embroidery Reader\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Embroidery Reader\Embroidery Reader.lnk" "$INSTDIR\embroideryReader.exe" "" "$INSTDIR\embroideryReader.exe" 0
  
SectionEnd

Section "Associate with .PES files"
 
  ; back up old value of .pes
!define Index "Line${__LINE__}"
  ReadRegStr $1 HKCR ".pes" ""
  StrCmp $1 "" "${Index}-NoBackup"
    StrCmp $1 "EmbroideryDesign" "${Index}-NoBackup"
    WriteRegStr HKCR ".pes" "backup_val" $1
"${Index}-NoBackup:"
  WriteRegStr HKCR ".pes" "" "EmbroideryDesign"
  WriteRegStr HKCR ".pes" "PerceivedType" "image"
  ReadRegStr $0 HKCR "EmbroideryDesign" ""
  StrCmp $0 "" 0 "${Index}-Skip"
	WriteRegStr HKCR "EmbroideryDesign" "" "Embroidery Design File"
	WriteRegStr HKCR "EmbroideryDesign\shell" "" "open"
	WriteRegStr HKCR "EmbroideryDesign\DefaultIcon" "" "$INSTDIR\embroideryReader.exe,0"
"${Index}-Skip:"
  WriteRegStr HKCR "EmbroideryDesign\shell\open\command" "" '$INSTDIR\embroideryReader.exe "%1"'



	# Tell shell to use this for thumbnails
	;StrCmp $DoThumbs 0 +3
	WriteRegStr HKCR ".pes\ShellEx" "" ""
	WriteRegStr HKCR ".pes\ShellEx\{BB2E617C-0920-11d1-9A0B-00C04FC2D6C1}" "" "{7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E}"

	# Add thumbnail extractor classes
	WriteRegStr HKCR "PESIcon.Extractor" "" "Embroidery Design Thumbnail Extractor"
	WriteRegStr HKCR "PESIcon.Extractor\CLSID" "" "{7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E}"
	WriteRegStr HKCR "PESIcon.Extractor\CurVer" "" "PESIcon.Extractor.1"
	WriteRegStr HKCR "PESIcon.Extractor.1" "" "Embroidery Design Thumbnail Extractor"
	WriteRegStr HKCR "PESIcon.Extractor.1\CLSID" "" "{7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E}"

	# Add CLSID
	WriteRegStr HKCR "CLSID\{7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E}" "" "Embroidery Design Thumbnail Extractor"
	WriteRegStr HKCR "CLSID\{7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E}\InProcServer32" "" "$INSTDIR\embroideryThumbs.dll"
	WriteRegStr HKCR "CLSID\{7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E}\InProcServer32" "ThreadingModel" "Apartment"
	WriteRegStr HKCR "CLSID\{7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E}\ProgId" "" "PESIcon.Extractor.1"
	WriteRegStr HKCR "CLSID\{7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E}\VersionIndependantProgId" "" "PESIcon.Extractor"
	
	# Add to shell approved extensions list
	WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved" "{7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E}" "Embroidery Design Thumbnail Extractor"

	# Cause explorer shell to reload settings
	#System::Call "shell32::SHChangeNotify(i,i,i,i) (${SHCNE_ASSOCCHANGED}, ${SHCNF_FLUSH}, 0, 0)"
 
  System::Call 'Shell32::SHChangeNotify(i 0x8000000, i 0, i 0, i 0)'
!undef Index

SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
;start of restore script
!define Index "Line${__LINE__}"
  ReadRegStr $1 HKCR ".pes" ""
  StrCmp $1 "EmbroideryDesign" 0 "${Index}-NoOwn" ; only do this if we own it
    ReadRegStr $1 HKCR ".pes" "backup_val"
    StrCmp $1 "" 0 "${Index}-Restore" ; if backup="" then delete the whole key
      DeleteRegKey HKCR ".pes"
    Goto "${Index}-NoOwn"
"${Index}-Restore:"
      WriteRegStr HKCR ".pes" "" $1
      DeleteRegValue HKCR ".pes" "backup_val"
   
    DeleteRegKey HKCR "EmbroideryDesign" ;Delete key with association settings
 
    System::Call 'Shell32::SHChangeNotify(i 0x8000000, i 0, i 0, i 0)'
"${Index}-NoOwn:"
!undef Index


  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Embroidery Reader"
  DeleteRegKey HKLM "SOFTWARE\Embroidery Reader"

  ; Remove files and uninstaller
  Delete $INSTDIR\embroideryReader.exe
  Delete $INSTDIR\uninstall.exe
  Delete $INSTDIR\nc_settings.dll
  Delete $INSTDIR\embroideryreader.ini
  Delete $INSTDIR\PesFile.dll
  Delete $INSTDIR\nc_Updater.dll

  ; Remove obsolete files from previous versions, if they exist
  Delete $INSTDIR\UpdateInstaller.exe
  RMDir "$INSTDIR\update"

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Embroidery Reader\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\Embroidery Reader"
  RMDir "$INSTDIR"

SectionEnd
