@echo off

:begin


C:\cygwin64\bin\ssh root@crane-head -i cornFieldKey > jobs.csv

timeout /t 5

goto begin
