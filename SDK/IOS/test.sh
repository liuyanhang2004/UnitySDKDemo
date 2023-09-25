#! /bin/bash
p1=$1
p2=$2
p3=$3
echo "$p1"
echo "$p2"
echo "$p3"

function name() {
    echo "function execute"
    return 1
}

name
echo "v $?"
