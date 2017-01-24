netsh http delete urlacl url=http://+:8300/
netsh http add urlacl url=http://+:8300/ user=%USERNAME%
pause
