#!/bin/bash
sNUM=`grep JOBID temp.csv -c`
numID=`expr $sNUM - 1`
awk -v n=$numID  '/^JOBID/{l++} l>n{exit} l==n' temp.csv | tail -n +2 | head -n -3 > jobs.csv
