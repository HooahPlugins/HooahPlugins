set BUILD_PATH=%1
set TARGET_PATH=%2
set ASSEMBLY_NAME=%3
if not %TARGET_PATH% == "" (
   if not exist %TARGET_PATH% ( mkdir "%BUILD_PATH%" )
   copy /Y "%BUILD_PATH%.dll" "%TARGET_PATH%\%ASSEMBLY_NAME%.dll"
)