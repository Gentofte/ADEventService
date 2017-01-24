netsh http delete urlacl url=http://+:8333/
netsh http add urlacl url=http://+:8333/ user=everyone
netsh http delete urlacl url=http://+:8300/
netsh http add urlacl url=http://+:8300/ user=everyone
netsh http delete urlacl url=http://+:8810/
netsh http add urlacl url=http://+:8810/ user=everyone
pause
