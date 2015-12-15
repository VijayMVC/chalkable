start cmd /C "CALL run-redis-cache.cmd"
start cmd /C "CALL run-static-server.cmd"
cd Chalkable.Web
start cmd /C "CALL compass watch"
cd ..\