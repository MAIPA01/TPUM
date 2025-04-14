@echo off
setlocal enabledelayedexpansion

set REBUILD_RSP=false
set BASE=%CD%\
set "OUTPUT_DLL_PATH=Temp\XmlsObjects.dll"
set "OUTPUT_SCHEMA_DIR=Schema"

rem Sprawdź, czy podano flagę --rebuild-rsp
if "%1"=="--rebuild-rsp" (
    set REBUILD_RSP=true
)

rem Jeśli nie istnieje sources.rsp lub flaga jest ustawiona, generujemy nowy plik
if not exist sources.rsp (
    set REBUILD_RSP=true
)

if "%REBUILD_RSP%"=="true" (
    echo Generating sources.rsp...
	
	echo -out:"%OUTPUT_DLL_PATH%"
    echo -out:"%OUTPUT_DLL_PATH%" > sources.rsp
	
	echo -target:library
    echo -target:library >> sources.rsp
	
	for %%f in (Original\*.cs) do (
		echo "%%f"
		echo "%%f" >> sources.rsp
	)

    for /R Original\Request %%f in (*.cs) do (
		set "FULL=%%f"
		set "REL=!FULL:%BASE%=!"
		echo "!REL!"
        echo "!REL!" >> sources.rsp
    )
	
	for /R Original\Response %%f in (*.cs) do (
		set "FULL=%%f"
		set "REL=!FULL:%BASE%=!"
		echo "!REL!"
		echo "!REL!" >> sources.rsp 
	)
) else (
    echo Using existing sources.rsp
)

REM Pobierz tylko ścieżkę do folderu
for %%I in ("%OUTPUT_DLL_PATH%") do set "OUTPUT_DLL_DIR=%%~dpI"

if not exist %OUTPUT_DLL_DIR% (
	REM Stwórz foldery, jeśli nie istnieją
	mkdir "%OUTPUT_DLL_DIR%" 2>nul
)

rem Kompilacja
echo Compiling...
csc @sources.rsp

rem czyszczenie folderu schemy
echo Cleaning %OUTPUT_SCHEMA_DIR%...
for /d %%D in (%OUTPUT_SCHEMA_DIR%\*) do rd /s /q "%%D"
del /q %OUTPUT_SCHEMA_DIR%\*

rem Generowanie xsd
echo Generating Schema...
xsd "%OUTPUT_DLL_PATH%" /out:"%OUTPUT_SCHEMA_DIR%"

rem tworzenie listy plików xsd
set "xsdFiles="
for %%f in (%OUTPUT_SCHEMA_DIR%\*.xsd) do (
	set "xsdFiles=!xsdFiles! "%%f""
)

rem stworzenie klas na podstawie schemy
xsd !xsdFiles! /out:Generated /classes /namespace:TPUM.XmlShared.Generated

endlocal