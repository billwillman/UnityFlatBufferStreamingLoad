cd %~dp0

echo "build flatbuffer data"
flatc.exe -b ./StreamLoad/Assets/Resources/MonsterCfg.fbs ./StreamLoad/Assets/Resources/MonsterCfg.json
echo f | xcopy "%~dp0MonsterCfg.bin" "%~dp0StreamLoad/Assets/Resources/MonsterCfg_flatbuffer.bytes" /y
del /f %~dp0MonsterCfg.bin

echo "build idMap proto"
cd %~dp0StreamLoad/Assets/Resources/
protoc --csharp_out=./ IdMap.proto

pause