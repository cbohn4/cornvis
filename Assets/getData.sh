#!/bin/bash
sNUM=`grep JOBID temp.csv -c`
numID=`expr $sNUM - 1`
awk -v n=$numID  '/^JOBID/{l++} l>n{exit} l==n' temp.csv > jobs.csv
