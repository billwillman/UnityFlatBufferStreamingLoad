cd %~dp0
flatc.exe -b ./StreamLoad/Assets/Resources/MonsterCfg.fbs ./StreamLoad/Assets/Resources/MonsterCfg.json
echo f | xcopy "%~dp0MonsterCfg.bin" "%~dp0StreamLoad/Assets/Resources/MonsterCfg.bytes" /y
del /f %~dp0MonsterCfg.bin
pause